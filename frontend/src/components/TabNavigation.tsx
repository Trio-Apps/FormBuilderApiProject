import { FormTab } from '../types/form'
import './TabNavigation.css'

interface TabNavigationProps {
  tabs: FormTab[]
  activeIndex: number
  onTabChange: (index: number) => void
}

const TabNavigation = ({ tabs, activeIndex, onTabChange }: TabNavigationProps) => {
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
            {tab.tabName}
          </button>
        ))}
      </div>
    </div>
  )
}

export default TabNavigation












