import { useState, useEffect } from 'react'
import { DocumentType, CreateDocumentTypeDto, UpdateDocumentTypeDto } from '../../types/documentType'
import { ApiService } from '../../services/api'
import './DocumentTypes.css'

const DocumentTypesComponent = () => {
  const [documentTypes, setDocumentTypes] = useState<DocumentType[]>([])
  const [parentMenuOptions, setParentMenuOptions] = useState<DocumentType[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  
  // Modal states
  const [showModal, setShowModal] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  
  // Form states
  const [name, setName] = useState('')
  const [code, setCode] = useState('')
  const [formBuilderId, setFormBuilderId] = useState<number | undefined>(undefined)
  const [menuCaption, setMenuCaption] = useState('')
  const [menuOrder, setMenuOrder] = useState(0)
  const [parentMenuId, setParentMenuId] = useState<number | undefined>(undefined)
  const [isActive, setIsActive] = useState(true)

  useEffect(() => {
    loadDocumentTypes()
    loadParentMenuOptions()
  }, [])

  const loadDocumentTypes = async () => {
    try {
      setLoading(true)
      setError(null)
      const data = await ApiService.getAllDocumentTypes()
      setDocumentTypes(Array.isArray(data) ? data : [])
    } catch (err) {
      console.error('Error loading document types:', err)
      setError(err instanceof Error ? err.message : 'Failed to load document types')
    } finally {
      setLoading(false)
    }
  }

  const loadParentMenuOptions = async () => {
    try {
      const data = await ApiService.getActiveDocumentTypes()
      setParentMenuOptions(Array.isArray(data) ? data : [])
    } catch (err) {
      console.error('Error loading parent menu options:', err)
    }
  }

  const handleNew = () => {
    setEditingId(null)
    setName('')
    setCode('')
    setFormBuilderId(undefined)
    setMenuCaption('')
    setMenuOrder(0)
    setParentMenuId(undefined)
    setIsActive(true)
    setShowModal(true)
    setError(null)
    setSuccess(null)
    loadParentMenuOptions() // Reload options when opening modal
  }

  const handleEdit = (docType: DocumentType) => {
    setEditingId(docType.id)
    setName(docType.name)
    setCode(docType.code)
    setFormBuilderId(docType.formBuilderId)
    setMenuCaption(docType.menuCaption)
    setMenuOrder(docType.menuOrder)
    setParentMenuId(docType.parentMenuId)
    setIsActive(docType.isActive)
    setShowModal(true)
    setError(null)
    setSuccess(null)
    loadParentMenuOptions() // Reload options when opening modal
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this document type?\n\nNote: If this document type has children or related submissions, the deletion will fail.')) {
      return
    }

    try {
      setError(null)
      setSuccess(null)
      await ApiService.deleteDocumentType(id)
      setSuccess('Document type deleted successfully')
      await loadDocumentTypes()
    } catch (err) {
      console.error('Error deleting document type:', err)
      const errorMessage = err instanceof Error ? err.message : 'Failed to delete document type'
      setError(errorMessage)
      setSuccess(null)
    }
  }

  const handleSave = async () => {
    try {
      setError(null)
      setSuccess(null)

      if (!name.trim() || !code.trim() || !menuCaption.trim()) {
        setError('Name, Code, and Menu Caption are required')
        return
      }

      if (editingId) {
        // Update
        const updateDto: UpdateDocumentTypeDto = {
          name: name.trim(),
          code: code.trim(),
          formBuilderId: formBuilderId && formBuilderId > 0 ? formBuilderId : undefined,
          menuCaption: menuCaption.trim(),
          menuOrder,
          parentMenuId: parentMenuId !== undefined && parentMenuId !== null ? (parentMenuId > 0 ? parentMenuId : 0) : undefined,
          isActive
        }
        await ApiService.updateDocumentType(editingId, updateDto)
        setSuccess('Document type updated successfully')
      } else {
        // Create - only include optional IDs if they have valid values
        const createDto: CreateDocumentTypeDto = {
          name: name.trim(),
          code: code.trim(),
          menuCaption: menuCaption.trim(),
          menuOrder,
          isActive
        }
        
        // Only add formBuilderId if it's a valid number > 0
        if (formBuilderId && formBuilderId > 0) {
          createDto.formBuilderId = formBuilderId
        }
        
        // Include parentMenuId if it's set (including 0, which backend will normalize to null)
        if (parentMenuId !== undefined && parentMenuId !== null) {
          createDto.parentMenuId = parentMenuId > 0 ? parentMenuId : 0
        }
        
        await ApiService.createDocumentType(createDto)
        setSuccess('Document type created successfully')
      }

      setShowModal(false)
      await loadDocumentTypes()
    } catch (err) {
      console.error('Error saving document type:', err)
      setError(err instanceof Error ? err.message : 'Failed to save document type')
    }
  }

  const handleCancel = () => {
    setShowModal(false)
    setEditingId(null)
    setError(null)
    setSuccess(null)
  }

  if (loading) {
    return <div className="document-types-loading">Loading document types...</div>
  }

  return (
    <div className="document-types">
      <div className="document-types-header">
        <h2>Document Types</h2>
        <button onClick={handleNew} className="btn-primary">
          + Create New Document Type
        </button>
      </div>

      {error && (
        <div className="document-types-error">
          {error}
        </div>
      )}

      {success && (
        <div className="document-types-success">
          {success}
        </div>
      )}

      {documentTypes.length === 0 ? (
        <div className="empty-state">
          <p>No document types found. Create your first document type to get started.</p>
        </div>
      ) : (
        <div className="document-types-table">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Code</th>
                <th>Menu Caption</th>
                <th>Menu Order</th>
                <th>Form Builder</th>
                <th>Active</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {documentTypes.map(docType => (
                <tr key={docType.id} className={!docType.isActive ? 'inactive' : ''}>
                  <td>{docType.id}</td>
                  <td>{docType.name}</td>
                  <td>{docType.code}</td>
                  <td>{docType.menuCaption}</td>
                  <td>{docType.menuOrder}</td>
                  <td>{docType.formBuilderName || '-'}</td>
                  <td>
                    <span className={`status-badge ${docType.isActive ? 'active' : 'inactive'}`}>
                      {docType.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td>
                    <div className="action-buttons">
                      <button
                        onClick={() => handleEdit(docType)}
                        className="btn-edit"
                        title="Edit"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDelete(docType.id)}
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
            <h3>{editingId ? 'Edit Document Type' : 'Create New Document Type'}</h3>

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
                  placeholder="e.g., Lease Contract"
                  maxLength={200}
                />
              </div>

              <div className="form-group">
                <label>Code *</label>
                <input
                  type="text"
                  value={code}
                  onChange={(e) => setCode(e.target.value.toUpperCase())}
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
                  placeholder="Label shown in menu"
                  maxLength={200}
                />
              </div>

              <div className="form-group">
                <label>Menu Order</label>
                <input
                  type="number"
                  value={menuOrder}
                  onChange={(e) => setMenuOrder(parseInt(e.target.value) || 0)}
                  min="0"
                />
              </div>

              <div className="form-group">
                <label>Form Builder ID (Optional)</label>
                <input
                  type="number"
                  value={formBuilderId ?? ''}
                  onChange={(e) => {
                    const value = e.target.value.trim()
                    setFormBuilderId(value === '' ? undefined : parseInt(value))
                  }}
                  placeholder="Leave empty if not applicable"
                  min="1"
                />
              </div>

              <div className="form-group">
                <label>Parent Menu (Optional)</label>
                <select
                  value={parentMenuId ?? ''}
                  onChange={(e) => {
                    const value = e.target.value
                    if (value === '' || value === '0') {
                      setParentMenuId(undefined)
                    } else {
                      const numValue = parseInt(value)
                      setParentMenuId(isNaN(numValue) ? undefined : numValue)
                    }
                  }}
                  style={{
                    width: '100%',
                    padding: '8px',
                    border: '1px solid #ddd',
                    borderRadius: '4px',
                    fontSize: '14px'
                  }}
                >
                  <option value="">-- No Parent (Root Menu) --</option>
                  {parentMenuOptions
                    .filter(dt => !editingId || dt.id !== editingId) // Exclude current document type when editing
                    .map(dt => (
                      <option key={dt.id} value={dt.id}>
                        {dt.name} {dt.menuCaption ? `(${dt.menuCaption})` : ''}
                      </option>
                    ))}
                </select>
                <small style={{color: '#666', fontSize: '12px'}}>
                  Select a parent menu item, or leave as "No Parent" for root menu.
                </small>
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
              </div>
            </div>

            <div className="modal-actions">
              <button onClick={handleCancel} className="btn-secondary">
                Cancel
              </button>
              <button onClick={handleSave} className="btn-primary">
                {editingId ? 'Update' : 'Create'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

export default DocumentTypesComponent


