"""
api/main.py — Rafeeq Trip Planner · FastAPI Backend
=====================================================
Production-ready REST API wrapping the KemetPath itinerary engine.

Base URL  : /api/v1/
Docs      : /docs  (Swagger)   |  /redoc (ReDoc)

Endpoints
---------
POST /api/v1/generate-trip  → Generate personalised itinerary
GET  /api/v1/health         → System + model status
GET  /api/v1/categories     → All known site categories
GET  /api/v1/cities         → Cities with datasets available
GET  /api/v1/sites          → Browse sites (with filters)

Run locally
-----------
    uvicorn api.main:app --reload --port 8000
"""

from __future__ import annotations

import json
import logging
import os
import sys
import time
from typing import List, Optional

# ── allow importing from parent directory ──────────────────────────────────────
_HERE    = os.path.dirname(os.path.abspath(__file__))
_ROOT    = os.path.dirname(_HERE)
if _ROOT not in sys.path:
    sys.path.insert(0, _ROOT)

from fastapi import FastAPI, HTTPException, Query, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from pydantic import BaseModel, Field, field_validator, model_validator

# ── constants ──────────────────────────────────────────────────────────────────
MODEL_FILE        = os.path.join(_ROOT, "duration_model.pkl")
CITY_DATASETS_DIR = os.path.join(_ROOT, "city_datasets")
API_VERSION       = "1.0.0"
SYSTEM_NAME       = "Rafeeq Trip Planner"

# ── logging ───────────────────────────────────────────────────────────────────
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s — %(message)s",
    datefmt="%H:%M:%S",
)
log = logging.getLogger("rafeeq.api")

# ── FastAPI app ────────────────────────────────────────────────────────────────
app = FastAPI(
    title=SYSTEM_NAME,
    description=(
        "AI-powered Egypt travel itinerary planner. "
        "Supports multi-region loading, ML duration prediction, "
        "beam-search route optimization, and optional budget constraints."
    ),
    version=API_VERSION,
    docs_url="/docs",
    redoc_url="/redoc",
    openapi_tags=[
        {"name": "Trip Planning", "description": "Core itinerary generation"},
        {"name": "System",        "description": "Health & diagnostics"},
        {"name": "Sites",         "description": "Site catalogue browsing"},
    ],
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],   # restrict to your mobile app domain in production
    allow_methods=["*"],
    allow_headers=["*"],
)

# ── singleton city resolver (created once at startup) ─────────────────────────
_city_resolver = None

@app.on_event("startup")
async def _startup():
    global _city_resolver
    try:
        from city_resolver import CityResolver
        _city_resolver = CityResolver(
            datasets_dir=CITY_DATASETS_DIR,
            index_file=os.path.join(CITY_DATASETS_DIR, "cities_index.json"),
        )
        cities = _city_resolver.list_cities()
        log.info("City resolver ready — %d cities available.", len(cities))
    except ImportError:
        log.warning("city_resolver not found — multi-city support disabled.")
    except Exception as exc:
        log.warning("City resolver init failed: %s", exc)


# ── request-level logging middleware ──────────────────────────────────────────
@app.middleware("http")
async def _log_requests(request: Request, call_next):
    t0 = time.perf_counter()
    response = await call_next(request)
    elapsed = (time.perf_counter() - t0) * 1000
    log.info("%s %s → %d  (%.0f ms)",
             request.method, request.url.path,
             response.status_code, elapsed)
    return response


# ══════════════════════════════════════════════════════════════════════════════
# Pydantic Schemas
# ══════════════════════════════════════════════════════════════════════════════

