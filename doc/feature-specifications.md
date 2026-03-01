# KitchenAI — Feature Specification (mid-level)

Version: 1.0  
Date: 2026-03-01  
Author: Product / Business Analyst

Table of contents
- Executive summary
- Goals & success metrics
- Users & personas
- Scope (MVP) and priorities
- High-level product features
- User flows (high-level)
- Data model (entities)
- API outline (endpoints)
- UI screens (high-level) & example copy
- User stories, acceptance criteria & tests
- Developer & integration notes
- AI Recipe generation design
- Localization & accessibility
- GDPR / Privacy / Security
- Analytics & dashboard indicators
- Non-functional requirements & deployment notes
- Roadmap & milestones
- Appendix: glossary

---

Executive summary
-----------------
KitchenAI helps families cook with what they already have in their home storage (fridge/freezer/pantry) by managing the inventory and automatically generating or adapting recipes using available ingredients. The product is a responsive web multi-page SPA (React + React Router + Tailwind). The backend is ASP.NET Core Web API with MediatR and SQLite. Users have simple accounts so data is persisted server-side and can be shared across devices.

Goals & success metrics
-----------------------
Primary goals
- Help people (families) reduce food waste and cook meals from available ingredients.
- Make it fast and easy to add/manage products and get useful recipes.
- Provide at least two recipe suggestions per request and explain substitution choices.

Success metrics (examples)
- Feature adoption: % of active users who add ≥10 items in first 14 days.
- Recipes generated per user per month.
- Food-waste proxy: number of expired items removed vs baseline.
- Time-to-recipe: median time from "generate recipe" click to recipe displayed < 3s (excluding external API latency).
- Retention: 30-day retention %.

Users & personas
----------------
Primary persona: The Family Cook
- Age: 30–50, cooks most family meals, manages weekly groceries.
- Needs: Quickly find recipes from what's in fridge, avoid waste, plan shopping lists.
- Behaviour: Uses phone/tablet in kitchen, expects clear instructions and nutrition information.

Secondary persona: The Busy Parent
- Wants quick/easy recipes, dietary filters, visual clarity, and notifications for expiring items.

Scope (MVP) and priorities
--------------------------
MVP (must-have)
1. User accounts with simple authentication (email + password and passwordless option).
2. Create and manage inventory (add/edit/delete items) with quantity, units (metric only), fractional quantities, purchase & expiry date, storage location, brand, price.
3. Generate recipes from current inventory (hybrid approach — external recipe DB + LLM adaptation). Always return at least two recipe suggestions.
4. Select a stored recipe and verify if it can be cooked with inventory; show missing items (shopping list gaps).
5. Notifications (in-app & push/email): expiring soon, low stock, recipe suggestions.
6. Basic nutrition estimate per recipe.
7. Analytics indicators on main dashboard: food waste reduced (tracked), money saved estimate, most-used ingredients.
8. Basic admin layout for configuration (API keys, recipe-source settings).
9. Unit tests for backend handlers and key frontend components; CI pipeline gate for tests.

Nice-to-have (post-MVP)
- Image recognition or barcode scanning.
- Integrations with grocery retailers, smart fridges, voice assistants.
- Offline mode (currently not required).
- Family/household advanced roles (admin/guest) — not required for v1.
- Multi-language beyond EN/PL.

High-level product features
---------------------------
Inventory Management
- Add items manually with quantity (supports fractional), unit, purchase date, expiry date, best-by/use-by type, storage location (fridge/freezer/pantry), brand, price.
- Merge duplicates automatically when metadata matches exactly (same name, storage, best-by type, expiry/purchase date optionally).
- Sort suggestions and inventory by expiry date (soonest first).
- Expired items are flagged; app shows alarms; removal is user-initiated.

