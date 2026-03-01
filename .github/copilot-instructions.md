# Copilot Instructions for KitchenAI

## Build, test, and lint commands

Use these commands from the project specs when scaffolding is present:

### Backend (.NET)
- Build: `dotnet build`
- Test suite: `dotnet test`
- Lint/format check: `dotnet format --verify-no-changes`
- Single test: `dotnet test --filter "FullyQualifiedName~<TestName>"`

### Frontend (React)
- Build: `npm run build`
- Test suite: `npm test -- --watchAll=false --ci`
- Lint: `npm run lint`
- Single test file/pattern: `npm test -- <test-file-or-pattern>`

## High-level architecture

- Product shape: React SPA frontend + ASP.NET Core Web API backend.
- Backend architecture follows CQRS with MediatR and layered projects:
  - `KitchenAI.Api` (controllers, middleware, DI)
  - `KitchenAI.Application` (handlers, DTOs, services)
  - `KitchenAI.Infrastructure` (EF Core DbContext, repositories, adapters, LLM client)
  - `KitchenAI.Domain` (entities, value objects)
- Persistence is EF Core with SQLite.
- Recipe generation is a hybrid pipeline: query recipe adapters first, then call LLM (Gemini) for adaptation/generation, then enrich/store results.
- Core app domain is household-scoped inventory + recipes + notifications + analytics.

## Key conventions

- Scope by household: Items, Recipes, Notifications, and Analytics are modeled with `HouseholdId`.
- Recipe generation contract:
  - Return at least 2 distinct recipes.
  - Include substitution rationale per recipe.
  - Validate LLM output schema; use fallback behavior when output is invalid.
  - Store generated output/audit data for traceability.
- Inventory rules:
  - Metric units only (`g`, `kg`, `ml`, `L`, `pcs`).
  - Fractional quantities are supported.
  - Inventory views prioritize expiry date.
- Localization/accessibility defaults:
  - Default locale `pl-PL`; supported locales EN/PL.
  - Polish date format `dd.MM.yyyy`.
  - WCAG AA target on core flows.
- Security/privacy expectations:
  - JWT auth and hashed passwords (bcrypt/Argon2).
  - GDPR flows include data export/deletion and explicit consent controls.
  - Admin configuration endpoints are role-restricted.

## Source documents to prioritize

- `README.md`
- `doc/feature-specifications.md`
- `doc/issues/TECH-001-project-setup.md`
- `doc/issues/TECH-001-BE-project-setup.md`
- `doc/issues/TECH-001-FE-project-setup.md`
- `doc/issues/TECH-002-data-model.md`
- `doc/issues/US-003-generate-recipe.md`
- `doc/issues/US-003-BE-generate-recipe.md`
- `doc/issues/US-006-household-sharing.md`
- `doc/issues/US-007-unit-tests-ci.md`
- `doc/issues/US-007-BE-unit-tests-ci.md`
- `doc/issues/US-007-FE-unit-tests-ci.md`
