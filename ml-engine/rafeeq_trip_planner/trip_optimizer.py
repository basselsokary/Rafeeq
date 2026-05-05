"""
trip_optimizer.py — KemetPath Route Optimization Engine v2
===========================================================
Generates personalized 1-day historical itineraries for Cairo using
a Beam Search optimization engine with a formal 5-term objective function.

AI Components:
  1. Beam Search Route Builder     — explores K=5 partial routes simultaneously
  2. 5-Term Objective Function     — preference, engagement, diversity, travel, redundancy
  3. Log-Based Diversity Scoring   — Shannon entropy + log-diminishing repeat penalties
  4. Budget Fill Phase             — packs residual time with shorter stops
  5. Cairo Geo-Validation          — bounding-box filter removes out-of-city noise
  6. ML Batch Inference            — RandomForest with uncertainty estimation
  7. Behavioral Walking Tolerance  — distance_weight + per-leg hard cap
  8. Explainability Logging        — budget %, entropy, km totals, objective score

Usage:
    python trip_optimizer.py        # runs 3 demo profiles
    from trip_optimizer import generate_trip
"""

import json
import os
import math
import copy
import logging
from datetime import datetime, timedelta
from typing import Optional
import numpy as np

# ---------------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------------
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    datefmt="%H:%M:%S",
)
logger = logging.getLogger(__name__)

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------
MODEL_FILE  = "duration_model.pkl"
RANDOM_SEED = 42

# Travel constants — walking-based model
WALK_SPEED_KMH  = 4.5    # km/h — pedestrian pace
URBAN_SPEED_KMH = 25.0   # km/h — used for scoring reverse-engineering only
MIN_TRAVEL_MIN  = 1.0    # absolute floor
ZONE_CROSS_MULT = 1.3    # 30% extra when crossing zone boundaries
# Legacy aliases
DRIVE_SPEED_KMH  = 25.0
WALK_DRIVE_SPLIT = 1.5

# ── Fix 1: Hard per-leg travel-time cap by tolerance ────────────────────────
# Sites requiring MORE than this to reach are REJECTED, not just penalised.
# Based on realistic transport time (not walking):
#   low    = walking only  (~10 min walk = 0.75 km)
#   medium = taxi/tuktuk   (~20 min by road)
#   high   = car/microbus  (~40 min by road)
MAX_LEG_TRAVEL_MIN = {
    "low":    10,   # walking only
    "medium": 20,   # short taxi hop
    "high":   40,   # car / long taxi
}

# Hardcoded CAIRO_BBOX and ZONE_CENTROIDS removed in v3 refactor.
# All geographic constraints are now data-driven via city_resolver.py.

# ── Fix 2: Local-first radius — small initial filter, expand only if needed ─
# These replace the old wide radii (15/60/300 km) that caused 90-min travel.
WALKING_TOLERANCE_RADIUS = {
    "low":    5.0,    # walking distance only
    "medium": 12.5,   # short ride / dense city
    "high":   20.0,   # local region
}

# Minimum sites found locally before expanding to neighbouring cities.
MIN_LOCAL_SITES = 3

# ── Fix 5: Walking distance budget cap per route ────────────────────────────
# Cumulative straight-line km across all legs in the route.
# Sized to allow realistic taxi+walking distances per day.
MAX_TOTAL_WALK_KM = {
    "low":    2.0,    # walkable neighbourhood only
    "medium": 10.0,   # mix of walking and short taxi hops
    "high":   25.0,   # full day with taxi/transport
}

# ── Fix 6: Category alias map — broader matching ─────────────────────────────
# When user asks for cat X, sites in CATEGORY_ALIASES[X] also match (at 0.6x score).
CATEGORY_ALIASES: dict[str, list] = {
    # Existing aliases
    "Historical Site":    ["Monument", "Landmark", "Archaeological Site", "Citadel", "Fortress", "Historical"],
    "Monument":           ["Historical Site", "Landmark", "Historical"],
    "Archaeological Site":["Historical Site", "Landmark", "Necropolis", "Archaeological"],
    "Landmark":           ["Historical Site", "Monument", "Archaeological Site"],
    "Religious Site":     ["Mosque", "Church", "Synagogue", "Monastery", "Temple", "Religious"],
    "Mosque":             ["Religious Site", "Church", "Religious"],
    "Church":             ["Religious Site", "Mosque", "Religious"],
    "Museum":             ["Cultural Center", "Gallery", "Cultural"],
    "Cultural Center":    ["Museum", "Library", "Cultural", "Gallery"],
    "Beach":              ["Nature Reserve", "Island", "Promenade", "Walkway", "Natural"],
    "Island":             ["Beach", "Nature Reserve", "Natural"],
    "Nature Reserve":     ["Park", "Beach", "Island", "Natural", "Mountain", "River"],
    "Park":               ["Nature Reserve", "Natural"],
    "Temple":             ["Archaeological Site", "Historical Site", "Religious", "Archaeological"],
    "Pyramid":            ["Archaeological Site", "Historical Site", "Monument", "Archaeological"],
    "Fortress":           ["Historical Site", "Citadel", "Historical"],
    "Citadel":            ["Historical Site", "Fortress", "Historical"],
    # New Excel-matching category aliases
    "Religious":          ["Mosque", "Church", "Synagogue", "Monastery", "Temple", "Religious Site"],
    "Archaeological":     ["Historical", "Monument", "Landmark", "Archaeological Site", "Historical Site"],
    "Historical":         ["Archaeological", "Monument", "Landmark", "Historical Site", "Archaeological Site"],
    "Natural":            ["Park", "Beach", "Island", "Mountain", "River", "Nature Reserve"],
    "Mountain":           ["Natural", "Nature Reserve", "Park"],
    "River":              ["Natural", "Nature Reserve"],
    "Gallery":            ["Museum", "Cultural", "Cultural Center"],
    "Cultural":           ["Museum", "Gallery", "Library", "Cultural Center"],
    "Library":            ["Cultural", "Cultural Center", "Museum"],
    "Walkway":            ["Promenade", "Beach", "Park"],
    "Therapeutic":        ["Spa & Thermal Springs", "Natural"],
}

# Behavioral config per tolerance level.
# max_leg_km: straight-line distance cap consistent with MAX_LEG_TRAVEL_MIN + transport speed:
#   low:    10 min @ 4.5 km/h walk  = 0.75 km
#   medium: 20 min @ 25 km/h taxi   = 8.3  km
#   high:   40 min @ 35 km/h car    = 23.3 km
WALKING_CONFIG = {
    "low":    {"distance_weight": 1.8, "max_leg_km":  0.75},
    "medium": {"distance_weight": 1.0, "max_leg_km":  8.3},
    "high":   {"distance_weight": 0.5, "max_leg_km": 23.3},
}

# 5-term objective weights (must sum to 1.0)
#   w1_preference : reward category match
#   w2_engagement : reward high ML-duration sites
#   w3_diversity  : Shannon entropy bonus
#   w4_travel     : penalise travel time / budget fraction
#   w5_redundancy : penalise log-scaled category repetition
OBJECTIVE_WEIGHTS = {
    "w1_preference":  0.45,   # raised — primary driver
    "w2_engagement":  0.15,   # lowered — engagement is secondary
    "w3_diversity":   0.10,   # lowered — diversity is a bonus, not dominant
    "w4_travel":      0.15,   # unchanged
    "w5_redundancy":  0.10,   # unchanged
}

# Proximity bonus: fraction added to delta_J when next site is within CLUSTER_KM
PROXIMITY_BONUS = 0.20
CLUSTER_KM      = 1.0    # within 1 km → same cluster

# Log-based diversity penalty coefficients
DIVERSITY_CONFIG = {
    "category_alpha": 0.30,
    "zone_alpha":     0.20,
}

# Beam search width
BEAM_WIDTH = 5

# Fill phase tuning
MIN_FILL_MINUTES   = 45    # only start filling if > 45 min remain
FILL_MAX_LEG_KM    = 5.0   # fill stops must be within 5 km of last stop
FILL_MIN_VISIT_MIN = 45    # fill stops: minimum predicted duration
FILL_MAX_VISIT_MIN = 90    # fill stops: maximum predicted duration

# Target budget utilisation
BUDGET_TARGET_PCT = 0.85

# ── Multi-region geographic loading ─────────────────────────────────────────
# Radius used to sweep ALL city datasets for candidate sites.
# Chosen to cover one full governorate + immediate neighbours.
MULTI_REGION_RADIUS_KM = 150.0

# ── Hard constraint: total travel must not dominate the day ─────────────────
# Reject any itinerary where cumulative travel > this fraction of budget.
MAX_TRAVEL_BUDGET_RATIO        = 0.40   # normal areas: max 40% travel
MAX_TRAVEL_BUDGET_RATIO_SPARSE = 0.60   # sparse / low-density: relax to 60%

# ── Financial budget feature ─────────────────────────────────────────────────
# Exchange rate used to convert USD → EGP (update when needed).
USD_TO_EGP: float = 50.0

# Centre of Cairo (fallback GPS)
DEFAULT_LAT = 30.0478
DEFAULT_LON = 31.2336


def resolve_ticket_budget(profile: dict) -> float | None:
    """
    Return the user's ticket budget normalised to EGP, or None if unlimited.

    New implicit logic (budget_enabled is gone):
      - budget_amount absent or == 0  → unlimited (return None)
      - budget_amount  > 0            → constrained
          currency "USD" → multiply by USD_TO_EGP
          currency "EGP" → use as-is

    Callers treat None as "no financial constraint".
    """
    amount = float(profile.get("budget_amount") or 0)
    if amount < 0:
        return None   # negative → open / unlimited mode
    currency = str(profile.get("currency", "EGP")).upper().strip()
    if currency == "USD":
        amount = amount * USD_TO_EGP
    return amount



# ---------------------------------------------------------------------------
# Data Cleaning Constants
# ---------------------------------------------------------------------------

