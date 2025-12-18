import { FormTab } from '../types/form'
import { useLanguage } from '../contexts/LanguageContext'
import './TabNavigation.css'

interface TabNavigationProps {
  tabs: FormTab[]
  activeIndex: number
  onTabChange: (index: number) => void
}

const TabNavigation = ({ tabs, activeIndex, onTabChange }: TabNavigationProps) => {
  const { currentLanguage } = useLanguage()
  
  const getTabName = (tab: FormTab) => {
    // Support both naming patterns: name_ar/name_en and foreignTabName/TabName
    if (currentLanguage === 'ar') {
      return tab.name_ar || tab.foreignTabName || tab.tabName
    }
    return tab.name_en || tab.tabName
  }

  return (
    <div className="tab-navigation">
      <div className="tab-list">
        {tabs.map((tab, index) => (
          <button
            key={tab.id}
            className={`tab-button ${index === activeIndex ? 'active' : ''}`}
            onClick={() => onTabChange(index)}
            type="button"
          >
            {getTabName(tab)}
          </button>
        ))}
      </div>
    </div>
  )
}

export default TabNavigation