Recipe Generation & Management
- Hybrid generation: query free recipe DBs (adapter) for matches, then call LLM (Gemini) for adaptation and explainability.
- Constraints: dietary restrictions, allergies, cuisine types, prep time, calorie limits, servings.
- Provide at least two recipe variants per request (e.g., quick & fuller).
- Nutrition estimates per recipe (calories, macros); use recipe DB nutrition or aggregate ingredient data if necessary.

Shopping Lists & Gaps
- When user opens a saved recipe, the app checks inventory and produces a "missing items" shopping list only at user choice.

Notifications & Alerts
- Expiring soon (configurable thresholds), low stock (thresholds), and recipe suggestions (periodic or triggered).
- Channels: in-app, push, email (opt-in).

Analytics & Dashboard
- Indicators: items expiring soon, money potentially saved (simple calculation), estimated food waste reduced (tracked by deleted expired items), most-used ingredients.

User flows (high-level)
----------------------
1. Onboard (Account creation)
   - Email/password or passwordless link. Confirm email. Create or join household.

2. Inventory management
   - Main dashboard -> Add item -> Fill name, qty, unit, dates, location -> Save -> Item appears prioritized by expiry.

3. Generate recipes
   - From dashboard click "Generate recipes" -> Backend aggregates active inventory and user dietary prefs -> Backend queries recipe adapters + LLM -> Returns 2+ recipes with rationale and nutrition -> Frontend displays recipes; user can save them.

4. Check stored recipe against inventory
   - User opens saved recipe -> Click "Check ingredients" -> App verifies available quantities -> Shows "Ready to cook" or "Missing items" (optionally add to shopping list).

5. Notifications
   - System checks nightly -> pushes "Expiring soon: 3 items" -> User clicks to view inventory.

Data model (entities)
---------------------
Below are primary entities and suggested fields. Use EF Core + SQLite.

- User
  - Id (GUID)
  - Email (string, unique)
  - PasswordHash (string) or external auth fields
  - DisplayName
  - Locale (default "pl-PL")
  - CreatedAt, UpdatedAt
  - NotificationPreferences (JSON)
  - DietaryPreferences (JSON: allergies, diets, calorie target)

- Household
  - Id (GUID)
  - Name
  - OwnerUserId
  - Members (many-to-many through HouseholdMember)
  - CreatedAt

- HouseholdMember
  - Id
  - HouseholdId
  - UserId
  - Role (string) — basic roles (owner/member) for future use

- Item (inventory product)
  - Id (GUID)
  - HouseholdId
  - Name (string)
  - Quantity (decimal)
  - Unit (enum/string, metric units e.g., g, kg, ml, L, pcs)
  - AllowFraction (bool) — UI hint
  - PurchaseDate (date, optional)
  - ExpiryDate (date, optional)
  - BestByOrUseBy (enum)
  - StorageLocation (enum: Fridge/Freezer/Pantry/Other)
  - Brand (string)
  - Price (decimal, currency field)
  - Notes (text)
  - CreatedAt, UpdatedAt
  - IsArchived (bool) — for removed/consumed items

- Recipe (saved)
  - Id (GUID)
  - HouseholdId
  - Source (enum: generated/imported)
  - Title
  - Ingredients (JSON or normalized table RecipeIngredients)
  - Steps (text or JSON)
  - Nutrition (JSON)
  - Servings (int)
  - PrepTime, CookTime (int minutes)
  - Tags (list: cuisine, diet)
  - CreatedAt, UpdatedAt
  - GeneratedBy (if LLM, store LLM response & rationale, JSON)

- GeneratedRecipe
  - Id
  - RecipeJson (full result from LLM)
  - Rationale (text)
  - CreatedAt
  - RequestedBy (UserId)
  - MatchedInventorySnapshot (snapshot ids)

- RecipeIngredient (if normalized)
  - Id
  - RecipeId
  - Name
  - Quantity (decimal)
  - Unit

