# TECH-001 — Project setup & architecture

**Type:** Technical Task  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 0

> This task is split into frontend and backend sub-issues:
> - **Backend**: [TECH-001-BE — Project setup — Backend](TECH-001-BE-project-setup.md)
> - **Frontend**: [TECH-001-FE — Project setup — Frontend](TECH-001-FE-project-setup.md)

## Overview

Scaffold the full project structure for both frontend (React SPA) and backend (ASP.NET Core Web API) with CI pipeline.

## Frontend stack

- React (v18+), React Router, Tailwind CSS
- React Query (TanStack Query) for server state
- react-i18next for localization (EN/PL)

### Project structure

```
/src/pages        — Dashboard, Inventory, Recipes, Settings, Admin
/src/components   — reusable UI components
/src/layouts      — MainLayout, AdminLayout
/src/services     — API clients, adapters
/src/hooks        — useInventory, useRecipes, etc.
/src/i18n         — i18n strings
```

## Backend stack

- ASP.NET Core Web API (latest stable)
- MediatR for CQRS (Commands + Queries)
- EF Core with SQLite
- JWT authentication

### Project structure

```
KitchenAI.Api            — controllers and DI setup
KitchenAI.Application    — MediatR handlers, DTOs, business services
KitchenAI.Infrastructure — EF Core DbContext, repositories, recipe adapters, LLM client
KitchenAI.Domain         — domain entities & value objects
```

## UI layouts

- **Main layout**: header (logo, navigation: Dashboard, Inventory, Recipes, Shopping List, Settings), workspace, footer.
- **Admin layout**: admin nav and pages (API keys, metrics).

## CI/CD setup

- Run unit tests and linters on every PR.
- Deploy to staging for manual verification before production.
- Block merge on test failure.

## Acceptance criteria

- Repositories scaffolded with all listed projects/directories.
- CI pipeline configured and passing on an empty build.
- Both frontend (dev server) and backend (swagger) run locally.
- EF Core migrations set up and SQLite database created.
- OpenAPI/Swagger exposed for backend.
