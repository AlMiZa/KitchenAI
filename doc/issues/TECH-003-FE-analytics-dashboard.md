# TECH-003-FE — Analytics & dashboard — Frontend

**Type:** Technical Task — Frontend  
**Parent:** [TECH-003 Analytics & dashboard](TECH-003-analytics-dashboard.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 4

## Overview

Implement the dashboard indicators UI that displays analytics data fetched from the summary endpoint.

## Dashboard indicators

| Indicator | Description |
|---|---|
| Items expiring soon | Count and list of items expiring within threshold |
| Money saved estimate | Heuristic sum of prices of consumed items |
| Food waste reduced | Count of expired items removed vs baseline |
| Most used ingredients | Frequency display of top ingredients |

## Technical notes

- Fetch data with `GET /api/households/{hid}/analytics/summary` using React Query.
- Display each indicator as a card or stat widget at the top of the Dashboard page.
- Most-used ingredients can be shown as a simple ordered list or a small bar chart.
- Refresh on household change or after recipe generation/item actions.

## UI

- Screen: **Dashboard (Main)**
  - Top section: household selector, four indicator cards (expiring soon, money saved, waste reduced, top ingredients)
  - Primary action: "Generate recipes from my fridge" button
  - Expiring items list below indicators (soonest first)
  - Quick add item input at the bottom of the dashboard

## Acceptance criteria

- All four indicators render with correct data from the API.
- Loading skeleton shown while data is fetching.
- Error state shown gracefully if the summary endpoint fails.
- Dashboard refreshes indicators after significant user actions (add/delete item, generate recipe).