- Notification
  - Id
  - HouseholdId
  - Type (expiring, low-stock, recipe-suggestion)
  - Payload (JSON)
  - Delivered (bool)
  - CreatedAt

- AnalyticsEvent
  - Id
  - HouseholdId
  - EventType (item_added, item_removed, recipe_generated, recipe_cooked)
  - Metadata (JSON)
  - CreatedAt

Database constraints & indexes
- Index items by HouseholdId and ExpiryDate for efficient queries.
- Users by Email unique.
- Recipes indexed by Title and Tags.

API outline (high-level)
------------------------
Auth
- POST /api/auth/register
  - Body: { email, password?, passwordless?: bool }
  - Returns: user + token
- POST /api/auth/login
  - Body: { email, password }
  - Returns: token
- POST /api/auth/passwordless/request
  - Body: { email } -> send magic link
- GET /api/auth/me
  - Returns: current user profile

Households
- GET /api/households
- POST /api/households
  - Create household
- POST /api/households/{id}/join
  - Accept invite / add member (future)

Inventory (items)
- GET /api/households/{hid}/items
  - Query: filter by location, expiringSoon
- POST /api/households/{hid}/items
  - Body: item DTO
- PUT /api/households/{hid}/items/{itemId}
- DELETE /api/households/{hid}/items/{itemId}
- POST /api/households/{hid}/items/merge
  - Body: list of itemIds -> backend merges into one per rules

Recipes
- GET /api/households/{hid}/recipes
- POST /api/households/{hid}/recipes
  - Save a recipe (import or manually)
- GET /api/households/{hid}/recipes/{rid}
- POST /api/households/{hid}/recipes/{rid}/check
  - Returns availability vs inventory; missing items list
- POST /api/households/{hid}/recipes/generate
  - Body: { constraints?: { diet, allergies, maxTime, servings }, options?: { preferQuick: boolean } }
  - Behavior: Aggregate inventory and user prefs, call adapters + LLM. Returns 2+ generated/adapted recipes with rationale and nutrition.

Recipe API adapter endpoints (internal)
- Backend implements adapters for free recipe DBs:
  - GET /internal/adapters/recipes/search?ingredients=...
  - GET /internal/adapters/recipes/{externalId}/nutrition

AI / LLM endpoints (internal)
- POST /internal/llm/generate-recipe
  - Body: { ingredientList, externalMatches[], constraints, context }
  - Backend will orchestrate calls to LLM (Gemini) and combine results with adapter data.

Notifications
- GET /api/households/{hid}/notifications
- POST /api/households/{hid}/notifications/subscribe (email / push)
- POST /api/households/{hid}/notifications/test

Analytics
- GET /api/households/{hid}/analytics/summary
  - Returns: moneySavedEstimate, expiredItemsCount, recipesGeneratedCount, mostUsedIngredients

Admin (internal)
- GET /api/admin/config
- POST /api/admin/config (API keys for recipe providers, LLM config)

Authentication & security
- Use JWT bearer tokens.
- Password reset & passwordless flows optional.
- Rate-limit endpoints that call LLM or external recipe DBs.
- Expose OpenAPI/Swagger for backend.

UI screens (high-level) & example copy
-------------------------------------
Layouts:
- Main layout: header (logo, navigation: Dashboard, Inventory, Recipes, Shopping List, Settings), workspace, footer.
- Admin layout: admin nav and pages (API keys, metrics).

Pages / Screens
1. Sign up / Sign in (Account creation)
   - Example copy: "Create your KitchenAI account — use your email to save and sync your fridge."

2. Dashboard (Main)
   - Top: household selector, key indicators (expiring soon, money saved estimate, most-used ingredients)
   - Primary action: "Generate recipes from my fridge"
   - Expiring items list (soonest first)
   - Quick add item input

3. Inventory list
   - Filters: location, expiringSoon, lowStock
   - Item row: name, qty + unit, expiry date, storage location, quick edit button

