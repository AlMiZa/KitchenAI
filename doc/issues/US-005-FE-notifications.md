# US-005-FE — Notifications for expiring items — Frontend

**Type:** User Story — Frontend  
**Parent:** [US-005 Notifications](US-005-notifications.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 4

## Story

As a user I want to see in-app notifications and be able to configure my notification preferences so I stay informed about expiring items.

## Acceptance criteria

- A notification bell icon in the header shows a badge with the count of unread notifications.
- Clicking the bell opens a dropdown/panel listing recent notifications.
- Clicking a notification navigates to the inventory list filtered by "expiring soon".
- Settings screen includes a notification preferences section (threshold in days, channel opt-ins: email, push).

## Technical notes

- Poll or use a WebSocket / SSE connection to fetch unread notification count on the header.
- For MVP, polling `GET /api/households/{hid}/notifications` every N seconds is acceptable.
- Notification preferences form calls `POST /api/households/{hid}/notifications/subscribe`.
- Item Detail screen has a "Set low-stock threshold" field that updates the item via `PUT /api/households/{hid}/items/{itemId}`.

## UI

- **Header**: notification bell icon with unread count badge
- **Notification panel**: list of recent notifications with type icon, message, and timestamp; "Mark all as read" action
- **Settings screen** — Notifications section:
  - Expiring-soon threshold (number input, in days)
  - Email notifications toggle (opt-in)
  - Push notifications toggle (opt-in)
- **Item Detail screen**: "Set low-stock threshold" field

## Tests

- Frontend component test: notification bell renders badge with correct unread count.
- Frontend component test: notification panel lists notifications with correct messages.
- Frontend component test: clicking a notification triggers navigation to inventory with `expiringSoon` filter.
- Frontend component test: notification preferences form renders threshold and channel toggles.
