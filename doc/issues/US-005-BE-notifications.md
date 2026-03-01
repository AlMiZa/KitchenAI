# US-005-BE — Notifications for expiring items — Backend

**Type:** User Story — Backend  
**Parent:** [US-005 Notifications](US-005-notifications.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 4

## Story

As a user I want the backend to automatically detect expiring items and generate notifications so I am alerted in time.

## Acceptance criteria

- A nightly background job checks all active household items and creates `Notification` records for items expiring within the user-configured threshold.
- Notification types supported: `expiring`, `low-stock`, `recipe-suggestion`.
- Email and push channels are opt-in; user preferences are respected.
- Notification preferences are stored as JSON on the `User` entity.

## Technical notes

- Implement a scheduled background service (e.g., `IHostedService` with a timer or Hangfire).
- For each active household, query items where `ExpiryDate <= today + threshold` and `IsArchived = false`.
- Create `Notification` records with `Delivered = false`; mark as delivered after sending.
- Low-stock threshold is configurable per item (stored on the `Item` entity or user preferences).
- Email sending: use a configurable SMTP provider or transactional email service.
- Push: implement a placeholder for Web Push / FCM (can be stubbed in MVP).

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
- `POST /api/households/{hid}/notifications/subscribe` — email / push opt-in
- `POST /api/households/{hid}/notifications/test` — trigger test notification

## Tests

- Nightly job unit test: items with `ExpiryDate` within threshold → `Notification` records created.
- Nightly job unit test: already-archived items → no notifications generated.
- Handler test: `GET /api/households/{hid}/notifications` returns undelivered notifications for the household.
