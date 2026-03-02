# Prompts Log

This file logs all prompts used during the KitchenAI project development with GitHub Copilot.

## Prompts

| # | Prompt | Description |
|---|--------|-------------|
| 1 | Create PROMPTS.MD file to log all my prompts. | Create a markdown file to track all GitHub Copilot prompts used in this project. |
| 2 | Add a new feature specification document to the repository at doc/feature-specifications.md. | Create the KitchenAI mid-level feature specification document covering executive summary, MVP scope, personas, data model, REST API outline, user stories with acceptance criteria, developer notes (React + ASP.NET Core), AI recipe generation design, GDPR/privacy, localization, and roadmap. |
| 3 | Split specification on issues | Split the monolithic feature specification document into separate issue files (one per user story / technical task) under doc/issues/. |
| 4 | Split the issues into frontend and backend tasks where it makes sense | For each issue that contains both frontend and backend work, create `-BE` and `-FE` sub-issue files. Update parent issue files to reference the sub-issues. Update `doc/issues/README.md` to show the full hierarchy. TECH-002 (data model) is backend-only and is not split. |
| 5 | /create-issue Create all missed issues based on *.md files from doc/issues folder. | Create a GitHub Actions workflow (`.github/workflows/create-issues.yml`) that reads each `doc/issues/*.md` file, extracts the issue title from the H1 heading, and creates a GitHub issue for any file that does not already have a corresponding issue. |
| 6 | TECH-001-FE — Project setup & architecture — Frontend | Scaffold the React SPA under `/frontend` using Vite 6 + React 19 + TypeScript. Install and configure React Router v7, Tailwind CSS v4, TanStack Query v5, and react-i18next. Set up Jest 30 + React Testing Library for unit tests. Create placeholder pages (Dashboard, Inventory, Recipes, ShoppingList, Settings, Admin/ApiKeys, Admin/Metrics), MainLayout and AdminLayout components, EN/PL translation files, and a GitHub Actions CI workflow (`frontend-ci.yml`) running lint → test → build on every PR. |
| 7 | implement issues form https://github.com/AlMiZa/KitchenAI/issues according to implementation plan. | Implement all 20 open GitHub issues on branch `feature/implement-all-issues`. Backend: Application layer with MediatR handlers for Auth (register/login/me with BCrypt + JWT), Items (CRUD + merge + filters), Recipes (generate via LLM stub + save + availability check), Notifications (nightly expiry job + subscribe), Households (list/create/join), Analytics (summary), GDPR (export/delete), Admin (config). Infrastructure: TokenService, LlmService stub, ExpiryNotificationService. API controllers for all endpoints. 18 xUnit tests. Frontend: Auth context + Login/Register pages, full Inventory page (add/edit modal, filters, expiry badges), Recipes page (generate + cards), RecipeDetail page (availability check), NotificationBell, Settings page (profile/notifications/household/privacy), Dashboard (analytics cards), ShoppingList, Admin pages, CookieBanner, LanguageSwitcher. 16 Jest+RTL tests. Build, lint, and all tests pass. |
| 8 | @copilot open a new pull request to apply changes based on this feedback — ExpiryNotificationJob does not check whether a notification for the same item already exists before inserting a new one. | Fix duplicate expiry notifications: add a duplicate-check in `ExpiryNotificationJob.RunAsync` that loads pending undelivered `Expiring` notifications, extracts item IDs via a `TryGetItemIdFromPayload` helper, and skips items already notified. Add two new tests (second run produces no duplicates; delivered notification allows fresh notification on next run). All 20 tests pass. |
| 9 | @copilot open a new pull request to apply changes based on feedback about ApiKeys.tsx shape mismatch with backend AdminConfigDto. | Fix `ApiKeys.tsx` to use the correct `AdminConfigDto` shape (`geminiApiKeyConfigured`, `llmModel`, `version`) returned by `GET /api/admin/config`, and align the update mutation to send `UpdateAdminConfigCommand` (`geminiApiKeyPresent?`, `llmModel?`) instead of a generic `key`/`value` pair. |
| 10 | @copilot open a new pull request to apply changes based on review feedback: fix householdId nullable type mismatch. | Fix `AuthResult.householdId` and `UserProfile.householdId` in `frontend/src/services/auth.ts` from `string` to `string | null` to match the backend `AuthResultDto.HouseholdId: Guid?` and `UserDto.HouseholdId: Guid?`. |
| 11 | @copilot open a new pull request to apply changes based on the comments in the PR review thread (pullrequestreview-3873489689). | Fix several backend/frontend mismatches from the second PR review: add `GET /admin/metrics` endpoint with `AdminMetricsDto`; add `PUT /users/me` endpoint for profile updates; add `GET /households/{id}` endpoint; fix `analytics.ts` URL path to `/analytics/summary` and map PascalCase backend DTO to camelCase frontend interface; fix `Metrics.tsx` to use `/admin/metrics`; fix `auth.ts` `updateProfile` URL to `/users/me`; fix `recipes.ts` `checkRecipeAvailability` to map backend `AvailabilityItemDto` to frontend `RecipeIngredient` with computed `status`; fix `GenerateRecipeHandlerTests` to seed a `User` row satisfying the FK constraint. |
| 12 | fix problems on run CI workflows | Fix Frontend CI failure (add `ts-node` devDependency for `jest.config.ts` parsing) and Backend CI failure (`dotnet format` whitespace errors in multiple files: `GenerateRecipeHandlerTests.cs`, `GetItemsHandlerTests.cs`, `ExpiryNotificationServiceTests.cs`, `CheckRecipeAvailabilityHandlerTests.cs`, `ExportUserDataHandler.cs`). |
| 13 | US-001-FE — Account creation — Frontend | Implement frontend for US-001: fix post-auth redirect to `/dashboard` (Register + Login pages), add `/dashboard` route alias in AppRoutes, add passwordless "Send me a magic link" option on Login page with "Check your email" confirmation screen, add `sendMagicLink` service function in `auth.ts`, add EN/PL translation keys for magic-link flow, and add the missing unit test "navigates to /dashboard on successful registration". |
| 14 | US-001-BE — Account creation — Backend | Implement passwordless magic-link flow: add `MagicLinkToken` domain entity, `IEmailService`/`EmailService` stub, `PasswordlessRequestCommand`/`PasswordlessRequestHandler`, `POST /api/auth/passwordless/request` controller endpoint, EF migration, and two unit tests. All other auth endpoints (register, login, me) and their tests were already in place. |
| 15 | US-002-BE — Add inventory item — Backend | Strengthen `CreateItemHandlerTests.ValidItem_PersistedCorrectly` to assert all fields (AllowFraction, PurchaseDate, ExpiryDate, BestByOrUseBy, Brand, Notes). Add `FractionalQuantity_StoredWithoutRounding` test. Create `MergeItemsHandlerTests.cs` with tests for combining quantities, single-item guard, and not-found guard. |
| 16 | US-002-FE — Add inventory item — Frontend | Add missing Jest/RTL test: unit dropdown in Add/Edit Item modal only shows the five metric options (g, kg, ml, L, pcs), completing the three required frontend tests for the inventory feature (expiry-date sort, fractional qty, metric units). |

