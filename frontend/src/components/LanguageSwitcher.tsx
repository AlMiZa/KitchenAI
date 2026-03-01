import { useTranslation } from 'react-i18next';

/** Small button in the header to toggle EN / PL. */
export default function LanguageSwitcher() {
  const { i18n } = useTranslation();

  const toggle = () => {
    i18n.changeLanguage(i18n.language.startsWith('pl') ? 'en' : 'pl');
  };

  const label = i18n.language.startsWith('pl') ? 'EN' : 'PL';

  return (
    <button
      onClick={toggle}
      aria-label="Switch language"
      className="text-xs font-semibold px-2.5 py-1 rounded border border-gray-300 hover:bg-gray-100 text-gray-600 transition-colors"
    >
      {label}
    </button>
  );
}
