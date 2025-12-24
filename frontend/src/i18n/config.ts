import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'
import LanguageDetector from 'i18next-browser-languagedetector'

import enTranslations from '../locales/en.json'
import arTranslations from '../locales/ar.json'

i18n
  // Detect user language
  .use(LanguageDetector)
  // Pass the i18n instance to react-i18next
  .use(initReactI18next)
  // Initialize i18next
  .init({
    resources: {
      en: {
        translation: enTranslations
      },
      ar: {
        translation: arTranslations
      }
    },
    fallbackLng: 'en',
    supportedLngs: ['en', 'ar'],
    defaultNS: 'translation',
    interpolation: {
      escapeValue: false // React already escapes values
    },
    detection: {
      // Order of detection methods
      order: ['localStorage', 'navigator', 'htmlTag'],
      // Cache user language
      caches: ['localStorage'],
      // Look for 'lang' key in localStorage
      lookupLocalStorage: 'i18nextLng'
    },
    react: {
      useSuspense: false
    }
  })

export default i18n






















