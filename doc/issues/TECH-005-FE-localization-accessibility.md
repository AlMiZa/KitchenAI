# TECH-005-FE — Localization & accessibility — Frontend

**Type:** Technical Task — Frontend  
**Parent:** [TECH-005 Localization & accessibility](TECH-005-localization-accessibility.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

## Overview

Implement frontend localization using react-i18next (Polish and English) and ensure WCAG AA accessibility across all main flows.

## Localization

- **Default language**: Polish (pl-PL)
- **Supported languages**: Polish (pl-PL) and English (en)
- Use **react-i18next** with JSON translation files (`src/i18n/pl.json`, `src/i18n/en.json`).
- All user-visible strings must be externalized to translation files — no hardcoded UI copy.
- **Date format**: `dd.MM.yyyy` for Polish locale; `MM/dd/yyyy` for English locale.
- **Unit labels**: `szt` for pcs in Polish; `pcs` in English. Implement translation strings for all unit labels.

### Language switching

- Language setting available in **Settings** screen.
- Persisted in the authenticated user's `Locale` preference (via `PUT /api/users/me`).
- Switching language updates the UI immediately without a page reload.

## Accessibility

- Aim for **WCAG AA** compliance on all main user flows.
- Requirements:
  - Sufficient colour contrast ratios (minimum 4.5:1 for normal text, 3:1 for large text).
  - Visible focus indicators for all interactive elements (keyboard navigation).
  - Semantic HTML: `<nav>`, `<main>`, `<header>`, `<section>`, `<button>`, `<label>`, etc.
  - ARIA roles and labels where native HTML semantics are insufficient.
  - Screen reader compatibility for: inventory management, recipe generation, and account flows.
  - All modals/dialogs use `role="dialog"` and `aria-modal="true"`.

## Acceptance criteria

- All user-visible strings in translation files; `i18next` raises a warning for any missing key.
- Switching between EN and PL updates the full UI without a reload.
- Date fields display in `dd.MM.yyyy` format for Polish locale.
- No WCAG AA colour contrast failures on main screens (verified with an automated tool such as axe).
- All interactive elements reachable and operable by keyboard alone.
- Screen reader announces page transitions and modal open/close events.
