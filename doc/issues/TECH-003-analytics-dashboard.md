# TECH-003 — Analytics & dashboard

**Type:** Technical Task  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 4

> This task is split into frontend and backend sub-issues:
> - **Backend**: [TECH-003-BE — Analytics — Backend](TECH-003-BE-analytics-dashboard.md)
> - **Frontend**: [TECH-003-FE — Analytics — Frontend](TECH-003-FE-analytics-dashboard.md)

## Overview

Implement the analytics event tracking and dashboard indicators for the KitchenAI application.

## Tracked events

| Event | Trigger |
|---|---|
| `item_added` | User adds an item to inventory |
| `item_removed` | User deletes or archives an item |
| `item_consumed` | User marks item as consumed |
| `recipe_generated` | Recipe generation completes |
| `recipe_saved` | User saves a recipe |
| `recipe_cooked` | User marks a recipe as cooked |

## Dashboard indicators

| Indicator | Description |
|---|---|
| Items expiring soon | Count and list of items expiring within threshold |
| Money saved estimate | Sum of price of items used vs baseline (heuristic) |
| Food waste reduced | Count of expired items removed vs baseline |
| Most used ingredients | Frequency chart of ingredients used in recipes |

## Privacy for analytics

- Aggregate analytics only; send only non-identifiable aggregated signals to any external analytics provider unless user opts in.
- Explicit consent dialog required before enabling external analytics.

## API endpoints

- `GET /api/households/{hid}/analytics/summary`
  - Returns: `{ moneySavedEstimate, expiredItemsCount, recipesGeneratedCount, mostUsedIngredients }`

## Acceptance criteria

- `AnalyticsEvent` records are created for each tracked event.
- Dashboard summary endpoint aggregates and returns all four indicators.
- No PII included in analytics payloads sent externally.

## UI

- Screen: **Dashboard (Main)**
  - Top: household selector, key indicators (expiring soon, money saved estimate, most-used ingredients)
  - Primary action: "Generate recipes from my fridge"
  - Expiring items list (soonest first)
  - Quick add item input
