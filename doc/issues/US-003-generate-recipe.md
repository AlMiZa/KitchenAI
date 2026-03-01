# US-003 — Generate recipe from stored products

**Type:** User Story  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 2

## Story

As a user I want to get a generated recipe based on my inventory.

## Acceptance criteria

- User clicks "Generate recipes".
- Backend aggregates non-archived items (with quantities > 0) and constraints, calls recipe adapters & LLM, and returns **at least 2 recipes**.
- Each recipe includes: title, ingredients with quantities, steps, nutrition, and a short rationale for substitutions.
- UI displays recipes within a reasonable timeout and shows a loading state.

## Technical notes — Hybrid generation flow

1. Backend queries configured free recipe DB adapters with the current ingredient list and constraints to find candidate recipes.
2. Backend POSTs aggregated ingredient list, selected candidate recipes (optional), and user constraints to LLM (Gemini) to adapt or generate new recipes.
3. LLM returns recipe steps, ingredient adjustments and a short rationale for substitutions.
4. Backend enriches the recipe with nutrition data using recipe DB nutrition endpoints or an ingredients-nutrition aggregator.
5. Save generated recipe in `GeneratedRecipe` store with rationale.

## Key rules

- Always return at least two distinct recipe options.
- Provide explainability: for every substitution or omission include a short rationale (1–3 sentences).
- LLM calls must include a structured prompt template and a max token limit; prompt must be stored for traceability.
- Keep a record (audit) of LLM-generated text and timestamp.
- Validate the LLM output structure (ingredients array, steps array, nutrition object).
- If LLM output is invalid, fall back to adapted external recipe or inform user gracefully.

## Rate limiting & cost controls

- Set default rate limits per household.
- Cache frequent generation results for identical inventory snapshots.
- Rate-limit endpoints that call LLM or external recipe DBs.

## API endpoints

- `POST /api/households/{hid}/recipes/generate`
  - Body: `{ constraints?: { diet, allergies, maxTime, servings }, options?: { preferQuick: boolean } }`
  - Returns: 2+ generated/adapted recipes with rationale and nutrition.
- `GET /internal/adapters/recipes/search?ingredients=...`
- `GET /internal/adapters/recipes/{externalId}/nutrition`
- `POST /internal/llm/generate-recipe`
  - Body: `{ ingredientList, externalMatches[], constraints, context }`

## Tests

- Integration test: generate handler calls adapter mocks and LLM mock, returns 2 recipes.
- Frontend test: recipe list rendering and rationale visibility.

## UI

- Screen: **Suggested Recipes** (after generation)
  - List view of at least two recipe cards; each shows title, time, servings, nutrition summary, rationale (explain substitutions)
  - CTA: Save recipe / View recipe
