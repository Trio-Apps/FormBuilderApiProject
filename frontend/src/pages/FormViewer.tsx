import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { FormBuilder } from '../types/form'
import { ApiService } from '../services/api'
import TabNavigation from '../components/TabNavigation'
import FormFieldRenderer from '../components/FormFieldRenderer'
import LanguageSwitcher from '../components/LanguageSwitcher'
import { useLanguage } from '../contexts/LanguageContext'
import './FormViewer.css'

const FormViewer = () => {
  const { formPublicId } = useParams<{ formPublicId: string }>()
  const { t } = useLanguage()
  const [form, setForm] = useState<FormBuilder | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [activeTabIndex, setActiveTabIndex] = useState(0)

  useEffect(() => {
    const fetchForm = async () => {
      if (!formPublicId) {
        setError('Form ID is required')
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const formData = await ApiService.getFormByCode(formPublicId)
        setForm(formData)
      } catch (err) {
        setError(err instanceof Error ? err.message : t('errors.failedToFetchForm'))
      } finally {
        setLoading(false)
      }
    }

    fetchForm()
  }, [formPublicId, t])

  if (loading) {
    return (
      <div className="form-viewer-container">
        <div className="form-viewer-loading">{t('common.loading')}</div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="form-viewer-container">
        <LanguageSwitcher />
        <div className="form-viewer-error">
          <h2>{t('errors.formNotFound')}</h2>
          <p>{error}</p>
        </div>
      </div>
    )
  }

  if (!form) {
    return (
      <div className="form-viewer-container">
        <LanguageSwitcher />
        <div className="form-viewer-error">
          <h2>{t('errors.formNotFound')}</h2>
          <p>{t('errors.formNotFound')}</p>
        </div>
      </div>
    )
  }

  const { currentLanguage } = useLanguage()
  const activeTabs = form.tabs.filter(tab => tab.isActive).sort((a, b) => a.tabOrder - b.tabOrder)
  const activeTab = activeTabs[activeTabIndex]

  // Get multilingual form name and description
  const getFormName = () => {
    if (currentLanguage === 'ar' && form.foreignFormName) {
      return form.foreignFormName
    }
    return form.formName
  }

  const getFormDescription = () => {
    if (currentLanguage === 'ar' && form.foreignDescription) {
      return form.foreignDescription
    }
    return form.description
  }

  return (
    <div className="form-viewer-container">
      <div className="form-viewer-content">
        {/* Language Switcher */}
        <div style={{ position: 'absolute', top: '20px', right: '20px' }}>
          <LanguageSwitcher />
        </div>

        {/* Form Header */}
        <div className="form-header">
          <h1 className="form-title">{getFormName()}</h1>
          {getFormDescription() && (
            <p className="form-description">{getFormDescription()}</p>
          )}
        </div>

        {/* Tab Navigation */}
        {activeTabs.length > 1 && (
          <TabNavigation
            tabs={activeTabs}
            activeIndex={activeTabIndex}
            onTabChange={setActiveTabIndex}
          />
        )}

        {/* Form Fields */}
        {activeTab && (
          <div className="form-tab-content">
            <div className="form-fields-container">
              {activeTab.fields
                .filter(field => field.isVisible)
                .sort((a, b) => a.fieldOrder - b.fieldOrder)
                .map((field) => (
                  <FormFieldRenderer key={field.id} field={field} />
                ))}
            </div>
            
            {/* Submit Button - Only show on last tab */}
            {activeTabIndex === activeTabs.length - 1 && (
              <div className="form-submit-container">
                <button type="button" className="form-submit-button">
                  {t('common.submit')}
                </button>
              </div>
            )}
          </div>
        )}

        {activeTabs.length === 0 && (
          <div className="form-empty">
            <p>{t('form.noFields')}</p>
          </div>
        )}
      </div>
    </div>
  )
}

export default FormViewer





