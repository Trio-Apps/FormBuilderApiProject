import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { FormBuilder } from '../types/form'
import { ApiService } from '../services/api'
import TabNavigation from '../components/TabNavigation'
import FormFieldRenderer from '../components/FormFieldRenderer'
import './FormViewer.css'

const FormViewer = () => {
  const { formPublicId } = useParams<{ formPublicId: string }>()
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
        setError(err instanceof Error ? err.message : 'Failed to load form')
      } finally {
        setLoading(false)
      }
    }

    fetchForm()
  }, [formPublicId])

  if (loading) {
    return (
      <div className="form-viewer-container">
        <div className="form-viewer-loading">Loading form...</div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="form-viewer-container">
        <div className="form-viewer-error">
          <h2>Form Not Found</h2>
          <p>{error}</p>
        </div>
      </div>
    )
  }

  if (!form) {
    return (
      <div className="form-viewer-container">
        <div className="form-viewer-error">
          <h2>Form Not Found</h2>
          <p>The requested form could not be found.</p>
        </div>
      </div>
    )
  }

  const activeTabs = form.tabs.filter(tab => tab.isActive).sort((a, b) => a.tabOrder - b.tabOrder)
  const activeTab = activeTabs[activeTabIndex]

  return (
    <div className="form-viewer-container">
      <div className="form-viewer-content">
        {/* Form Header */}
        <div className="form-header">
          <h1 className="form-title">{form.formName}</h1>
          {form.description && (
            <p className="form-description">{form.description}</p>
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
                  Submit
                </button>
              </div>
            )}
          </div>
        )}

        {activeTabs.length === 0 && (
          <div className="form-empty">
            <p>This form has no tabs or fields to display.</p>
          </div>
        )}
      </div>
    </div>
  )
}

export default FormViewer