class TripRequest(BaseModel):
    """
    Input body for POST /api/v1/generate-trip.
    Maps directly to the generate_trip() user_profile dict.
    """
    # ── location ──────────────────────────────────────────────────────────────
    start_lat: float = Field(
        ..., ge=-90.0, le=90.0,
        description="GPS latitude of trip start",
        examples=[30.0478],
    )
    start_lon: float = Field(
        ..., ge=-180.0, le=180.0,
        description="GPS longitude of trip start",
        examples=[31.2336],
    )
    city: Optional[str] = Field(
        default=None,
        description="Optional city hint (e.g. 'Luxor'). Overrides coordinate-based resolution.",
        examples=["Cairo"],
    )

    # ── time ──────────────────────────────────────────────────────────────────
    available_hours: float = Field(
        default=6.0, ge=0.5, le=24.0,
        description="Time budget in hours (0.5–24)",
        examples=[6.0],
    )
    start_time: str = Field(
        default="09:00",
        description="Trip start time in HH:MM format",
        examples=["09:00"],
    )

    # ── preferences ───────────────────────────────────────────────────────────
    preferred_categories: List[str] = Field(
        default=[],
        description="Site categories the user prefers (e.g. ['Museum', 'Mosque'])",
        examples=[["Museum", "Historical Site"]],
    )
    visited_sites: List[str] = Field(
        default=[],
        description="Site names to exclude (already visited)",
        examples=[["Egyptian Museum"]],
    )
    walking_tolerance: str = Field(
        default="medium",
        description="Mobility range: 'low' (tight radius), 'medium', or 'high' (wide radius)",
        examples=["medium"],
    )

    # ── budget (implicit: 0 = unlimited, >0 = constrained) ───────────────────
    budget_amount: float = Field(
        default=0.0, ge=0,
        description=(
            "Ticket budget. "
            "0 = open / unlimited (no price filtering). "
            ">0 = constrained (requires currency)."
        ),
        examples=[300.0],
    )
    currency: Optional[str] = Field(
        default=None,
        description="Currency for budget_amount when >0: 'EGP' or 'USD'",
        examples=["EGP"],
    )

    # ── validators ────────────────────────────────────────────────────────────
    @field_validator("walking_tolerance")
    @classmethod
    def _check_tolerance(cls, v: str) -> str:
        v = v.strip().lower()
        if v not in ("low", "medium", "high"):
            raise ValueError("walking_tolerance must be 'low', 'medium', or 'high'")
        return v

    @field_validator("start_time")
    @classmethod
    def _check_time(cls, v: str) -> str:
        import re
        if not re.match(r"^\d{2}:\d{2}$", v):
            raise ValueError("start_time must be in HH:MM format (e.g. '09:30')")
        h, m = map(int, v.split(":"))
        if not (0 <= h <= 23 and 0 <= m <= 59):
            raise ValueError("start_time has invalid hour or minute value")
        return v

    @field_validator("preferred_categories")
    @classmethod
    def _check_categories(cls, v: List[str]) -> List[str]:
        if not all(isinstance(c, str) for c in v):
            raise ValueError("preferred_categories must be a list of strings")
        return v

    @model_validator(mode="after")
    def _check_budget_fields(self) -> "TripRequest":
        if self.budget_amount > 0:
            if self.currency is None:
                raise ValueError(
                    "currency is required when budget_amount > 0 (use 'EGP' or 'USD')"
                )
            if self.currency.upper() not in ("EGP", "USD"):
                raise ValueError("currency must be 'EGP' or 'USD'")
        return self

    # ── Egypt coordinate guard ─────────────────────────────────────────────────
    @model_validator(mode="after")
    def _check_egypt_bounds(self) -> "TripRequest":
        # Egypt bounding box (very rough): lat 21–32, lon 24–37
        if not (21.0 <= self.start_lat <= 32.5):
            raise ValueError(
                f"start_lat {self.start_lat} is outside Egypt's latitude range (21–32.5)"
            )
        if not (24.0 <= self.start_lon <= 37.5):
            raise ValueError(
                f"start_lon {self.start_lon} is outside Egypt's longitude range (24–37.5)"
            )
        return self


# ── response schemas ──────────────────────────────────────────────────────────

class StopOut(BaseModel):
    name: str
    arrival_time: str
    duration_minutes: float
    travel_time_minutes: float
    ticket_price: float
    category: str
    zone: str
    latitude: Optional[float] = None
    longitude: Optional[float] = None
    description: Optional[str] = None
    fill_phase: Optional[bool] = None


class SummaryOut(BaseModel):
    total_stops: int
    total_time_minutes: float
    total_ticket_cost: float         # always in EGP
    currency: str = "EGP"            # always EGP (all internal values normalised)
    budget_limit: Optional[float]    # EGP limit, or null when unlimited
    budget_used_percentage: Optional[float] = None   # only when budget_limit set
    budget_status: Optional[str] = None              # "within_budget" | "over_budget"
    start_time: str
    end_time: str


class TripResponseData(BaseModel):
    stops: List[StopOut]
    summary: SummaryOut


class TripResponse(BaseModel):
    status: str = "success"
    system: str = SYSTEM_NAME
    data: TripResponseData


class ErrorResponse(BaseModel):
    status: str = "error"
    message: str


class SiteOut(BaseModel):
    name: str
    category: str
    zone: str
    ticket_price: float
    size_score: int
    crowd_score: int
    latitude: Optional[float] = None
    longitude: Optional[float] = None
    description: str = ""


# ── shared helper ──────────────────────────────────────────────────────────────

def _get_sites_for_city(city: Optional[str]) -> list:
    """Load sites for browsing endpoints (categories/sites/zones)."""
    # Resolve a named city from the city_datasets/ directory
    if _city_resolver and city:
        try:
            info = _city_resolver.resolve(city_name=city)
            return _city_resolver.load_sites(info)
        except Exception:
            pass
    # Default: load Cairo from the modern city_datasets/ directory
    cairo_json = os.path.join(CITY_DATASETS_DIR, "cairo_sites.json")
    if os.path.exists(cairo_json):
        with open(cairo_json, "r", encoding="utf-8") as f:
            return json.load(f)
    return []