# Categories accepted from ANY city dataset (covers both legacy Cairo data
# and the new multi-city normalized categories from generate_city_datasets.py).
ALLOWED_CATEGORIES = {
    # Legacy Cairo scrape categories
    "Pyramid", "Museum", "Mosque", "Church", "Monument",
    "Historical Site", "Palace", "Fortress", "Tomb",
    "Gate", "Bazaar", "Necropolis", "Temple", "Synagogue",
    "Archaeological Site", "Citadel",
    # Old normalized names (kept for backward compat)
    "Religious Site", "Nature Reserve", "Diving Site", "Spa & Thermal Springs",
    "Park", "Promenade", "Marina", "Market", "Entertainment", "Aquarium",
    "Cultural Center", "Landmark", "Monastery", "Beach", "Island",
    # New Excel-matching category names
    "Religious", "Natural", "Historical", "Archaeological",
    "Mountain", "River", "Therapeutic", "Gallery", "Library",
    "Cultural", "Walkway",
}
# ALLOWED_ZONES removed — zones are now city-specific and not whitelisted.

# Soft cap on how many times the same category can appear in one route.
MAX_SAME_CATEGORY = 2

# Bonus score for the first stop of a brand-new category.
NEW_CATEGORY_BONUS = 0.15

# Sites that must never be removed by the cleaning pipeline.
# Kept for backward compatibility with cairo_historical_sites.json.
IMPORTANT_SITES = {
    "Great Pyramid of Giza",
    "Pyramid of Khafre",
    "Pyramid of Menkaure",
    "Egyptian Museum",
    "Sphinx",
    "Khan el-Khalili",
    "Citadel of Saladin",
}

# ---------------------------------------------------------------------------
# In-process model cache
# ---------------------------------------------------------------------------
_model_cache: dict = {"path": None, "bundle": None}


# ============================================================
# SECTION 1 — Geo & Time Utilities
# ============================================================

def haversine(lat1: float, lon1: float, lat2: float, lon2: float) -> float:
    """Great-circle distance in km between two GPS coordinates."""
    R = 6371.0
    phi1, phi2   = math.radians(lat1), math.radians(lat2)
    dphi         = math.radians(lat2 - lat1)
    dlambda      = math.radians(lon2 - lon1)
    a = math.sin(dphi / 2) ** 2 + math.cos(phi1) * math.cos(phi2) * math.sin(dlambda / 2) ** 2
    return R * 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))


def travel_time_minutes(
    lat1: float, lon1: float,
    lat2: float, lon2: float,
    zone1: str = "", zone2: str = "",
    tolerance: str = "medium",
) -> float:
    """
    Multi-modal travel time model.

    Speed by tolerance:
      low    : walking only  (4.5 km/h) — same neighbourhood
      medium : mixed walk + taxi (walk if <0.5 km, else 25 km/h road speed)
      high   : taxi / car   (35 km/h) — cross-city transport

    Zone-crossing adds 30% extra for medium/high.
    Hard tolerance cap is enforced in beam search after this returns.
    """
    km = haversine(lat1, lon1, lat2, lon2)

    if tolerance == "low":
        speed = WALK_SPEED_KMH          # pure walking
    elif tolerance == "medium":
        speed = WALK_SPEED_KMH if km < 0.5 else URBAN_SPEED_KMH
    else:   # high
        speed = 35.0                    # car / microbus

    t = (km / speed) * 60.0

    # Zone-crossing surcharge (medium and high only)
    cross_zone = zone1 and zone2 and zone1 != zone2
    if cross_zone and tolerance != "low":
        t *= ZONE_CROSS_MULT

    t = max(MIN_TRAVEL_MIN, t)
    logger.debug(
        "[Travel] %.3fkm | tol=%s | zones='%s'->'%s' cross=%s | time=%.1fmin",
        km, tolerance, zone1, zone2, cross_zone, t,
    )
    return round(t, 1)


def format_time(start_str: str, offset_minutes: float) -> str:
    """Add offset_minutes to 'HH:MM' and return 'HH:MM'."""
    try:
        base   = datetime.strptime(start_str, "%H:%M")
        result = base + timedelta(minutes=offset_minutes)
        return result.strftime("%H:%M")
    except ValueError:
        return "??:??"


def safe_coords(site: dict) -> tuple:
    """Return (lat, lon) from site dict, defaulting to downtown Cairo."""
    lat = site.get("latitude")
    lon = site.get("longitude")
    if lat is None or lon is None:
        return DEFAULT_LAT, DEFAULT_LON
    return float(lat), float(lon)


# ============================================================
# SECTION 2 — Full Data Cleaning Pipeline
# ============================================================

def clean_site_data(sites: list) -> list:
    """
    Multi-city data cleaning pipeline (v3).

    Removed in v3:
      - Cairo bounding-box filter (geography is now data-driven)
      - Cairo zone-whitelist and zone-centroid correction

    Retained:
      - Non-place name filter (battles, dynasty names, people, etc.)
      - Short name filter (< 3 chars)
      - Missing name / category filter
      - Category whitelist (expanded for multi-city categories)
      - Deduplication (exact name OR within 100 m)
    """
    import re as _re

    _NON_PLACE = [
        r"\(\d{3,4}\)",
        r"^Battle of", r"^Capture of", r"^Siege of", r"^Treaty of",
        r"^Fall of",   r"^Conquest of", r"^History of", r"^List of",
        r" Period$",   r" Dynasty$",    r" Era$",
        r"\(Egyptologist\)", r"\(activist\)", r"\(architect\)",
        r"\(novel\)",  r"\(film\)",  r"\(book\)",
        r"^Giovanni ", r"^Johann ",  r"^Howard ", r"^Henry Salt",
        r"^Silvio ",   r"^Decimus ", r"^Max Geller", r"^John Greaves",
        r"^Pachomian", r"^Elephant", r"^Rhacotis",
        r"^Stone quarries", r"^Pyramid inch",
        r"Project$",   r"^Studies in",
    ]
    _is_non_place = lambda n: any(_re.search(p, n) for p in _NON_PLACE)

    removed_events   = []
    removed_short    = []
    removed_missing  = []
    removed_category = []
    removed_dupes    = []

    # Step 0: non-place filter
    step0 = []
    for s in sites:
        name = s.get("name", "").strip()
        if name in IMPORTANT_SITES or not _is_non_place(name):
            step0.append(s)
        else:
            removed_events.append(name)

    # Step 0b: short name filter (>= 3 chars; relaxed for multi-city names)
    step0b = []
    for s in step0:
        name = s.get("name", "").strip()
        if name in IMPORTANT_SITES or len(name) >= 3:
            step0b.append(s)
        else:
            removed_short.append(name)

    # Step 1: missing name / category  (coords optional — site can still appear)
    step1 = []
    for s in step0b:
        name = s.get("name", "").strip()
        cat  = s.get("category", "").strip()
        if not name or not cat:
            removed_missing.append(name or "<no-name>")
            continue
        step1.append(s)

    # Step 2: category whitelist (IMPORTANT_SITES bypass)
    step2 = []
    for s in step1:
        name = s.get("name", "").strip()
        cat  = s.get("category", "").strip()
        if cat in ALLOWED_CATEGORIES or name in IMPORTANT_SITES:
            step2.append(s)
        else:
            removed_category.append(f"{name} (cat='{cat}')")

    # Step 3: deduplication — exact name OR within 100 m
    seen_names:  set  = set()
    seen_coords: list = []
    step3 = []
    for s in step2:
        name = s["name"].strip()
        lat  = s.get("latitude")
        lon  = s.get("longitude")
        if name in IMPORTANT_SITES:
            if name.lower() not in seen_names:
                seen_names.add(name.lower())
                if lat is not None and lon is not None:
                    seen_coords.append((float(lat), float(lon)))
                step3.append(s)
            continue
        if name.lower() in seen_names:
            removed_dupes.append(f"{name} [name]")
            continue
        if lat is not None and lon is not None:
            if any(haversine(float(lat), float(lon), p, q) < 0.10
                   for p, q in seen_coords):
                removed_dupes.append(f"{name} [100m]")
                continue
            seen_coords.append((float(lat), float(lon)))
        seen_names.add(name.lower())
        step3.append(s)

    logger.info(
        "[DataClean] %d -> %d sites | "
        "non_visitable=%d bad_cat=%d duplicates=%d",
        len(sites), len(step3),
        len(removed_events) + len(removed_short) + len(removed_missing),
        len(removed_category),
        len(removed_dupes),
    )
    for label, lst in [
        ("non_place",  removed_events[:50]),
        ("short_name", removed_short[:20]),
        ("missing",    removed_missing[:20]),
        ("bad_cat",    removed_category[:20]),
        ("duplicates", removed_dupes[:50]),
    ]:
        if lst:
            logger.info("[DataClean]   %s: %s", label, "; ".join(lst))

    return step3


def validate_sites_geo(sites: list) -> list:
    """Legacy alias — kept for backward compatibility."""
    return clean_site_data(sites)


# ============================================================
# SECTION 3 — ML Model Loading & Prediction (Upgrades 1, 6)
# ============================================================

def _load_model_bundle(model_path: str) -> Optional[dict]:
    """
    Load the RandomForest bundle from disk (cached in memory after first call).
    Bundle must contain: 'model', 'encoders', 'feature_columns'.
    Logs clearly on success or failure.
    """
    global _model_cache
    if _model_cache["path"] == model_path and _model_cache["bundle"] is not None:
        return _model_cache["bundle"]

    if not os.path.exists(model_path):
        logger.warning("[ML] Model file '%s' not found — using rule-based fallback.", model_path)
        return None

    try:
        import joblib
        bundle  = joblib.load(model_path)
        missing = [k for k in ("model", "feature_columns", "encoders") if k not in bundle]
        if missing:
            logger.warning(
                "[ML] Bundle missing keys %s. Re-run train_duration_model.py — "
                "using rule-based fallback.", missing
            )
            return None

        _model_cache = {"path": model_path, "bundle": bundle}
        logger.info(
            "[ML] Bundle loaded | features: %s", bundle["feature_columns"]
        )
        return bundle

    except Exception as exc:
        logger.warning("[ML] Load failed (%s) — using rule-based fallback.", exc)
        return None


