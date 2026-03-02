import { useState } from 'react';
import { useTranslation } from 'react-i18next';

/** Appears at bottom of page until user accepts or declines — uses localStorage for persistence. */
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

  const handleDecline = () => {
    localStorage.setItem('cookieConsent', 'declined');
    setVisible(false);
  };

  return (
    <div
      className="fixed bottom-0 left-0 right-0 bg-gray-900 text-white px-4 py-3 flex flex-col sm:flex-row items-center justify-between gap-3 z-50 shadow-lg"
      role="dialog"
      aria-modal="true"
      aria-label={t('cookie.bannerLabel')}
    >
      <p className="text-sm text-gray-200">{t('cookie.message')}</p>
      <div className="flex gap-2 shrink-0">
        <button
          onClick={handleDecline}
          className="whitespace-nowrap border border-gray-400 hover:border-gray-200 text-gray-300 hover:text-white px-5 py-2 rounded-lg text-sm font-medium transition-colors"
        >
          {t('cookie.decline')}
        </button>
        <button
          onClick={handleAccept}
          className="whitespace-nowrap bg-green-600 hover:bg-green-700 text-white px-5 py-2 rounded-lg text-sm font-medium transition-colors"
        >
          {t('cookie.accept')}
        </button>
      </div>
    </div>
  );
}
