# TECH-001-FE — Project setup & architecture — Frontend

**Type:** Technical Task — Frontend  
**Parent:** [TECH-001 Project setup & architecture](TECH-001-project-setup.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 0

## Overview

Scaffold the React SPA with React Router, Tailwind CSS, React Query, and react-i18next.

## Stack

- React (v18+)
- React Router for client-side routing
- Tailwind CSS for styling
- TanStack Query (React Query) for server state
- react-i18next for localization (EN / PL)
- Jest + React Testing Library for unit tests
- Playwright for E2E tests

## Project structure

```
/src/pages        — Dashboard, Inventory, Recipes, Settings, Admin
/src/components   — reusable UI components
/src/layouts      — MainLayout, AdminLayout
/src/services     — API clients, adapters
/src/hooks        — useInventory, useRecipes, etc.
/src/i18n         — translation string files (en.json, pl.json)
```

## UI layouts

- **Main layout**: header (logo + navigation: Dashboard, Inventory, Recipes, Shopping List, Settings), workspace area, footer.
- **Admin layout**: admin-specific navigation, API keys page, metrics page.

## CI/CD — frontend steps

- `npm run build` on every PR.
- `npm test -- --watchAll=false --ci` on every PR; block merge on failure.
- `npm run lint` for linting.

## Acceptance criteria

- React app scaffolded; `npm run dev` starts the development server.
- All route paths resolve to placeholder pages without console errors.
- Tailwind CSS applied to at least one component.
- React Query provider and react-i18next initialised in the app entry point.
- Frontend CI steps configured and passing on an empty build.
