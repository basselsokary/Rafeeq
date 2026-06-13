# Rafeeq — رفيق

> **Companion** — A location-based mobile tourist guide for Egypt.

[![Flutter](https://img.shields.io/badge/Flutter-3.x-02569B?style=flat-square&logo=flutter)](https://flutter.dev)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![React](https://img.shields.io/badge/React-Vite-61DAFB?style=flat-square&logo=react)](https://vitejs.dev)
[![Python](https://img.shields.io/badge/Python-FastAPI-009688?style=flat-square&logo=fastapi)](https://fastapi.tiangolo.com)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Azure](https://img.shields.io/badge/Azure-App_Service-0078D4?style=flat-square&logo=microsoftazure)](https://azure.microsoft.com)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

Rafeeq helps tourists discover, explore, and navigate Egypt's historical and cultural sites. It provides detailed site information, interactive maps, trip planning, artifact image recognition, and sponsor offers — all in one place.

---

## Table of Contents

- [System Overview](#system-overview)
- [Repositories](#repositories)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [User Roles](#user-roles)
- [Screenshots](#screenshots)
- [License](#license)

---

## System Overview

Rafeeq is a full-stack system with five components:

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                            │
│   Flutter Mobile App              React + Vite Admin Panel      │
│   JWT Bearer · Google Sign-In     HttpOnly Cookie · Axios       │
└──────────────────┬──────────────────────────┬───────────────────┘
                   │ HTTPS                    │ HTTPS
┌──────────────────▼──────────────────────────▼───────────────────┐
│                    ASP.NET Core Web API (.NET 8)                 │
│         Clean Architecture · DDD · Policy-based Auth            │
└──────────┬──────────────────────┬──────────────────┬────────────┘
           │ SQL                  │ HTTP             │ HTTP
    ┌──────▼──────┐    ┌──────────▼────────┐  ┌─────▼──────────┐
    │  SQL Server │    │  Scanner ML       │  │  Trip ML       │
    │  EF Core    │    │  Python FastAPI   │  │  Python FastAPI│
    └─────────────┘    │  Artifact Recog.  │  │  Trip Planning │
                       └───────────────────┘  └────────────────┘
```

---

## Repositories

This is a monorepo. Each component lives in its own folder:

| Folder | Component | README |
|---|---|---|
| `backend/` | ASP.NET Core Web API | [backend/README.md](backend/README.md) |
| `mobile/` | Flutter mobile app |
| `web-app/` | React + Vite admin panel |
| `ml-engine/scanner/` | Python FastAPI — artifact recognition |
| `ml-engine/trip/` | Python FastAPI — trip planning |

---

## Tech Stack

| Component | Technology |
|---|---|
| Mobile app | Flutter (Android 9+ / iOS 13+) |
| Admin panel | React + Vite (JavaScript) |
| Backend API | ASP.NET Core 10, EF Core, SQL Server |
| Scanner ML | Python, FastAPI |
| Trip ML | Python, FastAPI, recommendation algorithms |
| Image storage | Cloudinary |
| Authentication | Google Sign-In + JWT + HttpOnly cookies |
| Hosting | Azure App Service + GitHub Actions CI/CD |

---

## Features

### Tourist (Mobile App)
- **Discover** — browse historical and cultural sites across Egypt with images, descriptions, opening hours, and ticket prices
- **Search & Filter** — filter sites by city, type, rating, free entry, and hidden gems
- **Maps & Navigation** — Google Maps integration with driving, walking, and transit routing
- **Trip Planner** — create multi-day itineraries; the ML service generates an optimized route based on your preferences, location, and available time
- **Artifact Scanner** — photograph an artifact and the ML service identifies it and returns its full details
- **Favourites** — save sites to a personal favourites list
- **Sponsors & Offers** — discover nearby partner businesses with active discounts, shown contextually while browsing sites and planning trips
- **Multi-language** — English and Arabic supported throughout

### Admin Panel (Web)
- **Dashboard** — KPI cards, interactive Egypt map, site statistics
- **Site Management** — full CRUD for sites, attractions, opening hours, transportation, facilities, and localized content
- **Sponsor Management** — manage sponsor tiers (Bronze / Silver / Gold / Platinum), branches, and time-limited offers
- **User Management** — create moderators, manage roles, lock/suspend accounts, view activity logs
---

## User Roles

| Role | Access |
|---|---|
| **Tourist** | Mobile app — discover sites, plan trips, and scan artifacts |
| **Moderator** | Admin panel — manage sites, attractions, and cities|
| **Admin** | Admin panel — everything Moderator can do, plus manage sponsors, users, and system settings |

There is no public registration for Admin or Moderator accounts. The first Admin is seeded on deployment. Admins create Moderator accounts through the admin panel and can promote Moderators to Admin.

---

## Environment Requirements

| Component | Requirement |
|---|---|
| Mobile | Android 9.0+ or iOS 13+ · GPS · Camera · Internet |
| Backend | .NET 10 SDK · SQL Server · Azure (production) |
| Admin Panel | Modern browser (Chrome, Edge, Brave) |
| ML Services | Python 3.10+ · CUDA optional (CPU supported) |

---

## Screenshots

---

## License

MIT License — see [LICENSE](LICENSE) for details.

---

<div align="center">
  Graduation Project · Egypt 🇪🇬
</div>
