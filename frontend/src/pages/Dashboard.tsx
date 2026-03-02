import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import { getAnalytics } from '../services/analytics';
import { getItems } from '../services/items';
import LanguageSwitcher from '../components/LanguageSwitcher';
import { formatDate } from '../utils/dateFormat';

export default function DashboardPage() {
  const { t, i18n } = useTranslation();
  const { householdId } = useAuth();
  const navigate = useNavigate();

  const { data: analytics, isLoading: analyticsLoading } = useQuery({
    queryKey: ['analytics', householdId],
    queryFn: () => getAnalytics(householdId!),
    enabled: !!householdId,
  });

  const { data: expiringItems = [], isLoading: itemsLoading } = useQuery({
    queryKey: ['items-expiring', householdId],
    queryFn: () => getItems(householdId!, { expiringSoon: 'true' }),
    enabled: !!householdId,
  });

  const cards = [
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
    {
      label: t('dashboard.topIngredients'),
      value: analyticsLoading
        ? null
        : (analytics?.topIngredients?.slice(0, 3).join(', ') || '—'),
      icon: '🥗',
      bg: 'bg-purple-50 border-purple-200',
    },
  ];

  return (
    <div>
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold text-gray-800">{t('pages.dashboard')}</h1>
        <LanguageSwitcher />
      </div>

      {/* Analytics cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {cards.map((card) => (
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
      <section className="bg-white rounded-xl border border-gray-200 p-6">
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
    </div>
  );
}
