# TECH-005-BE — Localization — Backend

**Type:** Technical Task — Backend  
**Parent:** [TECH-005 Localization & accessibility](TECH-005-localization-accessibility.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 5

## Overview

Implement server-side localization for API error messages and server-generated content.

## Localization

- **Default language**: Polish (pl-PL)
- **Supported languages**: Polish (pl-PL) and English (en)
- Use .NET resource files (`.resx`) for server-side messages (validation errors, email templates, notification messages).
- Detect locale from the `Accept-Language` request header or from the authenticated user's `Locale` field.

## Translation scope (backend)

- Validation error messages returned from the API.
- Notification text in email messages.
- Unit labels used in server-generated content (g, kg, ml, L, szt for Polish; g, kg, ml, L, pcs for English).

## Date format

- API responses use ISO 8601 (`yyyy-MM-dd`) for all date fields (locale-specific formatting is the frontend's responsibility).

## Acceptance criteria

- All user-visible server-side strings are stored in `.resx` resource files for both `pl-PL` and `en`.
- API returns localised validation messages based on the authenticated user's `Locale` preference.
- No hardcoded user-visible strings in C# source files.
