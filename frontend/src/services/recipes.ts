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

export const checkRecipeAvailability = (householdId: string, recipeId: string) =>
  apiFetch<IngredientCheckResult>(
    `/households/${householdId}/recipes/${recipeId}/check`,
    { method: 'POST' },
  );
