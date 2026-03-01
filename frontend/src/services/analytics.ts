import { apiFetch } from './api';

export interface AnalyticsSummary {
  expiringSoonCount: number;
  moneySaved: number;
  recipesGenerated: number;
  topIngredients: string[];
}

// Mirrors the backend AnalyticsSummaryDto (camelCase via JSON serialiser)
interface BackendAnalyticsSummary {
  moneySavedEstimate: number;
  expiredItemsCount: number;
  recipesGeneratedCount: number;
  mostUsedIngredients: string[];
}

export const getAnalytics = async (householdId: string): Promise<AnalyticsSummary> => {
  // Backend path is /analytics/summary, not /analytics
  const dto = await apiFetch<BackendAnalyticsSummary>(
    `/households/${householdId}/analytics/summary`,
  );
  return {
    // Note: backend tracks already-expired items; mapped here as expiringSoonCount until
    // the backend exposes a dedicated expiring-soon count.
    expiringSoonCount: dto.expiredItemsCount ?? 0,
    moneySaved: dto.moneySavedEstimate ?? 0,
    recipesGenerated: dto.recipesGeneratedCount ?? 0,
    topIngredients: dto.mostUsedIngredients ?? [],
  };
};
