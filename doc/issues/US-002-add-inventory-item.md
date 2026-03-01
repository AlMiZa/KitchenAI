# US-002 — Add inventory item (core)

**Type:** User Story  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 1

## Story

As a user I want to add products with quantity, units and expiry so I can track them.

## Acceptance criteria

- User can enter fractional quantity (e.g., 0.5); unit must be metric (g, kg, ml, L, pcs).
- Item appears in household inventory immediately.
- Items are ordered by expiry date when viewing "expiring soon".

## Technical notes

- Merge duplicates automatically when metadata matches exactly (same name, storage, best-by type, expiry/purchase date optionally).
- Expired items are flagged; app shows alarms; removal is user-initiated.
- Support `AllowFraction` boolean as a UI hint per item.

## Data model — Item entity

| Field | Type | Notes |
|---|---|---|
| Id | GUID | |
| HouseholdId | GUID | |
| Name | string | |
| Quantity | decimal | supports fractions |
| Unit | enum/string | g, kg, ml, L, pcs |
| AllowFraction | bool | UI hint |
| PurchaseDate | date | optional |
| ExpiryDate | date | optional |
| BestByOrUseBy | enum | |
| StorageLocation | enum | Fridge/Freezer/Pantry/Other |
| Brand | string | |
| Price | decimal | |
| Notes | text | |
| CreatedAt | datetime | |
| UpdatedAt | datetime | |
| IsArchived | bool | for removed/consumed items |

## API endpoints

- `GET /api/households/{hid}/items` — Query: filter by location, expiringSoon
- `POST /api/households/{hid}/items` — Body: item DTO
- `PUT /api/households/{hid}/items/{itemId}`
- `DELETE /api/households/{hid}/items/{itemId}`
- `POST /api/households/{hid}/items/merge` — Body: list of itemIds → backend merges per rules

## Tests

- Handler test for item creation: persists all fields correctly.
- Frontend component test: validates fractional input.

## UI

- Screen: **Add / Edit Item** (modal or page)
- Fields: name, qty, unit (metric-only), purchase date, expiry date, best-by/use-by, storage location, brand, price, notes
- CTA: "Save item"
- Screen: **Inventory list**
  - Filters: location, expiringSoon, lowStock
  - Item row: name, qty + unit, expiry date, storage location, quick edit button
