"""
test_multi_day.py — Smoke tests for POST /generate-multi-day-trip
Run: python test_multi_day.py
"""
import json
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from fastapi.testclient import TestClient
import api.main as m
from city_resolver import CityResolver

# ── bootstrap ─────────────────────────────────────────────────────────────────
m._city_resolver = CityResolver(
    datasets_dir=m.CITY_DATASETS_DIR,
    index_file=os.path.join(m.CITY_DATASETS_DIR, "cities_index.json"),
)
client = TestClient(m.app, raise_server_exceptions=False)

PASS = 0
FAIL = 0


def check(label: str, condition: bool, detail: str = ""):
    global PASS, FAIL
    if condition:
        PASS += 1
        print(f"  [PASS] {label}")
    else:
        FAIL += 1
        print(f"  [FAIL] {label}" + (f" | {detail}" if detail else ""))


# ==============================================================================
# T1 — 3-day Cairo with EGP budget
# ==============================================================================
print("\n-- T1: 3-day Cairo, 900 EGP total budget --------------------------------")
r1 = client.post("/generate-multi-day-trip", json={
    "start_lat": 30.0444, "start_lon": 31.2357,
    "days": 3, "total_budget": 900,
    "available_hours_per_day": 5, "start_time": "09:00",
    "preferred_categories": ["Museum", "Archaeological"],
    "walking_tolerance": "medium", "currency": "EGP",
})
j1 = r1.json()
s1 = j1.get("trip_summary", {})

check("HTTP 200",                  r1.status_code == 200, str(r1.status_code))
check("has trip_summary",          "trip_summary" in j1)
check("has days list",             isinstance(j1.get("days"), list))
check("total_days == 3",           s1.get("total_days") == 3, str(s1.get("total_days")))
check("daily_budget_egp == 300",   s1.get("daily_budget_egp") == 300.0,
      str(s1.get("daily_budget_egp")))
check("currency == EGP",           s1.get("currency") == "EGP")
check("total_sites_visited > 0",   (s1.get("total_sites_visited") or 0) > 0,
      str(s1.get("total_sites_visited")))
check("3 day objects returned",    len(j1.get("days", [])) == 3)

# No duplicates across days
all_names = [x["name"] for d in j1.get("days", []) for x in d["itinerary"]]
dups = [n for n in set(all_names) if all_names.count(n) > 1]
check("no duplicate sites",        len(dups) == 0, f"dups={dups}")

# Each day has required keys
for day in j1.get("days", []):
    d = day["day"]
    check(f"day {d} has itinerary",       "itinerary" in day)
    check(f"day {d} has city",            "city" in day)
    check(f"day {d} has start_location",  "start_location" in day)
    check(f"day {d} has day_ticket_cost", "day_ticket_cost_egp" in day)
    check(f"day {d} has total_time",      "total_time_minutes" in day)
    check(f"day {d} has fallback_used",   "fallback_used" in day)
    # Each stop has required keys
    for stop in day.get("itinerary", []):
        for k in ("name", "arrival_time", "predicted_duration_minutes",
                  "travel_time_minutes", "ticket_price_egp", "category"):
            check(f"day {d} stop has '{k}'", k in stop)
        break  # only check first stop per day

# Budget respected per day (cost <= daily_budget + small rounding tolerance)
for day in j1.get("days", []):
    d   = day["day"]
    c   = day.get("day_ticket_cost_egp", 0)
    lim = day.get("day_budget_egp") or float("inf")
    check(f"day {d} cost {c} <= budget {lim}", c <= lim + 0.01,
          f"cost={c} limit={lim}")

print(f"\n  Sites: {all_names}")

# ==============================================================================
# T2 — 2-day, unlimited budget
# ==============================================================================
print("\n-- T2: 2-day Cairo, unlimited budget ------------------------------------")
r2 = client.post("/generate-multi-day-trip", json={
    "start_lat": 30.0444, "start_lon": 31.2357,
    "days": 2, "available_hours_per_day": 4,
    "walking_tolerance": "medium",
})
j2 = r2.json()
s2 = j2.get("trip_summary", {})

check("HTTP 200",               r2.status_code == 200, str(r2.status_code))
check("total_budget_egp None",  s2.get("total_budget_egp") is None,
      str(s2.get("total_budget_egp")))
check("daily_budget_egp None",  s2.get("daily_budget_egp") is None,
      str(s2.get("daily_budget_egp")))
all2 = [x["name"] for d in j2.get("days", []) for x in d["itinerary"]]
dups2 = [n for n in set(all2) if all2.count(n) > 1]
check("no duplicate sites",     len(dups2) == 0, f"dups={dups2}")

# ==============================================================================
# T3 — Validation: budget > 0 without currency → 400
# ==============================================================================
print("\n-- T3: validation — budget without currency ----------------------------")
r3 = client.post("/generate-multi-day-trip", json={
    "start_lat": 30.0444, "start_lon": 31.2357,
    "days": 2, "total_budget": 500,
})
check("HTTP 400",         r3.status_code == 400, str(r3.status_code))
check("error message",    "currency" in r3.json().get("message", "").lower(),
      r3.json().get("message",""))

# ==============================================================================
# T4 — Validation: days = 0 → 400
# ==============================================================================
print("\n-- T4: validation — days=0 ---------------------------------------------")
r4 = client.post("/generate-multi-day-trip", json={
    "start_lat": 30.0444, "start_lon": 31.2357, "days": 0,
})
check("HTTP 400", r4.status_code == 400, str(r4.status_code))

# ==============================================================================
# T5 — Validation: out of Egypt → 400
# ==============================================================================
print("\n-- T5: validation — coords outside Egypt --------------------------------")
r5 = client.post("/generate-multi-day-trip", json={
    "start_lat": 99.0, "start_lon": 31.2357, "days": 2,
})
check("HTTP 400", r5.status_code == 400, str(r5.status_code))

# ==============================================================================
# T6 — 1-day trip (edge case: same as single-day)
# ==============================================================================
print("\n-- T6: 1-day trip (edge case) -------------------------------------------")
r6 = client.post("/generate-multi-day-trip", json={
    "start_lat": 25.6872, "start_lon": 32.6396,
    "days": 1, "available_hours_per_day": 6,
    "preferred_categories": ["Archaeological", "Temple"],
    "walking_tolerance": "high",
})
j6 = r6.json()
check("HTTP 200",           r6.status_code == 200, str(r6.status_code))
check("exactly 1 day",      len(j6.get("days", [])) == 1)
check("total_days == 1",    j6.get("trip_summary", {}).get("total_days") == 1)

# ==============================================================================
# Summary
# ==============================================================================
total = PASS + FAIL
print(f"\n{'='*60}")
print(f"  PASS: {PASS}   FAIL: {FAIL}   TOTAL: {total}")
print(f"{'='*60}")
sys.exit(0 if FAIL == 0 else 1)
