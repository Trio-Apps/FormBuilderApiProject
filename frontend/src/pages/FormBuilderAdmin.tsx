import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { FormBuilder } from '../types/form'
import DocumentSettingsComponent from '../components/admin/DocumentSettings'
import './FormBuilderAdmin.css'

const FormBuilderAdmin = () => {
  const { id } = useParams<{ id: string }>()
  const formBuilderId = id ? parseInt(id) : 0
  const [formBuilder, setFormBuilder] = useState<FormBuilder | null>(null)
  const [loading, setLoading] = useState(true)
  const [activeTab, setActiveTab] = useState<'settings' | 'document-settings'>('settings')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (formBuilderId > 0) {
      loadFormBuilder()
    } else {
      setError('Form Builder ID is required')
      setLoading(false)
    }
  }, [formBuilderId])

  const loadFormBuilder = async () => {
    try {
      setLoading(true)
      setError(null)
      const token = localStorage.getItem('token') || localStorage.getItem('accessToken') || ''
      const response = await fetch(`/api/FormBuilder/${formBuilderId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      })

      if (!response.ok) {
        throw new Error('Failed to load form builder')
      }

      const data = await response.json()
      const formData: FormBuilder = data.success !== undefined ? data.data : data
      setFormBuilder(formData)
    } catch (err) {
      console.error('Error loading form builder:', err)
      setError(err instanceof Error ? err.message : 'Failed to load form builder')
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="form-builder-admin">
        <div className="loading">Loading Form Builder...</div>
      </div>
    )
  }

  if (error || !formBuilder) {
    return (
      <div className="form-builder-admin">
        <div className="error">
          {error || 'Form Builder not found'}
        </div>
      </div>
    )
  }

  return (
    <div className="form-builder-admin">
      <div className="form-builder-header">
        <h1>Form Builder: {formBuilder.formName}</h1>
        <p className="form-code">Code: {formBuilder.formCode}</p>
      </div>

      {/* Tab Navigation */}
      <div className="admin-tabs">
        <button
          className={`tab-button ${activeTab === 'settings' ? 'active' : ''}`}
          onClick={() => setActiveTab('settings')}
        >
          General Settings
        </button>
        <button
          className={`tab-button ${activeTab === 'document-settings' ? 'active' : ''}`}
          onClick={() => setActiveTab('document-settings')}
        >
          Document Settings
        </button>
      </div>

      {/* Tab Content */}
      <div className="admin-tab-content">
        {activeTab === 'settings' && (
          <div className="settings-tab">
            <h2>General Settings</h2>
            <p>General form settings will be displayed here.</p>
            <div className="form-info">
              <p><strong>Form Name:</strong> {formBuilder.formName}</p>
              <p><strong>Form Code:</strong> {formBuilder.formCode}</p>
              <p><strong>Version:</strong> {formBuilder.version}</p>
              <p><strong>Is Published:</strong> {formBuilder.isPublished ? 'Yes' : 'No'}</p>
              <p><strong>Is Active:</strong> {formBuilder.isActive ? 'Yes' : 'No'}</p>
              {formBuilder.description && (
                <p><strong>Description:</strong> {formBuilder.description}</p>
              )}
            </div>
          </div>
        )}

        {activeTab === 'document-settings' && (
          <DocumentSettingsComponent formBuilderId={formBuilderId} />
        )}
      </div>
    </div>
  )
}

export default FormBuilderAdmin

