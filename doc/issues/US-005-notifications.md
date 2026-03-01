# US-005 — Notifications for expiring items

**Type:** User Story  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 4

> This story is split into frontend and backend sub-issues:
> - **Backend**: [US-005-BE — Notifications — Backend](US-005-BE-notifications.md)
> - **Frontend**: [US-005-FE — Notifications — Frontend](US-005-FE-notifications.md)

## Story

As a user I want to be notified about items expiring soon.

## Acceptance criteria

- User can configure threshold (e.g., 3 days).
- System creates notifications and displays them in-app and optionally via email/push.
- Clicking a notification navigates to the items list filtered by expiring soon.

## Technical notes

- Nightly job checks all active household items and generates notifications for items expiring within the configured threshold.
- Notification channels: in-app, push (opt-in), email (opt-in).
- Notification types: expiring soon, low stock, recipe suggestions (periodic or triggered).
- Low stock threshold is configurable per item (set from Item Detail screen).
- User notification preferences stored as JSON on the User entity.

## Data model — Notification entity

| Field | Type | Notes |
|---|---|---|
| Id | GUID | |
| HouseholdId | GUID | |
| Type | enum | expiring, low-stock, recipe-suggestion |
| Payload | JSON | |
| Delivered | bool | |
| CreatedAt | datetime | |

## API endpoints

- `GET /api/households/{hid}/notifications`
- `POST /api/households/{hid}/notifications/subscribe` — email / push
- `POST /api/households/{hid}/notifications/test`

## Tests

- Nightly job test: items with expiry within threshold generate notifications.

## UI

- Screen: **Settings** — notification preferences (threshold, channels)
- Screen: **Item Detail** — set low-stock threshold action
- In-app notification bell / badge in header
- Notification links back to inventory filtered by expiring soon
