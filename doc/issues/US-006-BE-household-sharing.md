# US-006-BE — Persisted household & sharing — Backend

**Type:** User Story — Backend  
**Parent:** [US-006 Household sharing](US-006-household-sharing.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 1

## Story

As a user I want household data stored on the backend so all family members share the same inventory.

## Acceptance criteria

- A `Household` is created automatically when a new user registers; the user is assigned as `owner`.
- All Items, Recipes, and Notifications are scoped to a `HouseholdId`.
- A basic invite endpoint exists as a placeholder for the join flow (full implementation can be deferred).
- All household members receive the same inventory when querying items.

## Technical notes

- `HouseholdMember` join table stores `HouseholdId`, `UserId`, and `Role` (owner / member).
- Basic roles stored for future use; in MVP only `owner` is functional.
- Advanced roles (admin/guest) are deferred to post-MVP.
- Cascade delete: deleting a Household cascades to all its Items, Recipes, Notifications, and HouseholdMember records.

## Data model

**Household**

| Field | Type | Notes |
|---|---|---|
| Id | GUID | |
| Name | string | |
| OwnerUserId | GUID | FK → User |
| CreatedAt | datetime | |

**HouseholdMember**

| Field | Type | Notes |
|---|---|---|
| Id | GUID | |
| HouseholdId | GUID | FK → Household |
| UserId | GUID | FK → User |
| Role | string | owner / member |

## API endpoints

- `GET /api/households` — list households for the current user
- `POST /api/households` — create a household
- `POST /api/households/{id}/join` — accept invite / add member (placeholder for MVP)

## Tests

- Backend unit test: on registration, user is automatically linked to a new household with role `owner`.
- Backend unit test: `GET /api/households/{hid}/items` scoped correctly to the requesting household.