4. Add / Edit Item (modal or page)
   - Fields: name, qty, unit (metric-only), purchase date, expiry date, best-by/use-by, storage location, brand, price, notes
   - CTA: "Save item"

5. Item Detail
   - Full metadata, history (added/edited), actions: mark as consumed, archive, set low-stock threshold

6. Suggested Recipes (after generation)
   - List view of at least two recipe cards; each shows title, time, servings, nutrition summary, rationale (explain substitutions)
   - CTA: Save recipe / View recipe

7. Recipe View (saved or generated)
   - Ingredient checklist (checkbox for each item; "Check against my inventory" button)
   - Step-by-step instructions, nutrition panel, rationales, tags
   - CTA: "Add missing to shopping list" (only shows if missing items exist)

8. Shopping List
   - Auto-generated from recipe gaps or manual additions; check off items

9. Settings
   - Profile, household management, notification preferences, language (EN/PL), units (metric-only default), data export/delete

10. Admin
    - API keys (recipe adapters, LLM), system logs, usage metrics

User stories, acceptance criteria & tests
----------------------------------------
Note: the spec level is mid; below are representative user stories for MVP with acceptance criteria.

US-001 — Account creation
- Story: As a user I want to create an account to persist my inventory across devices.
- Acceptance criteria:
  - User can register with email + password or request a passwordless magic link.
  - After registration user is redirected to Dashboard with an empty household created.
  - Backend stores user with hashed password.
- Tests:
  - Backend unit test for registration handler: valid email -> user and household created.
  - Frontend unit test: register form validation.

US-002 — Add inventory item (core)
- Story: As a user I want to add products with quantity, units and expiry so I can track them.
- Acceptance criteria:
  - User can enter fractional quantity (e.g., 0.5), unit must be metric (g, kg, ml, L, pcs).
  - Item appears in household inventory immediately.
  - Items are ordered by expiry date when viewing "expiring soon".
- Tests:
  - Handler test for item creation persists fields correctly.
  - Frontend component test validates fractional input.

US-003 — Generate recipe from stored products
- Story: As a user I want to get a generated recipe based on my inventory.
- Acceptance criteria:
  - User clicks "Generate recipes".
  - Backend aggregates non-archived items (with quantities > 0) and constraints, calls recipe adapters & LLM, and returns at least 2 recipes.
  - Each recipe includes: title, ingredients with quantities, steps, nutrition, and a short rationale for substitutions.
  - UI displays recipes within a reasonable timeout and shows a loading state.
- Tests:
  - Integration test: Generate handler calls adapter mocks and LLM mock, returns 2 recipes.
  - Frontend test: recipe list rendering and rationale visibility.

US-004 — Check recipe availability & gaps
- Story: As a user I want to check a saved recipe against my inventory to know what's missing.
- Acceptance criteria:
  - User opens recipe -> clicks "Check ingredients".
  - System compares required quantities to inventory quantities and returns "Ready to cook" if all satisfied or "Missing items" with list and quantities if not.
  - Option to add missing items to shopping list.
- Tests:
  - Unit test for availability calculation (exact, partial, missing).

US-005 — Notifications for expiring items
- Story: As a user I want to be notified about items expiring soon.
- Acceptance criteria:
  - User can configure threshold (e.g., 3 days).
  - System creates notifications and displays them in-app and optionally via email/push.
  - Clicking notification navigates to items list filtered by expiring soon.
- Tests:
  - Nightly job test: items with expiry within threshold generate notifications.

US-006 — Persisted household & sharing (basic)
- Story: As a user I want household data persisted so family members can access the same inventory.
- Acceptance criteria:
  - Household created at signup; invite flow (simple share link or email invite) for other users to join (can be deferred but must be placeholder).
  - Members see same inventory.
- Tests:
  - Backend test: household membership association.

