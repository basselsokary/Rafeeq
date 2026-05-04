"""
city_resolver.py — Dynamic City Dataset Resolver
==================================================
Resolves a target city from a name string or GPS coordinates,
then loads the matching <city>_sites.json from city_datasets/.

Priority: city_name > coordinates > default fallback.

Zero code changes needed when a new city file is added to city_datasets/.
"""

import json
import math
import os
from typing import Optional

# ---------------------------------------------------------------------------
# Paths (resolve relative to this file's location)
# ---------------------------------------------------------------------------
_HERE = os.path.dirname(os.path.abspath(__file__))
CITY_DATASETS_DIR  = os.path.join(_HERE, "city_datasets")
CITIES_INDEX_FILE  = os.path.join(CITY_DATASETS_DIR, "cities_index.json")

# Common aliases so the API accepts "alex", "Sharm", etc.
_ALIASES: dict[str, str] = {
    "alex":             "alexadria",
    "alexandria":       "alexadria",
    "sharm":            "sinai",
    "sharm el sheikh":  "sinai",
    "south sinai":      "sinai",
    "hurghada":         "north red sea",
    "el gouna":         "north red sea",
    "red sea":          "north red sea",
    "north red sea":    "north red sea",
    "marsa alam":       "southern red sea",
    "southern red sea": "southern red sea",
    "ismailia":         "suez canal cities",
    "port said":        "suez canal cities",
    "suez":             "suez canal cities",
    "fayoum":           "fayoum",
    "faiyum":           "fayoum",
    "minya":            "minya",
    "luxor":            "luxor",
    "aswan":            "southern red sea",
    "matruh":           "marsa matruh",
    "marsa matruh":     "marsa matruh",
    "tanta":            "gharbia",
    "gharbia":          "gharbia",
}


def _haversine_km(lat1: float, lon1: float, lat2: float, lon2: float) -> float:
    """Haversine distance in km."""
    R = 6371.0
    p1, p2 = math.radians(lat1), math.radians(lat2)
    dp = math.radians(lat2 - lat1)
    dl = math.radians(lon2 - lon1)
    a  = math.sin(dp / 2) ** 2 + math.cos(p1) * math.cos(p2) * math.sin(dl / 2) ** 2
    return R * 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))


