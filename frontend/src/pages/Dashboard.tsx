import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import { getAnalytics } from '../services/analytics';
import { getItems, createItem } from '../services/items';
import HouseholdSelector from '../components/HouseholdSelector';
import LanguageSwitcher from '../components/LanguageSwitcher';
import { formatDate } from '../utils/dateFormat';

export default function DashboardPage() {
  const { t, i18n } = useTranslation();
  const { householdId } = useAuth();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [quickAddName, setQuickAddName] = useState('');

  const {
    data: analytics,
    isLoading: analyticsLoading,
    isError: analyticsError,
    refetch: refetchAnalytics,
  } = useQuery({
    queryKey: ['analytics', householdId],
    queryFn: () => getAnalytics(householdId!),
    enabled: !!householdId,
  });

  const {
    data: expiringItems = [],
    isLoading: itemsLoading,
    isError: itemsError,
    refetch: refetchItems,
  } = useQuery({
    queryKey: ['items-expiring', householdId],
    queryFn: () => getItems(householdId!, { expiringSoon: 'true' }),
    enabled: !!householdId,
  });

  const addItemMutation = useMutation({
    mutationFn: (name: string) =>
      createItem(householdId!, { name, quantity: 1, unit: 'pcs' }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['items-expiring', householdId] });
      setQuickAddName('');
    },
  });

  const handleQuickAdd = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = quickAddName.trim();
    if (trimmed) {
      addItemMutation.mutate(trimmed);
    }
  };

  const simpleCards = [
    {
      label: t('dashboard.expiringSoon'),
      value: analyticsLoading ? null : (analytics?.expiringSoonCount ?? 0),
      icon: '⚠️',
      bg: 'bg-amber-50 border-amber-200',
    },
    {
      label: t('dashboard.moneySaved'),
      value: analyticsLoading ? null : `$${(analytics?.moneySaved ?? 0).toFixed(2)}`,
      icon: '💰',
      bg: 'bg-green-50 border-green-200',
    },
    {
      label: t('dashboard.recipesGenerated'),
      value: analyticsLoading ? null : (analytics?.recipesGenerated ?? 0),
      icon: '🍳',
      bg: 'bg-blue-50 border-blue-200',
    },
  ];

  const topIngredients = analytics?.topIngredients?.slice(0, 5) ?? [];

  return (
    <div>
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <h1 className="text-2xl font-semibold text-gray-800">{t('pages.dashboard')}</h1>
          <HouseholdSelector />
        </div>
        <LanguageSwitcher />
      </div>

      {/* Error banner */}
      {(analyticsError || itemsError) && (
        <div className="mb-6 flex items-center justify-between gap-4 rounded-xl border border-red-200 bg-red-50 px-5 py-3 text-sm text-red-700">
          <span>{t('dashboard.analyticsError')}</span>
          <button
            onClick={() => { void refetchAnalytics(); void refetchItems(); }}
            className="shrink-0 rounded-md border border-red-300 bg-white px-3 py-1 font-medium hover:bg-red-50"
          >
            {t('common.retry')}
          </button>
        </div>
      )}

      {/* Analytics cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {simpleCards.map((card) => (
          <div key={card.label} className={`p-5 rounded-xl border ${card.bg}`}>
            {card.value === null ? (
              <div className="animate-pulse space-y-2">
                <div className="h-8 bg-white/60 rounded w-1/2" />
                <div className="h-4 bg-white/40 rounded w-3/4" />
              </div>
            ) : (
              <>
                <div className="text-2xl mb-1">{card.icon}</div>
                <div className="text-2xl font-bold text-gray-800">{card.value}</div>
                <div className="text-sm text-gray-500 mt-0.5">{card.label}</div>
              </>
            )}
          </div>
        ))}

        {/* Top ingredients card — ordered list */}
        <div className="p-5 rounded-xl border bg-purple-50 border-purple-200">
          {analyticsLoading ? (
            <div className="animate-pulse space-y-2">
              <div className="h-8 bg-white/60 rounded w-1/2" />
              <div className="h-4 bg-white/40 rounded w-3/4" />
            </div>
          ) : (
            <>
              <div className="text-2xl mb-1">🥗</div>
              <div className="text-sm text-gray-500 mb-1">{t('dashboard.topIngredients')}</div>
              {topIngredients.length === 0 ? (
                <span className="text-sm font-bold text-gray-800">—</span>
              ) : (
                <ol className="list-decimal list-inside text-sm text-gray-700 space-y-0.5">
                  {topIngredients.map((ing) => (
                    <li key={ing}>{ing}</li>
                  ))}
                </ol>
              )}
            </>
          )}
        </div>
      </div>

      {/* Primary CTA */}
      <div className="mb-8">
        <button
          onClick={() => navigate('/recipes')}
          className="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-xl font-medium"
        >
          🍳 {t('dashboard.generateRecipes')}
        </button>
      </div>

      {/* Expiring items list */}
      <section className="bg-white rounded-xl border border-gray-200 p-6 mb-6">
        <h2 className="text-base font-semibold text-gray-800 mb-4">
          {t('dashboard.expiringItems')}
        </h2>

        {itemsLoading ? (
          <div className="animate-pulse space-y-2">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-8 bg-gray-100 rounded" />
            ))}
          </div>
        ) : expiringItems.length === 0 ? (
          <p className="text-gray-400 text-sm">{t('dashboard.noExpiringItems')}</p>
        ) : (
          <ul className="divide-y divide-gray-100">
            {expiringItems.map((item) => (
              <li key={item.id} className="py-2.5 flex items-center justify-between">
                <span className="text-gray-800 text-sm font-medium">{item.name}</span>
                <span className="text-xs text-amber-600 font-medium">
                  {item.expiryDate
                    ? formatDate(item.expiryDate, i18n.language)
                    : ''}
                </span>
              </li>
            ))}
          </ul>
        )}
      </section>

      {/* Quick add item */}
      <section className="bg-white rounded-xl border border-gray-200 p-6">
        <form onSubmit={handleQuickAdd} className="flex gap-2">
          <input
            type="text"
            value={quickAddName}
            onChange={(e) => setQuickAddName(e.target.value)}
            placeholder={t('dashboard.quickAddPlaceholder')}
            aria-label={t('dashboard.quickAddPlaceholder')}
            className="flex-1 rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
          />
          <button
            type="submit"
            disabled={!quickAddName.trim() || addItemMutation.isPending}
            className="rounded-lg bg-green-600 px-4 py-2 text-sm font-medium text-white hover:bg-green-700 disabled:opacity-50"
          >
            {t('dashboard.quickAddButton')}
          </button>
        </form>
      </section>
    </div>
  );
}
