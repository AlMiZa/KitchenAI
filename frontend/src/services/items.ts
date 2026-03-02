import { apiFetch } from './api';

export interface InventoryItem {
  id: string;
  householdId: string;
  name: string;
  quantity: number;
  unit: 'g' | 'kg' | 'ml' | 'L' | 'pcs';
  purchaseDate?: string;
  expiryDate?: string;
  /** Matches backend StorageLocation enum (camelCase): fridge | freezer | pantry | other */
  storageLocation?: 'fridge' | 'freezer' | 'pantry' | 'other';
  brand?: string;
  price?: number;
  notes?: string;
  /** Matches backend BestByOrUseBy enum (camelCase): bestBy | useBy */
  bestByOrUseBy?: 'bestBy' | 'useBy';
}

export type CreateItemInput = Omit<InventoryItem, 'id' | 'householdId'>;
export type UpdateItemInput = Partial<CreateItemInput>;

export const getItems = (householdId: string, params?: Record<string, string>) => {
  const query = params ? '?' + new URLSearchParams(params).toString() : '';
  return apiFetch<InventoryItem[]>(`/households/${householdId}/items${query}`);
};

export const createItem = (householdId: string, data: CreateItemInput) =>
  apiFetch<InventoryItem>(`/households/${householdId}/items`, {
    method: 'POST',
    body: JSON.stringify(data),
  });

export const updateItem = (householdId: string, itemId: string, data: UpdateItemInput) =>
  apiFetch<InventoryItem>(`/households/${householdId}/items/${itemId}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });

export const deleteItem = (householdId: string, itemId: string) =>
  apiFetch<void>(`/households/${householdId}/items/${itemId}`, { method: 'DELETE' });
