# TECH-001-BE — Project setup & architecture — Backend

**Type:** Technical Task — Backend  
**Parent:** [TECH-001 Project setup & architecture](TECH-001-project-setup.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 0

## Overview

Scaffold the ASP.NET Core Web API backend with MediatR, EF Core, JWT authentication, and Swagger.

## Stack

- ASP.NET Core Web API (latest stable)
- MediatR for CQRS (Commands + Queries)
- EF Core with SQLite
- JWT bearer authentication
- OpenAPI / Swagger

## Project structure

```
KitchenAI.Api            — controllers, middleware, DI setup
KitchenAI.Application    — MediatR handlers, DTOs, business services
KitchenAI.Infrastructure — EF Core DbContext, repositories, recipe adapters, LLM client
KitchenAI.Domain         — domain entities & value objects
KitchenAI.Tests          — xUnit test project
```

## CI/CD — backend steps

- `dotnet build` on every PR.
- `dotnet test` on every PR; block merge on failure.
- `dotnet format --verify-no-changes` for linting.

## Acceptance criteria

- All four backend projects scaffolded and building successfully.
- `dotnet run` starts the API and Swagger UI is accessible at `/swagger`.
- EF Core migrations set up; SQLite database created on first run.
- JWT authentication middleware registered and functional.
- Backend CI steps configured and passing on an empty build.
