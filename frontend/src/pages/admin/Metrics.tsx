import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { apiFetch } from '../../services/api';

interface MetricsSummary {
  totalUsers: number;
  totalHouseholds: number;
  totalRecipesGenerated: number;
  totalItemsTracked: number;
}

export default function MetricsPage() {
  const { t } = useTranslation();

  const { data: metrics, isLoading } = useQuery({
    queryKey: ['admin-metrics'],
    queryFn: () => apiFetch<MetricsSummary>('/admin/metrics'),
  });

  const cards = metrics
    ? [
        { label: t('admin.metrics.totalUsers'),      value: metrics.totalUsers },
        { label: t('admin.metrics.totalHouseholds'), value: metrics.totalHouseholds },
        { label: t('admin.metrics.totalRecipes'),    value: metrics.totalRecipesGenerated },
        { label: t('admin.metrics.totalItems'),      value: metrics.totalItemsTracked },
      ]
    : [];

  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-800 mb-6">
        {t('pages.admin.metrics')}
      </h1>

      <h2 className="text-sm font-semibold text-gray-500 mb-4">{t('admin.metrics.summary')}</h2>

      {isLoading ? (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="animate-pulse bg-white rounded-xl border border-gray-200 p-5 h-20" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {cards.map(({ label, value }) => (
            <div key={label} className="bg-white rounded-xl border border-gray-200 p-5">
              <div className="text-2xl font-bold text-gray-800">{value}</div>
              <div className="text-sm text-gray-500 mt-1">{label}</div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
