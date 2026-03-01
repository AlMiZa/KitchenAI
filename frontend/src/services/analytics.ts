import { apiFetch } from './api';

export interface AnalyticsSummary {
  expiringSoonCount: number;
  moneySaved: number;
  recipesGenerated: number;
  topIngredients: string[];
}

export const getAnalytics = (householdId: string) =>
  apiFetch<AnalyticsSummary>(`/households/${householdId}/analytics`);