def _format_end_time(start_time: str, total_minutes: float) -> str:
    """Add total_minutes to start_time and return HH:MM string."""
    try:
        h, m = map(int, start_time.split(":"))
        total = h * 60 + m + int(total_minutes)
        return f"{(total // 60) % 24:02d}:{total % 60:02d}"
    except Exception:
        return "??"


# ══════════════════════════════════════════════════════════════════════════════
# Versioned Router  (/api/v1/)
# ══════════════════════════════════════════════════════════════════════════════

from fastapi import APIRouter
v1 = APIRouter(prefix="/api/v1")


# ── POST /api/v1/generate-trip ────────────────────────────────────────────────

@v1.post(
    "/generate-trip",
    response_model=TripResponse,
    responses={
        400: {"model": ErrorResponse, "description": "Invalid input"},
        404: {"model": ErrorResponse, "description": "No attractions found"},
        500: {"model": ErrorResponse, "description": "Internal engine error"},
    },
    tags=["Trip Planning"],
    summary="Generate a personalized itinerary",
)
def generate_trip_endpoint(body: TripRequest):
    """
    **Core endpoint** — called by the Rafeeq mobile app.

    Accepts the user's GPS location, time budget, preferences, and optional
    ticket budget, then returns a fully optimised daily itinerary.

    **Algorithm pipeline**
    1. Multi-region site loading — all datasets within 150 km radius  
    2. Soft category filtering with progressive radius expansion  
    3. ML duration prediction (RandomForest, trained on 300+ sites)  
    4. Beam-search route optimization (K=5)  
    5. Budget fill phase — maximises time usage up to 85%  
    6. Optional ticket-budget hard constraint at every step
    """
    # -- import engine lazily so startup errors surface cleanly ----------------
    try:
        from trip_optimizer import generate_trip, resolve_ticket_budget
    except ImportError as exc:
        log.error("Could not import trip_optimizer: %s", exc)
        raise HTTPException(status_code=500,
                            detail=f"Engine unavailable: {exc}")

    # -- build profile dict ----------------------------------------------------
    profile = {
        "start_lat":            body.start_lat,
        "start_lon":            body.start_lon,
        "city":                 body.city,
        "available_hours":      body.available_hours,
        "preferred_categories": body.preferred_categories,
        "visited_sites":        body.visited_sites,
        "walking_tolerance":    body.walking_tolerance,
        "start_time":           body.start_time,
        # budget: 0 = unlimited, >0 = constrained (engine reads these directly)
        "budget_amount":        body.budget_amount,
        "currency":             (body.currency or "EGP").upper(),
    }

    is_constrained = body.budget_amount > 0
    log.info(
        "[Rafeeq] generate-trip | (%.4f, %.4f) | %.1fh | tol=%s | budget=%s",
        body.start_lat, body.start_lon,
        body.available_hours, body.walking_tolerance,
        f"{body.budget_amount} {body.currency}" if is_constrained else "unlimited",
    )

    # -- run engine ------------------------------------------------------------
    try:
        raw = generate_trip(
            profile,
            model_path=MODEL_FILE,
            city_resolver=_city_resolver,
        )
    except Exception as exc:
        log.exception("Engine error: %s", exc)
        raise HTTPException(status_code=500,
                            detail=f"Trip generation failed: {exc}")

    if not raw:
        raise HTTPException(
            status_code=404,
            detail=(
                "No attractions found near your location. "
                "Try 'walking_tolerance': 'high', a longer available_hours, "
                "or remove preferred_categories to accept any type of site."
            ),
        )

    # -- normalise stops -------------------------------------------------------
    stops: List[StopOut] = []
    for s in raw:
        stops.append(StopOut(
            name=s.get("name", ""),
            arrival_time=s.get("arrival_time", ""),
            duration_minutes=round(s.get("predicted_duration_minutes", 0), 1),
            travel_time_minutes=round(s.get("travel_time_minutes", 0), 1),
            ticket_price=float(s.get("ticket_price_egp") or 0),
            category=s.get("category", ""),
            zone=s.get("zone", ""),
            latitude=s.get("latitude"),
            longitude=s.get("longitude"),
            description=(s.get("description") or "")[:300],
            fill_phase=s.get("fill_phase"),
        ))

    # -- compute summary -------------------------------------------------------
    total_cost        = sum(s.ticket_price for s in stops)
    total_time        = raw[-1].get("cumulative_time_minutes", 0)
    end_time          = _format_end_time(body.start_time, total_time)
    ticket_budget_egp = resolve_ticket_budget(profile)   # None = unlimited

    if ticket_budget_egp is not None:
        pct     = round(total_cost / ticket_budget_egp * 100, 1)
        bstatus = "within_budget" if total_cost <= ticket_budget_egp else "over_budget"
    else:
        pct     = None
        bstatus = None

    summary = SummaryOut(
        total_stops=len(stops),
        total_time_minutes=round(total_time, 1),
        total_ticket_cost=total_cost,
        currency="EGP",
        budget_limit=round(ticket_budget_egp, 2) if ticket_budget_egp is not None else 0,
        budget_used_percentage=pct,
        budget_status=bstatus,
        start_time=body.start_time,
        end_time=end_time,
    )

    log.info(
        "[Rafeeq] done | %d stops | %.0f min | %.0f EGP | budget=%s",
        len(stops), total_time, total_cost,
        f"{pct:.0f}% of {ticket_budget_egp:.0f} EGP" if ticket_budget_egp else "unlimited",
    )

    return TripResponse(
        data=TripResponseData(stops=stops, summary=summary)
    )


