"""
generate_city_datasets.py — Rafeeq Multi-City Tourism Dataset Generator
========================================================================
Reads:
  - "Rafeeq Data Collection - Cities.csv"
  - "Rafeeq Data Collection - Sites.csv"

Produces:
  city_datasets/<city_name>_sites.json   (one per city)
  city_datasets/cities_index.json         (master index)

Run:
  python generate_city_datasets.py
"""

import csv
import json
import math
import os
import re
import unicodedata

# ─────────────────────────────────────────────────
# CONFIG
# ─────────────────────────────────────────────────
BASE_DIR     = os.path.dirname(os.path.abspath(__file__))
CITIES_CSV   = os.path.join(BASE_DIR, "Rafeeq Data Collection - Cities.csv")
SITES_CSV    = os.path.join(BASE_DIR, "Rafeeq Data Collection - Sites.csv")
OUTPUT_DIR   = os.path.join(BASE_DIR, "city_datasets")

# ─────────────────────────────────────────────────
# CATEGORY NORMALISATION MAP
# Maps raw Type/category values → canonical category names
# ─────────────────────────────────────────────────
CATEGORY_MAP = {
    # Religious
    "religious":         "Religious Site",
    "mosque":            "Mosque",
    "church":            "Church",
    "synagogue":         "Synagogue",
    "monastery":         "Monastery",
    # Nature / outdoor
    "natural":           "Nature Reserve",
    "nature":            "Nature Reserve",
    "beach":             "Beach",
    "island":            "Island",
    "divingsite":        "Diving Site",
    "diving site":       "Diving Site",
    "marine":            "Diving Site",
    "therapeutic":       "Spa & Thermal Springs",
    "mountain":          "Nature Reserve",
    "river":             "Nature Reserve",
    # Historical / cultural
    "historical":        "Historical Site",
    "archaeological":    "Archaeological Site",
    "monument":          "Monument",
    "fortress":          "Fortress",
    "palace":            "Palace",
    "castle":            "Fortress",
    "temple":            "Temple",
    "library":           "Cultural Center",
    # Museums & art
    "museum":            "Museum",
    "gallery":           "Museum",
    "cultural":          "Cultural Center",
    "aquarium":          "Aquarium",
    # Entertainment & leisure
    "entertainment":     "Entertainment",
    "marina":            "Marina",
    "park":              "Park",
    "walkway":           "Promenade",
    "food/commercial":   "Market",
    "market":            "Market",
    # Misc
    "other":             "Landmark",
    "landmark":          "Landmark",
}

# ─────────────────────────────────────────────────
# SIZE / CROWD defaults per canonical category
# ─────────────────────────────────────────────────
CATEGORY_DEFAULTS = {
    "Archaeological Site":      {"size": 8, "crowd": 7},
    "Monument":                 {"size": 7, "crowd": 7},
    "Fortress":                 {"size": 7, "crowd": 6},
    "Palace":                   {"size": 7, "crowd": 6},
    "Museum":                   {"size": 6, "crowd": 6},
    "Cultural Center":          {"size": 5, "crowd": 5},
    "Religious Site":           {"size": 5, "crowd": 6},
    "Mosque":                   {"size": 5, "crowd": 6},
    "Church":                   {"size": 4, "crowd": 5},
    "Monastery":                {"size": 5, "crowd": 4},
    "Synagogue":                {"size": 3, "crowd": 3},
    "Nature Reserve":           {"size": 8, "crowd": 5},
    "Beach":                    {"size": 6, "crowd": 7},
    "Island":                   {"size": 6, "crowd": 6},
    "Diving Site":              {"size": 5, "crowd": 5},
    "Spa & Thermal Springs":    {"size": 3, "crowd": 3},
    "Park":                     {"size": 5, "crowd": 6},
    "Promenade":                {"size": 4, "crowd": 7},
    "Marina":                   {"size": 6, "crowd": 6},
    "Market":                   {"size": 5, "crowd": 8},
    "Entertainment":            {"size": 5, "crowd": 7},
    "Aquarium":                 {"size": 5, "crowd": 6},
    "Historical Site":          {"size": 5, "crowd": 5},
    "Landmark":                 {"size": 4, "crowd": 5},
}

# Featured sites get boosted scores
FEATURED_SIZE_BOOST  = 1
FEATURED_CROWD_BOOST = 1

