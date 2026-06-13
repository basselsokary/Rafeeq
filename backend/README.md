# Rafeeq Backend API

> **Rafeeq** (رفيق) — *Companion* in Arabic. A location-based mobile tourist guide for Egypt, helping tourists discover sites, plan trips, and explore cultural artifacts.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=flat-square&logo=dotnet)](https://learn.microsoft.com/aspnet/core)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Azure](https://img.shields.io/badge/Azure-App_Service-0078D4?style=flat-square&logo=microsoftazure)](https://azure.microsoft.com)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

---

## Table of Contents

- [Overview](#overview)
- [System Architecture](#system-architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Domain Model](#domain-model)
- [Key Features](#key-features)
- [Authentication & Authorization](#authentication--authorization)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database](#database)
- [Deployment](#deployment)
- [Contributing](#contributing)

---

## Overview

Rafeeq is a graduation project — a full-stack tourism application for Egypt consisting of:

| Component | Technology | Purpose |
|---|---|---|
| **Mobile App** | Flutter | Tourist-facing app |
| **Admin Panel** | React + Vite | Admin & moderator dashboard |
| **Backend API** | ASP.NET Core (.NET 8) | This repository |
| **Scanner ML Service** | Python FastAPI | Artifact image recognition |
| **Trip ML Service** | Python FastAPI | Optimized trip planning |

This repository contains the **ASP.NET Core Web API** — the central backend serving both the Flutter mobile app and the React admin panel from a single codebase with dual authentication schemes.

---

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                            │
│                                                                 │
│   Flutter Mobile App            React + Vite Admin Panel        │
│   JWT Bearer · Google Sign-In   HttpOnly Cookie · Axios         │
└──────────────────┬──────────────────────────┬───────────────────┘
                   │ HTTPS                    │ HTTPS
┌──────────────────▼──────────────────────────▼───────────────────┐
│                      ASP.NET Core Web API                       │
│                                                                 │
│  Clean Architecture · DDD · Simple CQRS · Policy-based Auth     │
│  Dual JWT + Cookie · Rate Limiting · Domain Events · Serilog    │
└──────────┬──────────────────────┬──────────────────┬────────────┘
           │ SQL                  │ internal HTTP    │ internal HTTP
┌──────────▼────────┐  ┌──────────▼──────────┐  ┌──▼─────────────┐
│    SQL Server     │  │   Scanner ML (CLIP)  │  │  Trip ML       │
│    EF Core        │  │   Python FastAPI     │  │  Python FastAPI│
└───────────────────┘  └─────────────────────┘  └────────────────┘
```

The API follows **Clean Architecture** with four layers:

```
API → Application → Domain → Infrastructure
```

- **Domain** — pure C# business logic, zero framework dependencies
- **Application** — use cases, commands, queries, DTOs, validation
- **Infrastructure** — EF Core, Identity, JWT, Cloudinary, email, ML clients
- **API** — controllers, middleware, DI wiring, Swagger

---

## Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| ORM | Entity Framework Core 10 |
| Database | SQL Server 2022 |
| Authentication | ASP.NET Core Identity + JWT Bearer + HttpOnly Cookies |
| Authorization | Policy-based with custom `IAuthorizationRequirement` handlers |
| Validation | FluentValidation |
| Logging | Serilog (Console + File sinks) |
| Image Storage | Cloudinary SDK |
| Email | MailKit |
| Background Jobs | `IHostedService` / `BackgroundService` |
| HTTP Client | `IHttpClientFactory` |
| Caching | `IMemoryCache` (cache-aside pattern) |
| API Docs | Swagger / OpenAPI |
| CI/CD | GitHub Actions + Azure OIDC |
| Hosting | Azure App Service |

---

## Project Structure

```
backend/
└── Rafeeq/
    └── src/
        ├── API/                          ← Entry point, controllers, middleware
        │   ├── Controllers/
        │   │   ├── Admin/            ← Cookie auth (admin/moderator)
        │   │   └── ...
        │   ├── Middleware/
        │   │   └── GlobalExceptionHandler.cs
        │   ├── Configurations/
        │   │   ├── RateLimitingExtensions.cs
        │   │   └── LocalizationExtensions.cs
        │   └── Program.cs
        │
        ├── Application/                  ← Use cases, CQRS, DTOs
        │   ├── Commands/
        │   │   ├── Sites/
        │   │   ├── Trips/
        │   │   ├── Reviews/
        │   │   ├── Sponsors/
        │   │   └── Users/
        │   ├── Queries/
        │   │   ├── Sites/
        │   │   ├── Trips/
        │   │   └── Users/
        │   ├── DTOs/
        │   └── Common/
        │       ├── Interfaces/
        |       │   ├── Messaging/
        |       │   │   ├── ICommandHandler.cs
        |       │   │   ├── IQueryHandler.cs
        |       │   │   └── ...
        |       │   └── QueryServices/
        |       │       ├── ISiteQueryService.cs
        |       │       ├── ITouristQueryService.cs
        |       │       └── ...
        |       └── Behaviors/
        |           └── ValidationBehavior.cs
        │
        ├── Domain/                       ← Pure business logic, zero dependencies
        │   ├── Entities/
        │   │   ├── SiteAggregate/
        │   │   │   ├── Site.cs
        │   │   │   ├── SiteImage.cs
        │   │   │   ├── SiteLocalizedContent.cs
        │   │   │   ├── SiteOpeningHours.cs
        │   │   │   ├── NearestTransportation.cs
        │   │   │   └── NearestTransportationLocalizedContent.cs
        │   │   ├── AttractionAggregate/
        │   │   │   ├── Attraction.cs
        │   │   │   ├── AttractionImage.cs
        │   │   │   └── AttractionLocalizedContent.cs
        │   │   ├── ArtifactAggregate/
        │   │   │   ├── Artifact.cs
        │   │   │   ├── ArtifactImage.cs
        │   │   │   └── ArtifactLocalizedContent.cs
        │   │   ├── CityAggregate/
        │   │   │   ├── City.cs
        │   │   │   └── CityLocalizedContent.cs
        │   │   ├── SponsorAggregate/
        │   │   │   ├── Sponsor.cs
        │   │   │   ├── SponsorImage.cs
        │   │   │   ├── SponsorLocalizedContent.cs
        │   │   │   ├── Offer.cs
        │   │   │   └── OfferLocalizedContent.cs
        │   │   ├── TripAggregate/
        │   │   │   ├── Trip.cs
        │   │   │   ├── TripDay.cs
        │   │   │   └── TripSite.cs
        │   │   └── TouristAggregate/
        │   │       ├── Tourist.cs
        │   │       ├── Favourite.cs
        │   │       └── VisitedSite.cs
        │   ├── ValueObjects/
        │   │   ├── Location.cs
        │   │   ├── Money.cs
        │   │   ├── TimeRange.cs
        │   │   └── ...
        │   ├── Enums/
        │   │   ├── SiteType.cs
        │   │   └── ...
        │   ├── Repositories/
        │   │   └── (repository interfaces)
        │   └── Common/
        │       ├── AggregateRoot.cs
        │       ├── BaseEntity.cs
        │       ├── BaseAuditableEntity.cs
        │       ├── IDomainEvent.cs
        │       └── ...
        │
        └── Infrastructure/               ← EF Core, Identity, external services
        |   ├── Persistence/
        |   │   ├── ApplicationContext/
        |   |   │   ├── ApplicationDbContext.cs
        |   |   │   ├── Configurations/       ← EF IEntityTypeConfiguration per table
        |   │   │   └── ...
        |   │   ├── Interceptors/
        |   │   │   ├── AuditableEntityInterceptor.cs
        |   │   │   └── DomainEventDispatcherInterceptor.cs
        |   │   ├── Repositories/
        |   │   ├── QueryServices/
        |   │   └── Seeding/
        |   ├── Authorization/
        |   │   ├── Policies.cs
        |   │   ├── Requirements/
        |   │   │   └── AuthorizationRequirements.cs
        |   │   └── Handlers/
        |   |       └── AuthorizationHandlers.cs
        |   ├── Authentication/
        |   │   ├── JwtTokenService.cs
        |   │   ├── CurrentUserService.cs
        |   │   ├── RefreshToken.cs
        |   │   └── ...
        |   ├── ExternalServices/
        |   ├── Identity/
        |   ├── Services/
        |   ├── Localization/
        |   ├── BackgroundJobs/
        |   │   └── RefreshTokenCleanupJob.cs
        |   └── DI.cs
        └── Shared/
            ├── Result.cs
            └── Error.cs
```

---

## Domain Model

The domain is organized into **7 aggregate roots**, each owning its child entities:

```
City
└── CityLocalizedContent[]

Site  ──────────────────────────────────────── owns ──►  Attraction[]
├── SiteLocalizedContent[]                               ├── AttractionLocalizedContent[]
├── SiteImage[]                                          └── AttractionImage[]
├── SiteOpeningHours[]
└── NearestTransportation[]
    └── NearestTransportationLocalizedContent[]

Artifact
├── ArtifactLocalizedContent[]
└── ArtifactImage[]

Sponsor
├── SponsorLocalizedContent[]
├── SponsorImage[]
└── Offer[]
    └── OfferLocalizedContent[]

Trip
└── TripDay[]
    └── TripSite[]          ← denormalized snapshot of site data

Tourist
├── Favourite[]
└── VisitedSite[]
```

### Design Decisions

- **No `Name`/`Description` on aggregate roots** — all text content lives in `LocalizedContent` child entities. There is no default language shortcut.
- **`TripSite` is a denormalized snapshot** — site name, image, city, and price are copied at trip creation time so trip views never depend on external service calls.
- **`Facilities` stored as JSON string** on `Sites` — simple enum flags, not a separate table.
- **`Artifacts` use `SET NULL` cascade** from `Sites` — artifacts survive site deletion for use by the ML scanner.
- **`StoredFiles` dedup registry** — SHA-256 hash prevents duplicate uploads; `ReferenceCount` tracks shared usage across image tables.

---

## Key Features

### CQRS (without MediatR)

Commands and queries are dispatched through the using of the interface itself `ICommandHandler` and `IQueryHandler`, resolved via DI:

```csharp
// Command with result
var result = await commanHanlder.HandleAsync(new CreateSiteCommand(...));

// Query
var sites = await queryHandler.HandleAsync(new GetSitesQuery(...));
```

Each handler lives in a co-located file alongside its command/query and FluentValidation validator.

### Domain Events

Aggregates raise domain events via `RaiseDomainEvent()`. The `DomainEventDispatcherInterceptor` fires all pending events after `SaveChangesAsync()` completes, keeping the event dispatch transactionally safe.

### Dual Authentication

The same API serves two different clients with two different authentication schemes registered side-by-side:

| Client | Scheme | Token location |
|---|---|---|
| Flutter mobile app | JWT Bearer | `Authorization: Bearer {token}` header |
| React admin panel | Cookie | HttpOnly cookie set by server |

Controllers declare which scheme they require via policy attributes — no duplication of endpoints.

### Policy-Based Authorization

Twelve named policies covering role and feature access:

| Policy | Admin | Moderator | Tourist |
|---|:---:|:---:|:---:|
| `AdminOnly` | ✅ | ❌ | ❌ |
| `ModeratorOrAdmin` | ✅ | ✅ | ❌ |
| `CanManageUsers` | ✅ | ❌ | ❌ |
| `CanManageSites` | ✅ | ✅ | ❌ |
| `CanManageSponsors` | ✅ | ❌ | ❌ |
| `CanModerateContent` | ✅ | ✅ | ❌ |
| `CanViewAnalytics` | ✅ | ✅ | ❌ |
| `CanManageSettings` | ✅ | ❌ | ❌ |
| `CanManageOwnTrips` | ❌ | ❌ | ✅ |

### Image Processing Pipeline

Images are processed before storage:

1. **Deduplication** — SHA-256 hash checked against `StoredFiles` registry
2. **Compression** — JPEG re-encode with quality reduction for oversized uploads
3. **ML preprocessing** — `CompressForMlAsync` applies JPEG passthrough, dimension peek via `Image.IdentifyAsync`, and `KnownResamplers.Triangle` resize for the scanner service
4. **Storage** — Cloudinary upload with SHA-256 in context metadata for recovery

### Caching

Cache-aside pattern with `IMemoryCache`. Write operations (create/update/delete) invalidate all related cache keys via a static `AllKeysFor(Guid id)` enumeration. Short TTL serves as a safety fallback.

### Rate Limiting

Two limiters registered as middleware:

- `AuthPerIp` — tighter limit on auth endpoints to prevent brute force
- `GlobalLimiter` — broad limit across all endpoints per IP

### Refresh Token Cleanup

`RefreshTokenCleanupJob` is a `BackgroundService` that runs on a configurable interval and hard-deletes expired, revoked refresh tokens from the database.

---

## Authentication & Authorization

### Tourist Login Flow (Flutter)

```
Flutter → Google Sign-In → ID Token
    → POST /api/auth/google-login
    → API validates token via GoogleJsonWebSignature.ValidateAsync()
    → API issues JWT (short-lived) + Refresh Token
    → Flutter stores JWT in memory, refresh token in secure storage
```

### Admin/Moderator Login Flow (React)

```
React → POST /api/auth/web/login  { email, password }
    → API validates credentials via UserManager
    → API sets HttpOnly cookie (SameSite=Lax)
    → React uses Axios withCredentials: true
    → No token ever touches JavaScript
```

### First Admin Account

There is no public registration endpoint for Admin or Moderator accounts. The first admin is seeded via `AdminSeeder` on startup (development) or via a secure CLI tool (production). Admins create moderator accounts through the admin panel. Moderators can be promoted to Admin via the role management endpoint.

---

## API Endpoints

The API is versioned under `/api/`. Full Swagger documentation is available at `/swagger` when running in Development.

### Public (no auth)

| Method | Path | Description |
|---|---|---|
| GET | `/api/cities` | List all cities |
| GET | `/api/sites` | List sites with filtering |
| GET | `/api/sites/{id}` | Site detail |
| GET | `/api/sites/{id}/attractions` | Attractions for a site |
| GET | `/api/artifacts` | List artifacts |

### Mobile — Tourist (JWT Bearer)

| Method | Path | Description |
|---|---|---|
| POST | `/api/auth/refresh` | Refresh JWT |
| GET | `/api/trips` | Tourist's trips |
| POST | `/api/trips` | Create trip (calls Trip ML) |
| PUT | `/api/trips/{id}` | Update trip |
| DELETE | `/api/trips/{id}` | Delete trip |
| GET | `/api/favourites` | Tourist's favourites |
| POST | `/api/favourites` | Add favourite |
| POST | `/api/scanner/identify` | Identify artifact from image |

### Admin Panel (Cookie)

| Method | Path | Description |
|---|---|---|
| POST | `/api/auth/web/login` | Admin/Moderator login |
| POST | `/api/auth/web/logout` | Logout (clears cookie) |
| GET | `/api/admins/dashboard` | Dashboard KPIs |
| GET/POST/PUT/DELETE | `/api/admins/sites` | Site management |
| GET/POST/PUT/DELETE | `/api/admins/sponsors` | Sponsor management |
| GET/POST | `/api/admins/users` | User management |
| POST | `/api/admins/users/moderators` | Create moderator |
| POST | `/api/admins/users/{id}/promote-to-admins` | Promote moderator |
| POST | `/api/admins/users/{id}/lock` | Lock account |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [SQL Server 2022](https://www.microsoft.com/sql-server) or SQL Server Express
- [Cloudinary account](https://cloudinary.com) (free tier works)
- [Google Cloud Console project](https://console.cloud.google.com) with OAuth 2.0 credentials

### Clone & Run

```bash
# Clone the repository and
cd Rafeeq/backend/Rafeeq

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project src/Infrastructure --startup-project src/API

# Run the API
dotnet run --project src/API
```

The API will start at `https://localhost:5043`. Swagger UI is available at `https://localhost:5043/swagger`.

### First Admin Account

On first run in Development, the admin seeder creates an initial admin account using credentials from `appsettings.Development.json`:

```json
{
  "AdminSeed": {
    "Email": "admin@rafeeq.live",
    "Password": "***"
  }
}
```

Change the password immediately after first login.

---

## Configuration

### `appsettings.json` structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Rafeeq;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "RafeeqAPI",
    "Audience": "RafeeqClients",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  },
  "CookieSettings": {
    "Name": "rafeeq_admin",
    "ExpirationMinutes": 480,
    "SecurePolicy": "Always",
    "SameSite": "Strict"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "Google": {
    "ClientId": "your-google-client-id.apps.googleusercontent.com"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "FromEmail": "noreply@rafeeq.com",
    "FromName": "Rafeeq",
    "ApiKey": "your-sendgrid-api-key"
  },
  "MlServices": {
    "ScannerBaseUrl": "http://localhost:8001",
    "TripBaseUrl": "http://localhost:8002",
    "ApiKey": "your-internal-ml-api-key"
  },
  "RateLimiting": {
    "GlobalPermitLimit": 100,
    "GlobalWindowSeconds": 60,
    "AuthPermitLimit": 10,
    "AuthWindowSeconds": 60
  },
  "AdminSeed": {
    "Email": "admin@rafeeq.com",
    "Password": "***"
  }
}
```

### Production — Azure App Service

All secrets are stored as **Application Settings** in Azure App Service (never committed to source control). Use double underscore `__` as the section separator:

```
ConnectionStrings__DefaultConnection  →  your production connection string
JwtSettings__SecretKey                →  your production secret
Cloudinary__ApiSecret                 →  your cloudinary secret
Google__ClientId                      →  your google client id
```

For highly sensitive secrets (JWT key, API keys), use **Azure Key Vault** with Managed Identity.

---

## Database

### Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName \
  --project src/Infrastructure \
  --startup-project src/API \
  --output-dir Persistence/Migrations

# Apply migrations
dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/API

# Revert last migration
dotnet ef migrations remove \
  --project src/Infrastructure \
  --startup-project src/API
```

### Schema Overview

The database uses the `rafeeq` schema. All tables except `StoredFiles` and `Site_OpeningHours` include full audit columns (`CreatedAt`, `CreatedBy`, `CreatedByName`, `LastModifiedAt`, `LastModifiedBy`, `LastModifiedByName`) populated automatically by the `AuditableEntityInterceptor`.

Key schema decisions:

- **Soft deletes** on `Users` via `DeletedAt` nullable column + global query filter
- **Discriminator column** `UserRole` on `Users` table for TPH inheritance (Admin, Moderator share the Users table)
- **JSON columns** — `Facilities`, `HistoricalPeriods`, `PreferredSiteTypes` stored as serialized arrays
- **Owned types** — `Location`, `Money`, `TimeRange` mapped as EF Core owned entities (no separate tables)

---

## Deployment

### GitHub Actions CI/CD

The pipeline is defined in `.github/workflows/deploy.yml` and triggers only when changes are pushed to `backend/**`:

```yaml
on:
  push:
    branches: [main]
    paths:
      - 'backend/**'
```

Authentication to Azure uses **OIDC** (no stored secrets). The workflow:

1. Checks out the monorepo
2. Sets up .NET 10
3. Restores and builds `src/API/API.csproj`
4. Publishes the release artifact
5. Deploys to Azure App Service via `azure/webapps-deploy`

### Manual Deploy

```bash
dotnet publish src/API/API.csproj \
  -c Release \
  -o ./publish \
  --no-restore

# Then deploy ./publish to Azure App Service
```

---

## Contributing

This is a graduation project. The codebase follows these conventions:

- **No MediatR** — simple `ICommandHandler<TCommand, TResult>` / `IQueryHandler<TQuery, TResult>` interfaces
- **Result pattern** — never throw exceptions for business logic failures; use `Result<T>` and `Result` return types
- **Co-located handlers** — command, handler, and validator live in the same file
- **No hardcoded policy strings** — always reference `Policies.*` constants

---

## License

MIT License — see [LICENSE](LICENSE) for details.

---

<div align="center">
  Built with ❤️Py as a graduation project · Egypt 🇪🇬
</div>
