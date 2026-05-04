#!/usr/bin/env python3
"""
test_city_centers_real.py
========================
Validates KemetPath / trip_optimizer behavior in REAL city centers.

Goals:
- Ensure multi-region loading works in dense areas
- Ensure filtering does NOT collapse results
- Ensure beam search produces multi-stop itineraries
"""

from trip_optimizer import generate_trip


def run_test_case(name, profile, expect_non_empty=True):
    print("=" * 80)
    print(f"TEST CASE: {name}")
    print("=" * 80)

    try:
        itinerary = generate_trip(profile)

        if not itinerary:
            status = "FAIL" if expect_non_empty else "EMPTY"
            print(f"[{status}] No itinerary generated.\n")
            return status

        total_minutes = 0

        print(f"[PASS ] {len(itinerary)} stop(s) generated")

        # 🔴 sanity flags
        if len(itinerary) < 2:
            print("[WARNING] Only 1 stop — possible filtering issue")

        for stop in itinerary:
            arrival = stop.get("arrival_time", "??:??")
            site_name = stop.get("name", "Unknown")
            duration = stop.get("predicted_duration_minutes", 0)
            travel = stop.get("travel_time_minutes", stop.get("travel_minutes", "?"))

            print(f"[{arrival}] {site_name} — {duration:.0f} min | travel={travel}")

            # 🚨 detect insane travel
            if isinstance(travel, (int, float)) and travel > profile["available_hours"] * 60:
                print(f"[WARNING] Travel time exceeds total budget: {travel} min")

            total_minutes += duration

        print(f"Total Planned Visit Time: {total_minutes:.0f} minutes")
        print(f"Total Planned Hours: {total_minutes / 60:.2f} hours\n")

        return "PASS"

    except Exception as e:
        print(f"[FAIL ] Error while running test case '{name}': {e}\n")
        return "FAIL"


if __name__ == "__main__":

    # ✅ CITY CENTER TEST CASES
    test_cases = {

        "City Center — Cairo Downtown": {
            "start_lat": 30.0444,
            "start_lon": 31.2357,
            "available_hours": 4,
            "preferred_categories": ["Museum", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
        },

        "City Center — Giza Plateau": {
            "start_lat": 29.9792,
            "start_lon": 31.1342,
            "available_hours": 4,
            "preferred_categories": ["Historical Site", "Monument"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
        },

        "City Center — Alexandria": {
            "start_lat": 31.2001,
            "start_lon": 29.9187,
            "available_hours": 4,
            "preferred_categories": ["Museum", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "10:00",
        },

        "City Center — Luxor East Bank": {
            "start_lat": 25.6872,
            "start_lon": 32.6396,
            "available_hours": 5,
            "preferred_categories": ["Temple", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "high",
            "start_time": "08:00",
        },

        "City Center — Aswan": {
            "start_lat": 24.0889,
            "start_lon": 32.8998,
            "available_hours": 4,
            "preferred_categories": ["Historical Site", "Museum"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
        },

        "City Center — Mansoura": {
            "start_lat": 31.0409,
            "start_lon": 31.3785,
            "available_hours": 3,
            "preferred_categories": ["Museum"],
            "visited_sites": [],
            "walking_tolerance": "low",
            "start_time": "10:00",
        },

        "City Center — Tanta": {
            "start_lat": 30.7865,
            "start_lon": 31.0004,
            "available_hours": 3,
            "preferred_categories": ["Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "low",
            "start_time": "10:00",
        },

        "City Center — Suez": {
            "start_lat": 29.9668,
            "start_lon": 32.5498,
            "available_hours": 4,
            "preferred_categories": ["Museum", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:30",
        },

        "City Center — Port Said": {
            "start_lat": 31.2653,
            "start_lon": 32.3019,
            "available_hours": 4,
            "preferred_categories": ["Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
        },

        "City Center — Minya": {
            "start_lat": 28.1099,
            "start_lon": 30.7503,
            "available_hours": 4,
            "preferred_categories": ["Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
        },
    }

    results = {"PASS": 0, "EMPTY": 0, "FAIL": 0}

    for name, profile in test_cases.items():
        # All city centers → must NOT be empty
        status = run_test_case(name, profile, expect_non_empty=True)
        results[status] += 1

    print("=" * 80)
    print("TEST SUMMARY (City Centers)")
    print("=" * 80)
    print(f"PASS: {results['PASS']}   EMPTY: {results['EMPTY']}   FAIL: {results['FAIL']}   TOTAL: {len(test_cases)}")
    print("=" * 80)

    # =========================================================================
    # BUDGET FEATURE TEST CASES
    # =========================================================================
    print("\n" + "=" * 80)
    print("BUDGET FEATURE TESTS")
    print("=" * 80)

    budget_tests = {

        # 1. No budget field at all — unlimited (existing behaviour unchanged)
        "Budget: unlimited (no field)": {
            "start_lat": 30.0478,
            "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Museum", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
        },

        # 2. budget_amount=0 explicitly — unlimited (open mode)
        "Budget: 0 EGP (open / unlimited)": {
            "start_lat": 30.0478,
            "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Museum", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
            "budget_amount": 0,
        },

        # 3. Tight EGP budget (50 EGP) — forces cheap / free sites
        "Budget: 50 EGP tight (Cairo)": {
            "start_lat": 30.0478,
            "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Historical Site", "Mosque"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
            "budget_amount": 50,
            "currency": "EGP",
        },

        # 4. Mid-range EGP budget (300 EGP) — allows some paid sites
        "Budget: 300 EGP (Cairo museums)": {
            "start_lat": 30.0478,
            "start_lon": 31.2336,
            "available_hours": 5,
            "preferred_categories": ["Museum", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "09:00",
            "budget_amount": 300,
            "currency": "EGP",
        },

        # 5. USD budget — 6 USD = 300 EGP at 50x rate
        "Budget: 6 USD (= 300 EGP, Cairo)": {
            "start_lat": 30.0478,
            "start_lon": 31.2336,
            "available_hours": 4,
            "preferred_categories": ["Museum", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "medium",
            "start_time": "10:00",
            "budget_amount": 6,
            "currency": "USD",
        },

        # 6. USD budget in Luxor
        "Budget: 10 USD (= 500 EGP, Luxor temples)": {
            "start_lat": 25.6872,
            "start_lon": 32.6396,
            "available_hours": 6,
            "preferred_categories": ["Temple", "Historical Site"],
            "visited_sites": [],
            "walking_tolerance": "high",
            "start_time": "08:00",
            "budget_amount": 10,
            "currency": "USD",
        },
    }

    budget_results = {"PASS": 0, "EMPTY": 0, "FAIL": 0}
    for name, profile in budget_tests.items():
        status = run_test_case(name, profile, expect_non_empty=True)
        budget_results[status] += 1

    print("=" * 80)
    print("TEST SUMMARY (Budget Feature)")
    print("=" * 80)
    print(f"PASS: {budget_results['PASS']}   EMPTY: {budget_results['EMPTY']}   "
          f"FAIL: {budget_results['FAIL']}   TOTAL: {len(budget_tests)}")
    print("=" * 80)
