# US-007-FE — Unit tests & CI — Frontend

**Type:** Developer Story — Frontend  
**Parent:** [US-007 Unit tests & CI](US-007-unit-tests-ci.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

## Story

As a developer I want automated unit and E2E tests for key frontend components and a CI gate to prevent regressions.

## Acceptance criteria

- Key frontend components are covered by unit tests using Jest + React Testing Library.
- At least one Playwright E2E test covers the core happy path (register → add item → generate recipe).
- CI pipeline runs `npm test` and fails the build on any test failure.
- No PR can be merged while the frontend test step is failing.

## Technical notes

- Framework: **Jest + React Testing Library** for unit/component tests.
- E2E: **Playwright** (recommended); tests can target the local dev server.
- Use `msw` (Mock Service Worker) to mock API calls in component tests.
- Aim for tests that focus on user-visible behaviour, not implementation details.

## Tests required

| Component / Feature | Test description |
|---|---|
| Register form | Validation error on empty email; error on invalid email format |
| Inventory list | Renders items sorted by expiry date; fractional qty input accepts decimals |
| Recipe list | Renders ≥ 2 recipe cards with title, nutrition, and rationale |
| Recipe view | "Check against my inventory" button visible; missing items CTA appears after check |
| Notification bell | Badge shows unread count; panel lists notifications |
| E2E: happy path | Register → add item → generate recipe → recipe list displayed |

## CI pipeline — frontend step

```yaml
- name: Frontend tests
  run: npm test -- --watchAll=false --ci
```

- Lint: `npm run lint`
- Gate: fail PR on any test or lint failure.