# ── GET /api/v1/health ────────────────────────────────────────────────────────

@v1.get(
    "/health",
    tags=["System"],
    summary="System health check",
)
def health():
    """Returns server status, model availability, and dataset count."""
    num_cities = len(_city_resolver.list_cities()) if _city_resolver else 0
    return {
        "status":        "ok",
        "system":        SYSTEM_NAME,
        "version":       API_VERSION,
        "model_ready":   os.path.exists(MODEL_FILE),
        "cities_loaded": num_cities,
    }


# ── GET /api/v1/categories ────────────────────────────────────────────────────

@v1.get(
    "/categories",
    tags=["Sites"],
    summary="List all site categories",
)
def list_categories(city: Optional[str] = Query(None, description="City name filter")):
    """Returns all unique site categories. Pass `city` to filter to one region."""
    sites = _get_sites_for_city(city)
    cats  = sorted({s.get("category", "") for s in sites if s.get("category")})
    return {"system": SYSTEM_NAME, "city": city, "categories": cats}


# ── GET /api/v1/cities ────────────────────────────────────────────────────────

@v1.get(
    "/cities",
    tags=["Sites"],
    summary="List all cities with datasets",
)
def list_cities():
    """Returns every city that has a dataset file in city_datasets/."""
    if _city_resolver is None:
        return {"system": SYSTEM_NAME, "cities": []}
    return {"system": SYSTEM_NAME, "cities": _city_resolver.list_cities()}


# ── GET /api/v1/sites ─────────────────────────────────────────────────────────

@v1.get(
    "/sites",
    response_model=List[SiteOut],
    tags=["Sites"],
    summary="Browse sites (with optional filters)",
)
def list_sites(
    city:       Optional[str]  = Query(None,  description="City to load (e.g. 'Luxor')"),
    category:   Optional[str]  = Query(None,  description="Filter by category"),
    zone:       Optional[str]  = Query(None,  description="Filter by zone"),
    has_coords: bool           = Query(False, description="Only return sites with GPS coordinates"),
    max_price:  Optional[float]= Query(None,  description="Max ticket price filter (EGP)"),
):
    """
    Browse all sites for a given city.
    Useful for map pin display in the mobile app.
    Defaults to Cairo when no city is given.
    """
    sites = _get_sites_for_city(city)
    if category:
        sites = [s for s in sites if s.get("category","").lower() == category.lower()]
    if zone:
        sites = [s for s in sites if s.get("zone","").lower() == zone.lower()]
    if has_coords:
        sites = [s for s in sites if s.get("latitude") and s.get("longitude")]
    if max_price is not None:
        sites = [s for s in sites if float(s.get("ticket_price") or 0) <= max_price]
    return [
        SiteOut(
            name=s.get("name",""),
            category=s.get("category",""),
            zone=s.get("zone",""),
            ticket_price=float(s.get("ticket_price") or 0),
            size_score=int(s.get("size_score") or 5),
            crowd_score=int(s.get("crowd_score") or 5),
            latitude=s.get("latitude"),
            longitude=s.get("longitude"),
            description=s.get("description",""),
        )
        for s in sites
    ]


# ── register router ───────────────────────────────────────────────────────────
app.include_router(v1)


# ── global validation-error handler (returns 400 instead of 422) ──────────────
from fastapi.exceptions import RequestValidationError

@app.exception_handler(RequestValidationError)
async def _validation_error(request: Request, exc: RequestValidationError):
    msgs = "; ".join(
        f"{' -> '.join(str(l) for l in e['loc'][1:])}: {e['msg']}"
        for e in exc.errors()
    )
    return JSONResponse(
        status_code=400,
        content={"status": "error", "message": f"Invalid input: {msgs}"},
    )


# ══════════════════════════════════════════════════════════════════════════════
# POST /generate-trip
# ──────────────────────────────────────────────────────────────────────────────
# Flat endpoint at the server root (http://127.0.0.1:8000/generate-trip).
# Schema matches generate_trip() 1-to-1.
# Response is the compact format expected by the mobile app and test scripts.
# ══════════════════════════════════════════════════════════════════════════════

