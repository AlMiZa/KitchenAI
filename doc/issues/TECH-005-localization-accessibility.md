# TECH-005 — Localization & accessibility

**Type:** Technical Task  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

> This task is split into frontend and backend sub-issues:
> - **Backend**: [TECH-005-BE — Localization — Backend](TECH-005-BE-localization.md)
> - **Frontend**: [TECH-005-FE — Localization & accessibility — Frontend](TECH-005-FE-localization-accessibility.md)

## Overview

Implement localization (Polish and English) and accessibility (WCAG AA) requirements.

## Localization

- **Default language**: Polish (pl-PL)
- **Supported languages**: Polish and English
- Frontend: use `react-i18next` with translation string files
- Backend: resource files for server-side messages
- **Date format**: `dd.MM.yyyy` (Polish default)
- **Units**: Metric-only for MVP — g, kg, ml, L, szt (pcs in Polish). Implement translation strings for unit labels.

### Language switching

- Language setting available in **Settings** screen.
- Persisted per user account.

## Accessibility

- Aim for **WCAG AA** compliance on all main flows.
- Specific requirements:
  - Sufficient color contrast ratios.
  - Visible focus states for keyboard navigation.
  - Semantic HTML elements (`<nav>`, `<main>`, `<header>`, `<section>`, `<button>`, etc.).
  - ARIA roles and labels where native HTML semantics are insufficient.
  - Screen reader compatibility for key flows (inventory management, recipe generation).

## Acceptance criteria

- All user-visible strings are externalized to i18n resource files.
- Switching between EN and PL updates the UI without reload.
- Date fields display in `dd.MM.yyyy` format for Polish locale.
- No WCAG AA contrast failures on main screens.
- All interactive elements reachable and operable by keyboard alone.
