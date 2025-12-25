import { useState, useEffect } from 'react'
import { DocumentSettings, SaveDocumentSettings, DocumentSeries, SaveDocumentSeries, Project } from '../../types/documentSettings'
import { ApiService } from '../../services/api'
import './DocumentSettings.css'

interface DocumentSettingsProps {
  formBuilderId: number
}

const DocumentSettingsComponent = ({ formBuilderId }: DocumentSettingsProps) => {
  const [settings, setSettings] = useState<DocumentSettings | null>(null)
  const [projects, setProjects] = useState<Project[]>([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  // Form state
  const [documentName, setDocumentName] = useState('')
  const [documentCode, setDocumentCode] = useState('')
  const [menuCaption, setMenuCaption] = useState('')
  const [menuOrder, setMenuOrder] = useState(0)
  const [parentMenuId, setParentMenuId] = useState<number | undefined>(undefined)
  const [isActive, setIsActive] = useState(true)
  const [seriesList, setSeriesList] = useState<SaveDocumentSeries[]>([])

  useEffect(() => {
    loadSettings()
    loadProjects()
  }, [formBuilderId])

  const loadSettings = async () => {
    try {
      setLoading(true)
      setError(null)
      
      const settingsData = await ApiService.getDocumentSettings(formBuilderId)
      
      if (!settingsData) {
        // No settings exist yet, initialize empty form
        setSettings(null)
        setDocumentName('')
        setDocumentCode('')
        setMenuCaption('')
        setMenuOrder(0)
        setParentMenuId(undefined)
        setIsActive(true)
        setSeriesList([])
        return
      }

      setSettings(settingsData)
      setDocumentName(settingsData.documentName || '')
      setDocumentCode(settingsData.documentCode || '')
      setMenuCaption(settingsData.menuCaption || '')
      setMenuOrder(settingsData.menuOrder || 0)
      setParentMenuId(settingsData.parentMenuId)
      setIsActive(settingsData.isActive ?? true)
      setSeriesList(settingsData.documentSeries.map(s => ({
        id: s.id,
        projectId: s.projectId,
        seriesCode: s.seriesCode,
        nextNumber: s.nextNumber,
        isDefault: s.isDefault,
        isActive: s.isActive
      })))
    } catch (err) {
      console.error('Error loading document settings:', err)
      setError(err instanceof Error ? err.message : 'Failed to load document settings')
    } finally {
      setLoading(false)
    }
  }

  const loadProjects = async () => {
    try {
      const projectsData = await ApiService.getActiveProjects()
      setProjects(projectsData as Project[])
    } catch (err) {
      console.error('Error loading projects:', err)
    }
  }

  const handleAddSeries = () => {
    const newSeries: SaveDocumentSeries = {
      projectId: projects.length > 0 ? projects[0].id : 0,
      seriesCode: '',
      nextNumber: 1,
      isDefault: false,
      isActive: true
    }
    setSeriesList([...seriesList, newSeries])
  }

  const handleUpdateSeries = (index: number, field: keyof SaveDocumentSeries, value: any) => {
    const updated = [...seriesList]
    updated[index] = { ...updated[index], [field]: value }
    
    // If setting as default, unset other defaults for the same project
    if (field === 'isDefault' && value === true) {
      updated.forEach((s, i) => {
        if (i !== index && s.projectId === updated[index].projectId) {
          s.isDefault = false
        }
      })
    }
    
    setSeriesList(updated)
  }

  const handleRemoveSeries = (index: number) => {
    if (confirm('Are you sure you want to remove this series?')) {
      const updated = seriesList.filter((_, i) => i !== index)
      setSeriesList(updated)
    }
  }

  const handleSave = async () => {
    try {
      setSaving(true)
      setError(null)
      setSuccess(null)

      // Validation
      if (!documentName.trim()) {
        setError('Document Name is required')
        return
      }
      if (!documentCode.trim()) {
        setError('Document Code is required')
        return
      }
      if (!menuCaption.trim()) {
        setError('Menu Caption is required')
        return
      }

      const saveData: SaveDocumentSettings = {
        formBuilderId,
        documentName: documentName.trim(),
        documentCode: documentCode.trim(),
        menuCaption: menuCaption.trim(),
        menuOrder,
        parentMenuId,
        isActive,
        documentSeries: seriesList
      }

      await ApiService.saveDocumentSettings(saveData)
      setSuccess('Document settings saved successfully')
      await loadSettings()
    } catch (err) {
      console.error('Error saving document settings:', err)
      setError(err instanceof Error ? err.message : 'Failed to save document settings')
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return <div className="document-settings-loading">Loading document settings...</div>
  }

  return (
    <div className="document-settings">
      <div className="document-settings-header">
        <h2>Document Settings</h2>
        <p className="form-builder-name">Form: {settings?.formBuilderName || 'N/A'}</p>
      </div>

      {error && (
        <div className="document-settings-error">
          {error}
        </div>
      )}

      {success && (
        <div className="document-settings-success">
          {success}
        </div>
      )}

      <div className="document-settings-content">
        {/* Document Type Section */}
        <div className="document-settings-section">
          <h3>Document Type</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>Document Name *</label>
              <input
                type="text"
                value={documentName}
                onChange={(e) => setDocumentName(e.target.value)}
                placeholder="e.g., Lease Contract"
                maxLength={200}
              />
            </div>

            <div className="form-group">
              <label>Document Code *</label>
              <input
                type="text"
                value={documentCode}
                onChange={(e) => setDocumentCode(e.target.value.toUpperCase())}
                placeholder="e.g., LC"
                maxLength={100}
              />
            </div>

            <div className="form-group">
              <label>Menu Caption *</label>
              <input
                type="text"
                value={menuCaption}
                onChange={(e) => setMenuCaption(e.target.value)}
                placeholder="Label shown in user menu"
                maxLength={200}
              />
            </div>

            <div className="form-group">
              <label>Menu Order</label>
              <input
                type="number"
                value={menuOrder}
                onChange={(e) => setMenuOrder(parseInt(e.target.value) || 0)}
                min={0}
              />
            </div>

            <div className="form-group">
              <label>Parent Menu ID (Optional)</label>
              <input
                type="number"
                value={parentMenuId || ''}
                onChange={(e) => setParentMenuId(e.target.value ? parseInt(e.target.value) : undefined)}
                placeholder="Leave empty for root menu"
              />
            </div>

            <div className="form-group checkbox-group">
              <label>
                <input
                  type="checkbox"
                  checked={isActive}
                  onChange={(e) => setIsActive(e.target.checked)}
                />
                Is Active
              </label>
            </div>
          </div>
        </div>

        {/* Document Series Section */}
        <div className="document-settings-section">
          <div className="section-header">
            <h3>Document Series</h3>
            <button type="button" onClick={handleAddSeries} className="btn-add-series">
              + Add Series
            </button>
          </div>

          {seriesList.length === 0 ? (
            <p className="empty-message">No document series configured. Click "Add Series" to add one.</p>
          ) : (
            <div className="series-list">
              {seriesList.map((series, index) => (
                <div key={index} className="series-item">
                  <div className="series-form-grid">
                    <div className="form-group">
                      <label>Project *</label>
                      <select
                        value={series.projectId}
                        onChange={(e) => handleUpdateSeries(index, 'projectId', parseInt(e.target.value))}
                      >
                        <option value={0}>Select Project</option>
                        {projects.map(project => (
                          <option key={project.id} value={project.id}>
                            {project.name} ({project.code})
                          </option>
                        ))}
                      </select>
                    </div>

                    <div className="form-group">
                      <label>Series Code / Prefix *</label>
                      <input
                        type="text"
                        value={series.seriesCode}
                        onChange={(e) => handleUpdateSeries(index, 'seriesCode', e.target.value)}
                        placeholder="e.g., LC-AND1-2025"
                        maxLength={50}
                      />
                    </div>

                    <div className="form-group">
                      <label>Next Number</label>
                      <input
                        type="number"
                        value={series.nextNumber}
                        onChange={(e) => handleUpdateSeries(index, 'nextNumber', parseInt(e.target.value) || 1)}
                        min={1}
                      />
                    </div>

                    <div className="form-group checkbox-group">
                      <label>
                        <input
                          type="checkbox"
                          checked={series.isDefault}
                          onChange={(e) => handleUpdateSeries(index, 'isDefault', e.target.checked)}
                        />
                        Is Default
                      </label>
                    </div>

                    <div className="form-group checkbox-group">
                      <label>
                        <input
                          type="checkbox"
                          checked={series.isActive}
                          onChange={(e) => handleUpdateSeries(index, 'isActive', e.target.checked)}
                        />
                        Is Active
                      </label>
                    </div>

                    <div className="form-group">
                      <button
                        type="button"
                        onClick={() => handleRemoveSeries(index)}
                        className="btn-remove"
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Save Button */}
        <div className="document-settings-actions">
          <button
            type="button"
            onClick={handleSave}
            disabled={saving}
            className="btn-save"
          >
            {saving ? 'Saving...' : 'Save Document Settings'}
          </button>
        </div>
      </div>
    </div>
  )
}

export default DocumentSettingsComponent