class FlatTripRequest(BaseModel):
    """
    Minimal request body — mirrors the generate_trip() profile dict exactly.
    budget_amount = 0 (or omitted) → unlimited.
    budget_amount > 0              → constrained (currency required).
    """
    # ── required ──────────────────────────────────────────────────────────────
    start_lat:  float = Field(..., ge=-90,  le=90,
                              description="GPS latitude of trip start",
                              examples=[30.0444])
    start_lon:  float = Field(..., ge=-180, le=180,
                              description="GPS longitude of trip start",
                              examples=[31.2357])

    # ── optional with sensible defaults ───────────────────────────────────────
    available_hours:      float     = Field(default=6.0, ge=0.5, le=24.0,
                                            examples=[4])
    preferred_categories: List[str] = Field(default=[],
                                            examples=[["Museum", "Historical Site"]])
    visited_sites:        List[str] = Field(default=[],
                                            examples=[[]])
    walking_tolerance:    str       = Field(default="medium",
                                            examples=["medium"])
    start_time:           str       = Field(default="09:00",
                                            examples=["09:00"])

    # ── optional budget ────────────────────────────────────────────────────────
    budget_amount: float          = Field(default=0.0, ge=0,
                                          description="0 = unlimited",
                                          examples=[300])
    currency:      Optional[str]  = Field(default=None,
                                          description="Required when budget_amount > 0",
                                          examples=["EGP"])

    # ── validators ────────────────────────────────────────────────────────────
    @field_validator("walking_tolerance")
    @classmethod
    def _tol(cls, v: str) -> str:
        v = v.strip().lower()
        if v not in ("low", "medium", "high"):
            raise ValueError("walking_tolerance must be 'low', 'medium', or 'high'")
        return v

    @field_validator("start_time")
    @classmethod
    def _time(cls, v: str) -> str:
        import re
        if not re.match(r"^\d{2}:\d{2}$", v):
            raise ValueError("start_time must be HH:MM (e.g. '09:00')")
        h, m = map(int, v.split(":"))
        if not (0 <= h <= 23 and 0 <= m <= 59):
            raise ValueError("start_time is out of range")
        return v

    @model_validator(mode="after")
    def _budget(self) -> "FlatTripRequest":
        if self.budget_amount > 0:
            if not self.currency:
                raise ValueError(
                    "currency is required when budget_amount > 0 (use 'EGP' or 'USD')"
                )
            if self.currency.upper() not in ("EGP", "USD"):
                raise ValueError("currency must be 'EGP' or 'USD'")
        return self

    # ── Egypt bounds guard ────────────────────────────────────────────────────
    @model_validator(mode="after")
    def _egypt(self) -> "FlatTripRequest":
        if not (21.0 <= self.start_lat <= 32.5):
            raise ValueError(
                f"start_lat {self.start_lat} is outside Egypt (21–32.5)"
            )
        if not (24.0 <= self.start_lon <= 37.5):
            raise ValueError(
                f"start_lon {self.start_lon} is outside Egypt (24–37.5)"
            )
        return self


def _detect_city_name(lat: float, lon: float) -> str:
    """Return the nearest city name from the resolver, or 'Unknown'."""
    if _city_resolver is None:
        return "Unknown"
    try:
        info = _city_resolver.resolve(lat=lat, lon=lon)
        return info.get("city", "Unknown")
    except Exception:
        return "Unknown"


