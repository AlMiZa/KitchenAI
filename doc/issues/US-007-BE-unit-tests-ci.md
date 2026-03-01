# US-007-BE — Unit tests & CI — Backend

**Type:** Developer Story — Backend  
**Parent:** [US-007 Unit tests & CI](US-007-unit-tests-ci.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

## Story

As a developer I want automated unit tests for all backend handlers and a CI gate to ensure correctness on every PR.

## Acceptance criteria

- Each MediatR command/query handler has at least one positive and one negative test case.
- CI pipeline runs `dotnet test` and fails the build on any test failure.
- No PR can be merged while the backend test step is failing.

## Technical notes

- Framework: **xUnit**
- Use **EF Core InMemory** or **SQLite in-memory** provider for handler tests (avoid hitting a real database).
- Follow the Arrange-Act-Assert pattern.
- Mock external dependencies (LLM client, email service, recipe adapters) using a mocking library (e.g., Moq or NSubstitute).

## Tests required

| Handler / Feature | Positive test | Negative test |
|---|---|---|
| Registration handler | Valid email → user + household created | Duplicate email → validation error |
| Item creation handler | All fields persisted correctly | Missing required field → error |
| Generate recipe handler | Adapter + LLM mocks → 2 recipes returned | LLM invalid output → fallback recipe returned |
| Check availability query | All ingredients available → `ready` | Missing ingredient → `missing` with deficit |
| Nightly notification job | Items within threshold → notifications created | Archived items → no notifications |
| Household membership | User linked to household on registration | — |

## CI pipeline — backend step

```yaml
- name: Backend tests
  run: dotnet test --no-build --verbosity normal
```

- Lint: `dotnet format --verify-no-changes`
- Gate: fail PR on any test or lint failure.
