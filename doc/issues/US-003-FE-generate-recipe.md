# US-003-FE — Generate recipe from stored products — Frontend

**Type:** User Story — Frontend  
**Parent:** [US-003 Generate recipe](US-003-generate-recipe.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 2

## Story

As a user I want to see the generated recipes displayed clearly with a loading indicator while they are being prepared.

## Acceptance criteria

- Clicking "Generate recipes" triggers the API call and shows a visible loading state.
- At least 2 recipe cards are displayed once the response arrives.
- Each card shows: title, prep/cook time, servings, nutrition summary, and the substitution rationale.
- User can save a recipe or navigate to its detail view from the card.

## Technical notes

- Use React Query `useMutation` to call `POST /api/households/{hid}/recipes/generate`.
- Display a spinner or skeleton loader while the mutation is pending.
- On error (timeout, rate-limit) show a user-friendly error message with a retry option.
- Rationale text is collapsible / expandable to keep the card compact.
- Saved recipe state is reflected immediately (optimistic update or re-fetch).

## UI

- Screen: **Suggested Recipes** (after generation)
  - List of ≥ 2 recipe cards; each shows title, time, servings, nutrition summary, and rationale.
  - CTA per card: "Save recipe" and "View recipe"
- Screen: **Dashboard**
  - Primary action button: "Generate recipes from my fridge"

## Tests

- Frontend component test: "Generate recipes" button renders and is clickable.
- Frontend component test: loading spinner is displayed while request is in flight.
- Frontend component test: recipe list renders with title, nutrition, and rationale for each recipe.
- Frontend component test: rationale section is visible / toggleable.
