import { useTranslation } from 'react-i18next';

/** Placeholder admin page for API Keys. */
export default function ApiKeysPage() {
  const { t } = useTranslation();
  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-800">{t('pages.admin.apiKeys')}</h1>
      <p className="mt-2 text-gray-500">Content coming soon.</p>
    </div>
  );
}