## US-003-BE: Recipe Generation Backend

### What was implemented
- Added `HouseholdId`, `PromptTemplate`, and `LlmResponseJson` fields to `GeneratedRecipe` entity
- Added `IRecipeAdapter` interface in Application layer for querying external recipe databases
- Added `StubRecipeAdapter` in Infrastructure (stub returning 2 fallback recipes)
- Added `IGenerationRateLimiter` interface and `InMemoryGenerationRateLimiter` (sliding window, per-household, 1-hour window using IMemoryCache with per-key locking to prevent race conditions)
- Added `GenerationRateLimitOptions` (configurable via `RecipeGeneration:MaxRequestsPerHour`)
- Updated `ILlmService.GenerateRecipesAsync` to accept `IList<GeneratedRecipeDto> candidateRecipes`
- Updated `LlmService` to match new signature
- Rewrote `GenerateRecipeHandler` with hybrid adapter+LLM flow, result caching (10 min, keyed by item IDs+quantities+constraints), and fallback logic
- Added `RateLimitExceededException` mapped to HTTP 429
- Added `InternalController` with admin-only endpoints for adapter search, nutrition, and direct LLM access
- Added EF migration `AddGeneratedRecipeHouseholdId`
- Added 4 new tests: valid items returns 2 recipes, adapter+LLM integration, fallback to adapter on invalid LLM output, rate limit exceeded

### Architecture decisions
- `IRecipeAdapter` and `IGenerationRateLimiter` are in Application (no infra dependency)
- `InMemoryGenerationRateLimiter` is registered as Singleton (cache-backed); uses per-household locks to prevent concurrent request races
- `GenerateRecipeHandler` caches results by inventory snapshot (item IDs + quantities) + constraints for 10 minutes; quantities included in key to invalidate when inventory changes
- Fallback: LLM output validated (≥2 recipes, non-empty title/ingredients/steps); falls back to adapter if invalid
- `PromptTemplate` is built for audit after LLM call completes (not passed to stub LLM service)
