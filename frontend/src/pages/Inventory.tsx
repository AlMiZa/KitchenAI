import { useTranslation } from 'react-i18next';

/** Placeholder page for Inventory. */
export default function InventoryPage() {
  const { t } = useTranslation();
  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-800">{t('pages.inventory')}</h1>
      <p className="mt-2 text-gray-500">Content coming soon.</p>
    </div>
  );
}
