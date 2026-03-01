# US-003-BE — Generate recipe from stored products — Backend

**Type:** User Story — Backend  
**Parent:** [US-003 Generate recipe](US-003-generate-recipe.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 2

## Story

As a user I want the backend to aggregate my inventory and return at least two generated recipe options.

## Acceptance criteria

- Backend aggregates non-archived items (quantity > 0) and user constraints before calling adapters.
- At least 2 distinct recipe options are returned per request.
- Each recipe includes: title, ingredients with quantities, steps, nutrition, and a rationale for substitutions.
- LLM output is validated for structure (ingredients array, steps array, nutrition object); invalid output falls back to adapted external recipe.
- Generated recipe and rationale are stored in the `GeneratedRecipe` table with timestamp.

## Technical notes — Hybrid generation flow

1. Query configured recipe DB adapters with the current ingredient list and constraints.
2. POST aggregated ingredient list, candidate recipes, and constraints to LLM (Gemini).
3. LLM returns recipe steps, ingredient adjustments, and a short rationale per substitution.
4. Enrich recipe with nutrition data from recipe DB or ingredient-nutrition aggregator.
5. Store generated recipe in `GeneratedRecipe` with full LLM response JSON and prompt for traceability.

## Key rules

- Always return ≥ 2 distinct recipe options.
- Each substitution must include a rationale (1–3 sentences).
- LLM prompt template and max token limit must be stored for audit.
- Keep a record (audit log) of LLM-generated text and timestamp.
- Validate LLM output schema; graceful fallback on invalid output.

## Rate limiting & cost controls

- Default rate limits per household (configurable).
- Cache generation results for identical inventory snapshots.
- Rate-limit endpoints calling LLM or external recipe DBs.

## API endpoints

- `POST /api/households/{hid}/recipes/generate`
  - Body: `{ constraints?: { diet, allergies, maxTime, servings }, options?: { preferQuick: boolean } }`
  - Returns: 2+ generated/adapted recipes with rationale and nutrition.
- `GET /internal/adapters/recipes/search?ingredients=...`
- `GET /internal/adapters/recipes/{externalId}/nutrition`
- `POST /internal/llm/generate-recipe`
  - Body: `{ ingredientList, externalMatches[], constraints, context }`

## Tests

- Integration test: generate handler calls adapter mocks and LLM mock, returns exactly 2 recipes.
- Unit test: LLM output validation rejects malformed response and triggers fallback.
- Unit test: rate limiter blocks generation when household quota is exceeded.
