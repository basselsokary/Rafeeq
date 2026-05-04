"""
train_duration_model.py — Phase 2: Visit Duration Estimation Model
===================================================================
Generates synthetic training samples from ALL city datasets in
city_datasets/, trains a RandomForestRegressor to predict optimal
visit duration in minutes, evaluates with MAE, and saves the bundle.

Output: duration_model.pkl
"""

import json
import os
import random
import logging
import numpy as np
import pandas as pd
import joblib
from sklearn.ensemble import RandomForestRegressor
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import mean_absolute_error

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
RANDOM_SEED     = 42
random.seed(RANDOM_SEED)
np.random.seed(RANDOM_SEED)

# Primary data source: all *_sites.json files under city_datasets/
_HERE           = os.path.dirname(os.path.abspath(__file__))
CITY_DATASETS_DIR = os.path.join(_HERE, "city_datasets")
# Kept for legacy fallback only
LEGACY_SITES_FILE = os.path.join(_HERE, "cairo_historical_sites.json")
MODEL_FILE      = "duration_model.pkl"
# Samples per site — total = N_SAMPLES_PER_SITE * len(sites)
N_SAMPLES_PER_SITE = 25   # 221 sites × 25 = ~5 500 samples

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    datefmt="%H:%M:%S",
)
logger = logging.getLogger(__name__)

# Category duration bonuses (minutes) — realistic visit estimates.
# Covers both legacy Cairo categories and the new multi-city taxonomy.
CATEGORY_BONUS = {
    # ── Legacy Cairo ──────────────────────────────────────────────────
    "Museum":              60,   # 1.5–2 hr typical museum
    "Pyramid":             60,   # 1.5–2 hr at pyramid complex
    "Fortress":            30,
    "Citadel":             30,
    "Mosque":              20,   # 30–40 min per mosque
    "Church":              15,
    "Synagogue":           10,
    "Palace":              25,
    "Temple":              35,   # Karnak-scale temples need more time
    "Tomb":                15,
    "Gate":                 8,
    "Bazaar":              30,   # Khan el-Khalili: 1–2 hr
    "Monument":            10,
    "Park":                15,
    "Cemetery":             8,
    "Necropolis":          25,
    "Archaeological Site": 40,
    "Historical Site":     20,
    # ── New multi-city categories ─────────────────────────────────────
    "Religious Site":        20,
    "Nature Reserve":        50,   # Ras Mohammed-scale parks
    "Beach":                 60,   # Leisure time on site
    "Island":                45,
    "Diving Site":           90,   # Includes gearing-up time
    "Spa & Thermal Springs": 60,
    "Promenade":             20,
    "Marina":                25,
    "Market":                30,
    "Entertainment":         45,
    "Aquarium":              40,
    "Cultural Center":       30,
    "Landmark":              15,
    "Monastery":             25,
}

# Zone-based additions (navigation / orientation overhead in minutes).
# Unknown zones default to 5; well-known zones get tuned values.
ZONE_BONUS: dict = {
    # Cairo zones
    "Giza":          15,
    "Islamic Cairo": 10,
    "Old Cairo":      8,
    "Downtown":       5,
    "Zamalek":        5,
    "Heliopolis":     3,
    # Common zones in new datasets
    "Sharm El Sheikh":  5,
    "Dahab":            5,
    "St. Catherine":   10,
    "Luxor":            8,
    "Aswan":            8,
    "Karnak":          10,
    "City Center":      3,
    "Other":            0,
}



