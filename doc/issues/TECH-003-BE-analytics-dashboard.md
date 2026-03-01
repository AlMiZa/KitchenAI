# TECH-003-BE — Analytics & dashboard — Backend

**Type:** Technical Task — Backend  
**Parent:** [TECH-003 Analytics & dashboard](TECH-003-analytics-dashboard.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 4

## Overview

Implement analytics event recording and the analytics summary API endpoint.

## Tracked events

| Event | Trigger |
|---|---|
| `item_added` | User adds an item to inventory |
| `item_removed` | User deletes or archives an item |
| `item_consumed` | User marks item as consumed |
| `recipe_generated` | Recipe generation completes |
| `recipe_saved` | User saves a recipe |
| `recipe_cooked` | User marks a recipe as cooked |

## Technical notes

- Create `AnalyticsEvent` records from within the relevant MediatR handlers (or via a domain event / notification pipeline behaviour).
- Payload must not contain PII; use only IDs and event metadata.
- Aggregate analytics in the summary query handler; do not expose raw events via public API.

## API endpoint

- `GET /api/households/{hid}/analytics/summary`
  - Returns: `{ moneySavedEstimate, expiredItemsCount, recipesGeneratedCount, mostUsedIngredients }`

## Privacy

- No PII included in analytics payloads.
- Explicit user consent required before enabling any external analytics provider.

## Acceptance criteria

- `AnalyticsEvent` records are created for each of the six tracked event types.
- Summary endpoint aggregates and returns all four indicators correctly.
- No PII fields present in `AnalyticsEvent.Metadata`.
