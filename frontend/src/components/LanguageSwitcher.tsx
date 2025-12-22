import React from 'react'
import { useLanguage } from '../contexts/LanguageContext'
import './LanguageSwitcher.css'

const LanguageSwitcher: React.FC = () => {
  const { currentLanguage, changeLanguage } = useLanguage()

  return (
    <div className="language-switcher">
      <button
        className={`lang-btn ${currentLanguage === 'en' ? 'active' : ''}`}
        onClick={() => changeLanguage('en')}
        type="button"
      >
        English
      </button>
      <button
        className={`lang-btn ${currentLanguage === 'ar' ? 'active' : ''}`}
        onClick={() => changeLanguage('ar')}
        type="button"
      >
        العربية
      </button>
    </div>
  )
}

export default LanguageSwitcher













