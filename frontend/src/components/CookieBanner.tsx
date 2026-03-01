import { useState } from 'react';
import { useTranslation } from 'react-i18next';

/** Appears at bottom of page until user accepts — uses localStorage for persistence. */
export default function CookieBanner() {
  const { t } = useTranslation();
  const [visible, setVisible] = useState<boolean>(
    () => !localStorage.getItem('cookieConsent'),
  );

  if (!visible) return null;

  const handleAccept = () => {
    localStorage.setItem('cookieConsent', 'accepted');
    setVisible(false);
  };

  return (
    <div
      className="fixed bottom-0 left-0 right-0 bg-gray-900 text-white px-4 py-3 flex flex-col sm:flex-row items-center justify-between gap-3 z-50 shadow-lg"
      role="dialog"
      aria-label="Cookie consent"
    >
      <p className="text-sm text-gray-200">{t('cookie.message')}</p>
      <button
        onClick={handleAccept}
        className="whitespace-nowrap bg-green-600 hover:bg-green-700 text-white px-5 py-2 rounded-lg text-sm font-medium transition-colors"
      >
        {t('cookie.accept')}
      </button>
    </div>
  );
}