def _rule_based_duration(site: dict) -> float:
    """
    Rule-based duration with continuous variation (Fix 3).

    Result varies across sites via:
      - size_score  (0-10): larger site = longer
      - crowd_score (0-10): busier site = slightly more time
      - ticket_price: higher price usually = more elaborate site

    Per-category (min, base, max) ensures realistic bounds:
      Museum:   90-150 min    Pyramid: 120-180 min
      Mosque:   30-75 min     Gate:     20-50 min
    """
    CAT_RANGE = {
        # category:            (min, base, max)
        "Museum":              ( 90, 110, 150),
        "Pyramid":             (120, 140, 180),
        "Fortress":            ( 50,  65,  95),
        "Citadel":             ( 50,  70,  95),
        "Mosque":              ( 30,  50,  75),
        "Church":              ( 30,  45,  70),
        "Synagogue":           ( 25,  40,  65),
        "Palace":              ( 50,  65,  95),
        "Temple":              ( 50,  65,  95),
        "Tomb":                ( 30,  48,  75),
        "Gate":                ( 20,  32,  50),
        "Bazaar":              ( 45,  65,  95),
        "Monument":            ( 20,  32,  50),
        "Cemetery":            ( 20,  32,  55),
        "Necropolis":          ( 45,  65,  95),
        "Archaeological Site": ( 50,  68,  95),
        "Historical Site":     ( 45,  60,  95),
    }
    cat = site.get("category", "Historical Site")
    low, base, high = CAT_RANGE.get(cat, (30, 55, 95))

    size   = float(site.get("size_score")  or 5)
    crowd  = float(site.get("crowd_score") or 5)
    price  = float(site.get("ticket_price") or 0)

    # Continuous variation: size dominant, crowd secondary, price minor
    dur = base + (size - 5) * 4.0 + (crowd - 5) * 1.5 + (price / 100) * 3.0
    return float(np.clip(dur, low, high))


