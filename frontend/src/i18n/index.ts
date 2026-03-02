import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import en from './en.json';
import pl from './pl.json';

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: { translation: en },
      pl: { translation: pl },
    },
    lng: 'pl',
    fallbackLng: 'en',
    supportedLngs: ['en', 'pl'],
    interpolation: {
      escapeValue: false,
    },
    saveMissing: true,
    missingKeyHandler: (_lngs: readonly string[], _ns: string, key: string) => {
      // Only log in non-production environments to avoid cluttering production console.
      if (process.env.NODE_ENV !== 'production') {
        console.warn(`[i18n] Missing translation key: "${key}"`);
      }
    },
  });

export default i18n;
