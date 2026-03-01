# US-006 — Persisted household & sharing (basic)

**Type:** User Story  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 1

> This story is split into frontend and backend sub-issues:
> - **Backend**: [US-006-BE — Household sharing — Backend](US-006-BE-household-sharing.md)
> - **Frontend**: [US-006-FE — Household sharing — Frontend](US-006-FE-household-sharing.md)

## Story

As a user I want household data persisted so family members can access the same inventory.

## Acceptance criteria

- Household is created automatically at signup.
- Invite flow (simple share link or email invite) for other users to join — can be deferred but must be a placeholder.
- All household members see the same inventory.

## Technical notes

- Household is the central sharing unit; all Items, Recipes, and Notifications are scoped to a HouseholdId.
- Basic roles (owner/member) are stored for future use but not enforced in MVP beyond admin vs member distinction.
- Advanced roles (admin/guest) deferred to post-MVP.

## Data model

**Household**

| Field | Type | Notes |
|---|---|---|
| Id | GUID | |
| Name | string | |
| OwnerUserId | GUID | |
| CreatedAt | datetime | |

**HouseholdMember**

| Field | Type | Notes |
|---|---|---|
| Id | GUID | |
| HouseholdId | GUID | |
| UserId | GUID | |
| Role | string | owner / member |

## API endpoints

- `GET /api/households`
- `POST /api/households` — create household
- `POST /api/households/{id}/join` — accept invite / add member (future placeholder)

## Tests

- Backend test: household membership association — user is linked to household on registration.

## UI

- Screen: **Settings** — household management section (invite link, member list)
- Screen: **Dashboard** — household selector at top