US-007 — Unit tests & CI
- Story: As a developer I want automated unit tests for backend handlers to ensure correctness.
- Acceptance criteria:
  - Each MediatR command/query handler has unit tests.
  - CI pipeline runs tests and fails on test failure.

Developer & integration notes
-----------------------------
Frontend
- Tech: React (v18+), React Router, Tailwind CSS.
- Project structure:
  - /src/pages — page-level components (Dashboard, Inventory, Recipes, Settings, Admin)
  - /src/components — reusable UI components
  - /src/layouts — MainLayout, AdminLayout
  - /src/services — API clients, adapters
  - /src/hooks — domain hooks (useInventory, useRecipes)
  - /src/i18n — i18n strings (react-i18next)
- State & data fetching:
  - Recommended: React Query (TanStack Query) for server state + local caches.
  - Local UI state with context or useState.
- Styling:
  - Tailwind + component library of minimal design tokens.
- Tests:
  - Jest + React Testing Library for unit tests.
  - E2E tests: Playwright (recommended).

Backend
- Tech: ASP.NET Core Web API (latest stable), MediatR for CQRS (Commands + Queries).
- ORM: EF Core with SQLite for persistence.
- Project structure:
  - KitchenAI.Api — controllers and DI setup
  - KitchenAI.Application — MediatR handlers, DTOs, business services
  - KitchenAI.Infrastructure — EF Core dbcontext, repositories, recipe adapters, LLM client
  - KitchenAI.Domain — domain entities & value objects
- Authentication:
  - JWT (bearer) with refresh tokens optional.
  - Passwordless magic link implemented via signed short-lived token emailed to user.
- External integrations:
  - Recipe adapters (strategy pattern) for free recipe DBs (Edamam, Spoonacular, others).
  - LLM client (Gemini) implemented as service with configurable timeouts/limits and retry/backoff.
  - Keep costs in mind: limit LLM calls per household per minute and batch requests.
- Config:
  - API keys stored in env variables or secure secrets store (Azure Key Vault, etc).
- Tests:
  - xUnit for unit & integration tests.
  - Use EF Core InMemory or SQLite in-memory for handler tests.

AI recipe Generation
--------------------
Overview
- Hybrid architecture:
  1. Backend queries configured free recipe DB adapters with the current ingredient list and constraints to find candidate recipes.
  2. Backend POSTs aggregated ingredient list, selected candidate recipes (optional), and user constraints to LLM endpoint to:
     - Adapt candidate recipes to available items (substitutions) OR
     - Generate new recipe using available ingredients.
  3. LLM returns recipe steps, ingredient adjustments and a short rationale for substitutions.
  4. Backend enriches the recipe with nutrition data using recipe DB nutrition endpoints or an ingredients-nutrition aggregator.
  5. Save generated recipe in GeneratedRecipe store with rationale.

Key rules
- Always return at least two distinct recipe options.
- Provide explainability: for every substitution or omission the response must include a short rationale (1–3 sentences).
- LLM calls must include a structured prompt template and a max token limit; prompt must be stored for traceability.
- Keep a record (audit) of LLM-generated text and timestamp in case of review.

LLM / prompt & safety
- Validate the LLM output structure (ingredients array, steps array, nutrition object).
- Limit LLM outputs and validate ingredients against known controlled vocabulary to prevent hallucinations.
- If LLM output is invalid, fallback to adapted external recipe or inform user gracefully.

Rate limiting & cost controls
- Set default rate limits per household.
- Cache frequent generation results for identical inventory snapshots.

Localization & accessibility
--------------------------------
Localization
- Default language: Polish (pl-PL).
- Supported languages: Polish and English.
- Use react-i18next on frontend and resource files for server messages.
- Date format: dd.MM.yyyy (Polish default).
- Units: Metric-only for MVP (g, kg, ml, L, szt (pcs) for Polish copy). Implement translation strings for unit labels.

