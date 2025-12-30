import { useState, useEffect } from 'react'
import { AttachmentType, CreateAttachmentTypeDto, UpdateAttachmentTypeDto } from '../../types/attachmentType'
import { ApiService } from '../../services/api'
import './AttachmentTypes.css'

const AttachmentTypesComponent = () => {
  const [attachmentTypes, setAttachmentTypes] = useState<AttachmentType[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  
  // Modal states
  const [showModal, setShowModal] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  
  // Form states
  const [name, setName] = useState('')
  const [code, setCode] = useState('')
  const [description, setDescription] = useState('')
  const [maxSizeMB, setMaxSizeMB] = useState(10)
  const [isActive, setIsActive] = useState(true)

  useEffect(() => {
    loadAttachmentTypes()
  }, [])

  const loadAttachmentTypes = async () => {
    try {
      setLoading(true)
      setError(null)
      const data = await ApiService.getAllAttachmentTypes()
      setAttachmentTypes(Array.isArray(data) ? data : [])
    } catch (err) {
      console.error('Error loading attachment types:', err)
      setError(err instanceof Error ? err.message : 'Failed to load attachment types')
    } finally {
      setLoading(false)
    }
  }

  const handleNew = () => {
    setEditingId(null)
    setName('')
    setCode('')
    setDescription('')
    setMaxSizeMB(10)
    setIsActive(true)
    setShowModal(true)
    setError(null)
    setSuccess(null)
  }

  const handleEdit = (attachmentType: AttachmentType) => {
    setEditingId(attachmentType.id)
    setName(attachmentType.name)
    setCode(attachmentType.code)
    setDescription(attachmentType.description || '')
    setMaxSizeMB(attachmentType.maxSizeMB)
    setIsActive(attachmentType.isActive)
    setShowModal(true)
    setError(null)
    setSuccess(null)
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this attachment type?\n\nNote: If this attachment type is used in forms, the deletion may fail.')) {
      return
    }

    try {
      setError(null)
      setSuccess(null)
      await ApiService.deleteAttachmentType(id)
      setSuccess('Attachment type deleted successfully')
      await loadAttachmentTypes()
    } catch (err) {
      console.error('Error deleting attachment type:', err)
      const errorMessage = err instanceof Error ? err.message : 'Failed to delete attachment type'
      setError(errorMessage)
      setSuccess(null)
    }
  }

  const handleToggleActive = async (id: number, currentStatus: boolean) => {
    try {
      setError(null)
      setSuccess(null)
      await ApiService.toggleAttachmentTypeActive(id, !currentStatus)
      setSuccess(`Attachment type ${!currentStatus ? 'activated' : 'deactivated'} successfully`)
      await loadAttachmentTypes()
    } catch (err) {
      console.error('Error toggling attachment type status:', err)
      const errorMessage = err instanceof Error ? err.message : 'Failed to toggle attachment type status'
      setError(errorMessage)
      setSuccess(null)
    }
  }

  const handleSave = async () => {
    try {
      setError(null)
      setSuccess(null)

      if (!name.trim() || !code.trim()) {
        setError('Name and Code are required')
        return
      }

      // Validate code format (should be uppercase, no spaces, alphanumeric)
      const codeRegex = /^[A-Z0-9_]+$/
      if (!codeRegex.test(code.trim().toUpperCase())) {
        setError('Code must contain only uppercase letters, numbers, and underscores')
        return
      }

      // Validate maxSizeMB
      if (maxSizeMB < 1 || maxSizeMB > 1000) {
        setError('Max Size must be between 1 and 1000 MB')
        return
      }

      if (editingId) {
        // Update
        const updateDto: UpdateAttachmentTypeDto = {
          name: name.trim(),
          code: code.trim().toUpperCase(),
          description: description.trim() || undefined,
          maxSizeMB,
          isActive
        }
        await ApiService.updateAttachmentType(editingId, updateDto)
        setSuccess('Attachment type updated successfully')
      } else {
        // Create
        const createDto: CreateAttachmentTypeDto = {
          name: name.trim(),
          code: code.trim().toUpperCase(),
          description: description.trim() || undefined,
          maxSizeMB,
          isActive
        }
        await ApiService.createAttachmentType(createDto)
        setSuccess('Attachment type created successfully')
      }

      setShowModal(false)
      await loadAttachmentTypes()
    } catch (err) {
      console.error('Error saving attachment type:', err)
      const errorMessage = err instanceof Error ? err.message : 'Failed to save attachment type'
      setError(errorMessage)
    }
  }

  const handleCancel = () => {
    setShowModal(false)
    setEditingId(null)
    setError(null)
    setSuccess(null)
  }

  if (loading) {
    return <div className="attachment-types-loading">Loading attachment types...</div>
  }

  return (
    <div className="attachment-types">
      <div className="attachment-types-header">
        <h2>File Extensions (Attachment Types)</h2>
        <button onClick={handleNew} className="btn-primary">
          + Add New Extension
        </button>
      </div>

      {error && (
        <div className="attachment-types-error">
          {error}
        </div>
      )}

      {success && (
        <div className="attachment-types-success">
          {success}
        </div>
      )}

      {attachmentTypes.length === 0 ? (
        <div className="empty-state">
          <p>No attachment types found. Add your first file extension to get started.</p>
        </div>
      ) : (
        <div className="attachment-types-table">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Code</th>
                <th>Description</th>
                <th>Max Size (MB)</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {attachmentTypes.map(attachmentType => (
                <tr key={attachmentType.id} className={!attachmentType.isActive ? 'inactive' : ''}>
                  <td>{attachmentType.id}</td>
                  <td>{attachmentType.name}</td>
                  <td>
                    <code className="code-badge">{attachmentType.code}</code>
                  </td>
                  <td>{attachmentType.description || '-'}</td>
                  <td>{attachmentType.maxSizeMB} MB</td>
                  <td>
                    <span className={`status-badge ${attachmentType.isActive ? 'active' : 'inactive'}`}>
                      {attachmentType.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td>
                    <div className="action-buttons">
                      <button
                        onClick={() => handleEdit(attachmentType)}
                        className="btn-edit"
                        title="Edit"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleToggleActive(attachmentType.id, attachmentType.isActive)}
                        className={attachmentType.isActive ? 'btn-deactivate' : 'btn-activate'}
                        title={attachmentType.isActive ? 'Deactivate' : 'Activate'}
                      >
                        {attachmentType.isActive ? 'Deactivate' : 'Activate'}
                      </button>
                      <button
                        onClick={() => handleDelete(attachmentType.id)}
                        className="btn-delete"
                        title="Delete"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Modal */}
      {showModal && (
        <div className="modal-overlay" onClick={handleCancel}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h3>{editingId ? 'Edit File Extension' : 'Add New File Extension'}</h3>

            {error && (
              <div className="modal-error">
                {error}
              </div>
            )}

            <div className="modal-form">
              <div className="form-group">
                <label>Name *</label>
                <input
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="e.g., PDF Document, Image File, Excel Spreadsheet"
                  maxLength={200}
                />
                <small>Display name for this file extension type</small>
              </div>

              <div className="form-group">
                <label>Code *</label>
                <input
                  type="text"
                  value={code}
                  onChange={(e) => setCode(e.target.value.toUpperCase())}
                  placeholder="e.g., PDF, JPG, XLSX"
                  maxLength={100}
                />
                <small>Extension code (uppercase letters, numbers, and underscores only)</small>
              </div>

              <div className="form-group">
                <label>Description (Optional)</label>
                <textarea
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  placeholder="Brief description of this file extension type"
                  rows={3}
                  maxLength={500}
                />
              </div>

              <div className="form-group">
                <label>Max File Size (MB) *</label>
                <input
                  type="number"
                  value={maxSizeMB}
                  onChange={(e) => {
                    const value = parseInt(e.target.value)
                    if (!isNaN(value) && value > 0) {
                      setMaxSizeMB(value)
                    } else if (e.target.value === '') {
                      setMaxSizeMB(1)
                    }
                  }}
                  min="1"
                  max="1000"
                  step="1"
                />
                <small>Maximum file size allowed for this extension type (1-1000 MB)</small>
              </div>

              <div className="form-group">
                <label>
                  <input
                    type="checkbox"
                    checked={isActive}
                    onChange={(e) => setIsActive(e.target.checked)}
                  />
                  Active
                </label>
                <small>Only active extensions will be available for selection in forms</small>
              </div>
            </div>

            <div className="modal-actions">
              <button onClick={handleCancel} className="btn-secondary">
                Cancel
              </button>
              <button onClick={handleSave} className="btn-primary">
                {editingId ? 'Update Extension' : 'Add Extension'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

export default AttachmentTypesComponent




