# Rafeeq Trip Planner — Complete Documentation

> **System name:** Rafeeq Trip Planner  
> **Engine:** KemetPath (ML + Beam Search)  
> **Purpose:** AI-powered single-day and multi-day itinerary generator for Egypt  
> **Coverage:** 18 governorates / regions, 250+ sites

---

## Table of Contents

1. [What the system does](#what-the-system-does)
2. [Quick start — run the API](#quick-start--run-the-api)
3. [API reference](#api-reference)
   - [POST /generate-trip (primary)](#post-generate-trip)
   - [POST /generate-multi-day-trip](#post-generate-multi-day-trip)
   - [GET /api/v1/health](#get-apiv1health)
   - [GET /api/v1/categories](#get-apiv1categories)
   - [GET /api/v1/cities](#get-apiv1cities)
   - [GET /api/v1/sites](#get-apiv1sites)
   - [POST /api/v1/generate-trip (full)](#post-apiv1generate-trip)
4. [Use the engine directly (no server)](#use-the-engine-directly-no-server)
5. [Budget system](#budget-system)
6. [Covered cities & regions](#covered-cities--regions)
7. [File guide — what each file does](#file-guide--what-each-file-does)
8. [Files you can safely delete](#files-you-can-safely-delete)
9. [Run the test suites](#run-the-test-suites)
10. [How the algorithm works](#how-the-algorithm-works)

---

## What the system does

Rafeeq takes a user's GPS location inside Egypt, their available time, category
preferences, and an optional ticket budget, and returns a realistic day-trip
itinerary optimised for:

- **Proximity** — nearby sites ranked first via exponential distance decay
- **Diversity** — Shannon entropy bonus prevents visiting the same category twice
- **ML duration** — RandomForest predicts visit length per site
- **Budget** — hard ticket-cost cap enforced at every beam-search step
- **Realism** — hard travel-time caps (low / medium / high tolerance)

---

## Quick start — run the API

### 1. Install dependencies

```bash
python -m pip install -r requirements.txt
```

### 2. Start the server

```bash
# from the project root
python -m uvicorn api.main:app --host 127.0.0.1 --port 8000
```

Server starts at **http://127.0.0.1:8000**

Interactive docs: **http://127.0.0.1:8000/docs** (Swagger UI)

### 3. Make your first request

```python
import requests

response = requests.post("http://127.0.0.1:8000/generate-trip", json={
    "start_lat": 30.0444,
    "start_lon": 31.2357,
    "available_hours": 4,
    "preferred_categories": ["Museum", "Historical Site"],
    "visited_sites": [],
    "walking_tolerance": "medium",
    "start_time": "09:00",
    "budget_amount": 300,
    "currency": "EGP"
})

print("STATUS:", response.status_code)
print("OUTPUT:", response.json())
```

---

## API reference

### POST /generate-trip

**The primary endpoint.** Call this from the mobile app or any HTTP client.

```
POST http://127.0.0.1:8000/generate-trip
Content-Type: application/json
```

#### Request body

| Field | Type | Required | Default | Description |
|---|---|---|---|---|
| `start_lat` | float | ✅ | — | GPS latitude (Egypt: 21–32.5) |
| `start_lon` | float | ✅ | — | GPS longitude (Egypt: 24–37.5) |
| `available_hours` | float | — | `6.0` | Trip duration in hours (0.5–24) |
| `preferred_categories` | list[str] | — | `[]` | Site types to prioritise |
| `visited_sites` | list[str] | — | `[]` | Site names to exclude |
| `walking_tolerance` | string | — | `"medium"` | `"low"` / `"medium"` / `"high"` |
| `start_time` | string | — | `"09:00"` | Trip start in HH:MM format |
| `budget_amount` | float | — | `0` | `0` = unlimited; `>0` = constrained |
| `currency` | string | ⚠️ if budget>0 | — | `"EGP"` or `"USD"` |

> **Walking tolerance**
> - `low` → max ~10 min travel per leg, tight cluster
> - `medium` → max ~25 min per leg
> - `high` → max ~45 min per leg, wide range

#### Minimal request (no budget)

```json
{
  "start_lat": 30.0444,
  "start_lon": 31.2357
}
```

#### Full request (with USD budget)

```json
{
  "start_lat": 25.6872,
  "start_lon": 32.6396,
  "available_hours": 6,
  "preferred_categories": ["Temple", "Historical Site"],
  "visited_sites": ["Karnak Temple"],
  "walking_tolerance": "high",
  "start_time": "08:00",
  "budget_amount": 10,
  "currency": "USD"
}
```

#### Success response (200)

```json
{
  "location": [30.0444, 31.2357],
  "city": "Cairo",
  "itinerary": [
    {
      "name": "Modern Egyptian Art Museum",
      "arrival_time": "09:02",
      "predicted_duration_minutes": 120.0,
      "travel_time_minutes": 2.5,
      "ticket_price_egp": 20.0,
      "category": "Museum",
      "zone": "Zamalek"
    },
    {
      "name": "House of Gamal al-Din al-Dhahabi",
      "arrival_time": "11:13",
      "predicted_duration_minutes": 82.0,
      "travel_time_minutes": 10.5,
      "ticket_price_egp": 30.0,
      "category": "Historical Site",
      "zone": "Al-Gamaliya"
    }
  ],
  "total_time_minutes": 215.0,
  "total_ticket_cost_egp": 50.0,
  "budget_limit_egp": 300.0,
  "currency": "EGP"
}
```

> `budget_limit_egp` is `null` when `budget_amount = 0` (unlimited mode).

#### Error responses

| Code | Cause | Body |
|---|---|---|
| 400 | Invalid input | `{"status":"error","message":"..."}` |
| 404 | No sites found / budget too tight | `{"itinerary":[],"reason":"..."}` |
| 500 | Engine crash | `{"detail":"..."}` |

#### Common 400 causes

```
budget_amount > 0 but currency missing
currency not "EGP" or "USD"
walking_tolerance not "low"/"medium"/"high"
start_time not HH:MM
budget_amount < 0
coordinates outside Egypt
```

---

### GET /api/v1/health

```
GET http://127.0.0.1:8000/api/v1/health
```

```json
{
  "status": "ok",
  "system": "Rafeeq Trip Planner",
  "version": "1.0.0",
  "model_ready": true,
  "cities_loaded": 18
}
```

---

### GET /api/v1/categories

```
GET http://127.0.0.1:8000/api/v1/categories
GET http://127.0.0.1:8000/api/v1/categories?city=Luxor
```

Returns all unique site categories (optionally filtered to one city).

```json
{
  "categories": [
    "Archaeological Site", "Church", "Historical Site",
    "Landmark", "Monument", "Mosque", "Museum",
    "Nature Reserve", "Palace", "Pyramid", "Temple"
  ]
}
```

---

### GET /api/v1/cities

```
GET http://127.0.0.1:8000/api/v1/cities
```

Returns all cities that have a dataset file.

```json
{
  "cities": ["Cairo", "Giza", "Alexandria", "Luxor", "Aswan", ...]
}
```

---

### GET /api/v1/sites

Browse the site catalogue with optional filters.

```
GET http://127.0.0.1:8000/api/v1/sites
GET http://127.0.0.1:8000/api/v1/sites?city=Luxor&category=Temple&has_coords=true
GET http://127.0.0.1:8000/api/v1/sites?max_price=50
```

| Query param | Type | Description |
|---|---|---|
| `city` | string | Filter to one city dataset |
| `category` | string | Exact category match |
| `zone` | string | Exact zone match |
| `has_coords` | bool | Only sites with GPS coordinates |
| `max_price` | float | Max ticket price in EGP |

---

### POST /api/v1/generate-trip

The **versioned** version of the same endpoint. Accepts the same body as
`POST /generate-trip` but returns a richer response with a `summary` block
and the `budget_used_percentage` field. See Swagger at `/docs` for full schema.

---

### POST /generate-multi-day-trip

**Multi-day planner.** Orchestrates N sequential calls to the single-day engine,
guaranteeing no duplicate site visits across days.

```
POST http://127.0.0.1:8000/generate-multi-day-trip
Content-Type: application/json
```

#### Request body

| Field | Type | Required | Default | Description |
|---|---|---|---|---|
| `start_lat` | float | ✅ | — | GPS latitude of trip start (Egypt: 21–32.5) |
| `start_lon` | float | ✅ | — | GPS longitude of trip start (Egypt: 24–37.5) |
| `days` | int | ✅ | — | Number of days (1–30) |
| `total_budget` | float | — | `0` | `0` = unlimited; `>0` = constrained (split evenly per day) |
| `currency` | string | ⚠️ if budget>0 | — | `"EGP"` or `"USD"` |
| `available_hours_per_day` | float | — | `6.0` | Hours available each day (0.5–24) |
| `start_time` | string | — | `"09:00"` | Trip start time each day (HH:MM) |
| `preferred_categories` | list[str] | — | `[]` | Site types to prioritise |
| `walking_tolerance` | string | — | `"medium"` | `"low"` / `"medium"` / `"high"` |

#### How budget is split

```
daily_budget = total_budget / days
```

Each day independently enforces `daily_budget`. If `total_budget = 0` (or
omitted), every day runs in **unlimited mode**.

#### How location rolls between days

At the end of each day, the start location for the **next day** is set to
the GPS coordinates of the **last stop** visited. This keeps the trip
geographically continuous.

#### No-duplicate guarantee

A global visited-set is maintained across all days. Any site added to Day 1
cannot appear on Day 2 or later. The engine enforces this by passing
`visited_sites` to `generate_trip()` on each call.

#### Fallback strategy (empty day)

If a day returns no results, two retries are attempted automatically:

| Pass | What changes |
|---|---|
| 1 | Drop `preferred_categories` — search all categories |
| 2 | Also clear `visited_sites` — last resort, may repeat a site |

The response includes `"fallback_used": true` and `"fallback_level": 1|2`
for any day where a retry was needed.

#### Example request

```json
{
  "start_lat": 30.0444,
  "start_lon": 31.2357,
  "days": 3,
  "total_budget": 900,
  "available_hours_per_day": 5,
  "start_time": "09:00",
  "preferred_categories": ["Museum", "Archaeological"],
  "walking_tolerance": "medium",
  "currency": "EGP"
}
```

#### Minimal request (unlimited budget)

```json
{
  "start_lat": 30.0444,
  "start_lon": 31.2357,
  "days": 3
}
```

#### Success response (200)

```json
{
  "trip_summary": {
    "total_days": 3,
    "total_sites_visited": 9,
    "total_ticket_cost_egp": 230.0,
    "total_budget_egp": 900.0,
    "daily_budget_egp": 300.0,
    "currency": "EGP"
  },
  "days": [
    {
      "day": 1,
      "city": "Cairo",
      "start_location": [30.0444, 31.2357],
      "itinerary": [
        {
          "name": "Madrasa of al-Mansur Qalawun",
          "arrival_time": "09:03",
          "predicted_duration_minutes": 89.0,
          "travel_time_minutes": 3.0,
          "ticket_price_egp": 0.0,
          "category": "Archaeological",
          "zone": "Al-Gamaliya",
          "latitude": 30.0486,
          "longitude": 31.2621
        }
      ],
      "day_ticket_cost_egp": 30.0,
      "day_budget_egp": 300.0,
      "total_time_minutes": 268.0,
      "fallback_used": false,
      "fallback_level": 0
    }
  ]
}
```

> When `total_budget = 0`, both `total_budget_egp` and `daily_budget_egp` are `null`.

#### Stop fields (same as single-day, plus GPS)

Each stop inside `days[n].itinerary` contains all the same fields as
`/generate-trip` stops, **plus**:

| Field | Type | Description |
|---|---|---|
| `latitude` | float \| null | GPS latitude of the site |
| `longitude` | float \| null | GPS longitude of the site |

#### Error responses

| Code | Cause | Body |
|---|---|---|
| 400 | Invalid input | `{"status":"error","message":"..."}` |
| 404 | No sites found for any day | `{"status":"error","reason":"...","days":[]}` |
| 500 | Engine crash | `{"detail":"..."}` |

#### Common 400 causes

```
days < 1 or days > 30
total_budget > 0 but currency missing
currency not "EGP" or "USD"
coordinates outside Egypt
```

#### Python example

```python
import requests

response = requests.post(
    "http://127.0.0.1:8000/generate-multi-day-trip",
    json={
        "start_lat": 30.0444,
        "start_lon": 31.2357,
        "days": 3,
        "total_budget": 900,
        "available_hours_per_day": 5,
        "start_time": "09:00",
        "preferred_categories": ["Museum", "Archaeological"],
        "walking_tolerance": "medium",
        "currency": "EGP",
    }
)

data = response.json()
print("Total sites:", data["trip_summary"]["total_sites_visited"])

for day in data["days"]:
    print(f"\n=== Day {day['day']} — {day['city']} ===")
    for stop in day["itinerary"]:
        print(f"  {stop['arrival_time']}  {stop['name']}  ({stop['category']})")
```

---

## Use the engine directly (no server)

You can call `generate_trip()` directly in Python without starting the API:

```python
from trip_optimizer import generate_trip

profile = {
    "start_lat":            30.0444,
    "start_lon":            31.2357,
    "available_hours":      4,
    "preferred_categories": ["Museum", "Historical Site"],
    "visited_sites":        [],
    "walking_tolerance":    "medium",
    "start_time":           "09:00",
    # budget: omit both fields (or budget_amount=0) for unlimited
    "budget_amount":        300,
    "currency":             "EGP",
}

itinerary = generate_trip(profile)

for stop in itinerary:
    print(stop["arrival_time"], stop["name"],
          stop["predicted_duration_minutes"], "min")
```

Each stop in the returned list is a dict with these keys:

| Key | Type | Description |
|---|---|---|
| `name` | str | Site name |
| `category` | str | Site category |
| `zone` | str | Neighbourhood / zone |
| `arrival_time` | str | HH:MM arrival |
| `predicted_duration_minutes` | float | ML-predicted visit length |
| `travel_time_minutes` | float | Travel time from previous stop |
| `ticket_price_egp` | int | Entrance fee in EGP |
| `latitude` / `longitude` | float | GPS coordinates |
| `cumulative_time_minutes` | float | Total time used so far |
| `leg_km` | float | Distance from previous stop |
| `fill_phase` | bool | True if added in the fill phase |
| `description` | str | Short site description |

---

## Budget system

| `budget_amount` | Behaviour |
|---|---|
| **0** (or omitted) | **Unlimited** — no price filtering at all |
| **> 0** | **Constrained** — total ticket cost ≤ budget; `currency` required |

**Currency conversion:** `1 USD = 50 EGP` (hardcoded in `trip_optimizer.py` as `USD_TO_EGP`).

**Where the budget is enforced:**

1. **Beam search** — each candidate stop is rejected if `spent + ticket_price > budget`
2. **Scoring** — expensive sites get a soft penalty; free sites get a small bonus
3. **Fill phase** — the budget-fill loop also checks the ticket cap before adding stops

---

## Covered cities & regions

| City / Region | Sites | Center |
|---|---|---|
| Cairo | 42 | 30.05°N 31.26°E |
| Alexandria | 26 | 31.19°N 29.89°E |
| Sinai | 26 | 28.55°N 34.21°E |
| North Red Sea | 24 | 27.17°N 33.84°E |
| Suez Canal Cities | 20 | 31.00°N 32.23°E |
| The New Valley | 20 | 25.66°N 29.21°E |
| Southern Red Sea | 14 | 24.93°N 34.95°E |
| Gharbia | 14 | 30.85°N 31.11°E |
| Marsa Matruh | 14 | 30.75°N 26.71°E |
| Fayoum | 10 | 29.34°N 30.66°E |
| Aswan | 10 | 24.18°N 32.73°E |
| Luxor | 9 | 25.67°N 32.61°E |
| Giza | 7 | 29.92°N 31.17°E |
| Qalyubia | 6 | 30.24°N 31.25°E |
| Sohag | 3 | 26.43°N 31.73°E |
| Sharqia | 2 | 30.77°N 31.70°E |
| Menoufia | 2 | 30.62°N 30.88°E |
| Qena | 1 | 26.14°N 32.67°E |

The engine loads **all cities within a 150 km radius** of the user's location,
so administrative boundaries never limit the itinerary.

---

## File guide — what each file does

### Core runtime files (never delete)

| File | Role |
|---|---|
| `trip_optimizer.py` | **Main engine.** Multi-region loading, beam search, ML scoring, budget enforcement. All logic lives here. |
| `city_resolver.py` | Maps GPS coordinates → nearest city. Loads and caches city index. |
| `duration_model.pkl` | Trained RandomForest model (8 MB). Predicts visit duration per site. |
| `api/main.py` | FastAPI app. Defines all HTTP endpoints. |
| `api/__init__.py` | Makes `api/` a Python package. |
| `requirements.txt` | Python dependency list. |

### Data files (never delete)

| File/Folder | Role |
|---|---|
| `city_datasets/` | **All site data.** One JSON per region + `cities_index.json`. |
| `city_datasets/cities_index.json` | Registry of all cities with centre coordinates and filenames. |
| `city_datasets/*_sites.json` | Individual region datasets (42 Cairo sites, 26 Alexandria sites, etc.). |

### Utility / tooling files (keep if you ever need to rebuild)

| File | Role |
|---|---|
| `train_duration_model.py` | Trains the RandomForest duration model and saves `duration_model.pkl`. Run when you add many new sites. |
| `generate_city_datasets.py` | Reads the raw CSV and generates all `city_datasets/*.json` files. Run when you add new cities to the spreadsheet. |
| `clean_and_enrich.py` | Cleans raw scraped data: fixes categories, assigns zones, normalises ticket prices. |
| `scrape_data.py` | Web scraper used during data collection. Not needed for runtime. |
| `Rafeeq Data Collection - Sites.csv` | Raw site data spreadsheet (source of truth for the dataset). |
| `Rafeeq Data Collection - Cities.csv` | Raw city data spreadsheet. |

### Test files (keep)

| File | Role |
|---|---|
| `test_api.py` | **API test suite** (151 checks). Covers single-day `/generate-trip` endpoint — budget modes, validation, response schema. Run with `python test_api.py`. |
| `test_cases.py` | **Engine test suite** — calls `generate_trip()` directly. Covers 10 city-center cases + 6 budget cases. Run with `python test_cases.py`. |
| `test_multi_day.py` | **Multi-day test suite** (59 checks). Tests `/generate-multi-day-trip` — no-duplicate guarantee, budget split, validation errors, fallback behaviour, edge cases. Run with `python test_multi_day.py`. |


---

## Run the test suites

### API tests (no server required)

```bash
python test_api.py
```

Expected output:

```
[Setup] City resolver primed — 18 cities.
...
  PASS: 151   FAIL: 0   TOTAL: 151
```

### Engine tests

```bash
python test_cases.py
```

Expected output:

```
TEST SUMMARY (City Centers)
PASS: 10   EMPTY: 0   FAIL: 0   TOTAL: 10

TEST SUMMARY (Budget Feature)
PASS: 6   EMPTY: 0   FAIL: 0   TOTAL: 6
```

### Multi-day endpoint tests (no server required)

```bash
python test_multi_day.py
```

Expected output:

```
-- T1: 3-day Cairo, 900 EGP total budget
  [PASS] HTTP 200
  [PASS] total_days == 3
  [PASS] daily_budget_egp == 300
  [PASS] no duplicate sites
  [PASS] day 1 cost 30.0 <= budget 300.0
  ...

-- T2: 2-day Cairo, unlimited budget
  [PASS] total_budget_egp None
  [PASS] no duplicate sites

-- T3: validation — budget without currency
  [PASS] HTTP 400

  PASS: 59   FAIL: 0   TOTAL: 59
```

---

## How the algorithm works

```
User input (lat, lon, hours, prefs, budget)
        │
        ▼
┌───────────────────────────────┐
│  Multi-region site loading    │  Load all city datasets within 150 km
│  (city_resolver + all JSON)   │  Merge into one candidate pool
└──────────────┬────────────────┘
               │
        ▼
┌───────────────────────────────┐
│  Soft category filtering      │  Try preferred categories first
│  Progressive radius expansion │  5 km → 10 km → 20 km → 40 km
│  Last-resort: nearest site    │  Always return ≥ 1 result
└──────────────┬────────────────┘
               │
        ▼
┌───────────────────────────────┐
│  ML duration prediction       │  RandomForest on size, crowd, category
│  (duration_model.pkl)         │  Per-site visit length in minutes
└──────────────┬────────────────┘
               │
        ▼
┌───────────────────────────────┐
│  Beam Search (K=5)            │  Explores top-5 candidate paths per step
│  Objective: J = value × decay │  Exponential travel penalty (exp(-t/20))
│  Diversity bonus (Shannon H)  │  Penalises repeated categories
│  Hard constraints:            │
│    • travel time cap per leg  │
│    • total walk km cap        │
│    • ticket budget cap        │  ← skips sites that bust the budget
└──────────────┬────────────────┘
               │
        ▼
┌───────────────────────────────┐
│  Budget fill phase            │  Greedily fills remaining time to 85%
│  3-pass: strict → relaxed     │  Also respects ticket budget cap
└──────────────┬────────────────┘
               │
        ▼
┌───────────────────────────────┐
│  Travel-time guard            │  Trim if cumulative travel > 40% of day
└──────────────┬────────────────┘
               │
        ▼
   Ordered itinerary list
```

### Key constants (in `trip_optimizer.py`)

| Constant | Value | Meaning |
|---|---|---|
| `MULTI_REGION_RADIUS_KM` | 150 | Radius to sweep city datasets |
| `BEAM_WIDTH` | 5 | Number of parallel beam paths |
| `USD_TO_EGP` | 50 | Currency conversion rate |
| `BUDGET_TARGET_PCT` | 0.85 | Fill phase target (85% of time) |
| `MAX_TRAVEL_BUDGET_RATIO` | 0.40 | Max fraction of day spent travelling |
| `MAX_SAME_CATEGORY` | 2 | Soft cap on same-category repeats |

---

*Documentation updated 2026-05-06 for Rafeeq Trip Planner v1.1.0 — multi-day trip planning added*
