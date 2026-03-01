# TECH-002 — Data model & database setup

**Type:** Technical Task — Backend  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 0 / Sprint 1

## Overview

Define and implement all EF Core entity classes, database migrations and indexes for the KitchenAI SQLite database.

## Entities

### User
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| Email | string | unique |
| PasswordHash | string | bcrypt/Argon2 |
| DisplayName | string | |
| Locale | string | default "pl-PL" |
| CreatedAt | datetime | |
| UpdatedAt | datetime | |
| NotificationPreferences | JSON | |
| DietaryPreferences | JSON | allergies, diets, calorie target |

### Household
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| Name | string | |
| OwnerUserId | GUID | FK → User |
| CreatedAt | datetime | |

### HouseholdMember
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| HouseholdId | GUID | FK → Household |
| UserId | GUID | FK → User |
| Role | string | owner / member |

### Item (inventory product)
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| HouseholdId | GUID | FK → Household |
| Name | string | |
| Quantity | decimal | |
| Unit | string | g, kg, ml, L, pcs |
| AllowFraction | bool | |
| PurchaseDate | date | optional |
| ExpiryDate | date | optional |
| BestByOrUseBy | enum | |
| StorageLocation | enum | Fridge/Freezer/Pantry/Other |
| Brand | string | |
| Price | decimal | |
| Notes | text | |
| CreatedAt | datetime | |
| UpdatedAt | datetime | |
| IsArchived | bool | |

### Recipe (saved)
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| HouseholdId | GUID | FK → Household |
| Source | enum | generated / imported |
| Title | string | |
| Ingredients | JSON | or normalized RecipeIngredients table |
| Steps | JSON | |
| Nutrition | JSON | |
| Servings | int | |
| PrepTime | int | minutes |
| CookTime | int | minutes |
| Tags | string | list: cuisine, diet |
| CreatedAt | datetime | |
| UpdatedAt | datetime | |
| GeneratedBy | JSON | LLM response & rationale |

### GeneratedRecipe
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| RecipeJson | text | full LLM result |
| Rationale | text | |
| CreatedAt | datetime | |
| RequestedBy | GUID | FK → User |
| MatchedInventorySnapshot | JSON | snapshot ids |

### RecipeIngredient (normalized)
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| RecipeId | GUID | FK → Recipe |
| Name | string | |
| Quantity | decimal | |
| Unit | string | |

### Notification
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| HouseholdId | GUID | FK → Household |
| Type | enum | expiring / low-stock / recipe-suggestion |
| Payload | JSON | |
| Delivered | bool | |
| CreatedAt | datetime | |

### AnalyticsEvent
| Field | Type | Notes |
|---|---|---|
| Id | GUID | PK |
| HouseholdId | GUID | FK → Household |
| EventType | string | item_added, item_removed, item_consumed, recipe_generated, recipe_saved, recipe_cooked |
| Metadata | JSON | |
| CreatedAt | datetime | |

## Database constraints & indexes

- `User.Email` — unique index.
- `Item(HouseholdId, ExpiryDate)` — composite index for efficient expiry queries.
- `Recipe(Title)` — index.
- `Recipe(Tags)` — index.
- All FK relationships with cascade delete where appropriate (e.g., deleting a Household cascades to all its Items, Recipes, Notifications).

## Acceptance criteria

- EF Core entity classes defined in `KitchenAI.Domain`.
- DbContext configured in `KitchenAI.Infrastructure`.
- Initial migration created and applied successfully against SQLite.
- All indexes and constraints applied.
