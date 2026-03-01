# US-007 — Unit tests & CI

**Type:** Developer Story  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

> This story is split into frontend and backend sub-issues:
> - **Backend**: [US-007-BE — Unit tests & CI — Backend](US-007-BE-unit-tests-ci.md)
> - **Frontend**: [US-007-FE — Unit tests & CI — Frontend](US-007-FE-unit-tests-ci.md)

## Story

As a developer I want automated unit tests for backend handlers to ensure correctness.

## Acceptance criteria

- Each MediatR command/query handler has unit tests.
- CI pipeline runs tests and fails on test failure.

## Technical notes

### Backend tests
- Framework: xUnit
- Use EF Core InMemory or SQLite in-memory for handler tests.
- Cover each MediatR handler with at least one positive and one negative test case.

### Frontend tests
- Framework: Jest + React Testing Library for unit tests.
- E2E tests: Playwright (recommended).
- Cover key components: register form, inventory list, recipe display.

### CI pipeline
- Run backend unit tests (`dotnet test`).
- Run frontend unit tests (`npm test`).
- Run linters for both frontend and backend.
- Pipeline must fail (block merge) if any test fails.
- Deploy to staging only after all gates pass.

## Scope

Tests required per user story:
| User Story | Backend test | Frontend test |
|---|---|---|
| US-001 Account creation | Registration handler: valid email → user + household created | Register form validation |
| US-002 Add inventory item | Item creation handler: fields persisted correctly | Fractional input validation |
| US-003 Generate recipe | Generate handler: adapters + LLM mocked, returns 2 recipes | Recipe list rendering and rationale visibility |
| US-004 Check availability | Availability calculation: exact, partial, missing | — |
| US-005 Notifications | Nightly job: expiring items → notifications generated | — |
| US-006 Household sharing | Household membership association | — |
