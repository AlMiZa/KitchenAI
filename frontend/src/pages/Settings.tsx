import { useTranslation } from 'react-i18next';

/** Placeholder page for Settings. */
export default function SettingsPage() {
  const { t } = useTranslation();
  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-800">{t('pages.settings')}</h1>
      <p className="mt-2 text-gray-500">Content coming soon.</p>
    </div>
  );
}
