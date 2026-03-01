# US-002-FE — Add inventory item — Frontend

**Type:** User Story — Frontend  
**Parent:** [US-002 Add inventory item](US-002-add-inventory-item.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 1

## Story

As a user I want an easy-to-use form and inventory list so I can add and view my products at a glance.

## Acceptance criteria

- Add/Edit item form accepts fractional quantities (e.g., 0.5) and rejects non-numeric input.
- Unit field is restricted to metric values only (g, kg, ml, L, pcs).
- After saving, the new item appears in the inventory list immediately without a full page reload.
- Inventory list supports filtering by location and "expiring soon".

## Technical notes

- Use React Query (`useQuery` / `useMutation`) to fetch and update inventory data.
- Form built with controlled components or React Hook Form; validate on submit and on blur.
- Fractional quantity: `type="number"` with `step="any"` and min `0`.
- Display items sorted by expiry date (soonest first) when `expiringSoon` filter is active.
- Show a warning badge or colour on items that are expired or expiring within the configured threshold.

## UI

- Screen: **Add / Edit Item** (modal or dedicated page)
  - Fields: name, qty (fractional-capable), unit (metric-only dropdown), purchase date, expiry date, best-by/use-by, storage location, brand, price, notes
  - CTA: "Save item"
- Screen: **Inventory list**
  - Filter bar: location selector, "Expiring soon" toggle, "Low stock" toggle
  - Item row: name, qty + unit, expiry date, storage location, quick-edit icon button

## Tests

- Frontend component test: qty input accepts `0.5` and rejects `abc`.
- Frontend component test: unit dropdown only shows metric options.
- Frontend component test: inventory list renders items sorted by expiry date.