# ---------------------------------------------------------------------------
# Synthetic data generation
# ---------------------------------------------------------------------------
def generate_synthetic_data(sites: list[dict], n: int) -> pd.DataFrame:
    """
    Generate n synthetic visit records by randomly sampling from the sites
    list and computing ground-truth duration using a deterministic formula
    with Gaussian noise.

    Duration formula:
        base = 20
        + size_score  × 5   (max +50)
        + crowd_score × 2   (crowds slow you down, max +20)
        + category_bonus    (see CATEGORY_BONUS)
        + zone_bonus        (navigation overhead)
        + noise (Gaussian, std=10)
        clamped to [15, 240] minutes
    """
    records = []
    for _ in range(n):
        site = random.choice(sites)

        size_score   = site.get("size_score",  5)
        crowd_score  = site.get("crowd_score", 5)
        category     = site.get("category",   "Historical Site")
        ticket_price = site.get("ticket_price") or 0
        zone         = site.get("zone",        "City Center")

        base          = 20
        size_comp     = size_score  * 5
        crowd_comp    = crowd_score * 2
        cat_bonus     = CATEGORY_BONUS.get(category, 15)
        zone_bonus_v  = ZONE_BONUS.get(zone, 5)   # 5-min default for unknown zones
        noise         = np.random.normal(0, 10)

        duration = base + size_comp + crowd_comp + cat_bonus + zone_bonus_v + noise
        duration = float(np.clip(duration, 15, 240))

        records.append({
            "size_score":       size_score,
            "crowd_score":      crowd_score,
            "category":         category,
            "ticket_price":     ticket_price,
            "zone":             zone,
            "duration_minutes": round(duration, 2),
        })

    df = pd.DataFrame(records)
    logger.info("Generated %d synthetic training samples", len(df))
    logger.info("Duration stats (minutes):\n%s",
                df["duration_minutes"].describe().round(2).to_string())
    return df


# ---------------------------------------------------------------------------
# Feature engineering
# ---------------------------------------------------------------------------
def build_features(df: pd.DataFrame) -> tuple[pd.DataFrame, dict]:
    """
    Encode categorical features and return (feature_df, encoders_dict).
    Uses LabelEncoder for category and zone.
    Applies median imputation for any missing numeric values.
    Returns encoder dict so they can be saved alongside the model for inference.
    """
    df = df.copy()

    # Median imputation for numeric columns
    for col in ["size_score", "crowd_score", "ticket_price"]:
        median_val = df[col].median()
        df[col] = df[col].fillna(median_val)

    # Label encoding for categoricals
    encoders = {}
    for col in ["category", "zone"]:
        le = LabelEncoder()
        df[col + "_enc"] = le.fit_transform(df[col].fillna("Unknown"))
        encoders[col] = le
        logger.info(f"Encoded '{col}': {list(le.classes_)}")

    return df, encoders


def get_feature_columns() -> list[str]:
    """Return the list of feature column names used in training."""
    return ["size_score", "crowd_score", "category_enc", "ticket_price", "zone_enc"]


# ---------------------------------------------------------------------------
# Model training
# ---------------------------------------------------------------------------
def train_model(df: pd.DataFrame, encoders: dict) -> RandomForestRegressor:
    """
    Train a RandomForestRegressor on the engineered features.

    Why RandomForest?
    -  Handles nonlinear feature interactions (size × category interaction matters)
    -  Works well on small-to-medium tabular datasets without extensive tuning
    -  Robust to noise — individual trees are high-variance; ensemble averages out noise
    -  Low overfitting risk — bootstrap aggregation + feature subsampling acts as regularization
    -  Naturally handles mixed feature types without scaling
    -  Provides feature importance for interpretability (academic value)
    """
    feature_cols = get_feature_columns()
    X = df[feature_cols]
    y = df["duration_minutes"]

    # 80/20 train/test split (stratified by category makes results more representative)
    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.20, random_state=RANDOM_SEED
    )

    logger.info(f"Train size: {len(X_train)}, Test size: {len(X_test)}")

    # Train
    model = RandomForestRegressor(
        n_estimators=200,       # More trees = lower variance, negligible extra cost
        max_depth=None,         # Allow full-depth trees (forest handles overfitting)
        min_samples_split=5,    # Slight regularization to avoid tiny leaf nodes
        random_state=RANDOM_SEED,
        n_jobs=-1,              # Use all CPU cores
    )
    model.fit(X_train, y_train)

    # Evaluate
    y_pred_test  = model.predict(X_test)
    y_pred_train = model.predict(X_train)
    mae_test  = mean_absolute_error(y_test, y_pred_test)
    mae_train = mean_absolute_error(y_train, y_pred_train)

    print("\n" + "=" * 50)
    print("  Visit Duration Model — Training Results")
    print("=" * 50)
    print(f"  Train MAE : {mae_train:.2f} minutes")
    print(f"  Test  MAE : {mae_test:.2f} minutes  <-- key metric")
    print(f"  (Lower is better; target <= 20 min for tabular data)")
    print("=" * 50)

    # Feature importance
    print("\nFeature Importances:")
    importances = dict(zip(feature_cols, model.feature_importances_))
    for feat, imp in sorted(importances.items(), key=lambda x: -x[1]):
        bar = "#" * int(imp * 40)
        print(f"  {feat:<20} {imp:.4f}  {bar}")

    return model


