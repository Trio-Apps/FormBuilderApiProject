import React, { createContext, useContext, useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'

interface LanguageContextType {
  currentLanguage: string
  changeLanguage: (lang: 'en' | 'ar') => void
  t: (key: string, options?: any) => string
  isRTL: boolean
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined)

/**
 * Determine initial language based on priority:
 * 1. Query parameter (?lang=ar)
 * 2. User profile (from localStorage or API)
 * 3. Browser language
 * 4. LocalStorage preference
 * 5. Default: 'en'
 */
const determineInitialLanguage = (): string => {
  // 1. Check query parameter
  const urlParams = new URLSearchParams(window.location.search)
  const langParam = urlParams.get('lang')
  if (langParam === 'ar' || langParam === 'en') {
    return langParam
  }

  // 2. Check user profile (from localStorage - can be extended to API call)
  const userProfileLang = localStorage.getItem('userPreferredLanguage')
  if (userProfileLang === 'ar' || userProfileLang === 'en') {
    return userProfileLang
  }

  // 3. Check browser language
  const browserLang = navigator.language || (navigator as any).userLanguage
  if (browserLang.startsWith('ar')) {
    return 'ar'
  }

  // 4. Check localStorage preference
  const storedLang = localStorage.getItem('i18nextLng')
  if (storedLang === 'ar' || storedLang === 'en') {
    return storedLang
  }

  // 5. Default
  return 'en'
}

export const LanguageProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { i18n, t } = useTranslation()
  const initialLang = determineInitialLanguage()
  const [currentLanguage, setCurrentLanguage] = useState<string>(initialLang)

  useEffect(() => {
    // Set initial language
    if (i18n.language !== initialLang) {
      i18n.changeLanguage(initialLang)
    }

    const handleLanguageChange = (lng: string) => {
      setCurrentLanguage(lng)
      // Update document direction
      document.documentElement.dir = lng === 'ar' ? 'rtl' : 'ltr'
      document.documentElement.lang = lng
      // Update localStorage
      localStorage.setItem('i18nextLng', lng)
    }

    // Set initial direction
    handleLanguageChange(initialLang)

    // Listen for language changes
    i18n.on('languageChanged', handleLanguageChange)

    return () => {
      i18n.off('languageChanged', handleLanguageChange)
    }
  }, [i18n, initialLang])

  const changeLanguage = (lang: 'en' | 'ar') => {
    i18n.changeLanguage(lang)
    localStorage.setItem('i18nextLng', lang)
    // Update query parameter if exists
    const urlParams = new URLSearchParams(window.location.search)
    if (urlParams.has('lang')) {
      urlParams.set('lang', lang)
      const newUrl = `${window.location.pathname}?${urlParams.toString()}`
      window.history.replaceState({}, '', newUrl)
    }
  }

  const isRTL = currentLanguage === 'ar'

  return (
    <LanguageContext.Provider value={{ currentLanguage, changeLanguage, t, isRTL }}>
      {children}
    </LanguageContext.Provider>
  )
}

export const useLanguage = () => {
  const context = useContext(LanguageContext)
  if (!context) {
    throw new Error('useLanguage must be used within LanguageProvider')
  }
  return context
}