# ─────────────────────────────────────────────────
# HELPERS
# ─────────────────────────────────────────────────

def slugify(text: str) -> str:
    """Convert a city name to a safe filename stem."""
    text = unicodedata.normalize("NFKD", text)
    text = text.encode("ascii", "ignore").decode("ascii")
    text = re.sub(r"[^\w\s-]", "", text).strip()
    text = re.sub(r"[\s_-]+", "_", text)
    return text.lower()


def clean_text(raw: str) -> str:
    """Remove Wikipedia-style citations, excess whitespace, Arabic text in brackets."""
    if not raw:
        return ""
    # Remove [1], [2], etc.
    text = re.sub(r"\[\d+\]", "", raw)
    # Remove broken formatting sequences
    text = re.sub(r"\\u[0-9a-fA-F]{4}", "", text)
    # Collapse internal whitespace
    text = re.sub(r"\s+", " ", text)
    # Strip leading/trailing
    text = text.strip()
    return text


def shorten_description(text: str, max_sentences: int = 4) -> str:
    """Truncate to at most max_sentences. Keeps full sentences."""
    if not text:
        return ""
    # Split on sentence-ending punctuation followed by a space / end of string
    sentences = re.split(r"(?<=[.!?])\s+", text)
    # Some descriptions have tips; keep only the non-tip part
    main_sentences = []
    for s in sentences:
        if re.match(r"^tip[:.]", s.strip(), re.IGNORECASE):
            break
        main_sentences.append(s)
    if not main_sentences:
        main_sentences = sentences
    chosen = main_sentences[:max_sentences]
    return " ".join(chosen).strip()


def parse_float(value: str, default=None):
    """Safely parse a float, stripping degree symbols and cardinal letters."""
    if not value or value.strip().lower() in ("null", "none", "", "n/a"):
        return default
    # Handle "29.5102° N" or "32.8998° E" style
    val = re.sub(r"[°NSEWnsew]", "", str(value)).strip()
    # Handle compound coords like "30.55872171727794, 32.30120162791625" — take first
    if "," in val:
        val = val.split(",")[0].strip()
    try:
        return float(val)
    except (ValueError, TypeError):
        return default


def parse_ticket(value: str):
    """
    Return an integer ticket price (EGP) or None.
    Handles values like: "220, 110, 50, 25", "Null", "300", "Free (from shore)"
    Takes the first / maximum number found.
    """
    if not value or value.strip().lower() in ("null", "none", "", "free", "n/a"):
        return None
    nums = re.findall(r"\d+", value)
    if not nums:
        return None
    # Use the highest number as the "standard adult" price
    return max(int(n) for n in nums)


def normalise_category(raw_type: str) -> str:
    """Map raw Type column to a canonical category string."""
    key = raw_type.strip().lower()
    return CATEGORY_MAP.get(key, "Landmark")


def estimate_scores(category: str, is_featured: bool) -> dict:
    """Return size_score and crowd_score based on category + featured flag."""
    defaults = CATEGORY_DEFAULTS.get(category, {"size": 5, "crowd": 5})
    size  = defaults["size"]
    crowd = defaults["crowd"]
    if is_featured:
        size  = min(10, size  + FEATURED_SIZE_BOOST)
        crowd = min(10, crowd + FEATURED_CROWD_BOOST)
    return {"size_score": size, "crowd_score": crowd}


