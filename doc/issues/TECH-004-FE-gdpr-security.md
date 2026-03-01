# TECH-004-FE — GDPR, privacy & security — Frontend

**Type:** Technical Task — Frontend  
**Parent:** [TECH-004 GDPR, privacy & security](TECH-004-gdpr-security.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

## Overview

Implement frontend consent dialogs, cookie banner, and data management UI in the Settings screen.

## Consent & privacy UI

- **Cookie / consent banner**: displayed on first visit; user must accept or decline analytics and optional tracking.
- **Analytics consent dialog**: explicit opt-in before enabling any analytics or external tracking.
- **Email notification consent**: explicit opt-in before subscribing to email notifications.

## Settings screen — data management

- **Data export**: "Export my data" button calls `GET /api/users/me/export` and downloads the JSON file.
- **Account deletion**: "Delete my account" button with a confirmation dialog; calls `DELETE /api/users/me`; redirects to the sign-in page after success.
- Inform users about data retention policy (24 months for inactive households) inline in the settings copy.

## Admin screen

- API keys configuration page: input fields for recipe adapter keys and LLM config (values masked by default).
- System logs and usage metrics display.

## Accessibility

- All consent dialogs must be keyboard-navigable and announced by screen readers (use `role="dialog"` and `aria-modal`).

## Acceptance criteria

- Cookie banner appears on first visit and does not reappear after acceptance.
- "Export my data" initiates a JSON file download.
- "Delete my account" shows a confirmation dialog before proceeding.
- Admin page renders API key fields with masked values; admin nav is only visible to admin users.
