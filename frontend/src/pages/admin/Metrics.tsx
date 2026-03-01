import { useTranslation } from 'react-i18next';

/** Placeholder admin page for Metrics. */
export default function MetricsPage() {
  const { t } = useTranslation();
  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-800">{t('pages.admin.metrics')}</h1>
      <p className="mt-2 text-gray-500">Content coming soon.</p>
    </div>
  );
}
