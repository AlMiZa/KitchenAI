# US-004-FE — Check recipe availability & gaps — Frontend

**Type:** User Story — Frontend  
**Parent:** [US-004 Check recipe availability](US-004-check-recipe-availability.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 3

## Story

As a user I want to see clearly which ingredients I have and which are missing when I open a saved recipe.

## Acceptance criteria

- Recipe view shows an ingredient checklist with a "Check against my inventory" button.
- After clicking the button, each ingredient is marked as available, partially available, or missing.
- "Ready to cook" message is shown when all ingredients are satisfied.
- "Add missing to shopping list" CTA appears only when missing items exist.
- Shopping list screen displays the auto-generated missing items and allows manual additions.

## Technical notes

- Call `POST /api/households/{hid}/recipes/{rid}/check` on button click; show a loading indicator during the request.
- Update ingredient list items in-place with visual status indicators (e.g., green ✓, yellow partial, red ✗).
- Shopping list: on "Add missing" the missing items are appended to the shopping list state; allow manual check-off.

## UI

- Screen: **Recipe View** (saved or generated)
  - Ingredient checklist (one row per ingredient; status icon after "Check" is clicked)
  - Step-by-step instructions, nutrition panel, rationale/tags
  - "Check against my inventory" button
  - "Add missing to shopping list" CTA (conditionally shown)
- Screen: **Shopping List**
  - Items auto-generated from recipe gaps or added manually
  - Each item has a checkbox to mark as acquired

## Tests

- Frontend component test: "Check against my inventory" button is rendered on recipe view.
- Frontend component test: after check response, available ingredients show a success indicator.
- Frontend component test: missing ingredients show a missing indicator and the "Add to shopping list" CTA appears.
- Frontend component test: shopping list renders missing items with checkboxes.