# ---------------------------------------------------------------------------
# Model persistence
# ---------------------------------------------------------------------------
def save_model(model: RandomForestRegressor, encoders: dict, path: str = MODEL_FILE):
    """
    Save model + encoders + feature_columns as a single bundle using joblib.

    Fix 1: feature_columns are saved so inference can reindex the prediction
    DataFrame to exactly match training column order — preventing the
    'Feature names must match' sklearn error.
    """
    feature_columns = get_feature_columns()
    bundle = {
        "model":           model,
        "encoders":        encoders,
        "feature_columns": feature_columns,   # <-- new: exact training column order
    }
    joblib.dump(bundle, path)
    logger.info("Model bundle saved to '%s' | feature_columns: %s", path, feature_columns)


def load_model(path: str = MODEL_FILE) -> tuple:
    """Load model + encoders from disk. Returns (model, encoders)."""
    bundle = joblib.load(path)
    return bundle["model"], bundle["encoders"]


# ---------------------------------------------------------------------------
# Prediction interface (used by Phase 3)
# ---------------------------------------------------------------------------
def predict_duration(site_features: dict, model_path: str = MODEL_FILE) -> float:
    """
    Predict optimal visit duration in minutes for a single site.

    Args:
        site_features: dict with keys:
            - size_score   (int, 1–10)
            - crowd_score  (int, 1–10)
            - category     (str, e.g. "Museum")
            - ticket_price (int, EGP)
            - zone         (str, e.g. "Islamic Cairo")
        model_path: path to the saved .pkl bundle

    Returns:
        Predicted duration in minutes (float, clamped to [15, 300])

    Example:
        >>> predict_duration({
        ...     "size_score": 8, "crowd_score": 9,
        ...     "category": "Museum", "ticket_price": 200, "zone": "Downtown"
        ... })
        187.4
    """
    model, encoders = load_model(model_path)

    # Encode categoricals using saved encoders
    row = {}
    row["size_score"]   = site_features.get("size_score", 5)
    row["crowd_score"]  = site_features.get("crowd_score", 5)
    row["ticket_price"] = site_features.get("ticket_price", 100)

    for col in ["category", "zone"]:
        le: LabelEncoder = encoders[col]
        value = site_features.get(col, "Historical Site")
        # Handle unseen labels gracefully (transform to most common class)
        if value not in le.classes_:
            logger.warning(f"Unknown {col} value '{value}', using median class")
            value = le.classes_[len(le.classes_) // 2]
        row[col + "_enc"] = int(le.transform([value])[0])

    X = pd.DataFrame([row])[get_feature_columns()]
    prediction = float(model.predict(X)[0])
    return round(float(np.clip(prediction, 15, 300)), 2)


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------
def main():
    logger.info("=" * 60)
    logger.info("KemetPath — Duration Model Training (Multi-City)")
    logger.info("=" * 60)

    # ------------------------------------------------------------------
    # 1. Load sites from ALL city_datasets/*_sites.json files
    # ------------------------------------------------------------------
    sites: list[dict] = []

    if os.path.isdir(CITY_DATASETS_DIR):
        city_files = sorted(
            f for f in os.listdir(CITY_DATASETS_DIR)
            if f.endswith("_sites.json")
        )
        for fname in city_files:
            fpath = os.path.join(CITY_DATASETS_DIR, fname)
            try:
                with open(fpath, "r", encoding="utf-8") as fh:
                    city_sites = json.load(fh)
                sites.extend(city_sites)
                logger.info("  Loaded %3d sites from %s", len(city_sites), fname)
            except Exception as exc:
                logger.warning("  Skipped %s: %s", fname, exc)

    # Merge in legacy Cairo file (may contain sites not in city_datasets/)
    if os.path.exists(LEGACY_SITES_FILE):
        with open(LEGACY_SITES_FILE, "r", encoding="utf-8") as fh:
            legacy = json.load(fh)
        existing_names = {s.get("name", "").lower() for s in sites}
        new_legacy = [
            s for s in legacy
            if s.get("name", "").lower() not in existing_names
        ]
        sites.extend(new_legacy)
        logger.info("  Merged %d extra sites from legacy Cairo file", len(new_legacy))

    if not sites:
        logger.warning("No city datasets found — using built-in fallback sites.")
        sites = _get_fallback_sites()

    logger.info("Total training sites: %d across all cities", len(sites))

    # ------------------------------------------------------------------
    # 2. Scale sample count to dataset size
    # ------------------------------------------------------------------
    n_samples = max(5000, len(sites) * N_SAMPLES_PER_SITE)
    logger.info("Generating %d synthetic samples (%d sites x %d per site)",
                n_samples, len(sites), N_SAMPLES_PER_SITE)

    # ------------------------------------------------------------------
    # 3. Generate synthetic training data
    # ------------------------------------------------------------------
    df = generate_synthetic_data(sites, n=n_samples)

    # ------------------------------------------------------------------
    # 4. Feature engineering + train + save
    # ------------------------------------------------------------------
    df_enc, encoders = build_features(df)
    model = train_model(df_enc, encoders)
    save_model(model, encoders)

    # ------------------------------------------------------------------
    # 5. Sanity check — one sample from each major category group
    # ------------------------------------------------------------------
    logger.info("\nSanity check predictions:")
    test_cases = [
        # Cairo legacy
        {"size_score": 10, "crowd_score": 10, "category": "Pyramid",        "ticket_price": 450, "zone": "Giza",            "expected": "~165"},
        {"size_score": 9,  "crowd_score": 9,  "category": "Museum",         "ticket_price": 400, "zone": "Downtown",        "expected": "~155"},
        {"size_score": 5,  "crowd_score": 6,  "category": "Mosque",         "ticket_price": 0,   "zone": "Islamic Cairo",   "expected": "~ 77"},
        {"size_score": 3,  "crowd_score": 3,  "category": "Gate",           "ticket_price": 0,   "zone": "Islamic Cairo",   "expected": "~ 59"},
        # New categories
        {"size_score": 8,  "crowd_score": 6,  "category": "Nature Reserve", "ticket_price": 260, "zone": "Sharm El Sheikh", "expected": "~125"},
        {"size_score": 6,  "crowd_score": 7,  "category": "Beach",          "ticket_price": 0,   "zone": "City Center",     "expected": "~115"},
        {"size_score": 5,  "crowd_score": 5,  "category": "Diving Site",    "ticket_price": 0,   "zone": "Dahab",          "expected": "~140"},
        {"size_score": 7,  "crowd_score": 5,  "category": "Temple",         "ticket_price": 200, "zone": "Luxor",          "expected": "~110"},
    ]
    print("\nSample Predictions:")
    print(f"{'Category (size, crowd)':<42} {'Predicted':>10} {'Expected':>10}")
    print("-" * 64)
    for tc in test_cases:
        label = f"{tc['category']} (size={tc['size_score']}, crowd={tc['crowd_score']})"
        try:
            pred = predict_duration({k: v for k, v in tc.items() if k != "expected"})
            print(f"{label:<42} {pred:>8.1f}m {tc['expected']:>10}")
        except Exception as exc:
            print(f"{label:<42} ERROR: {exc}")


def _get_fallback_sites() -> list[dict]:
    """Fallback site list for when the scraper hasn't been run yet."""
    return [
        {"name": "Great Pyramid of Giza",  "category": "Pyramid",  "zone": "Giza",          "size_score": 10, "crowd_score": 10, "ticket_price": 450},
        {"name": "Egyptian Museum",         "category": "Museum",   "zone": "Downtown",      "size_score": 9,  "crowd_score": 9,  "ticket_price": 400},
        {"name": "Citadel of Cairo",        "category": "Fortress", "zone": "Islamic Cairo", "size_score": 9,  "crowd_score": 9,  "ticket_price": 550},
        {"name": "Al-Azhar Mosque",         "category": "Mosque",   "zone": "Islamic Cairo", "size_score": 8,  "crowd_score": 9,  "ticket_price": 0},
        {"name": "Hanging Church",          "category": "Church",   "zone": "Old Cairo",     "size_score": 5,  "crowd_score": 8,  "ticket_price": 0},
        {"name": "Sultan Hassan Mosque",    "category": "Mosque",   "zone": "Islamic Cairo", "size_score": 8,  "crowd_score": 8,  "ticket_price": 100},
        {"name": "Coptic Museum",           "category": "Museum",   "zone": "Old Cairo",     "size_score": 6,  "crowd_score": 6,  "ticket_price": 200},
        {"name": "Ben Ezra Synagogue",      "category": "Synagogue","zone": "Old Cairo",     "size_score": 3,  "crowd_score": 4,  "ticket_price": 0},
        {"name": "Saqqara Necropolis",      "category": "Necropolis","zone": "Giza",         "size_score": 9,  "crowd_score": 7,  "ticket_price": 200},
        {"name": "Mosque of Ibn Tulun",     "category": "Mosque",   "zone": "Islamic Cairo", "size_score": 7,  "crowd_score": 6,  "ticket_price": 0},
        {"name": "Muizz Street",            "category": "Historical Site","zone": "Islamic Cairo","size_score": 6,"crowd_score": 8, "ticket_price": 0},
        {"name": "Khan El Khalili",         "category": "Bazaar",   "zone": "Islamic Cairo", "size_score": 7,  "crowd_score": 10, "ticket_price": 0},
        {"name": "Cairo Tower",             "category": "Monument", "zone": "Zamalek",       "size_score": 4,  "crowd_score": 7,  "ticket_price": 200},
        {"name": "Baron Empain Palace",     "category": "Palace",   "zone": "Heliopolis",    "size_score": 5,  "crowd_score": 5,  "ticket_price": 150},
        {"name": "Dahshur Pyramids",        "category": "Pyramid",  "zone": "Giza",          "size_score": 8,  "crowd_score": 5,  "ticket_price": 150},
        {"name": "Memphis Open Air Museum", "category": "Museum",   "zone": "Giza",          "size_score": 6,  "crowd_score": 5,  "ticket_price": 200},
        {"name": "Bab Zuweila",             "category": "Gate",     "zone": "Islamic Cairo", "size_score": 3,  "crowd_score": 5,  "ticket_price": 50},
        {"name": "Bab el-Futuh",            "category": "Gate",     "zone": "Islamic Cairo", "size_score": 3,  "crowd_score": 4,  "ticket_price": 0},
        {"name": "Al-Rifa'i Mosque",        "category": "Mosque",   "zone": "Islamic Cairo", "size_score": 7,  "crowd_score": 7,  "ticket_price": 100},
        {"name": "Madrasa of Sultan Barquq","category": "Historical Site","zone": "Islamic Cairo","size_score": 5,"crowd_score": 4,"ticket_price": 50},
    ] * 10  # Repeat to give enough variety for 5000 samples


if __name__ == "__main__":
    main()