def infer_zone(address: str, city_name: str) -> str:
    """
    Attempt to extract a meaningful zone/district from the address.
    Falls back to city_name or 'Unknown'.
    """
    if not address:
        return "City Center"

    # Common Egyptian zone keywords to look for
    ZONE_PATTERNS = [
        r"\b(Downtown|Old Town|Old City|Corniche|Marina|Old Market|City Center)\b",
        r"\b(Islamic Cairo|Coptic Cairo|Old Cairo|Historic Cairo)\b",
        r"\b(West Bank|East Bank|Al Karnak|Karnak)\b",
        r"\b(Sharm El Sheikh|Dahab|Nuweiba|St\. Catherine|Taba|Ras Mohammed)\b",
        r"\b(Hurghada|El Gouna|Sahl Hasheesh|Soma Bay|Safaga|Makadi)\b",
        r"\b(Port Said|Ismailia|Suez|Ras El Bar|Damietta)\b",
        r"\b(Giza|Saqqara|Dahshur|Abu Simbel|Aswan|Luxor|Edfu|Esna|Kom Ombo)\b",
        r"\b(Marsa Alam|El Quseir|Port Ghalib)\b",
        r"\b(Zamalek|Maadi|Heliopolis|Nasr City)\b",
    ]
    for pat in ZONE_PATTERNS:
        m = re.search(pat, address, re.IGNORECASE)
        if m:
            return m.group(0).strip()

    # Try splitting on comma and taking meaningful parts
    parts = [p.strip() for p in address.split(",") if p.strip()]
    # Skip pure numbers / postcodes
    meaningful = [p for p in parts if not re.fullmatch(r"[\d\s]+", p)]
    if len(meaningful) >= 2:
        # Second-to-last part is often the district
        return meaningful[-2]
    if meaningful:
        return meaningful[0]

    return "City Center"


# ─────────────────────────────────────────────────
# CSV LOADING
# ─────────────────────────────────────────────────

def load_cities(path: str) -> dict:
    """
    Returns {city_id: {name, center_lat, center_lon, description}} 
    for all non-blank city records.
    """
    cities = {}
    with open(path, newline="", encoding="utf-8-sig") as f:
        reader = csv.DictReader(f)
        for row in reader:
            city_id   = row.get("City ID", "").strip()
            city_name = row.get("City Name (English)", "").strip()
            if not city_id or not city_name:
                continue  # skip placeholder rows

            lat = parse_float(row.get("Center Latitude", ""))
            lon = parse_float(row.get("Center Longitude", ""))
            cities[city_id] = {
                "name":       city_name,
                "center_lat": lat,
                "center_lon": lon,
                "description": clean_text(row.get("Description (English)", "")),
            }
    return cities


def load_sites(path: str) -> list:
    """
    Returns a list of raw site dicts from the CSV.
    """
    sites = []
    with open(path, newline="", encoding="utf-8-sig") as f:
        reader = csv.DictReader(f)
        for row in reader:
            sites.append(row)
    return sites


# ─────────────────────────────────────────────────
# SITE TRANSFORMATION
# ─────────────────────────────────────────────────

def transform_site(row: dict, city_name: str) -> dict | None:
    """
    Convert one CSV row into a clean site dict matching the target schema.
    Returns None if the site is hidden or has no usable name.
    """
    # Support both old column name and new column name
    name = clean_text(
        row.get("Name (English)") or row.get("Site Name (English)", "")
    ).strip()
    if not name:
        return None

    # Skip hidden sites
    if row.get("Is Hidden Gem?", "").strip().upper() == "TRUE" and \
       row.get("Status", "").strip().lower() == "hidden":
        return None

    # Category
    raw_type = row.get("Type", "").strip()
    category = normalise_category(raw_type)

    # Coordinates
    lat = parse_float(row.get("Latitude", ""))
    lon = parse_float(row.get("Longitude", ""))
    # Sanity check: both must be present and realistic
    if lat is not None and lon is not None:
        if not (-90 <= lat <= 90) or not (-180 <= lon <= 180):
            lat, lon = None, None
        # Some rows have the same value for lat and lon (data error)
        elif abs(lat) == abs(lon) and lat != 0:
            lat, lon = None, None

    # Ticket price
    ticket = parse_ticket(row.get("Entry Fee 'NULLABLE'", ""))
    # Support both old and new column name for Is Free?
    is_free_val = (
        row.get("Is Free?") or row.get("Is Free? 'NULLABLE'", "")
    ).strip().upper()
    if is_free_val == "TRUE":
        ticket = 0

    # Address → zone
    address = clean_text(row.get("Address (English)", ""))
    zone    = infer_zone(address, city_name)

    # Scores
    is_featured = row.get("Is Featured?", "").strip().upper() == "TRUE"
    scores      = estimate_scores(category, is_featured)

    # Description
    raw_desc  = row.get("Description (English)", "")
    desc      = clean_text(raw_desc)
    desc      = shorten_description(desc, max_sentences=4)

    return {
        "name":        name,
        "category":    category,
        "zone":        zone,
        "ticket_price": ticket,
        "size_score":  scores["size_score"],
        "crowd_score": scores["crowd_score"],
        "latitude":    lat,
        "longitude":   lon,
        "description": desc,
    }


