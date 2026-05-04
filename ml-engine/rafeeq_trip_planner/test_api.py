#!/usr/bin/env python3
"""
test_api.py — Rafeeq Trip Planner API Tests
============================================
Uses FastAPI's built-in TestClient (no server needed).

Run:
    python test_api.py
"""

import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from fastapi.testclient import TestClient
import api.main as _api_module
from api.main import app

# ── Prime the city resolver before tests run ──────────────────────────────────
# TestClient does NOT fire on_event("startup") in sync mode, so we do it manually.
try:
    from city_resolver import CityResolver
    _api_module._city_resolver = CityResolver(
        datasets_dir=_api_module.CITY_DATASETS_DIR,
        index_file=os.path.join(_api_module.CITY_DATASETS_DIR, "cities_index.json"),
    )
    print(f"[Setup] City resolver primed — {len(_api_module._city_resolver.list_cities())} cities.")
except Exception as exc:
    print(f"[Setup] City resolver unavailable: {exc}")

client = TestClient(app, raise_server_exceptions=False)

PASS = "[PASS]"
FAIL = "[FAIL]"
results = {"pass": 0, "fail": 0}


def check(label: str, cond: bool, detail: str = ""):
    tag = PASS if cond else FAIL
    msg = f"{tag}  {label}"
    if not cond and detail:
        msg += f"\n       {detail}"
    print(msg)
    if cond:
        results["pass"] += 1
    else:
        results["fail"] += 1


# ==============================================================================
# System endpoints
# ==============================================================================

def test_health():
    r = client.get("/api/v1/health")
    check("GET /api/v1/health -> 200", r.status_code == 200)
    j = r.json()
    check("health.status == 'ok'", j.get("status") == "ok", str(j))
    check("health.model_ready present", "model_ready" in j, str(j))
    check("health.cities_loaded present", "cities_loaded" in j, str(j))


def test_categories():
    r = client.get("/api/v1/categories")
    check("GET /api/v1/categories -> 200", r.status_code == 200)
    j = r.json()
    check("categories is list", isinstance(j.get("categories"), list), str(j))
    check("categories non-empty", len(j.get("categories", [])) > 0, str(j))


def test_cities():
    r = client.get("/api/v1/cities")
    check("GET /api/v1/cities -> 200", r.status_code == 200)
    j = r.json()
    check("cities is list", isinstance(j.get("cities"), list), str(j))


def test_sites():
    r = client.get("/api/v1/sites")
    check("GET /api/v1/sites -> 200", r.status_code == 200)
    check("sites is list", isinstance(r.json(), list))

    r2 = client.get("/api/v1/sites?has_coords=true")
    check("GET /api/v1/sites?has_coords=true -> 200", r2.status_code == 200)
    for site in r2.json():
        if site.get("latitude") is None:
            check("all sites have latitude when has_coords=true", False, site["name"])
            return
    check("all sites have latitude when has_coords=true", True)


# ==============================================================================
# Input validation (400 errors)
# ==============================================================================

def test_validation_errors():
    # missing required latitude
    r = client.post("/api/v1/generate-trip", json={
        "start_lon": 31.2336,
        "available_hours": 4,
    })
    check("Missing start_lat -> 400", r.status_code == 400, str(r.json()))

    # out-of-Egypt coords
    r = client.post("/api/v1/generate-trip", json={
        "start_lat": 99.0, "start_lon": 31.2336,
        "available_hours": 4,
    })
    check("start_lat=99 (out of Egypt) -> 400", r.status_code == 400, str(r.json()))

    # bad walking_tolerance
    r = client.post("/api/v1/generate-trip", json={
        "start_lat": 30.0478, "start_lon": 31.2336,
        "walking_tolerance": "superfast",
    })
    check("Invalid walking_tolerance -> 400", r.status_code == 400, str(r.json()))

    # negative budget_amount
    r = client.post("/api/v1/generate-trip", json={
        "start_lat": 30.0478, "start_lon": 31.2336,
        "budget_amount": -50,
    })
    check("budget_amount=-50 -> 400", r.status_code == 400, str(r.json()))

    # budget_amount > 0 without currency
    r = client.post("/api/v1/generate-trip", json={
        "start_lat": 30.0478, "start_lon": 31.2336,
        "budget_amount": 200,
    })
    check("budget_amount>0 without currency -> 400", r.status_code == 400, str(r.json()))

    # budget_amount > 0 with invalid currency
    r = client.post("/api/v1/generate-trip", json={
        "start_lat": 30.0478, "start_lon": 31.2336,
        "budget_amount": 100,
        "currency": "EUR",
    })
    check("budget_amount>0 with currency=EUR -> 400", r.status_code == 400, str(r.json()))

    # bad start_time
    r = client.post("/api/v1/generate-trip", json={
        "start_lat": 30.0478, "start_lon": 31.2336,
        "start_time": "9am",
    })
    check("Invalid start_time -> 400", r.status_code == 400, str(r.json()))

    # error body shape check
    r = client.post("/api/v1/generate-trip", json={
        "start_lat": 30.0478, "start_lon": 31.2336,
        "walking_tolerance": "invalid_value",
    })
    j = r.json()
    check("Error body has 'status' field", "status" in j, str(j))
    check("Error body has 'message' field", "message" in j, str(j))
    check("Error body status == 'error'", j.get("status") == "error", str(j))


