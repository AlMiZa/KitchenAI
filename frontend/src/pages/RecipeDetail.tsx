import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import { getRecipe, checkRecipeAvailability } from '../services/recipes';
import type { IngredientCheckResult } from '../services/recipes';

function statusIcon(status?: string): string {
  switch (status) {
    case 'available': return '✓';
    case 'partial':   return '⚠';
    case 'missing':   return '✗';
    default:          return '';
  }
}

function statusClass(status?: string): string {
  switch (status) {
    case 'available': return 'text-green-600';
    case 'partial':   return 'text-amber-500';
    case 'missing':   return 'text-red-500';
    default:          return '';
  }
}

export default function RecipeDetailPage() {
  const { t } = useTranslation();
  const { recipeId } = useParams<{ recipeId: string }>();
  const { householdId } = useAuth();

  const [checkResult, setCheckResult] = useState<IngredientCheckResult | null>(null);

  const { data: recipe, isLoading } = useQuery({
    queryKey: ['recipe', householdId, recipeId],
    queryFn: () => getRecipe(householdId!, recipeId!),
    enabled: !!householdId && !!recipeId,
  });

  const checkMut = useMutation({
    mutationFn: () => checkRecipeAvailability(householdId!, recipeId!),
    onSuccess: (result) => setCheckResult(result),
  });

  const addMissingToShoppingList = () => {
    if (!checkResult) return;
    const missing = checkResult.ingredients
      .filter((i) => i.status === 'missing' || i.status === 'partial')
      .map((i) => i.name);
    const existing: string[] = JSON.parse(
      localStorage.getItem('shoppingList') ?? '[]',
    );
    localStorage.setItem(
      'shoppingList',
      JSON.stringify([...new Set([...existing, ...missing])]),
    );
  };

  if (isLoading) {
    return (
      <div>
        <h1 className="text-2xl font-semibold text-gray-800 mb-4">{t('pages.recipes')}</h1>
        <div className="animate-pulse space-y-4 max-w-2xl">
          <div className="h-6 bg-gray-200 rounded w-1/2" />
          <div className="h-4 bg-gray-100 rounded w-full" />
          <div className="h-4 bg-gray-100 rounded w-3/4" />
        </div>
      </div>
    );
  }

  if (!recipe) {
    return (
      <div>
        <h1 className="text-2xl font-semibold text-gray-800">{t('pages.recipes')}</h1>
        <p className="text-gray-500 mt-4">{t('common.error')}</p>
      </div>
    );
  }

  const ingredients = checkResult ? checkResult.ingredients : recipe.ingredients;

  return (
    <div className="max-w-2xl">
      <h1 className="text-2xl font-semibold text-gray-800 mb-2">{recipe.title}</h1>

      <div className="flex gap-4 text-sm text-gray-500 mb-6">
        <span>⏱ {recipe.prepTime}m {t('recipes.prepTime')}</span>
        <span>🔥 {recipe.cookTime}m {t('recipes.cookTime')}</span>
        <span>👤 {recipe.servings} {t('recipes.servings')}</span>
      </div>

      {/* Nutrition */}
      <div className="grid grid-cols-4 gap-3 mb-6 bg-gray-50 rounded-xl p-4 text-center text-sm">
        {[
          { v: recipe.nutrition.calories,        k: t('recipes.calories'), u: '' },
          { v: recipe.nutrition.protein,         k: t('recipes.protein'),  u: 'g' },
          { v: recipe.nutrition.carbs,           k: t('recipes.carbs'),    u: 'g' },
          { v: recipe.nutrition.fat,             k: t('recipes.fat'),      u: 'g' },
        ].map(({ v, k, u }) => (
          <div key={k}>
            <div className="font-semibold text-gray-800">{v}{u}</div>
            <div className="text-xs text-gray-400">{k}</div>
          </div>
        ))}
      </div>

      {/* Check inventory button */}
      <div className="mb-6">
        <button
          onClick={() => checkMut.mutate()}
          disabled={checkMut.isPending || !householdId}
          className="bg-green-600 hover:bg-green-700 disabled:opacity-50 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors"
        >
          {checkMut.isPending ? t('common.loading') : t('recipes.checkInventory')}
        </button>
      </div>

      {/* Check results banner */}
      {checkResult && (
        <div className="mb-6">
          {checkResult.allAvailable ? (
            <div className="bg-green-50 border border-green-200 rounded-xl p-4 text-green-700 font-semibold text-center">
              {t('recipes.readyToCook')}
            </div>
          ) : (
            <div className="bg-amber-50 border border-amber-200 rounded-xl p-4 space-y-3">
              <p className="text-amber-700 text-sm font-medium">
                {checkResult.missingCount} {t('recipes.missing').toLowerCase()}
              </p>
              <button
                onClick={addMissingToShoppingList}
                className="bg-amber-500 hover:bg-amber-600 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors"
              >
                {t('recipes.addMissingToList')}
              </button>
            </div>
          )}
        </div>
      )}

      {/* Rationale */}
      <section className="mb-6">
        <h2 className="text-base font-semibold text-gray-800 mb-2">{t('recipes.rationale')}</h2>
        <p className="text-gray-600 text-sm leading-relaxed">{recipe.rationale}</p>
      </section>

      {/* Ingredients */}
      <section className="mb-6">
        <h2 className="text-base font-semibold text-gray-800 mb-3">{t('recipes.ingredients')}</h2>
        <ul className="space-y-2">
          {ingredients.map((ing, i) => (
            <li key={i} className="flex items-center gap-2 text-sm">
              {checkResult && (
                <span className={`font-bold w-4 text-center ${statusClass(ing.status)}`}>
                  {statusIcon(ing.status)}
                </span>
              )}
              <span className="text-gray-700">
                {ing.quantity} {ing.unit} {ing.name}
              </span>
            </li>
          ))}
        </ul>
      </section>

      {/* Steps */}
      <section>
        <h2 className="text-base font-semibold text-gray-800 mb-3">{t('recipes.steps')}</h2>
        <ol className="space-y-3">
          {recipe.steps.map((step, i) => (
            <li key={i} className="flex gap-3 text-sm text-gray-700">
              <span className="flex-shrink-0 w-6 h-6 bg-green-100 text-green-700 rounded-full flex items-center justify-center font-medium text-xs">
                {i + 1}
              </span>
              <span>{step}</span>
            </li>
          ))}
        </ol>
      </section>
    </div>
  );
}
