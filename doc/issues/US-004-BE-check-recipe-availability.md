# US-004-BE — Check recipe availability & gaps — Backend

**Type:** User Story — Backend  
**Parent:** [US-004 Check recipe availability](US-004-check-recipe-availability.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 3

## Story

As a user I want the backend to compare a recipe's ingredient requirements against my current inventory and tell me exactly what is missing.

## Acceptance criteria

- `POST /api/households/{hid}/recipes/{rid}/check` compares `RecipeIngredient` quantities against the sum of non-archived `Item.Quantity` for matching items in the household.
- Returns `{ status: "ready" }` when all ingredients are fully satisfied.
- Returns `{ status: "missing", items: [{ name, required, available, deficit }] }` when any ingredient is insufficient.
- Partial matches (some quantity available but not enough) are reported as missing with the deficit amount.

## Technical notes

- Availability check is triggered only on explicit user request (not automatically on recipe open).
- Match ingredients by name (case-insensitive); unit normalisation is desirable but out of scope for MVP (assume same units).
- Implement as a MediatR query: `CheckRecipeAvailabilityQuery`.
- The shopping list is constructed from the missing items list; a separate endpoint or the same response can seed the shopping list.

## API endpoints

- `GET /api/households/{hid}/recipes` — list saved recipes
- `POST /api/households/{hid}/recipes` — save a recipe
- `GET /api/households/{hid}/recipes/{rid}` — get recipe detail
- `POST /api/households/{hid}/recipes/{rid}/check` — returns availability result and missing items list

## Tests

- Unit test: all ingredients available → returns `ready`.
- Unit test: one ingredient missing entirely → returns `missing` with correct deficit.
- Unit test: one ingredient partially available → returns `missing` with correct deficit amount.