# ==============================================================================
# Core trip generation helpers
# ==============================================================================

def _post_trip(payload: dict):
    return client.post("/api/v1/generate-trip", json=payload)


def _assert_trip(label: str, r, budget_egp: float = None):
    """
    budget_egp: if not None, verify total_ticket_cost <= budget_egp
    """
    check(f"{label} -> 200", r.status_code == 200, r.text[:300])
    if r.status_code != 200:
        return

    j = r.json()
    check(f"{label} status=success", j.get("status") == "success", str(j)[:200])
    check(f"{label} system=Rafeeq Trip Planner", j.get("system") == "Rafeeq Trip Planner")

    data    = j.get("data", {})
    stops   = data.get("stops", [])
    summary = data.get("summary", {})

    check(f"{label} stops is list", isinstance(stops, list))
    check(f"{label} >= 1 stop", len(stops) >= 1, str(stops)[:200])

    # stop schema
    if stops:
        s = stops[0]
        for field in ("name", "arrival_time", "duration_minutes",
                      "travel_time_minutes", "ticket_price", "category", "zone"):
            check(f"{label} stop[0].{field}", field in s,
                  f"missing '{field}' in {list(s.keys())}")

    # summary schema
    for field in ("total_stops", "total_time_minutes", "total_ticket_cost",
                  "currency", "budget_limit", "start_time", "end_time"):
        check(f"{label} summary.{field}", field in summary,
              f"missing '{field}' in {list(summary.keys())}")

    check(f"{label} summary.currency == 'EGP'",
          summary.get("currency") == "EGP", str(summary))

    # budget constraint enforcement
    if budget_egp is not None:
        cost    = summary.get("total_ticket_cost", 999999)
        bstatus = summary.get("budget_status")
        blimit  = summary.get("budget_limit")
        check(f"{label} budget_limit set", blimit is not None, str(summary))
        check(f"{label} total_ticket_cost <= budget",
              cost <= budget_egp,
              f"cost={cost} > budget={budget_egp}")
        check(f"{label} budget_status=within_budget",
              bstatus == "within_budget",
              f"budget_status={bstatus}, cost={cost}, limit={blimit}")

    # unlimited: budget_limit must be null
    if budget_egp is None:
        blimit = summary.get("budget_limit")
        check(f"{label} budget_limit is null (unlimited)",
              blimit is None, f"budget_limit={blimit}")


# ==============================================================================
# Trip generation scenarios
# ==============================================================================

def test_trip_unlimited():
    """budget_amount omitted (default 0) = unlimited."""
    _assert_trip(
        "Cairo unlimited budget",
        _post_trip({
            "start_lat": 30.0478, "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Museum", "Historical Site"],
            "walking_tolerance": "medium",
            "start_time": "09:00",
        }),
        budget_egp=None,
    )


def test_trip_explicit_zero():
    """budget_amount=0 explicitly = unlimited."""
    _assert_trip(
        "Cairo budget_amount=0 (unlimited)",
        _post_trip({
            "start_lat": 30.0478, "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Museum"],
            "walking_tolerance": "medium",
            "start_time": "09:00",
            "budget_amount": 0,
        }),
        budget_egp=None,
    )


def test_trip_egp_constrained():
    """300 EGP constrained budget."""
    _assert_trip(
        "Cairo 300 EGP constrained",
        _post_trip({
            "start_lat": 30.0478, "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Historical Site", "Mosque"],
            "walking_tolerance": "medium",
            "start_time": "09:00",
            "budget_amount": 300,
            "currency": "EGP",
        }),
        budget_egp=300,
    )