# ─────────────────────────────────────────────────
# DEDUPLICATION
# ─────────────────────────────────────────────────

def deduplicate(sites: list) -> list:
    """Remove duplicates by exact name (case-insensitive) or within 100 m."""
    seen_names  = set()
    seen_coords = []
    result      = []

    def haversine_m(lat1, lon1, lat2, lon2):
        R = 6_371_000
        phi1 = math.radians(lat1)
        phi2 = math.radians(lat2)
        dphi = math.radians(lat2 - lat1)
        dlam = math.radians(lon2 - lon1)
        a = math.sin(dphi/2)**2 + math.cos(phi1)*math.cos(phi2)*math.sin(dlam/2)**2
        return R * 2 * math.atan2(math.sqrt(a), math.sqrt(1-a))

    for site in sites:
        name_key = site["name"].strip().lower()
        if name_key in seen_names:
            continue
        lat, lon = site.get("latitude"), site.get("longitude")
        if lat is not None and lon is not None:
            too_close = any(
                haversine_m(lat, lon, p, q) < 100
                for p, q in seen_coords
            )
            if too_close:
                continue
            seen_coords.append((lat, lon))
        seen_names.add(name_key)
        result.append(site)
    return result


# ─────────────────────────────────────────────────
# MAIN PIPELINE
# ─────────────────────────────────────────────────

def main():
    os.makedirs(OUTPUT_DIR, exist_ok=True)
    print(f"[Rafeeq] Output directory: {OUTPUT_DIR}")

    # Load source data
    cities = load_cities(CITIES_CSV)
    print(f"[Rafeeq] Loaded {len(cities)} cities")

    raw_sites = load_sites(SITES_CSV)
    print(f"[Rafeeq] Loaded {len(raw_sites)} raw site rows")

    # Group sites by city
    city_sites: dict[str, list] = {cid: [] for cid in cities}
    skipped = 0
    for row in raw_sites:
        city_id = row.get("Parent City ID", "").strip()
        if city_id not in cities:
            skipped += 1
            continue
        city_name = cities[city_id]["name"]
        site = transform_site(row, city_name)
        if site is None:
            skipped += 1
            continue
        city_sites[city_id].append(site)

    print(f"[Rafeeq] Skipped {skipped} rows (no city match or empty name)")

    # Build index
    index = []

    for city_id, site_list in city_sites.items():
        if not site_list:
            continue  # no sites for this city

        city_info = cities[city_id]
        city_name = city_info["name"]

        # Deduplicate
        site_list = deduplicate(site_list)

        # Compute centroid from actual site coords (fallback to CSV centre)
        lats = [s["latitude"]  for s in site_list if s["latitude"]  is not None]
        lons = [s["longitude"] for s in site_list if s["longitude"] is not None]
        if lats:
            center_lat = round(sum(lats) / len(lats), 6)
            center_lon = round(sum(lons) / len(lons), 6)
        else:
            center_lat = city_info["center_lat"]
            center_lon = city_info["center_lon"]

        # Write city JSON
        filename = slugify(city_name) + "_sites.json"
        filepath = os.path.join(OUTPUT_DIR, filename)
        with open(filepath, "w", encoding="utf-8") as f:
            json.dump(site_list, f, ensure_ascii=False, indent=2)

        print(f"  [OK] {city_name:30s} -> {filename:45s}  ({len(site_list)} sites)")

        # Add to index
        index.append({
            "city_id":    city_id,
            "city":       city_name,
            "filename":   filename,
            "num_sites":  len(site_list),
            "center_lat": center_lat,
            "center_lon": center_lon,
        })

    # Sort index by city_id
    index.sort(key=lambda x: x["city_id"])

    # Write master index
    index_path = os.path.join(OUTPUT_DIR, "cities_index.json")
    with open(index_path, "w", encoding="utf-8") as f:
        json.dump(index, f, ensure_ascii=False, indent=2)

    total_sites = sum(e["num_sites"] for e in index)
    print(f"\n[Rafeeq] Done!")
    print(f"         Cities with sites : {len(index)}")
    print(f"         Total sites output: {total_sites}")
    print(f"         Master index       : {index_path}")


if __name__ == "__main__":
    main()
