# TECH-004-BE — GDPR, privacy & security — Backend

**Type:** Technical Task — Backend  
**Parent:** [TECH-004 GDPR, privacy & security](TECH-004-gdpr-security.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

## Overview

Implement backend security controls, GDPR compliance endpoints, and admin configuration API.

## Security requirements

- Enforce HTTPS/TLS; redirect HTTP to HTTPS.
- Hash passwords with bcrypt or Argon2.
- JWT bearer tokens; admin endpoints require the `admin` role (return 403 for non-admin users).
- Store API keys in environment variables or Azure Key Vault — never in source code or API responses.
- Log sensitive events: login, password reset, account deletion; monitor for unusual behaviour.

## GDPR / data rights

- **Data export**: `GET /api/users/me/export` returns the full household data as JSON.
- **Account deletion**: `DELETE /api/users/me` deletes the user account and cascades deletion of all household data, items, recipes, and notifications.
- **Data minimisation**: store only email, display name, and preferences.
- Default data retention: 24 months for inactive households (configurable).

## Admin configuration

- `GET /api/admin/config` — returns current config (API key names/status, LLM config); never exposes key values.
- `POST /api/admin/config` — updates API keys, LLM configuration.
- Both endpoints require `admin` role authorisation.

## Compliance

- Document data processing activities.
- Data Processing Agreement required for third-party LLM provider.

## Acceptance criteria

- Data export endpoint returns complete household JSON for the authenticated user.
- Account deletion cascades to all related data; no orphaned records remain.
- Admin endpoints return 403 for non-admin users.
- API key values never appear in API responses.
- HTTPS enforced in production configuration.