class CityResolver:
    """
    Lazy-loads cities_index.json and resolves a target city on demand.
    One instance should be created at app startup and reused.
    """

    def __init__(
        self,
        datasets_dir: str = CITY_DATASETS_DIR,
        index_file:   str = CITIES_INDEX_FILE,
    ):
        self.datasets_dir = datasets_dir
        self.index_file   = index_file
        self._index: list[dict] = []

    # ------------------------------------------------------------------
    # Index management
    # ------------------------------------------------------------------

    def _ensure_loaded(self) -> None:
        if self._index:
            return
        if not os.path.exists(self.index_file):
            # Fall back to dynamic scan when the index file is absent
            self._index = self._build_index_from_scan()
            return
        with open(self.index_file, "r", encoding="utf-8") as f:
            self._index = json.load(f)

    def _build_index_from_scan(self) -> list[dict]:
        """
        Scan city_datasets/ for *_sites.json files and build a minimal index.
        Used when cities_index.json is missing (zero-downtime extensibility).
        """
        entries = []
        if not os.path.isdir(self.datasets_dir):
            return entries
        for fname in sorted(os.listdir(self.datasets_dir)):
            if not fname.endswith("_sites.json"):
                continue
            city_name = fname.replace("_sites.json", "").replace("_", " ").title()
            filepath  = os.path.join(self.datasets_dir, fname)
            try:
                with open(filepath, "r", encoding="utf-8") as f:
                    sites = json.load(f)
                lats = [s["latitude"]  for s in sites if s.get("latitude")]
                lons = [s["longitude"] for s in sites if s.get("longitude")]
                entries.append({
                    "city_id":    city_name,
                    "city":       city_name,
                    "filename":   fname,
                    "num_sites":  len(sites),
                    "center_lat": round(sum(lats) / len(lats), 6) if lats else None,
                    "center_lon": round(sum(lons) / len(lons), 6) if lons else None,
                })
            except Exception:
                pass
        return entries

    # ------------------------------------------------------------------
    # Public API
    # ------------------------------------------------------------------

    def list_cities(self) -> list[dict]:
        """Return all cities in the index (safe, no file loading)."""
        self._ensure_loaded()
        return [
            {
                "city_id":    e.get("city_id"),
                "city":       e["city"],
                "num_sites":  e.get("num_sites", 0),
                "center_lat": e.get("center_lat"),
                "center_lon": e.get("center_lon"),
            }
            for e in self._index
        ]

    def scan_files(self) -> list[str]:
        """List all *_sites.json filenames in datasets_dir (no index needed)."""
        if not os.path.isdir(self.datasets_dir):
            return []
        return sorted(
            f for f in os.listdir(self.datasets_dir)
            if f.endswith("_sites.json")
        )

    # ------------------------------------------------------------------
    # Resolution helpers
    # ------------------------------------------------------------------

    def _match_by_name(self, name: str) -> Optional[dict]:
        """Case-insensitive name and alias lookup."""
        key = name.strip().lower()
        # Alias → canonical name fragment
        canonical = _ALIASES.get(key, key)
        # Exact index match
        for e in self._index:
            if e["city"].strip().lower() == canonical:
                return e
        # Partial match (e.g. "North Red Sea" in "North Red Sea Governorate")
        for e in self._index:
            if canonical in e["city"].strip().lower():
                return e
        # Reverse: user name contains index city name
        for e in self._index:
            if e["city"].strip().lower() in key:
                return e
        return None

    def _nearest_by_coords(self, lat: float, lon: float) -> tuple[dict, float]:
        """Return (index_entry, distance_km) of the closest city centre."""
        best_entry = self._index[0]
        best_dist  = float("inf")
        for e in self._index:
            clat = e.get("center_lat")
            clon = e.get("center_lon")
            if clat is None or clon is None:
                continue
            d = _haversine_km(lat, lon, clat, clon)
            if d < best_dist:
                best_dist, best_entry = d, e
        return best_entry, best_dist

    # ------------------------------------------------------------------
    # Main resolve
    # ------------------------------------------------------------------

    def resolve(
        self,
        city_name: Optional[str] = None,
        lat: Optional[float] = None,
        lon: Optional[float] = None,
    ) -> dict:
        """
        Resolve a city and return an enriched info dict including 'filepath'.

        Priority:
          1. city_name  → name / alias lookup
          2. lat + lon  → nearest city by Haversine distance
          3. Raises ValueError if neither is provided or no match found.

        Returned dict keys:
          city, city_id, filename, num_sites, center_lat, center_lon,
          filepath, resolution_method
        """
        self._ensure_loaded()
        if not self._index:
            raise ValueError(
                "City index is empty. "
                "Run generate_city_datasets.py to populate city_datasets/."
            )

        entry  = None
        method = None

        if city_name:
            entry = self._match_by_name(city_name)
            if entry:
                method = f"name match for '{city_name}'"

        if entry is None and lat is not None and lon is not None:
            entry, dist = self._nearest_by_coords(lat, lon)
            method = f"nearest city ({entry['city']}, {dist:.1f} km away)"

        if entry is None:
            raise ValueError(
                "Could not resolve a city. "
                "Provide a valid city name or GPS coordinates within Egypt."
            )

        filepath = os.path.join(self.datasets_dir, entry["filename"])
        return {**entry, "filepath": filepath, "resolution_method": method}

    def get_cities_by_proximity(
        self, lat: float, lon: float
    ) -> list[dict]:
        """
        Return every city in the index sorted by distance from (lat, lon),
        nearest first.  Each entry is enriched with:
          - 'filepath'     : absolute path to the *_sites.json file
          - 'distance_km'  : Haversine distance from the given point

        Used by generate_trip() to drive the proximity fallback loop.
        """
        self._ensure_loaded()
        results = []
        for entry in self._index:
            clat = entry.get("center_lat")
            clon = entry.get("center_lon")
            dist = (
                _haversine_km(lat, lon, clat, clon)
                if clat is not None and clon is not None
                else float("inf")
            )
            results.append({
                **entry,
                "filepath":    os.path.join(self.datasets_dir, entry["filename"]),
                "distance_km": round(dist, 1),
            })
        return sorted(results, key=lambda x: x["distance_km"])

    def load_sites(self, city_info: dict) -> list[dict]:
        """Load and return the site list for an already-resolved city."""
        fp = city_info.get("filepath", "")
        if not fp or not os.path.exists(fp):
            raise FileNotFoundError(
                f"Dataset file not found: {fp!r}. "
                "Ensure city_datasets/ is populated."
            )
        with open(fp, "r", encoding="utf-8") as f:
            return json.load(f)

    def load_all_sites_in_radius(
        self,
        lat: float,
        lon: float,
        radius_km: float = 150.0,
        min_cities: int = 1,
    ) -> list[dict]:
        """
        Multi-region site loader — the core of geographic-first routing.

        Loads sites from EVERY city dataset whose centre falls within
        ``radius_km`` of the user's coordinates.  If that sweep returns
        fewer than ``min_cities`` cities, the nearest cities are added
        until the minimum is satisfied (guarantees at least one dataset).

        Returns a flat, deduplicated list of all raw site dicts.
        Callers should still run ``clean_site_data()`` on the result.
        """
        self._ensure_loaded()
        ordered = self.get_cities_by_proximity(lat, lon)

        loaded_files: set[str] = set()
        all_sites:   list[dict] = []
        cities_loaded = 0

        # Pass 1 — all cities within radius
        for entry in ordered:
            if entry["distance_km"] > radius_km:
                break
            fp = entry.get("filepath", "")
            if not fp or not os.path.exists(fp) or fp in loaded_files:
                continue
            try:
                with open(fp, "r", encoding="utf-8") as f:
                    batch = json.load(f)
                all_sites.extend(batch)
                loaded_files.add(fp)
                cities_loaded += 1
            except Exception:
                pass

        # Pass 2 — guarantee min_cities even beyond radius
        if cities_loaded < min_cities:
            for entry in ordered:
                if cities_loaded >= min_cities:
                    break
                fp = entry.get("filepath", "")
                if not fp or not os.path.exists(fp) or fp in loaded_files:
                    continue
                try:
                    with open(fp, "r", encoding="utf-8") as f:
                        batch = json.load(f)
                    all_sites.extend(batch)
                    loaded_files.add(fp)
                    cities_loaded += 1
                except Exception:
                    pass

        # Deduplicate by site name (case-insensitive)
        seen: set[str] = set()
        unique: list[dict] = []
        for s in all_sites:
            key = s.get("name", "").strip().lower()
            if key and key not in seen:
                seen.add(key)
                unique.append(s)

        return unique

