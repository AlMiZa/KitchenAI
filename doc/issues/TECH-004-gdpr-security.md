# TECH-004 — GDPR, privacy & security

**Type:** Technical Task  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

## Overview

Implement GDPR compliance, privacy controls, and security requirements for the KitchenAI application.

## Data minimization

- Collect minimal personal data: email, display name, preferences.
- No health data stored except optional dietary preferences (user-controlled).

## User rights

- **Data export**: endpoint for users to export their household data as JSON.
- **Data deletion**: user can delete account; deletion removes all household data and triggers cascade deletion.
- **Consent dialogs**: explicit consent for analytics and optional email notifications.
- **Cookie/consent banner** for tracking.

## Security requirements

- Enforce HTTPS/TLS across all API endpoints.
- Hash passwords with bcrypt or Argon2.
- Role-based authorization for admin endpoints only.
- Store API keys in secure secrets storage (env variables or Azure Key Vault).
- Log sensitive events: login, password reset, and monitor unusual behavior.

## Compliance

- Document data processing activities and Data Processing Agreement for third-party services (LLM provider).
- Default data retention: 24 months for inactive households (configurable).

## Admin configuration

- Screen: **Admin**
  - API keys (recipe adapters, LLM config), system logs, usage metrics
- API endpoints:
  - `GET /api/admin/config`
  - `POST /api/admin/config` — update API keys, LLM config

## Acceptance criteria

- Data export endpoint returns full household JSON.
- Account deletion cascades to all related data.
- Admin endpoints require admin role (return 403 for non-admin users).
- API keys never exposed in API responses.
- HTTPS enforced (HTTP redirects to HTTPS).
