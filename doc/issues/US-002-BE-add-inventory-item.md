# US-002-BE — Add inventory item — Backend

**Type:** User Story — Backend  
**Parent:** [US-002 Add inventory item](US-002-add-inventory-item.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 1

## Story

As a user I want the backend to persist inventory items with all metadata so data is not lost between sessions.

## Acceptance criteria

- Item creation persists all fields (name, quantity, unit, dates, location, brand, price, notes) correctly.
- Items are ordered by `ExpiryDate` ascending when queried with `expiringSoon` filter.
- Duplicate items (same name, storage, best-by type, expiry/purchase date) are auto-merged.
- Fractional quantities (e.g., 0.5) are stored as `decimal` without rounding.

## Technical notes

- Implement MediatR commands: `CreateItemCommand`, `UpdateItemCommand`, `DeleteItemCommand`.
- Implement MediatR query: `GetItemsQuery` with optional filters (`location`, `expiringSoon`).
- Implement `MergeItemsCommand` that combines selected item IDs into one record.
- Expired items are not auto-deleted; set `IsArchived = false` and flag for UI to display an alarm.
- `AllowFraction` flag stored as a boolean hint for the frontend.

## API endpoints

- `GET /api/households/{hid}/items` — Query params: `location`, `expiringSoon`
- `POST /api/households/{hid}/items` — Body: item DTO
- `PUT /api/households/{hid}/items/{itemId}`
- `DELETE /api/households/{hid}/items/{itemId}`
- `POST /api/households/{hid}/items/merge` — Body: list of `itemId`s → merges per deduplication rules

## Tests

- Handler test: `CreateItemCommand` persists all fields correctly.
- Handler test: `GetItemsQuery` with `expiringSoon=true` returns items ordered by `ExpiryDate` ascending.
- Handler test: `MergeItemsCommand` combines quantities of duplicate items.