@app.post(
    "/generate-trip",
    tags=["Trip Planning"],
    summary="Generate a personalised itinerary (flat endpoint)",
    responses={
        400: {"description": "Invalid input"},
        404: {"description": "No sites found near location"},
        500: {"description": "Engine error"},
    },
)
def flat_generate_trip(body: FlatTripRequest):
    """
    **Primary endpoint** — call this from the mobile app or any HTTP client.

    URL: `POST http://127.0.0.1:8000/generate-trip`

    Schema mirrors `generate_trip()` exactly.  
    `budget_amount = 0` (or omit the field) → unlimited.  
    `budget_amount > 0` → constrained; **currency is required**.

    ### Example request
    ```json
    {
      "start_lat": 30.0444,
      "start_lon": 31.2357,
      "available_hours": 4,
      "preferred_categories": ["Museum", "Historical Site"],
      "visited_sites": [],
      "walking_tolerance": "medium",
      "start_time": "09:00",
      "budget_amount": 300,
      "currency": "EGP"
    }
    ```

    ### Example response
    ```json
    {
      "location": [30.0444, 31.2357],
      "city": "Cairo",
      "itinerary": [
        {
          "name": "Egyptian Museum",
          "arrival_time": "09:30",
          "predicted_duration_minutes": 90,
          "travel_time_minutes": 12,
          "ticket_price_egp": 200,
          "category": "Museum",
          "zone": "Downtown Cairo"
        }
      ],
      "total_time_minutes": 240,
      "total_ticket_cost_egp": 200,
      "budget_limit_egp": 300,
      "currency": "EGP"
    }
    ```
    """
    # lazy import so startup errors surface cleanly
    try:
        from trip_optimizer import generate_trip, resolve_ticket_budget
    except ImportError as exc:
        log.error("Could not import trip_optimizer: %s", exc)
        raise HTTPException(status_code=500, detail=f"Engine unavailable: {exc}")

    # build profile dict — identical to what you'd pass directly to generate_trip()
    profile = {
        "start_lat":            body.start_lat,
        "start_lon":            body.start_lon,
        "available_hours":      body.available_hours,
        "preferred_categories": body.preferred_categories,
        "visited_sites":        body.visited_sites,
        "walking_tolerance":    body.walking_tolerance,
        "start_time":           body.start_time,
        "budget_amount":        body.budget_amount,
        "currency":             (body.currency or "EGP").upper(),
    }

    is_constrained = body.budget_amount > 0
    log.info(
        "[Rafeeq] /generate-trip | (%.4f, %.4f) | %.1fh | tol=%s | budget=%s",
        body.start_lat, body.start_lon,
        body.available_hours, body.walking_tolerance,
        f"{body.budget_amount} {body.currency}" if is_constrained else "unlimited",
    )

    # run the engine
    try:
        raw = generate_trip(
            profile,
            model_path=MODEL_FILE,
            city_resolver=_city_resolver,
        )
    except Exception as exc:
        log.exception("Engine error: %s", exc)
        raise HTTPException(status_code=500,
                            detail=f"Trip generation failed: {exc}")

    # handle empty result
    if not raw:
        ticket_budget_egp = resolve_ticket_budget(profile)
        reason = (
            "No valid route within budget"
            if is_constrained
            else "No attractions found near your location"
        )
        return JSONResponse(
            status_code=404,
            content={
                "status":    "error",
                "itinerary": [],
                "reason":    reason,
            },
        )

    # build compact itinerary list
    itinerary = [
        {
            "name":                       s.get("name", ""),
            "arrival_time":               s.get("arrival_time", ""),
            "predicted_duration_minutes": round(s.get("predicted_duration_minutes", 0), 1),
            "travel_time_minutes":        round(s.get("travel_time_minutes", 0), 1),
            "ticket_price_egp":           float(s.get("ticket_price_egp") or 0),
            "category":                   s.get("category", ""),
            "zone":                       s.get("zone", ""),
        }
        for s in raw
    ]

    total_time = raw[-1].get("cumulative_time_minutes", 0)
    total_cost = sum(s["ticket_price_egp"] for s in itinerary)
    city_name  = _detect_city_name(body.start_lat, body.start_lon)

    # budget summary fields
    ticket_budget_egp = resolve_ticket_budget(profile)

    log.info(
        "[Rafeeq] /generate-trip done | city=%s | %d stops | %.0f min | %.0f EGP",
        city_name, len(itinerary), total_time, total_cost,
    )

    return {
        "location":           [body.start_lat, body.start_lon],
        "city":               city_name,
        "itinerary":          itinerary,
        "total_time_minutes": round(total_time, 1),
        "total_ticket_cost_egp": total_cost,
        "budget_limit_egp":   round(ticket_budget_egp, 2) if ticket_budget_egp else 0,
        "currency":           "EGP",
    }


# ==============================================================================
# POST /generate-multi-day-trip
# ------------------------------------------------------------------------------
# Orchestrates N sequential calls to generate_trip().
#
# Key guarantees
#   • No site is visited twice across days (global visited-set)
#   • Budget is split evenly per day; respects currency conversion
#   • Start location rolls to the last stop of each completed day
#   • Empty days get 2 fallback retries before being recorded as empty
# ==============================================================================