def test_trip_usd_constrained():
    """6 USD = 300 EGP at 50x rate."""
    _assert_trip(
        "Cairo 6 USD (=300 EGP) constrained",
        _post_trip({
            "start_lat": 30.0478, "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Museum", "Historical Site"],
            "walking_tolerance": "medium",
            "start_time": "09:00",
            "budget_amount": 6,
            "currency": "USD",
        }),
        budget_egp=300,
    )


def test_trip_tight_egp():
    """50 EGP — only very cheap or free sites."""
    r = _post_trip({
        "start_lat": 30.0478, "start_lon": 31.2336,
        "available_hours": 4,
        "preferred_categories": [],
        "walking_tolerance": "medium",
        "start_time": "09:00",
        "budget_amount": 50,
        "currency": "EGP",
    })
    if r.status_code == 200:
        j = r.json()
        cost = j["data"]["summary"].get("total_ticket_cost", 0)
        check("50 EGP tight — cost <= 50", cost <= 50, f"cost={cost}")
    else:
        # acceptable if no sites fit the budget
        check("50 EGP tight -> 200 or 404", r.status_code == 404)


def test_trip_luxor():
    _assert_trip(
        "Luxor temples unlimited",
        _post_trip({
            "start_lat": 25.6872, "start_lon": 32.6396,
            "available_hours": 6,
            "preferred_categories": ["Temple", "Historical Site"],
            "walking_tolerance": "high",
            "start_time": "08:00",
        }),
    )


def test_trip_sparse_region():
    """Western Desert — sparse, expect 200 or 404 (no crash)."""
    r = _post_trip({
        "start_lat": 27.2, "start_lon": 28.5,
        "available_hours": 8,
        "preferred_categories": [],
        "walking_tolerance": "high",
        "start_time": "07:00",
    })
    check("Sparse region -> 200 or 404", r.status_code in (200, 404),
          f"unexpected {r.status_code}: {r.text[:200]}")


def test_full_response_structure():
    """Verify every contract field is present in the response."""
    r = _post_trip({
        "start_lat": 30.0478, "start_lon": 31.2336,
        "available_hours": 4,
        "walking_tolerance": "medium",
        "start_time": "10:00",
        "budget_amount": 500,
        "currency": "EGP",
    })
    check("Structure test -> 200", r.status_code == 200, r.text[:200])
    if r.status_code != 200:
        return

    j = r.json()
    # top-level
    for k in ("status", "system", "data"):
        check(f"top-level '{k}'", k in j)
    # data
    data = j["data"]
    check("data.stops", "stops" in data)
    check("data.summary", "summary" in data)
    # summary — spec-mandated fields
    s = data["summary"]
    for k in ("total_stops", "total_time_minutes", "total_ticket_cost",
              "budget_limit", "currency",
              "budget_used_percentage", "budget_status",
              "start_time", "end_time"):
        check(f"summary.{k}", k in s, f"missing in {list(s.keys())}")
    # currency always EGP
    check("summary.currency == EGP", s.get("currency") == "EGP")
    # budget_limit is the EGP value (500)
    check("summary.budget_limit == 500.0", s.get("budget_limit") == 500.0,
          f"got {s.get('budget_limit')}")


# ==============================================================================
# Runner
# ==============================================================================

if __name__ == "__main__":
    print("\n" + "=" * 70)
    print("  Rafeeq Trip Planner — API Test Suite")
    print("=" * 70)

    suites = [
        ("System endpoints",  [test_health, test_categories, test_cities, test_sites]),
        ("Input validation",  [test_validation_errors]),
        ("Trip generation",   [test_trip_unlimited, test_trip_explicit_zero,
                               test_trip_egp_constrained, test_trip_usd_constrained,
                               test_trip_tight_egp, test_trip_luxor,
                               test_trip_sparse_region, test_full_response_structure]),
    ]

    for suite_name, tests in suites:
        print(f"\n-- {suite_name} " + "-" * (50 - len(suite_name)))
        for t in tests:
            t()

    print("\n" + "=" * 70)
    print(f"  PASS: {results['pass']}   FAIL: {results['fail']}   "
          f"TOTAL: {results['pass'] + results['fail']}")
    print("=" * 70 + "\n")
    sys.exit(0 if results["fail"] == 0 else 1)
