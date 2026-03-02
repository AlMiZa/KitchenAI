import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import { getRecipes, generateRecipes, saveRecipe } from '../services/recipes';
import type { Recipe } from '../services/recipes';

interface RecipeCardProps {
  recipe: Recipe;
  onSave: (r: Recipe) => void;
}

function RecipeCard({ recipe, onSave }: RecipeCardProps) {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <article className="bg-white rounded-xl border border-gray-200 p-5 flex flex-col gap-3">
      <h3 className="text-lg font-semibold text-gray-800">{recipe.title}</h3>

      {/* Meta */}
      <div className="flex gap-4 text-xs text-gray-500">
        <span>⏱ {recipe.prepTime}m {t('recipes.prepTime')}</span>
        <span>🔥 {recipe.cookTime}m {t('recipes.cookTime')}</span>
        <span>👤 {recipe.servings} {t('recipes.servings')}</span>
      </div>

      {/* Nutrition */}
      <div className="grid grid-cols-4 gap-2 bg-gray-50 rounded-lg p-3 text-center text-xs">
        <div>
          <div className="font-semibold text-gray-800">{recipe.nutrition.calories}</div>
          <div className="text-gray-400">{t('recipes.calories')}</div>
        </div>
        <div>
          <div className="font-semibold text-gray-800">{recipe.nutrition.protein}g</div>
          <div className="text-gray-400">{t('recipes.protein')}</div>
        </div>
        <div>
          <div className="font-semibold text-gray-800">{recipe.nutrition.carbs}g</div>
          <div className="text-gray-400">{t('recipes.carbs')}</div>
        </div>
        <div>
          <div className="font-semibold text-gray-800">{recipe.nutrition.fat}g</div>
          <div className="text-gray-400">{t('recipes.fat')}</div>
        </div>
      </div>

      {/* Rationale — collapsible via <details> so text is always in DOM */}
      <details>
        <summary className="cursor-pointer text-sm text-green-600 hover:underline select-none">
          {t('recipes.rationale')}
        </summary>
        <p className="mt-2 text-sm text-gray-600 border-l-2 border-green-200 pl-3">
          {recipe.rationale}
        </p>
      </details>

      {/* Actions */}
      <div className="flex gap-2 mt-auto">
        <button
          onClick={() => navigate(`/recipes/${recipe.id}`)}
          className="flex-1 border border-gray-300 hover:bg-gray-50 text-gray-700 px-3 py-2 rounded-lg text-sm font-medium transition-colors"
        >
          {t('recipes.viewRecipe')}
        </button>
        <button
          onClick={() => onSave(recipe)}
          className="flex-1 bg-green-600 hover:bg-green-700 text-white px-3 py-2 rounded-lg text-sm font-medium transition-colors"
        >
          {t('recipes.saveRecipe')}
        </button>
      </div>
    </article>
  );
}

export default function RecipesPage() {
  const { t } = useTranslation();
  const { householdId } = useAuth();
  const queryClient = useQueryClient();

  const [generated, setGenerated] = useState<Recipe[]>([]);

  const { data: saved = [], isLoading } = useQuery({
    queryKey: ['recipes', householdId],
    queryFn: () => getRecipes(householdId!),
    enabled: !!householdId,
  });

  const generateMut = useMutation({
    mutationFn: () => generateRecipes(householdId!),
    onSuccess: (data) => setGenerated(data),
  });

  const [saveError, setSaveError] = useState<string | null>(null);

  const saveMut = useMutation({
    mutationFn: (recipe: Recipe) => saveRecipe(householdId!, recipe),
    onSuccess: () => {
      setSaveError(null);
      queryClient.invalidateQueries({ queryKey: ['recipes', householdId] });
    },
    onError: (err: Error) => setSaveError(err.message),
  });

  const display = generated.length > 0 ? generated : saved;

  return (
    <div>
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold text-gray-800">{t('pages.recipes')}</h1>
        <button
          onClick={() => generateMut.mutate()}
          disabled={generateMut.isPending || !householdId}
          className="inline-flex items-center gap-2 bg-green-600 hover:bg-green-700 disabled:opacity-50 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors"
        >
          {generateMut.isPending && (
            <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24" fill="none" aria-hidden="true">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.4 0 0 5.4 0 12h4z" />
            </svg>
          )}
          {generateMut.isPending ? t('recipes.generating') : t('recipes.generateFromFridge')}
        </button>
      </div>

      {/* Generate error with retry */}
      {generateMut.isError && (
        <div className="mb-4 flex items-center gap-3 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          <span>{t('recipes.generateError')}</span>
          <button
            onClick={() => generateMut.mutate()}
            className="ml-auto font-medium underline hover:no-underline"
          >
            {t('common.retry')}
          </button>
        </div>
      )}

      {/* Save error (e.g. duplicate) */}
      {saveError && (
        <div className="mb-4 flex items-center justify-between rounded-lg border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800">
          <span>{saveError}</span>
          <button onClick={() => setSaveError(null)} className="ml-4 font-bold">✕</button>
        </div>
      )}

      {/* Skeleton loading */}
      {isLoading && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {[1, 2].map((i) => (
            <div key={i} className="animate-pulse bg-white rounded-xl border border-gray-200 p-5 space-y-3">
              <div className="h-5 bg-gray-200 rounded w-2/3" />
              <div className="h-4 bg-gray-100 rounded w-full" />
              <div className="h-16 bg-gray-100 rounded" />
            </div>
          ))}
        </div>
      )}

      {/* Empty state */}
      {!isLoading && display.length === 0 && (
        <div className="text-center py-16 text-gray-400">
          <p className="text-4xl mb-3">🍳</p>
          <p>{t('recipes.noRecipes')}</p>
        </div>
      )}

      {/* Recipe grid */}
      {display.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {display.map((recipe) => (
            <RecipeCard
              key={recipe.id}
              recipe={recipe}
              onSave={(r) => saveMut.mutate(r)}
            />
          ))}
        </div>
      )}
    </div>
  );
}