class MultiDayTripRequest(BaseModel):
    """
    Request body for multi-day trip planning.
    total_budget = 0 (or omit) → unlimited per day.
    total_budget > 0 → daily_budget = total_budget / days (currency required).
    """
    # ── required ──────────────────────────────────────────────────────────────
    start_lat: float = Field(..., ge=-90,  le=90,  examples=[30.0444])
    start_lon: float = Field(..., ge=-180, le=180, examples=[31.2357])
    days:      int   = Field(..., ge=1,    le=30,  examples=[3],
                             description="Number of days (1–30)")

    # ── optional with defaults ─────────────────────────────────────────────────
    total_budget:           float     = Field(default=0.0, ge=0,
                                              description="0 = unlimited",
                                              examples=[900])
    available_hours_per_day: float    = Field(default=6.0, ge=0.5, le=24.0,
                                              examples=[5])
    start_time:             str       = Field(default="09:00", examples=["09:00"])
    preferred_categories:   List[str] = Field(default=[], examples=[["Museum"]])
    walking_tolerance:      str       = Field(default="medium", examples=["medium"])
    currency:               Optional[str] = Field(default=None, examples=["EGP"])

    # ── validators ────────────────────────────────────────────────────────────
    @field_validator("walking_tolerance")
    @classmethod
    def _tol(cls, v: str) -> str:
        v = v.strip().lower()
        if v not in ("low", "medium", "high"):
            raise ValueError("walking_tolerance must be 'low', 'medium', or 'high'")
        return v

    @field_validator("start_time")
    @classmethod
    def _time(cls, v: str) -> str:
        import re
        if not re.match(r"^\d{2}:\d{2}$", v):
            raise ValueError("start_time must be HH:MM (e.g. '09:00')")
        h, m = map(int, v.split(":"))
        if not (0 <= h <= 23 and 0 <= m <= 59):
            raise ValueError("start_time is out of range")
        return v

    @model_validator(mode="after")
    def _budget(self) -> "MultiDayTripRequest":
        if self.total_budget > 0:
            if not self.currency:
                raise ValueError(
                    "currency is required when total_budget > 0 (use 'EGP' or 'USD')"
                )
            if self.currency.upper() not in ("EGP", "USD"):
                raise ValueError("currency must be 'EGP' or 'USD'")
        return self

    @model_validator(mode="after")
    def _egypt(self) -> "MultiDayTripRequest":
        if not (21.0 <= self.start_lat <= 32.5):
            raise ValueError(f"start_lat {self.start_lat} is outside Egypt (21-32.5)")
        if not (24.0 <= self.start_lon <= 37.5):
            raise ValueError(f"start_lon {self.start_lon} is outside Egypt (24-37.5)")
        return self


def _build_day_itinerary(stops: list) -> list:
    """Convert raw generate_trip() output into the compact stop format."""
    return [
        {
            "name":                       s.get("name", ""),
            "arrival_time":               s.get("arrival_time", ""),
            "predicted_duration_minutes": round(s.get("predicted_duration_minutes", 0), 1),
            "travel_time_minutes":        round(s.get("travel_time_minutes", 0), 1),
            "ticket_price_egp":           float(s.get("ticket_price_egp") or 0),
            "category":                   s.get("category", ""),
            "zone":                       s.get("zone", ""),
            "latitude":                   s.get("latitude"),
            "longitude":                  s.get("longitude"),
        }
        for s in stops
    ]


def _run_day(
    generate_trip,
    base_profile: dict,
    visited: set,
    fallback_pass: int = 0,
) -> list:
    """
    Call generate_trip() once and return the raw stop list.

    fallback_pass controls how much we relax constraints:
      0 → full constraints (visited set + category filter)
      1 → keep visited set but drop preferred_categories
      2 → also clear visited set (last resort)
    """
    profile = dict(base_profile)

    if fallback_pass == 0:
        profile["visited_sites"] = sorted(visited)
    elif fallback_pass == 1:
        profile["visited_sites"] = sorted(visited)
        profile["preferred_categories"] = []
    else:  # fallback_pass == 2
        profile["visited_sites"] = []
        profile["preferred_categories"] = []

    try:
        return generate_trip(
            profile,
            model_path=MODEL_FILE,
            city_resolver=_city_resolver,
        ) or []
    except Exception as exc:
        log.warning("[MultiDay] Engine error on day pass=%d: %s", fallback_pass, exc)
        return []


