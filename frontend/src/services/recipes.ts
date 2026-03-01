import { apiFetch } from './api';

export interface NutritionInfo {
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface RecipeIngredient {
  name: string;
  quantity: number;
  unit: string;
  status?: 'available' | 'partial' | 'missing';
}

export interface Recipe {
  id: string;
  householdId: string;
  title: string;
  prepTime: number;
  cookTime: number;
  servings: number;
  ingredients: RecipeIngredient[];
  steps: string[];
  nutrition: NutritionInfo;
  rationale: string;
  savedAt?: string;
}

export interface IngredientCheckResult {
  ingredients: RecipeIngredient[];
  allAvailable: boolean;
  missingCount: number;
}

export const generateRecipes = (householdId: string) =>
  apiFetch<Recipe[]>(`/households/${householdId}/recipes/generate`, { method: 'POST' });

export const getRecipes = (householdId: string) =>
  apiFetch<Recipe[]>(`/households/${householdId}/recipes`);

export const getRecipe = (householdId: string, recipeId: string) =>
  apiFetch<Recipe>(`/households/${householdId}/recipes/${recipeId}`);

export const saveRecipe = (householdId: string, recipe: Recipe) =>
  apiFetch<Recipe>(`/households/${householdId}/recipes`, {
    method: 'POST',
    body: JSON.stringify(recipe),
  });

// ── Backend availability types ────────────────────────────────────────────────
// Backend AvailabilityResultDto shape returned by POST …/check
interface BackendAvailabilityItem {
  name: string;
  required: number;
  available: number;
  deficit: number;
}

interface BackendAvailabilityResult {
  status: string; // 'ready' | 'missing'
  items: BackendAvailabilityItem[];
}

export const checkRecipeAvailability = async (
  householdId: string,
  recipeId: string,
): Promise<IngredientCheckResult> => {
  const dto = await apiFetch<BackendAvailabilityResult>(
    `/households/${householdId}/recipes/${recipeId}/check`,
    { method: 'POST' },
  );

  // Map each backend item to a RecipeIngredient with a derived status:
  //   deficit === 0              → 'available'
  //   deficit > 0 && available > 0 → 'partial'
  //   available === 0            → 'missing'
  const ingredients: RecipeIngredient[] = (dto.items ?? []).map((item) => ({
    name: item.name,
    quantity: item.required,
    unit: '', // backend availability check does not return units; populated from recipe data if needed
    status:
      item.deficit === 0
        ? 'available'
        : item.available > 0
          ? 'partial'
          : 'missing',
  }));

  return {
    ingredients,
    allAvailable: dto.status === 'ready',
    missingCount: ingredients.filter((i) => i.status !== 'available').length,
  };
};
