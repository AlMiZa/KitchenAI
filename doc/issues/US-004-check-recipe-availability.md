# US-004 — Check recipe availability & gaps

**Type:** User Story  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 3

> This story is split into frontend and backend sub-issues:
> - **Backend**: [US-004-BE — Check recipe availability — Backend](US-004-BE-check-recipe-availability.md)
> - **Frontend**: [US-004-FE — Check recipe availability — Frontend](US-004-FE-check-recipe-availability.md)

## Story

As a user I want to check a saved recipe against my inventory to know what's missing.

## Acceptance criteria

- User opens recipe → clicks "Check ingredients".
- System compares required quantities to inventory quantities and returns:
  - "Ready to cook" if all items are satisfied, OR
  - "Missing items" with list and quantities if not.
- Option to add missing items to shopping list.

## Technical notes

- Only check availability at user's explicit request (not automatically on recipe open).
- Compare `RecipeIngredient.Quantity` against sum of non-archived `Item.Quantity` for matching items in the household.
- Partial match (some quantity available but not enough) should be reported as missing with the deficit amount.

## API endpoints

- `GET /api/households/{hid}/recipes` — list saved recipes
- `POST /api/households/{hid}/recipes` — save a recipe (import or manually)
- `GET /api/households/{hid}/recipes/{rid}` — get recipe detail
- `POST /api/households/{hid}/recipes/{rid}/check` — returns availability vs inventory; missing items list

## Tests

- Unit test for availability calculation: exact match, partial match, and missing items cases.

## UI

- Screen: **Recipe View** (saved or generated)
  - Ingredient checklist (checkbox for each item; "Check against my inventory" button)
  - Step-by-step instructions, nutrition panel, rationales, tags
  - CTA: "Add missing to shopping list" (only shows if missing items exist)
- Screen: **Shopping List**
  - Auto-generated from recipe gaps or manual additions; check off items