@app.post(
    "/generate-multi-day-trip",
    tags=["Trip Planning"],
    summary="Generate a multi-day personalised itinerary",
    responses={
        400: {"description": "Invalid input"},
        404: {"description": "No itinerary could be generated"},
        500: {"description": "Engine error"},
    },
)
def generate_multi_day_trip(body: MultiDayTripRequest):
    """
    **Multi-day trip planner** — orchestrates N sequential calls to the
    single-day engine, guaranteeing no duplicate site visits across days.

    URL: `POST http://127.0.0.1:8000/generate-multi-day-trip`

    ### Key behaviour
    - `total_budget` is split evenly per day (`daily_budget = total_budget / days`)
    - `total_budget = 0` (or omit) → unlimited budget every day
    - The start location rolls to the **last stop of each day** for the next day
    - A site visited on any day is excluded from all subsequent days
    - If a day returns empty, two fallback retries are attempted:
        1. Drop `preferred_categories` (wider search)
        2. Also clear `visited_sites` (last resort — may repeat a site)

    ### Example request
    ```json
    {
      "start_lat": 30.0444,
      "start_lon": 31.2357,
      "days": 3,
      "total_budget": 900,
      "available_hours_per_day": 5,
      "start_time": "09:00",
      "preferred_categories": ["Museum", "Historical"],
      "walking_tolerance": "medium",
      "currency": "EGP"
    }
    ```

    ### Example response
    ```json
    {
      "trip_summary": {
        "total_days": 3,
        "total_sites_visited": 9,
        "total_ticket_cost_egp": 540,
        "total_budget_egp": 900,
        "daily_budget_egp": 300,
        "currency": "EGP"
      },
      "days": [
        {
          "day": 1,
          "city": "Cairo",
          "start_location": [30.0444, 31.2357],
          "itinerary": [...],
          "day_ticket_cost_egp": 180,
          "day_budget_egp": 300,
          "total_time_minutes": 290,
          "fallback_used": false
        }
      ]
    }
    ```
    """
    try:
        from trip_optimizer import generate_trip, resolve_ticket_budget
    except ImportError as exc:
        log.error("Could not import trip_optimizer: %s", exc)
        raise HTTPException(status_code=500, detail=f"Engine unavailable: {exc}")

    currency   = (body.currency or "EGP").upper()
    is_budget  = body.total_budget > 0
    daily_budget = round(body.total_budget / body.days, 2) if is_budget else 0.0

    log.info(
        "[MultiDay] %d days | %.1fh/day | budget=%s | tol=%s | cats=%s",
        body.days, body.available_hours_per_day,
        f"{body.total_budget} {currency}" if is_budget else "unlimited",
        body.walking_tolerance,
        body.preferred_categories or "any",
    )

    # ── Shared state across days ───────────────────────────────────────────────
    global_visited: set  = set()
    current_lat:  float  = body.start_lat
    current_lon:  float  = body.start_lon
    trip_days:    list   = []
    grand_cost:   float  = 0.0

    for day_num in range(1, body.days + 1):
        day_start_lat = current_lat
        day_start_lon = current_lon

        base_profile = {
            "start_lat":            current_lat,
            "start_lon":            current_lon,
            "available_hours":      body.available_hours_per_day,
            "preferred_categories": list(body.preferred_categories),
            "walking_tolerance":    body.walking_tolerance,
            "start_time":           body.start_time,
            "budget_amount":        daily_budget,
            "currency":             currency,
        }

        # ── Try up to 3 passes (original → relax cats → clear visited) ─────────
        raw_stops   = []
        fallback_lvl = 0
        for attempt in range(3):
            raw_stops = _run_day(generate_trip, base_profile, global_visited, attempt)
            if raw_stops:
                fallback_lvl = attempt
                break
            log.info("[MultiDay] Day %d attempt %d returned empty — retrying", day_num, attempt + 1)

        # ── Build compact itinerary ────────────────────────────────────────────
        itinerary  = _build_day_itinerary(raw_stops)
        day_cost   = sum(s["ticket_price_egp"] for s in itinerary)
        total_time = raw_stops[-1].get("cumulative_time_minutes", 0) if raw_stops else 0
        city_name  = _detect_city_name(day_start_lat, day_start_lon)

        # ── Update global visited set ─────────────────────────────────────────
        for stop in itinerary:
            if stop["name"]:
                global_visited.add(stop["name"])

        grand_cost += day_cost

        # ── Roll start location to last stop with valid GPS ───────────────────
        for stop in reversed(raw_stops):
            slat = stop.get("latitude")
            slon = stop.get("longitude")
            if slat is not None and slon is not None:
                current_lat = slat
                current_lon = slon
                break
        # (if all stops lack GPS, keep current_lat/lon unchanged)

        trip_days.append({
            "day":                 day_num,
            "city":                city_name,
            "start_location":      [day_start_lat, day_start_lon],
            "itinerary":           itinerary,
            "day_ticket_cost_egp": round(day_cost, 2),
            "day_budget_egp":      round(daily_budget, 2) if is_budget else None,
            "total_time_minutes":  round(total_time, 1),
            "fallback_used":       fallback_lvl > 0,
            "fallback_level":      fallback_lvl,
        })

        log.info(
            "[MultiDay] Day %d done | city=%s | %d stops | %.0f min | %.0f EGP%s",
            day_num, city_name, len(itinerary), total_time, day_cost,
            f" [fallback={fallback_lvl}]" if fallback_lvl else "",
        )

    # ── Guard: every day was empty ─────────────────────────────────────────────
    total_stops = sum(len(d["itinerary"]) for d in trip_days)
    if total_stops == 0:
        return JSONResponse(
            status_code=404,
            content={
                "status": "error",
                "reason": "No attractions found near your location for any day",
                "days":   [],
            },
        )

    # ── Resolve total budget in EGP for summary ────────────────────────────────
    total_budget_egp: Optional[float] = None
    if is_budget:
        sample_profile = {"budget_amount": body.total_budget, "currency": currency}
        total_budget_egp = resolve_ticket_budget(sample_profile)

    log.info(
        "[MultiDay] Complete | %d days | %d total stops | %.0f EGP total cost",
        body.days, total_stops, grand_cost,
    )

    return {
        "trip_summary": {
            "total_days":            body.days,
            "total_sites_visited":   total_stops,
            "total_ticket_cost_egp": round(grand_cost, 2),
            "total_budget_egp":      round(total_budget_egp, 2) if total_budget_egp else None,
            "daily_budget_egp":      round(daily_budget, 2) if is_budget else None,
            "currency":              "EGP",
        },
        "days": trip_days,
    }
