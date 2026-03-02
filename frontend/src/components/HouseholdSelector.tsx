import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import { getHouseholds } from '../services/households';

/** Displays a dropdown of households when the user belongs to multiple, or
 *  a static label when there is only one. Allows switching the active household. */
export default function HouseholdSelector() {
  const { t } = useTranslation();
  const { activeHouseholdId, setActiveHouseholdId } = useAuth();

  const { data: households = [], isLoading } = useQuery({
    queryKey: ['households'],
    queryFn: getHouseholds,
  });

  if (isLoading) {
    return <span className="text-sm text-gray-400">{t('common.loading')}</span>;
  }

  if (households.length <= 1) {
    const name = households[0]?.name;
    return name ? (
      <span className="text-sm font-medium text-gray-600">{name}</span>
    ) : null;
  }

  return (
    <select
      aria-label={t('dashboard.householdSelector')}
      value={activeHouseholdId ?? ''}
      onChange={(e) => setActiveHouseholdId(e.target.value)}
      className="border border-gray-300 rounded-lg px-3 py-1.5 text-sm text-gray-700 focus:outline-none focus:ring-2 focus:ring-green-500"
    >
      {households.map((h) => (
        <option key={h.id} value={h.id}>
          {h.name}
        </option>
      ))}
    </select>
  );
}