def batch_predict_durations(candidates: list, model_path: str = MODEL_FILE) -> dict:
    """
    Predict visit durations for all candidates in one model.predict() call.

    Returns: {site_name: duration_minutes}

    Upgrade 6 additions:
      - Validates feature alignment with assertion before predict
      - Computes per-site prediction uncertainty (std across RF trees)
      - Logs prediction statistics
      - Stores uncertainty in returned dict as {site_name + '_std': ...} side dict
        (accessible via batch_predict_durations.uncertainties after call)
    """
    batch_predict_durations.uncertainties = {}   # reset per call

    if not candidates:
        return {}

    bundle = _load_model_bundle(model_path)

    if bundle is None:
        logger.warning("[ML] FALLBACK: rule-based for all %d sites.", len(candidates))
        return {s["name"]: _rule_based_duration(s) for s in candidates}

    try:
        import pandas as pd

        model           = bundle["model"]
        encoders        = bundle["encoders"]
        feature_columns = bundle["feature_columns"]
        enc_cat         = encoders["category"]
        enc_zone        = encoders["zone"]

        rows = []
        for s in candidates:
            cat  = s.get("category", "Historical Site")
            zone = s.get("zone", "Islamic Cairo")

            cat_enc  = (int(enc_cat.transform([cat])[0])
                        if cat in enc_cat.classes_
                        else int(len(enc_cat.classes_) // 2))
            zone_enc = (int(enc_zone.transform([zone])[0])
                        if zone in enc_zone.classes_
                        else int(len(enc_zone.classes_) // 2))

            rows.append({
                "size_score":   s.get("size_score")   or 5,
                "crowd_score":  s.get("crowd_score")  or 5,
                "category_enc": cat_enc,
                "ticket_price": s.get("ticket_price") or 0,
                "zone_enc":     zone_enc,
            })

        df = pd.DataFrame(rows)

        # Upgrade 6 — Strict feature alignment validation
        missing = [c for c in feature_columns if c not in df.columns]
        if missing:
            logger.error(
                "[ML] Feature mismatch — missing: %s. "
                "Re-run train_duration_model.py. Falling back.", missing
            )
            return {s["name"]: _rule_based_duration(s) for s in candidates}
        df = df[feature_columns]   # reindex to exact training order

        # Raw ML predictions
        raw_preds = model.predict(df)

        # Clip to per-category realistic ranges (Fix 5)
        preds = np.array([_rule_based_clamp(s, float(p))
                          for s, p in zip(candidates, raw_preds)])

        # Uncertainty: std across individual RF trees
        try:
            tree_preds = np.array([t.predict(df.values) for t in model.estimators_])
            std_preds  = np.clip(tree_preds.std(axis=0), 0, 60)
            uncertainties = {s["name"]: round(float(u), 1)
                             for s, u in zip(candidates, std_preds)}
        except Exception:
            uncertainties = {s["name"]: 0.0 for s in candidates}

        batch_predict_durations.uncertainties = uncertainties

        logger.info(
            "[ML] %d sites | duration mean=%.0f min=%.0f max=%.0f",
            len(candidates), preds.mean(), preds.min(), preds.max()
        )
        return {s["name"]: round(float(p), 1) for s, p in zip(candidates, preds)}

    except Exception as exc:
        logger.warning("[ML] FALLBACK triggered: %s", exc)
        return {s["name"]: _rule_based_duration(s) for s in candidates}


batch_predict_durations.uncertainties = {}


def predict_visit_duration(site: dict, model_path: str = MODEL_FILE) -> float:
    """Single-site prediction wrapper (uses batch internally)."""
    result = batch_predict_durations([site], model_path)
    return result.get(site["name"], _rule_based_duration(site))

def _rule_based_clamp(site: dict, raw_minutes: float) -> float:
    """Clamp a raw ML prediction into the per-category realistic range."""
    CAT_RANGE = {
        "Museum":             (60,  120),
        "Pyramid":            (90,  150),
        "Fortress":           (45,   90),
        "Citadel":            (45,   90),
        "Mosque":             (30,   75),
        "Church":             (30,   70),
        "Synagogue":          (20,   60),
        "Palace":             (45,   90),
        "Temple":             (45,   90),
        "Tomb":               (30,   75),
        "Gate":               (20,   50),
        "Bazaar":             (45,   90),
        "Monument":           (20,   50),
        "Cemetery":           (20,   55),
        "Necropolis":         (45,   90),
        "Archaeological Site":(45,   90),
        "Historical Site":    (45,   90),
    }
    cat = site.get("category", "Historical Site")
    lo, hi = CAT_RANGE.get(cat, (30, 90))
    return float(np.clip(raw_minutes, lo, hi))


def category_entropy(cat_counts: dict) -> float:
    """
    Shannon entropy (bits) of the category distribution in a route.
    Upgrade 3: higher entropy → more diverse → we reward it.
    H = 0 (single category)  →  ~2.32 (5 equal categories, log2(5))
    """
    total = sum(cat_counts.values())
    if total == 0:
        return 0.0
    entropy = 0.0
    for count in cat_counts.values():
        p = count / total
        if p > 0:
            entropy -= p * math.log2(p)
    return entropy


def log_redundancy_penalty(cat_counts: dict, zone_counts: dict,
                            site_cat: str, site_zone: str) -> float:
    """
    Upgrade 2 — log-based diminishing-returns redundancy penalty.
    Returns the incremental penalty for adding this site to a route
    that already has cat_counts / zone_counts.

    log1p(0) = 0.00  → first visit: no penalty
    log1p(1) = 0.69  → second visit: mild penalty
    log1p(2) = 1.10  → third visit: stronger
    log1p(3) = 1.39  → fourth visit: diminishing further
    """
    cfg      = DIVERSITY_CONFIG
    cat_rep  = cat_counts.get(site_cat, 0)
    zone_rep = zone_counts.get(site_zone, 0)
    return (cfg["category_alpha"] * math.log1p(cat_rep)
            + cfg["zone_alpha"]   * math.log1p(zone_rep))


def site_preference_score(site: dict, preferred: list) -> float:
    """
    How well does this site match the user's preferred categories?
    Returns 0.0 .. 1.0
    """
    RELATED = {
        "pyramid":          {"pyramid", "necropolis", "tomb", "monument", "temple"},
        "mosque":           {"mosque", "historical site"},
        "museum":           {"museum", "palace"},
        "church":           {"church", "synagogue"},
        "fortress":         {"fortress", "citadel", "gate"},
        "historical site":  {"historical site", "mosque", "church", "gate"},
        "bazaar":           {"bazaar", "historical site"},
    }
    if not preferred:
        return 0.50   # neutral when user has no preference

    cat = site.get("category", "").lower()
    for pref in [p.lower() for p in preferred]:
        if pref == cat:
            return 1.0
        related = RELATED.get(pref, {pref})
        if cat in related:
            return 0.55
    return 0.0


def site_engagement_score(duration_min: float) -> float:
    """
    Normalize predicted visit duration to [0, 1] as a proxy for site engagement value.
    Capped at 120 min (a 2-hour visit is the maximum we reward; beyond is just time cost).
    """
    return min(1.0, duration_min / 120.0)


def delta_objective(
    site: dict,
    preferred: list,
    duration_min: float,
    travel_min: float,
    budget_total: float,
    cat_counts: dict,
    zone_counts: dict,
    uncertainty: float = 0.0,
    walk_cfg_km: float = 1.88,
    last_zone: str = "",
    ticket_budget_egp: float | None = None,
    ticket_spent_egp: float = 0.0,
) -> float:
    """
    Spatially-realistic scoring function (v4).

    Fix 4: exponential travel penalty — score = site_value * exp(-travel_min/20)
           This sharply favours nearby sites over high-scoring distant ones.

    Fix 6 (alias): sites with alias_match=True receive 0.6x relevance.

    Fix 6 (zone): +0.10 bonus when next site is in the same zone/district.

    Components (all in [0,1]):
      w1 * relevance    — category match (full or 0.6x alias)
      w2 * diversity    — new-category bonus
      zone_bonus        — same-district reward
      w4 * budget_use   — engagement / time cost ratio
      penalty           — capped at 40% of positive
      * exp(-travel/20) — exponential proximity decay (Fix 4)
    """
    w   = OBJECTIVE_WEIGHTS
    cat = site.get("category", "")

    # ── Positive components (all in [0,1]) ──────────────────────────
    # w1: relevance — full score for exact match, 0.6x for alias match
    relevance = site_preference_score(site, preferred)
    if site.get("alias_match"):
        relevance *= 0.6

    # w2: diversity — 1.0 on first visit of a category, decays
    cat_seen = cat_counts.get(cat, 0)
    diversity = 1.0 / (1.0 + cat_seen)

    # Fix 6 — zone cluster bonus: +0.10 for same-district stop
    zone = site.get("zone", "")
    zone_bonus = 0.10 if (last_zone and zone and last_zone == zone) else 0.0

    # w4: budget value — engagement relative to time cost
    visit_value  = min(1.0, duration_min / 120.0)
    time_cost    = travel_min / max(duration_min, 1.0)
    budget_value = max(0.0, visit_value * (1.0 - min(1.0, time_cost)))

    positive = (
        w["w1_preference"] * relevance
      + w["w3_diversity"]  * diversity
      + zone_bonus
      + w["w2_engagement"] * budget_value
    )

    # ── Penalty (capped at 40% of positive score) ───────────────────
    zone_rep   = zone_counts.get(zone, 0)
    redundancy = min(1.0, (0.3 * math.log1p(cat_seen)
                          + 0.2 * math.log1p(zone_rep)))
    uncert_pen = min(1.0, max(0.0, (uncertainty - 20.0) / 60.0)) * 0.05
    raw_penalty = w["w5_redundancy"] * redundancy + uncert_pen
    penalty     = min(raw_penalty, 0.4 * positive)

    site_value = max(0.0, positive - penalty)

    # ── Cost-awareness (only when financial budget is active) ────────────────
    if ticket_budget_egp is not None and ticket_budget_egp > 0:
        site_cost = float(site.get("ticket_price") or 0)
        remaining  = max(0.0, ticket_budget_egp - ticket_spent_egp)

        if remaining > 0:
            # Penalise expensive sites relative to remaining budget
            cost_penalty = min(0.5, (site_cost / ticket_budget_egp) * 0.5)
            # Reward sites that give long visits per unit cost (value efficiency)
            if site_cost > 0:
                value_score = min(0.2, (duration_min / max(1.0, site_cost)) * 2.0)
            else:
                value_score = 0.10   # free sites get a small bonus
            site_value = max(0.0, site_value - cost_penalty + value_score)
        else:
            # No remaining budget — free sites only (ticket_price == 0)
            if float(site.get("ticket_price") or 0) > 0:
                return 0.0   # signal to caller: skip this site

    # Fix 4: exponential travel decay — heavily penalises far sites
    decay = math.exp(-travel_min / 20.0)
    J = site_value * decay

    # Normalise by sqrt(duration) for competitive shorter stops
    J = J / max(1.0, math.sqrt(duration_min))
    return round(J, 6)


# ============================================================
# SECTION 5 — Site Filter (Fix 2 from previous session)
# ============================================================

def filter_sites(sites: list, profile: dict) -> dict:
    """
    Robust filter with progressive fallbacks — never returns empty when data exists.

    Stages:
      1. Progressive radius expansion [base → 2x → 4x → 8x] until MIN_LOCAL_SITES found.
      2. Category filter with alias expansion.
         Fix 1 (soft category): if still 0 after aliases, retry with ALL categories.
      4. Nearest-1 guarantee: if everything fails, return the single closest site.
      5. Distance-priority sort on final pool.
      6. Low-density logging.
    """
    start_lat   = profile.get("start_lat", DEFAULT_LAT)
    start_lon   = profile.get("start_lon", DEFAULT_LON)
    tolerance   = profile.get("walking_tolerance", "medium")
    base_radius = WALKING_TOLERANCE_RADIUS.get(tolerance, 12.5)
    visited     = {v.lower() for v in profile.get("visited_sites", [])}
    preferred   = profile.get("preferred_categories", [])

    # Build alias-expanded preferred set
    pref_lower  = {c.lower() for c in preferred}
    alias_lower: set[str] = set()
    if preferred:
        for cat in preferred:
            for alias in CATEGORY_ALIASES.get(cat, []):
                alias_lower.add(alias.lower())
        alias_lower -= pref_lower

    # ── Stage 1: Progressive radius — [base, 2x, 4x, 8x] ───────────────────
    not_visited = [s for s in sites if s.get("name", "").lower() not in visited]

    stage1: list = []
    radius = base_radius
    for multiplier in (1, 2, 4, 8):
        radius = base_radius * multiplier
        stage1 = [
            s for s in not_visited
            if haversine(start_lat, start_lon, *safe_coords(s)) <= radius
        ]
        if len(stage1) >= MIN_LOCAL_SITES:
            break

    low_density = len(stage1) < MIN_LOCAL_SITES
    logger.info(
        "[Filter] Radius %.1fkm (base=%.1fkm), visited=%d -> %d / %d sites%s",
        radius, base_radius, len(visited), len(stage1), len(sites),
        " [LOW DENSITY]" if low_density else "",
    )
    if low_density:
        logger.warning(
            "[Filter] Low density area — limited nearby attractions (only %d sites within %.0fkm).",
            len(stage1), radius,
        )

    # ── Stage 2: Category filter + alias + soft fallback ────────────────────
    if stage1 and preferred:
        exact_hits = [s for s in stage1 if s.get("category", "").lower() in pref_lower]
        alias_hits = [
            s for s in stage1
            if s.get("category", "").lower() in alias_lower
            and s.get("category", "").lower() not in pref_lower
        ]
        for s in alias_hits:
            s["alias_match"] = True
        for s in exact_hits:
            s.pop("alias_match", None)

        stage2 = exact_hits + alias_hits
        logger.info(
            "[Filter] Category %s -> %d exact + %d alias / %d sites",
            preferred, len(exact_hits), len(alias_hits), len(stage1),
        )

        # Fix 1: Soft category fallback — use ALL nearby sites if category yields 0
        if not stage2:
            available = sorted({s.get("category", "?") for s in stage1[:30]})
            logger.warning(
                "[Filter] No match for %s (or aliases). Relaxing category filter — "
                "using all %d nearby sites. Available: %s",
                preferred, len(stage1), ", ".join(available),
            )
            # Clear any stale alias tags
            for s in stage1:
                s.pop("alias_match", None)
            stage2 = stage1   # soft fallback: all categories
        final = stage2
    elif stage1:
        final = stage1
    else:
        # ── Fix 4: Nearest-1 guarantee — return closest site regardless ──────
        if not_visited:
            closest = min(
                not_visited,
                key=lambda s: haversine(start_lat, start_lon, *safe_coords(s)),
            )
            dist_km = haversine(start_lat, start_lon, *safe_coords(closest))
            logger.warning(
                "[Filter] No sites within any radius. Returning nearest site: "
                "'%s' (%.1f km). LOW DENSITY AREA.",
                closest["name"], dist_km,
            )
            return {"candidates": [closest], "status": "nearest_only", "message":
                    f"Low density area — only nearest site returned ({closest['name']}, {dist_km:.0f} km away)."}
        return {
            "candidates": [],
            "status": "empty_radius",
            "message": "No sites in dataset at all.",
        }

    # ── Fix 5: Sort by distance — always prefer closer sites ────────────────
    final.sort(key=lambda s: haversine(start_lat, start_lon, *safe_coords(s)))

    logger.info("[Filter] Final candidate pool: %d sites", len(final))
    status = "low_density" if low_density else "ok"
    return {"candidates": final, "status": status, "message": ""}




# ============================================================
# SECTION 6 — Beam Search Route Builder (Upgrade 1)
# ============================================================

def beam_search_route(
    candidates: list,
    profile: dict,
    duration_cache: dict,
    uncertainties: dict,
) -> list:
    """
    Beam Search route builder — replaces the old greedy selector.

    Maintains BEAM_WIDTH=5 partial routes simultaneously, expanding each
    by all feasible next stops at every step, then pruning back to top K.
    This prevents premature commitment to sub-optimal early choices.

    Objective (incremental per stop):
        delta_J = w1*pref + w2*engagement - w4*travel - w5*log_redundancy

    At the end, the global diversity entropy (w3) is computed over the
    full route and added to the final beam score.

    Beam state (one dict per beam):
        stops         : list of stop dicts already selected
        time_used     : cumulative minutes used (travel + visit)
        total_km      : cumulative walking distance
        cum_score     : sum of delta_J for all stops in beam
        cat_counts    : {category: count} for diversity tracking
        zone_counts   : {zone: count}
        visited       : set of selected site names
        last_lat/lon  : GPS position of last selected stop
    """
    budget        = profile.get("available_hours", 6) * 60
    start_lat     = profile.get("start_lat", DEFAULT_LAT)
    start_lon     = profile.get("start_lon", DEFAULT_LON)
    preferred     = profile.get("preferred_categories", [])
    tolerance     = profile.get("walking_tolerance", "medium")
    walk_cfg      = WALKING_CONFIG.get(tolerance, WALKING_CONFIG["medium"])
    max_leg_km    = walk_cfg["max_leg_km"]
    max_leg_min   = MAX_LEG_TRAVEL_MIN.get(tolerance, 25)   # Fix 1 hard cap
    max_walk_km   = MAX_TOTAL_WALK_KM.get(tolerance, 5.0)   # Fix 5 route cap
    start_time    = profile.get("start_time", "09:00")

    # Financial budget (None = no constraint)
    ticket_budget_egp = resolve_ticket_budget(profile)
    if ticket_budget_egp is not None:
        logger.info(
            "[Budget] Financial mode ON: %.0f EGP (converted from %s %s).",
            ticket_budget_egp,
            profile.get("budget_amount", 0),
            profile.get("currency", "EGP"),
        )

    # Initialise a single empty beam
    empty_beam = {
        "stops":         [],
        "time_used":     0.0,
        "total_km":      0.0,
        "cum_score":     0.0,
        "cat_counts":    {},
        "zone_counts":   {},
        "visited":       set(),
        "last_lat":      start_lat,
        "last_lon":      start_lon,
        "ticket_spent":  0.0,
    }
    beams = [empty_beam]
    step  = 0

    while True:
        step        += 1
        new_beams    = []
        any_expanded = False

        for beam in beams:
            budget_left = budget - beam["time_used"]

            for site in candidates:
                name = site["name"]
                if name in beam["visited"]:
                    continue

                site_lat, site_lon = safe_coords(site)
                leg_km    = haversine(beam["last_lat"], beam["last_lon"], site_lat, site_lon)

                # Hard constraint: per-leg distance cap
                if leg_km > max_leg_km:
                    continue

                travel_min = travel_time_minutes(
                    beam["last_lat"], beam["last_lon"],
                    site_lat, site_lon,
                    zone1=beam.get("last_zone", ""),
                    zone2=site.get("zone", ""),
                    tolerance=tolerance,
                )
                visit_min  = duration_cache.get(name, _rule_based_duration(site))

                # Fix 1: Hard travel-time cap — REJECT, do not just penalise
                if travel_min > max_leg_min:
                    continue

                if travel_min + visit_min > budget_left:
                    continue   # doesn't fit in remaining time

                # Fix 5: Route walk-km cap — reject if this leg would bust the limit
                if beam["total_km"] + leg_km > max_walk_km:
                    continue

                # Financial hard constraint: reject if site cost exceeds remaining ticket budget
                site_ticket = float(site.get("ticket_price") or 0)
                if ticket_budget_egp is not None:
                    if beam["ticket_spent"] + site_ticket > ticket_budget_egp:
                        continue   # would bust the ticket budget

                cat  = site.get("category", "Unknown")
                zone = site.get("zone",     "Unknown")

                # Soft cap on same-category repetition
                cat_count = beam["cat_counts"].get(cat, 0)
                over_cap_penalty = 0.5 if cat_count >= MAX_SAME_CATEGORY else 0.0

                # Compute incremental objective contribution (with cost awareness)
                uncert = uncertainties.get(name, 0.0)
                d_j    = delta_objective(
                    site, preferred, visit_min, travel_min, budget,
                    beam["cat_counts"], beam["zone_counts"], uncert,
                    walk_cfg_km=max_leg_km,
                    last_zone=beam.get("last_zone", ""),
                    ticket_budget_egp=ticket_budget_egp,
                    ticket_spent_egp=beam["ticket_spent"],
                ) - over_cap_penalty
                if d_j <= 0 and ticket_budget_egp is not None:
                    continue   # cost-aware scoring signalled skip

                # Build child beam (shallow-copy mutable structures)
                child = {
                    "stops":         beam["stops"],   # list extended below
                    "time_used":     beam["time_used"] + travel_min + visit_min,
                    "total_km":      beam["total_km"] + leg_km,
                    "cum_score":     beam["cum_score"] + d_j,
                    "cat_counts":    dict(beam["cat_counts"]),
                    "zone_counts":   dict(beam["zone_counts"]),
                    "visited":       beam["visited"] | {name},
                    "last_lat":      site_lat,
                    "last_lon":      site_lon,
                    "last_zone":     site.get("zone", ""),
                    "ticket_spent":  beam["ticket_spent"] + site_ticket,
                }

                child["cat_counts"][cat]   = child["cat_counts"].get(cat, 0) + 1
                child["zone_counts"][zone] = child["zone_counts"].get(zone, 0) + 1

                # Build stop record
                arrival = format_time(start_time, child["time_used"] - visit_min)
                stop_record = {
                    "name":                       name,
                    "category":                   cat,
                    "zone":                       zone,
                    "ticket_price_egp":           site.get("ticket_price") or 0,
                    "size_score":                 site.get("size_score", 5),
                    "crowd_score":                site.get("crowd_score", 5),
                    "predicted_duration_minutes": round(visit_min, 1),
                    "prediction_std_minutes":     round(uncert, 1),
                    "travel_time_minutes":        round(travel_min, 1),
                    "arrival_time":               arrival,
                    "cumulative_time_minutes":    round(child["time_used"], 1),
                    "leg_km":                     round(leg_km, 2),
                    "objective_delta":            round(d_j, 4),
                    "latitude":                   site_lat,
                    "longitude":                  site_lon,
                    "description":                site.get("description", "")[:200],
                }
                child["stops"] = beam["stops"] + [stop_record]
                new_beams.append(child)
                any_expanded = True

        if not any_expanded:
            break   # no beam could be expanded further

        # Prune: keep top BEAM_WIDTH beams by cumulative score
        new_beams.sort(key=lambda b: -b["cum_score"])
        beams = new_beams[:BEAM_WIDTH]

        logger.info(
            "[Beam] Step %d: %d expansions → keeping top %d | "
            "best cum_score=%.3f, stops=%d, time=%.0f/%.0f min",
            step, len(new_beams), len(beams),
            beams[0]["cum_score"], len(beams[0]["stops"]),
            beams[0]["time_used"], budget
        )

    if not beams or not beams[0]["stops"]:
        return []

    # Select best beam — add global diversity entropy bonus
    for beam in beams:
        H = category_entropy(beam["cat_counts"])
        # Add w3 * H to total score (global term)
        beam["final_score"] = beam["cum_score"] + OBJECTIVE_WEIGHTS["w3_diversity"] * H

    beams.sort(key=lambda b: -b.get("final_score", b["cum_score"]))
    best = beams[0]

    # Annotate stops with final objective score (for output)
    final_J  = best.get("final_score", best["cum_score"])
    H        = category_entropy(best["cat_counts"])
    for stop in best["stops"]:
        stop["objective_score"] = round(final_J / max(len(best["stops"]), 1), 4)

    logger.info(
        "[Beam] Best route: %d stops | time=%.0f/%.0f min | "
        "J=%.3f | H=%.2f bits",
        len(best["stops"]), best["time_used"], profile.get("available_hours", 6) * 60,
        final_J, H
    )

    return best["stops"]


# ============================================================
# SECTION 7 — Budget Fill Phase (Upgrade 3)
# ============================================================

def fill_remaining_budget(
    current_stops: list,
    candidates: list,
    duration_cache: dict,
    profile: dict,
) -> list:
    """
    Aggressive fill phase (Fix 4) — targets 85–95% budget usage.

    3-pass strategy (each pass relaxes constraints):
      Pass 0: exact prefs, within 5 km, 20–90 min duration
      Pass 1: any category, within 10 km, 20–90 min duration
      Pass 2: any category, any distance (use max_leg_km), any duration ≥ 20 min

    Each pass greedily adds the best-fitting stop by (pref_score + 0.1) / duration.
    Travel >= visit guard is removed in fill phase (we want to use the time).
    """
    if not current_stops:
        return current_stops

    budget     = profile.get("available_hours", 6) * 60
    target_min = budget * BUDGET_TARGET_PCT
    time_used  = current_stops[-1]["cumulative_time_minutes"]

    if time_used >= target_min:
        return current_stops

    remaining_min = budget - time_used
    if remaining_min < 20:   # nothing useful fits in < 20 min
        return current_stops

    tolerance  = profile.get("walking_tolerance", "medium")
    walk_cfg   = WALKING_CONFIG.get(tolerance, WALKING_CONFIG["medium"])
    max_leg_km = walk_cfg["max_leg_km"]
    start_time = profile.get("start_time", "09:00")
    preferred  = profile.get("preferred_categories", [])
    selected   = {s["name"] for s in current_stops}
    last_lat   = current_stops[-1]["latitude"]
    last_lon   = current_stops[-1]["longitude"]

    # Financial budget state (track spent so far)
    ticket_budget_egp = resolve_ticket_budget(profile)
    ticket_spent = sum((s.get("ticket_price_egp") or 0) for s in current_stops)


    def _score(s, relax):
        dur  = max(1.0, duration_cache.get(s["name"], _rule_based_duration(s)))
        pref = site_preference_score(s, preferred) if not relax else 0.4
        return (pref + 0.1) / dur   # prefer high-pref short stops

    # Pass configs: (relax_pref, max_km, min_dur, max_dur)
    PASSES = [
        (False,  FILL_MAX_LEG_KM,  20,  90),
        (True,   10.0,              20,  90),
        (True,   max_leg_km,        20, 150),
    ]

    attempts = 0
    fills    = 0

    for pass_num, (relax, km_limit, min_dur, max_dur) in enumerate(PASSES):
        if time_used >= budget * BUDGET_TARGET_PCT:
            break

        pool = []
        for s in candidates:
            if s["name"] in selected:
                continue
            dur = duration_cache.get(s["name"], _rule_based_duration(s))
            if not (min_dur <= dur <= max_dur):
                continue
            site_lat, site_lon = safe_coords(s)
            if haversine(last_lat, last_lon, site_lat, site_lon) > km_limit:
                continue
            # Financial budget: skip if site would bust remaining ticket budget
            if ticket_budget_egp is not None:
                site_cost = float(s.get("ticket_price") or 0)
                if ticket_spent + site_cost > ticket_budget_egp:
                    continue
            pool.append(s)

        pool.sort(key=lambda s: _score(s, relax), reverse=True)

        for site in pool:
            remaining_min = budget - time_used
            if remaining_min < 20:
                break

            site_lat, site_lon = safe_coords(site)
            leg_km     = haversine(last_lat, last_lon, site_lat, site_lon)
            travel_min = travel_time_minutes(
                last_lat, last_lon, site_lat, site_lon,
                zone1=current_stops[-1].get("zone", "") if current_stops else "",
                zone2=site.get("zone", ""),
                tolerance=tolerance,
            )
            visit_min  = duration_cache.get(site["name"], _rule_based_duration(site))
            attempts  += 1

            if travel_min + visit_min > remaining_min:
                continue

            time_used    += travel_min + visit_min
            arrival       = format_time(start_time, time_used - visit_min)

            site_ticket = float(site.get("ticket_price") or 0)
            current_stops.append({
                "name":                       site["name"],
                "category":                   site.get("category", "?"),
                "zone":                       site.get("zone", "?"),
                "ticket_price_egp":           site_ticket,
                "size_score":                 site.get("size_score", 5),
                "crowd_score":                site.get("crowd_score", 5),
                "predicted_duration_minutes": round(visit_min, 1),
                "prediction_std_minutes":     0.0,
                "travel_time_minutes":        round(travel_min, 1),
                "arrival_time":               arrival,
                "cumulative_time_minutes":    round(time_used, 1),
                "leg_km":                     round(leg_km, 2),
                "objective_delta":            0.0,
                "objective_score":            0.0,
                "latitude":                   site_lat,
                "longitude":                  site_lon,
                "description":                site.get("description", "")[:200],
                "fill_phase":                 True,
                "fill_pass":                  pass_num,
            })
            ticket_spent += site_ticket
            selected.add(site["name"])
            last_lat, last_lon = site_lat, site_lon
            fills += 1

    fill_pct = round(100 * time_used / budget, 1)
    logger.info(
        "[FillPhase] attempts=%d added_stops=%d final_budget_usage=%.1f%%",
        attempts, fills, fill_pct,
    )
    if fill_pct < 80:
        logger.info(
            "[FillPhase] Still < 80%% after %d passes — dataset may lack "
            "short-duration sites near this location", len(PASSES)
        )
    return current_stops



# ============================================================
# SECTION 8 — Tight Budget UX (Upgrade 4)
# ============================================================

def get_tight_budget_suggestions(
    candidates: list,
    duration_cache: dict,
    profile: dict,
) -> dict:
    """
    Upgrade 4 — When 0 stops fit, return top-3 shortest alternatives
    with a helpful message. Does NOT produce an itinerary — advisory only.
    """
    tolerance  = profile.get("walking_tolerance", "medium")
    walk_cfg   = WALKING_CONFIG.get(tolerance, WALKING_CONFIG["medium"])
    max_leg_km = walk_cfg["max_leg_km"]
    start_lat  = profile.get("start_lat", DEFAULT_LAT)
    start_lon  = profile.get("start_lon", DEFAULT_LON)

    reachable = []
    for site in candidates:
        site_lat, site_lon = safe_coords(site)
        leg_km = haversine(start_lat, start_lon, site_lat, site_lon)
        if leg_km <= max_leg_km:
            dur = duration_cache.get(site["name"], _rule_based_duration(site))
            reachable.append((dur, site))

    reachable.sort(key=lambda x: x[0])
    top3 = reachable[:3]

    if not top3:
        msg = ("No sites found within walking distance. "
               "Try setting walking_tolerance='high'.")
        return {"status": "no_results", "suggestions": [], "message": msg}

    min_dur = top3[0][0]
    budget  = profile.get("available_hours", 6) * 60
    msg = (
        f"Your budget of {budget:.0f} min is tight — the shortest nearby site "
        f"takes ~{min_dur:.0f} min. "
        "Consider: (1) increasing available_hours, "
        "or (2) setting walking_tolerance='high' to access more sites."
    )

    return {
        "status":      "tight_budget",
        "suggestions": [
            {
                "name":     s["name"],
                "category": s.get("category", "?"),
                "zone":     s.get("zone", "?"),
                "duration_minutes": round(d, 1),
                "ticket_egp":       s.get("ticket_price") or 0,
            }
            for d, s in top3
        ],
        "message": msg,
    }


# ============================================================
# SECTION 9 — Explainability Logging (Upgrade 8)
# ============================================================

def _log_route_explainability(
    itinerary: list,
    profile: dict,
    final_J: float = 0.0,
) -> dict:
    """
    Log and return a structured explainability summary for the generated route.
    """
    if not itinerary:
        return {}

    budget       = profile.get("available_hours", 6) * 60
    time_used    = itinerary[-1]["cumulative_time_minutes"]
    fill_pct     = 100.0 * time_used / budget if budget > 0 else 0
    total_km     = sum(s["leg_km"] for s in itinerary)
    total_ticket = sum((s["ticket_price_egp"] or 0) for s in itinerary)

    from collections import Counter
    cat_counts  = dict(Counter(s["category"] for s in itinerary))
    zone_counts = dict(Counter(s["zone"] for s in itinerary))
    H           = category_entropy(cat_counts)

    summary = {
        "stops":            len(itinerary),
        "budget_used_min":  round(time_used, 1),
        "budget_total_min": round(budget, 1),
        "budget_fill_pct":  round(fill_pct, 1),
        "total_walk_km":    round(total_km, 2),
        "total_ticket_egp": total_ticket,
        "categories":       cat_counts,
        "zones":            zone_counts,
        "unique_categories":len(cat_counts),
        "diversity_entropy_bits": round(H, 3),
        "objective_J":      round(final_J, 4),
    }

    logger.info("=" * 55)
    logger.info("[Explainability] Route Summary")
    logger.info("=" * 55)
    logger.info("  Stops selected    : %d", summary["stops"])
    logger.info("  Budget used       : %.0f / %.0f min  (%.1f%%)",
                time_used, budget, fill_pct)
    logger.info("  Total walk dist   : %.2f km", total_km)
    logger.info("  Total ticket cost : %d EGP", total_ticket)
    logger.info("  Categories        : %s",
                " | ".join(f"{k}={v}" for k, v in sorted(cat_counts.items())))
    logger.info("  Unique categories : %d", summary["unique_categories"])
    logger.info("  Diversity H       : %.2f bits", H)
    logger.info("  Objective J       : %.4f", final_J)
    logger.info("=" * 55)

    return summary


# ============================================================
# SECTION 10 — KMeans Zone Clustering (Bonus)
# ============================================================

def suggest_zone_clusters(sites: list, n_clusters: int = 4) -> dict:
    """KMeans clustering on GPS coordinates to find natural geographic zones."""
    geo_sites = [s for s in sites if s.get("latitude") and s.get("longitude")]
    if len(geo_sites) < n_clusters:
        return {}
    try:
        from sklearn.cluster import KMeans
        coords  = np.array([[s["latitude"], s["longitude"]] for s in geo_sites])
        km      = KMeans(n_clusters=n_clusters, random_state=RANDOM_SEED, n_init=10)
        labels  = km.fit_predict(coords)
        clusters: dict = {}
        for i, site in enumerate(geo_sites):
            clusters.setdefault(int(labels[i]), []).append(site["name"])
        zone_labels = {}
        for cid, names in clusters.items():
            zone_counts: dict = {}
            for name in names:
                z = next((x.get("zone", "?") for x in geo_sites if x["name"] == name), "?")
                zone_counts[z] = zone_counts.get(z, 0) + 1
            zone_labels[cid] = max(zone_counts, key=zone_counts.get)
        return {zone_labels.get(cid, f"Cluster {cid}"): names
                for cid, names in clusters.items()}
    except ImportError:
        logger.warning("scikit-learn not installed — skipping KMeans.")
        return {}


# ============================================================
# SECTION 11 — Main Public API
# ============================================================

def generate_trip(
    user_profile: dict,
    sites_file: str = None,
    model_path: str = MODEL_FILE,
    city_resolver=None,
) -> list:
    """
    Generate a personalized 1-day itinerary for any Egyptian city.

    Args:
        user_profile (dict):
            start_lat            float   GPS latitude
            start_lon            float   GPS longitude
            city                 str     City name (optional, e.g. 'Luxor')
            available_hours      float   Time budget in hours
            preferred_categories list    e.g. ["Mosque", "Museum"]
            visited_sites        list    Names to skip
            walking_tolerance    str     "low" | "medium" | "high"
            start_time           str     "HH:MM"

        sites_file (str, optional):
            Direct path to a JSON sites file. When provided, skips city
            resolution (backward-compatible with the Cairo-only flow).

        city_resolver (CityResolver, optional):
            Pre-initialised CityResolver. Created on-demand if omitted.

    Returns:
        list of stop dicts — empty if no matching sites or budget too tight.
        Check generate_trip.last_suggestion for tight-budget advisory.
    """
    generate_trip.last_suggestion = None

    logger.info("=" * 60)
    logger.info("[KemetPath] Generating personalised itinerary")
    logger.info("=" * 60)
    logger.info("  Location   : (%.4f, %.4f)",
                user_profile.get("start_lat", 0), user_profile.get("start_lon", 0))
    logger.info("  City hint  : %s", user_profile.get("city", "(auto-detect)"))
    logger.info("  Available  : %sh  (%d min)",
                user_profile.get("available_hours"),
                int(user_profile.get("available_hours", 6) * 60))
    logger.info("  Preferences: %s", user_profile.get("preferred_categories"))
    logger.info("  Tolerance  : %s",  user_profile.get("walking_tolerance"))
    logger.info("  Start time : %s",  user_profile.get("start_time"))
    logger.info("  Visited    : %d",  len(user_profile.get("visited_sites", [])))

    # ----------------------------------------------------------------
    # Load raw sites — three paths:
    #   1. Direct file path provided (backward-compat / Cairo legacy)
    #   2. City resolver — resolve from city name or GPS coordinates
    #   3. Demo fallback (no data available)
    # ----------------------------------------------------------------
    raw_sites = None

    if sites_file and os.path.exists(sites_file):
        # Backward-compatible direct path (Cairo legacy / API override)
        with open(sites_file, "r", encoding="utf-8") as f:
            raw_sites = json.load(f)
        logger.info("[Data] Loaded %d sites from direct path '%s'.",
                    len(raw_sites), sites_file)
    else:
        # ── Multi-region geographic load ─────────────────────────────────────
        # Load sites from EVERY city dataset within MULTI_REGION_RADIUS_KM.
        # This replaces the old single-city resolve + iterative fallback loop.
        if city_resolver is None:
            try:
                from city_resolver import CityResolver
                city_resolver = CityResolver()
            except ImportError:
                city_resolver = None

        lat = user_profile.get("start_lat")
        lon = user_profile.get("start_lon")

        if city_resolver is not None and lat is not None and lon is not None:
            city_name = user_profile.get("city")
            # If an explicit city name was given, resolve it and load its
            # dataset first, then supplement with radius neighbours.
            if city_name:
                try:
                    city_info = city_resolver.resolve(city_name=city_name)
                    named_sites = city_resolver.load_sites(city_info)
                    logger.info("[Data] Named city '%s' — %d sites.", city_name, len(named_sites))
                    raw_sites = named_sites
                except Exception as exc:
                    logger.warning("[Data] Named city resolve failed (%s). Falling back to radius load.", exc)

            if raw_sites is None:
                raw_sites = city_resolver.load_all_sites_in_radius(
                    lat, lon,
                    radius_km=MULTI_REGION_RADIUS_KM,
                    min_cities=1,
                )
                logger.info(
                    "[Data] Multi-region load: %d sites from datasets within %.0f km of (%.4f, %.4f).",
                    len(raw_sites), MULTI_REGION_RADIUS_KM, lat, lon,
                )

        if raw_sites is None:
            # Final fallback: built-in demo sites (no external file needed)
            logger.warning("[Data] No dataset found — using built-in demo sites.")
            raw_sites = _get_demo_sites()

    # Phase A — Data cleaning
    sites = clean_site_data(raw_sites)

    # Phase B — Candidate filtering (distance-sorted, with soft category fallback)
    filter_result = filter_sites(sites, user_profile)
    candidates    = filter_result["candidates"]

    # Phase B-Fallback — if still empty, return the absolute nearest site
    if not candidates:
        visited_lower = {v.lower() for v in user_profile.get("visited_sites", [])}
        unvisited = [s for s in sites if s.get("name", "").lower() not in visited_lower]
        if unvisited:
            lat_fb = user_profile.get("start_lat", DEFAULT_LAT)
            lon_fb = user_profile.get("start_lon", DEFAULT_LON)
            nearest = min(unvisited, key=lambda s: haversine(lat_fb, lon_fb, *safe_coords(s)))
            dist_km = haversine(lat_fb, lon_fb, *safe_coords(nearest))
            logger.warning(
                "[KemetPath] No candidates after all filters — returning nearest site "
                "'%s' (%.0f km). Low density area.", nearest["name"], dist_km,
            )
            candidates = [nearest]
        else:
            logger.warning("[KemetPath] %s", filter_result.get("message", "No sites found."))
            return []


    # Phase C — Batch ML duration prediction (Fix 1 / Upgrade 6)
    logger.info("[ML] Predicting durations for %d candidates ...", len(candidates))
    duration_cache = batch_predict_durations(candidates, model_path)
    uncertainties  = batch_predict_durations.uncertainties

    # Phase D — Beam Search route building
    logger.info("[KemetPath] Running Beam Search (K=%d) ...", BEAM_WIDTH)
    itinerary = beam_search_route(candidates, user_profile, duration_cache, uncertainties)

    # Phase D.5 — Travel-time budget constraint (hard limit: ≤40% of day in transit)
    if itinerary:
        budget_min      = user_profile.get("available_hours", 6) * 60
        is_sparse       = filter_result.get("status") in ("low_density", "nearest_only")
        travel_cap_ratio = (
            MAX_TRAVEL_BUDGET_RATIO_SPARSE if is_sparse else MAX_TRAVEL_BUDGET_RATIO
        )
        total_travel = sum(s.get("travel_time_minutes", 0) for s in itinerary)
        if total_travel > budget_min * travel_cap_ratio:
            logger.warning(
                "[KemetPath] Total travel %.0f min exceeds %.0f%% of budget (%.0f min). "
                "Trimming itinerary.",
                total_travel, travel_cap_ratio * 100, budget_min * travel_cap_ratio,
            )
            # Trim stops from the back until travel is within cap
            while len(itinerary) > 1:
                running_travel = sum(s.get("travel_time_minutes", 0) for s in itinerary)
                if running_travel <= budget_min * travel_cap_ratio:
                    break
                itinerary.pop()
            # If even 1 stop violates cap, keep it anyway (no attractions ≠ better)


    # Phase E — Tight budget fallback (Upgrade 4) + single-stop last resort
    if not itinerary:
        suggestion = get_tight_budget_suggestions(candidates, duration_cache, user_profile)
        generate_trip.last_suggestion = suggestion

        # Last-resort: beam + tight-budget both failed (sites too far / budget too tight)
        # Return the single closest candidate ignoring distance constraints.
        lat  = user_profile.get("start_lat", DEFAULT_LAT)
        lon  = user_profile.get("start_lon", DEFAULT_LON)
        closest = min(candidates, key=lambda s: haversine(lat, lon, *safe_coords(s)))
        dist_km = haversine(lat, lon, *safe_coords(closest))
        visit_min = duration_cache.get(closest["name"], _rule_based_duration(closest))
        budget    = user_profile.get("available_hours", 6) * 60

        if visit_min <= budget:
            logger.warning(
                "[KemetPath] Low density — single-stop fallback: '%s' (%.0f km away).",
                closest["name"], dist_km,
            )
            start_time = user_profile.get("start_time", "09:00")
            travel_min = travel_time_minutes(
                lat, lon, *safe_coords(closest),
                tolerance=user_profile.get("walking_tolerance", "medium"),
            )
            arrival = format_time(start_time, travel_min)
            cat  = closest.get("category", "?")
            zone = closest.get("zone", "?")
            stop = {
                "name":                       closest["name"],
                "category":                   cat,
                "zone":                       zone,
                "ticket_price_egp":           closest.get("ticket_price") or 0,
                "size_score":                 closest.get("size_score", 5),
                "crowd_score":                closest.get("crowd_score", 5),
                "predicted_duration_minutes": round(visit_min, 1),
                "prediction_std_minutes":     0.0,
                "travel_time_minutes":        round(travel_min, 1),
                "arrival_time":               arrival,
                "cumulative_time_minutes":    round(travel_min + visit_min, 1),
                "leg_km":                     round(dist_km, 2),
                "objective_delta":            0.0,
                "objective_score":            0.0,
                "latitude":                   safe_coords(closest)[0],
                "longitude":                  safe_coords(closest)[1],
                "description":                closest.get("description", "")[:200],
                "_low_density_fallback":      True,
            }
            itinerary = [stop]
            summary = {
                "message": (
                    f"Low density area — limited nearby attractions. "
                    f"Nearest site: {closest['name']} ({dist_km:.0f} km away)."
                )
            }
            _print_itinerary(itinerary, user_profile, summary)
            return itinerary
        else:
            logger.warning(
                "[KemetPath] 0 stops fit budget. Suggestion: %s", suggestion.get("message", "")
            )
            _print_itinerary([], user_profile, suggestion)
            return []


    # Phase F — Budget fill phase (Upgrade 3)
    budget   = user_profile.get("available_hours", 6) * 60
    used     = itinerary[-1]["cumulative_time_minutes"]
    fill_pct = used / budget

    if fill_pct < BUDGET_TARGET_PCT:
        logger.info(
            "[Fill] Budget %.0f%% used (target %.0f%%) — running fill phase ...",
            fill_pct * 100, BUDGET_TARGET_PCT * 100
        )
        itinerary = fill_remaining_budget(itinerary, candidates, duration_cache, user_profile)

    # Phase G — Explainability (Upgrade 8)
    final_J = itinerary[-1].get("objective_score", 0.0) * len(itinerary) if itinerary else 0.0
    summary = _log_route_explainability(itinerary, user_profile, final_J)

    # Phase H — Pretty-print table
    _print_itinerary(itinerary, user_profile, summary=summary)

    return itinerary


generate_trip.last_suggestion = None


# ============================================================
# SECTION 12 — Pretty Printer
# ============================================================

def _print_itinerary(itinerary: list, profile: dict, suggestion: dict = None,
                     summary: dict = None):
    """Print a formatted itinerary table to stdout."""
    budget = profile.get("available_hours", 6) * 60
    print("\n" + "=" * 72)
    print("  KemetPath — Your Personalised Cairo Itinerary")
    print("=" * 72)
    print(f"  Budget: {profile.get('available_hours')}h ({budget:.0f} min)  |  "
          f"Start: {profile.get('start_time', '09:00')}  |  "
          f"Tolerance: {profile.get('walking_tolerance', 'medium')}")
    pref = profile.get("preferred_categories", [])
    if pref:
        print(f"  Preferences: {', '.join(pref)}")
    # Financial budget line (shown only when budget mode is active)
    ticket_budget_egp = resolve_ticket_budget(profile)
    if ticket_budget_egp is not None:
        currency = str(profile.get("currency", "EGP")).upper()
        original = profile.get("budget_amount", ticket_budget_egp)
        egp_note = f" (={ticket_budget_egp:.0f} EGP)" if currency == "USD" else ""
        print(f"  Ticket Budget: {original} {currency}{egp_note}")
    print("-" * 72)

    if not itinerary:
        if suggestion:
            print(f"\n  {suggestion.get('message', 'No stops fit budget.')}\n")
            sug_list = suggestion.get("suggestions", [])
            if sug_list:
                print("  Quickest alternatives near you:")
                for s in sug_list:
                    print(f"    • {s['name']} ({s['category']}) — "
                          f"~{s['duration_minutes']:.0f} min, {s['ticket_egp']} EGP")
        else:
            print("  No stops could be built for this profile.")
        print("=" * 72 + "\n")
        return

    total_ticket = 0
    for i, stop in enumerate(itinerary, 1):
        name = stop["name"].encode("ascii", "replace").decode()
        fill_tag = " [fill]" if stop.get("fill_phase") else ""
        print(
            f"  {i:2}. [{stop['arrival_time']}]  {name:<36}"
            f"  {stop['category']:<18}  {stop['predicted_duration_minutes']:>5.0f} min{fill_tag}"
        )
        std_str = (f"±{stop['prediction_std_minutes']:.0f}"
                   if stop.get("prediction_std_minutes", 0) > 0 else "")
        print(
            f"       Zone: {stop['zone']:<20}  "
            f"Travel: {stop['travel_time_minutes']:>4.0f} min  "
            f"Ticket: {stop['ticket_price_egp']:>4} EGP  "
            f"J_delta: {stop['objective_delta']:+.3f} {std_str}"
        )
        total_ticket += stop.get("ticket_price_egp", 0)

    if summary:
        used    = summary.get("budget_used_min", 0)
        end_t   = format_time(profile.get("start_time", "09:00"), used)
        print("-" * 72)
        print(f"  Stops: {summary['stops']}  |  "
              f"Budget: {used:.0f}/{budget:.0f} min ({summary['budget_fill_pct']:.0f}%)  |  "
              f"End: {end_t}")
        ticket_budget_egp = resolve_ticket_budget(profile)
        if ticket_budget_egp is not None:
            pct_used = (total_ticket / ticket_budget_egp * 100) if ticket_budget_egp > 0 else 0
            currency = str(profile.get("currency", "EGP")).upper()
            budget_amt = profile.get("budget_amount", ticket_budget_egp)
            note = "  [within budget]" if total_ticket <= ticket_budget_egp else "  [OVER BUDGET]"
            print(f"  Walk: {summary['total_walk_km']:.1f} km  |  "
                  f"Tickets: {total_ticket} EGP  |  "
                  f"Diversity H: {summary['diversity_entropy_bits']:.2f} bits  |  "
                  f"J: {summary['objective_J']:.3f}")
            print(f"  Ticket Budget: {budget_amt} {currency} | Used: {total_ticket} EGP "
                  f"({pct_used:.0f}%){note}")
        else:
            print(f"  Walk: {summary['total_walk_km']:.1f} km  |  "
                  f"Tickets: {total_ticket} EGP  |  "
                  f"Diversity H: {summary['diversity_entropy_bits']:.2f} bits  |  "
                  f"J: {summary['objective_J']:.3f}")

    print("=" * 72 + "\n")


# ============================================================
# SECTION 13 — Demo Fallback Sites (unchanged)
# ============================================================

def _get_demo_sites() -> list:
    """25-site hand-crafted Cairo dataset for demo / testing without JSON file."""
    return [
        {"name": "Great Pyramid of Giza",   "category": "Pyramid",        "zone": "Giza",          "ticket_price": 450, "size_score": 10, "crowd_score": 10, "latitude": 29.9792, "longitude": 31.1342, "description": "Oldest and largest of the Giza pyramids."},
        {"name": "Pyramid of Khafre",        "category": "Pyramid",        "zone": "Giza",          "ticket_price": 450, "size_score": 9,  "crowd_score": 9,  "latitude": 29.9763, "longitude": 31.1303, "description": "Second-largest pyramid with the Sphinx nearby."},
        {"name": "Pyramid of Menkaure",      "category": "Pyramid",        "zone": "Giza",          "ticket_price": 450, "size_score": 7,  "crowd_score": 7,  "latitude": 29.9727, "longitude": 31.1282, "description": "Smallest of the Giza pyramids."},
        {"name": "Great Sphinx of Giza",     "category": "Monument",       "zone": "Giza",          "ticket_price": 450, "size_score": 8,  "crowd_score": 10, "latitude": 29.9753, "longitude": 31.1376, "description": "Limestone sphinx monument."},
        {"name": "Solar Boat Museum",        "category": "Museum",         "zone": "Giza",          "ticket_price": 100, "size_score": 5,  "crowd_score": 6,  "latitude": 29.9791, "longitude": 31.1341, "description": "Houses the ancient Khufu Ship."},
        {"name": "Saqqara Necropolis",       "category": "Necropolis",     "zone": "Giza",          "ticket_price": 200, "size_score": 9,  "crowd_score": 7,  "latitude": 29.8711, "longitude": 31.2156, "description": "UNESCO site with Step Pyramid of Djoser."},
        {"name": "Dahshur Pyramids",         "category": "Pyramid",        "zone": "Giza",          "ticket_price": 150, "size_score": 8,  "crowd_score": 5,  "latitude": 29.8083, "longitude": 31.2103, "description": "The Bent and Red pyramids from Old Kingdom."},
        {"name": "Citadel of Cairo",         "category": "Fortress",       "zone": "Islamic Cairo", "ticket_price": 550, "size_score": 9,  "crowd_score": 9,  "latitude": 30.0287, "longitude": 31.2599, "description": "Medieval Islamic fortress built by Saladin."},
        {"name": "Sultan Hassan Mosque",     "category": "Mosque",         "zone": "Islamic Cairo", "ticket_price": 100, "size_score": 8,  "crowd_score": 8,  "latitude": 30.0347, "longitude": 31.2577, "description": "Mamluk-era masterpiece mosque and madrasa."},
        {"name": "Al-Azhar Mosque",          "category": "Mosque",         "zone": "Islamic Cairo", "ticket_price": 0,   "size_score": 8,  "crowd_score": 9,  "latitude": 30.0459, "longitude": 31.2626, "description": "One of the world's oldest universities and mosques."},
        {"name": "Khan El Khalili Bazaar",   "category": "Bazaar",         "zone": "Islamic Cairo", "ticket_price": 0,   "size_score": 7,  "crowd_score": 10, "latitude": 30.0478, "longitude": 31.2626, "description": "Iconic medieval marketplace dating to 14th century."},
        {"name": "Mosque of Ibn Tulun",      "category": "Mosque",         "zone": "Islamic Cairo", "ticket_price": 0,   "size_score": 7,  "crowd_score": 6,  "latitude": 30.0281, "longitude": 31.2503, "description": "Oldest surviving mosque in Africa."},
        {"name": "Al-Muizz Street",          "category": "Historical Site","zone": "Islamic Cairo", "ticket_price": 0,   "size_score": 6,  "crowd_score": 8,  "latitude": 30.0522, "longitude": 31.2611, "description": "Open-air museum of Islamic architecture."},
        {"name": "Bab Zuweila Gate",         "category": "Gate",           "zone": "Islamic Cairo", "ticket_price": 50,  "size_score": 3,  "crowd_score": 5,  "latitude": 30.0453, "longitude": 31.2589, "description": "Medieval southern gate of Fatimid city."},
        {"name": "Bab el-Futuh Gate",        "category": "Gate",           "zone": "Islamic Cairo", "ticket_price": 0,   "size_score": 3,  "crowd_score": 4,  "latitude": 30.0581, "longitude": 31.2622, "description": "Northern gate of medieval Fatimid Cairo."},
        {"name": "Al-Rifa'i Mosque",         "category": "Mosque",         "zone": "Islamic Cairo", "ticket_price": 100, "size_score": 7,  "crowd_score": 7,  "latitude": 30.0344, "longitude": 31.2580, "description": "Royal mausoleum mosque."},
        {"name": "Hanging Church",           "category": "Church",         "zone": "Old Cairo",     "ticket_price": 0,   "size_score": 5,  "crowd_score": 8,  "latitude": 30.0054, "longitude": 31.2296, "description": "Famous Coptic church over a Roman fortress."},
        {"name": "Coptic Museum",            "category": "Museum",         "zone": "Old Cairo",     "ticket_price": 200, "size_score": 6,  "crowd_score": 6,  "latitude": 30.0057, "longitude": 31.2293, "description": "Largest collection of Coptic Christian art."},
        {"name": "Ben Ezra Synagogue",       "category": "Synagogue",      "zone": "Old Cairo",     "ticket_price": 0,   "size_score": 3,  "crowd_score": 4,  "latitude": 30.0059, "longitude": 31.2288, "description": "One of Egypt's oldest synagogues."},
        {"name": "Church of St. Sergius",    "category": "Church",         "zone": "Old Cairo",     "ticket_price": 0,   "size_score": 3,  "crowd_score": 5,  "latitude": 30.0053, "longitude": 31.2287, "description": "Sheltered the Holy Family."},
        {"name": "Egyptian Museum",          "category": "Museum",         "zone": "Downtown",      "ticket_price": 400, "size_score": 9,  "crowd_score": 9,  "latitude": 30.0478, "longitude": 31.2336, "description": "World's largest Egyptian antiquities collection."},
        {"name": "Cairo Tower",              "category": "Monument",       "zone": "Zamalek",       "ticket_price": 200, "size_score": 4,  "crowd_score": 7,  "latitude": 30.0447, "longitude": 31.2236, "description": "187m tower on Gezira Island."},
        {"name": "Opera House Cairo",        "category": "Monument",       "zone": "Zamalek",       "ticket_price": 50,  "size_score": 6,  "crowd_score": 5,  "latitude": 30.0393, "longitude": 31.2256, "description": "Egypt's main opera and classical music venue."},
        {"name": "Baron Empain Palace",      "category": "Palace",         "zone": "Heliopolis",    "ticket_price": 150, "size_score": 5,  "crowd_score": 5,  "latitude": 30.1008, "longitude": 31.3297, "description": "Gothic Hindu palace by Belgian industrialist."},
        {"name": "Basilica of Our Lady",     "category": "Church",         "zone": "Heliopolis",    "ticket_price": 0,   "size_score": 4,  "crowd_score": 4,  "latitude": 30.0886, "longitude": 31.3247, "description": "Neo-Byzantine Catholic basilica."},
    ]


# ============================================================
# SECTION 14 — CLI Demo
# ============================================================

if __name__ == "__main__":
    print("\n" + "=" * 72)
    print("  KemetPath v2 — Beam Search Optimizer Demo")
    print("=" * 72)

    # Demo A: Ancient Egypt Fan — 8 hours from the Sphinx (high tolerance)
    print("\n[Demo A] Ancient Egypt Fan — Giza, 8h, high tolerance")
    it_a = generate_trip({
        "start_lat":            29.9753,
        "start_lon":            31.1376,
        "available_hours":      8.0,
        "preferred_categories": ["Pyramid", "Museum", "Monument"],
        "visited_sites":        ["Pyramid of Menkaure"],
        "walking_tolerance":    "high",
        "start_time":           "08:00",
    })

    # Demo B: Islamic Cairo Expert — 6h, medium tolerance
    print("\n[Demo B] Islamic History Fan — Al-Muizz area, 6h, medium")
    it_b = generate_trip({
        "start_lat":            30.0522,
        "start_lon":            31.2611,
        "available_hours":      6.0,
        "preferred_categories": ["Mosque", "Fortress", "Gate", "Bazaar", "Historical Site"],
        "visited_sites":        [],
        "walking_tolerance":    "medium",
        "start_time":           "09:30",
    })

    # Demo C: Tight budget test — 2h near Giza (expect suggestions)
    print("\n[Demo C] Tight Budget Test — Giza, 2h, medium tolerance")
    it_c = generate_trip({
        "start_lat":            29.9792,
        "start_lon":            31.1342,
        "available_hours":      2.0,
        "preferred_categories": [],
        "visited_sites":        [],
        "walking_tolerance":    "medium",
        "start_time":           "14:00",
    })
    sug = generate_trip.last_suggestion
    if not it_c and sug:
        print(f"  -> Tight budget suggestions returned: {len(sug.get('suggestions', []))} options")

    # Bonus: KMeans clustering (uses cairo_sites.json from city_datasets/)
    _cairo_modern = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                                 "city_datasets", "cairo_sites.json")
    if os.path.exists(_cairo_modern):
        print("\n[Bonus] KMeans Zone Clustering")
        import json as _j
        with open(_cairo_modern, "r", encoding="utf-8") as _f:
            all_s = _j.load(_f)
        all_s = validate_sites_geo(all_s)
        clusters = suggest_zone_clusters(all_s, n_clusters=4)
        for zone, names in clusters.items():
            print(f"  {zone} ({len(names)} sites):")
            for n in names[:3]:
                print(f"    - {n}")