Accessibility
- Aim for WCAG AA compliance on main flows (contrast, focus states, keyboard navigation).
- Use semantic HTML and ARIA roles where appropriate.

GDPR / Privacy / Security
-------------------------
Data minimization
- Collect minimal personal data: email, display name, preferences.
- No health data stored except optional dietary preferences (user-controlled).

User rights
- Data export: Provide endpoint for users to export their household data (JSON).
- Data deletion: User can delete account; deletion removes all household data and triggers cascade deletion.
- Consent & PII:
  - Provide explicit consent dialogs for analytics & optional email notifications.
  - Cookie/consent banner for tracking.

Security
- Enforce HTTPS/TLS across API.
- Hash passwords (bcrypt/Argon2) or delegate to secure auth provider.
- Use role-based authorization for admin endpoints only.
- Store API keys in secure secrets storage.
- Log sensitive events (login, password reset) and monitor unusual behavior.

Compliance
- Document data processing activities and Data Processing Agreement for third-party services (LLM provider).
- Retention policy: default 24 months for inactive households (configurable).

Analytics & dashboard indicators
--------------------------------
Tracked events (examples)
- item_added, item_removed, item_consumed, recipe_generated, recipe_saved, recipe_cooked.

Dashboard indicators
- Items expiring soon (count & list)
- Money saved estimate: sum of price of items used vs baseline (heuristic)
- Food waste reduced: count of expired items removed vs baseline
- Most used ingredients: frequency chart

Privacy for analytics
- Aggregate analytics; send only non-identifiable aggregated signals to any external analytics provider unless user opts in.

Non-functional requirements & deployment notes
----------------------------------------------
Performance
- API: 95th percentile response time for base endpoints < 300ms.
- Recipe generation depends on external APIs/LLM; show progress & estimated ETA.

Scalability
- Backend can scale horizontally; for SQLite consider migration to a server RDBMS for high concurrency (post-MVP).
- Cache recipe adapter results and LLM outputs where possible.

Availability
- Target 99.5% for MVP services excluding external provider downtime.

Backup & recovery
- Daily backups of SQLite (or production RDBMS) and periodic export of generated recipes/keys.

CI/CD
- Run unit tests and linters.
- Deploy to staging for manual verification before production.

Roadmap & milestones
--------------------
Sprint 0 — Setup (1–2 weeks)
- Repo scaffolding, CI, baseline frontend + backend projects, auth, DB migrations.
Sprint 1 — Core inventory & accounts (2–3 weeks)
- User registration/login, household creation, add/edit/delete items.
Sprint 2 — Recipes & generation MVP (3–4 weeks)
- Adapter interfaces, LLM client, generate recipes, show results.
Sprint 3 — Recipe check & shopping list (2 weeks)
- Check availability, missing list, add to shopping list.
Sprint 4 — Notifications & analytics (2 weeks)
- Expiry/low-stock notifications, dashboard indicators.
Sprint 5 — Testing & hardening (2 weeks)
- Unit tests, integration tests, accessibility checks, GDPR flows, admin pages.
Post-MVP
- Add barcode/image input, multi-language expansion, switch to server RDBMS if needed.

Appendix: glossary
------------------
- Adapter: component that connects to third-party recipe DB (e.g., Edamam).
- LLM: large language model (Gemini).
- Household: logical grouping of users sharing an inventory.
- GeneratedRecipe: recipe created/adapted by LLM and stored for user.

---

Notes & next steps
- This document is intended to be placed at doc/feature-specifications.md in the repository.
- Implementation decisions that must be agreed before dev:
  - Exact recipe adapters to wire initially (Edamam recommended as primary; keep adapter abstraction so others can be added).
  - LLM quota & billing controls.
  - Email provider for passwordless flows.
- If you want, I can:
  - Produce OpenAPI contract skeleton for the endpoints above.
  - Produce initial DB migration model (EF Core entity classes).
  - Produce frontend route map and skeleton components.

End of specification.
