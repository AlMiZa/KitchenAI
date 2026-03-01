# US-006-FE — Persisted household & sharing — Frontend

**Type:** User Story — Frontend  
**Parent:** [US-006 Household sharing](US-006-household-sharing.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 1

## Story

As a user I want to see my household's inventory on the dashboard and manage household membership in settings.

## Acceptance criteria

- Dashboard shows a household selector at the top (useful when a user belongs to multiple households).
- Settings screen includes a household management section with the current member list and an invite option.
- After joining a household, the user's inventory view reflects the shared household's items immediately.

## Technical notes

- Fetch available households via `GET /api/households`; allow switching active household.
- Store the active `householdId` in app state (React context or global store) and pass it in all inventory/recipe API calls.
- Invite flow: display a shareable link or email invite form; implementation can be a placeholder for MVP.
- Member list: fetched from the household detail endpoint; display name and role.

## UI

- **Dashboard** — household selector (dropdown or header indicator showing current household name)
- **Settings screen** — Household section:
  - Current household name and member list (name + role)
  - "Invite member" button — opens a share-link or email invite dialog (placeholder if not yet implemented)
  - "Leave household" option (for non-owner members)

## Tests

- Frontend component test: household selector renders available households and allows switching.
- Frontend component test: settings household section displays member list with names and roles.
- Frontend component test: "Invite member" button renders (placeholder state acceptable).
