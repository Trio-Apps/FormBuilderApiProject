import { useState } from 'react'
import './FieldDataSourceConfig.css'
import { FieldDataSource } from '../../types/form'

interface FieldDataSourceConfigProps {
  fieldId: number
  fieldName: string
  initialDataSource?: FieldDataSource
  onSave: (dataSource: FieldDataSource) => Promise<void>
  onCancel: () => void
}

const FieldDataSourceConfig = ({
  fieldId,
  fieldName,
  initialDataSource,
  onSave,
  onCancel
}: FieldDataSourceConfigProps) => {
  const [sourceType, setSourceType] = useState(initialDataSource?.sourceType || 'Static')
  const [apiUrl, setApiUrl] = useState(initialDataSource?.apiUrl || '')
  const [httpMethod, setHttpMethod] = useState(initialDataSource?.httpMethod || 'GET')
  const [requestBodyJson, setRequestBodyJson] = useState(initialDataSource?.requestBodyJson || '')
  const [valuePath, setValuePath] = useState(initialDataSource?.valuePath || '')
  const [textPath, setTextPath] = useState(initialDataSource?.textPath || '')
  const [isActive, setIsActive] = useState(initialDataSource?.isActive ?? true)
  const [loading, setLoading] = useState(false)
  const [previewData, setPreviewData] = useState<any[]>([])
  const [previewError, setPreviewError] = useState<string | null>(null)

  const handlePreview = async () => {
    if (sourceType === 'Static') {
      setPreviewError('Static sources use FIELD_OPTIONS table')
      return
    }

    if (!apiUrl) {
      setPreviewError('API URL is required')
      return
    }

    setLoading(true)
    setPreviewError(null)

    try {
      // Call backend preview endpoint
      const response = await fetch(`/api/FieldDataSources/preview`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          fieldId,
          sourceType,
          apiUrl,
          httpMethod,
          requestBodyJson: requestBodyJson || undefined,
          valuePath,
          textPath
        })
      })

      if (!response.ok) {
        throw new Error('Failed to preview data source')
      }

      const data = await response.json()
      setPreviewData(data.options || [])
    } catch (error) {
      setPreviewError(error instanceof Error ? error.message : 'Preview failed')
    } finally {
      setLoading(false)
    }
  }

  const handleSave = async () => {
    if (sourceType === 'Static') {
      // Static doesn't need data source configuration
      await onSave({
        id: initialDataSource?.id,
        fieldId,
        sourceType: 'Static',
        isActive: true
      })
      return
    }

    if (!apiUrl) {
      alert(sourceType === 'LookupTable' 
        ? 'Please select a table name' 
        : 'API URL is required')
      return
    }

    if (sourceType === 'LookupTable') {
      if (!valuePath || !textPath) {
        alert('Value Column and Text Column are required for LookupTable sources')
        return
      }
    }

    if (sourceType === 'Api') {
      if (!valuePath || !textPath) {
        alert('Value Path and Text Path are required for API sources')
        return
      }
    }

    setLoading(true)
    try {
      const dataSource = {
        id: initialDataSource?.id,
        fieldId,
        sourceType,
        // For LookupTable: apiUrl stores table name, valuePath/textPath store column names
        // For Api: apiUrl stores API URL, valuePath/textPath store JSON paths
        apiUrl: sourceType !== 'Static' ? apiUrl : null,
        httpMethod: sourceType === 'Api' ? httpMethod : null,
        requestBodyJson: sourceType === 'Api' && requestBodyJson ? requestBodyJson : null,
        valuePath: (sourceType === 'Api' || sourceType === 'LookupTable') ? valuePath : null,
        textPath: (sourceType === 'Api' || sourceType === 'LookupTable') ? textPath : null,
        isActive
      }

      await onSave(dataSource)
    } catch (error) {
      alert(error instanceof Error ? error.message : 'Failed to save data source')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="field-data-source-config">
      <div className="config-header">
        <h3>Data Source Configuration</h3>
        <p className="field-name">Field: {fieldName}</p>
      </div>

      <div className="config-section">
        <label>Source Type</label>
        <div className="radio-group">
          <label>
            <input
              type="radio"
              value="Static"
              checked={sourceType === 'Static'}
              onChange={(e) => setSourceType(e.target.value)}
            />
            Static (FIELD_OPTIONS)
          </label>
          <label>
            <input
              type="radio"
              value="Api"
              checked={sourceType === 'Api'}
              onChange={(e) => setSourceType(e.target.value)}
            />
            API
          </label>
          <label>
            <input
              type="radio"
              value="LookupTable"
              checked={sourceType === 'LookupTable'}
              onChange={(e) => setSourceType(e.target.value)}
            />
            Lookup Table
          </label>
          <label>
            <input
              type="radio"
              value="Custom"
              checked={sourceType === 'Custom'}
              onChange={(e) => setSourceType(e.target.value)}
            />
            Custom
          </label>
        </div>
      </div>

      {sourceType === 'Api' && (
        <>
          <div className="config-section">
            <label>External API URL *</label>
            <input
              type="url"
              value={apiUrl}
              onChange={(e) => setApiUrl(e.target.value)}
              placeholder="https://api.example.com/customers"
              className="full-width"
            />
            <small>Full URL of the external API endpoint</small>
          </div>

          <div className="config-section">
            <label>HTTP Method</label>
            <select
              value={httpMethod}
              onChange={(e) => setHttpMethod(e.target.value)}
            >
              <option value="GET">GET</option>
              <option value="POST">POST</option>
            </select>
          </div>

          {httpMethod === 'POST' && (
            <div className="config-section">
              <label>Request Body (JSON) - Optional</label>
              <textarea
                value={requestBodyJson}
                onChange={(e) => setRequestBodyJson(e.target.value)}
                placeholder='{"filter": "active", "status": "enabled"}'
                rows={4}
                className="full-width"
              />
              <small>JSON body for POST requests (optional)</small>
            </div>
          )}

          <div className="config-section">
            <label>Value Path (ID) *</label>
            <input
              type="text"
              value={valuePath}
              onChange={(e) => setValuePath(e.target.value)}
              placeholder="id"
              className="full-width"
            />
            <small>JSON path to extract the value from API response (e.g., "id" or "data.id")</small>
            <div className="example-box">
              <strong>Example Response:</strong>
              <pre>{`[
  { "id": 1, "name": "ABC Corp" },
  { "id": 2, "name": "XYZ Ltd" }
]`}</pre>
              <p>Value Path: <code>id</code></p>
            </div>
          </div>

          <div className="config-section">
            <label>Text Path (Display Name) *</label>
            <input
              type="text"
              value={textPath}
              onChange={(e) => setTextPath(e.target.value)}
              placeholder="name"
              className="full-width"
            />
            <small>JSON path to extract the display text from API response (e.g., "name" or "data.name")</small>
            <div className="example-box">
              <strong>Example Response:</strong>
              <pre>{`[
  { "id": 1, "name": "ABC Corp" },
  { "id": 2, "name": "XYZ Ltd" }
]`}</pre>
              <p>Text Path: <code>name</code></p>
            </div>
          </div>
        </>
      )}

      {sourceType === 'LookupTable' && (
        <>
          <div className="config-section">
            <label>Table Name (from AkhmanageItContext) *</label>
            <input
              type="text"
              value={apiUrl}
              onChange={(e) => setApiUrl(e.target.value)}
              placeholder="TblCustomer"
              className="full-width"
              list="lookup-tables-list"
            />
            <datalist id="lookup-tables-list">
              {/* Common tables - will be populated dynamically from API */}
              <option value="TblCustomer" />
              <option value="TblItem" />
              <option value="TblLegalEntity" />
              <option value="TblLookup" />
              <option value="TblLookupType" />
              <option value="TblCountry" />
              <option value="TblCity" />
              <option value="TblEmployee" />
              <option value="TblItemGroup" />
              <option value="TblJobTitle" />
              <option value="TblBank" />
              <option value="TblCostCenter" />
              <option value="TblGlaccount" />
            </datalist>
            <small>Enter table name from AkhmanageItContext (e.g., TblCustomer, TblItem). All tables are supported.</small>
          </div>

          <div className="config-section">
            <label>Value Column (ID) *</label>
            <input
              type="text"
              value={valuePath}
              onChange={(e) => setValuePath(e.target.value)}
              placeholder="Id"
              className="full-width"
            />
            <small>Column name for the value (usually "Id")</small>
          </div>

          <div className="config-section">
            <label>Text Column (Display Name) *</label>
            <input
              type="text"
              value={textPath}
              onChange={(e) => setTextPath(e.target.value)}
              placeholder="Name"
              className="full-width"
            />
            <small>Column name for the display text (usually "Name")</small>
          </div>
        </>
      )}

      {sourceType === 'Custom' && (
        <div className="config-section">
          <label>Custom Resolver Name</label>
          <input
            type="text"
            value={apiUrl}
            onChange={(e) => setApiUrl(e.target.value)}
            placeholder="CustomerResolver"
            className="full-width"
          />
          <small>Name of the custom resolver in backend</small>
        </div>
      )}

      <div className="config-section">
        <label>
          <input
            type="checkbox"
            checked={isActive}
            onChange={(e) => setIsActive(e.target.checked)}
          />
          Is Active
        </label>
      </div>

      {sourceType !== 'Static' && (
        <div className="config-section">
          <button
            type="button"
            onClick={handlePreview}
            disabled={loading}
            className="btn-preview"
          >
            {loading ? 'Testing...' : 'Preview'}
          </button>

          {previewError && (
            <div className="preview-error">{previewError}</div>
          )}

          {previewData.length > 0 && (
            <div className="preview-results">
              <h4>Preview Results:</h4>
              <ul>
                {previewData.slice(0, 5).map((item, index) => (
                  <li key={index}>
                    <strong>{item.value}</strong>: {item.text}
                  </li>
                ))}
                {previewData.length > 5 && (
                  <li>... and {previewData.length - 5} more</li>
                )}
              </ul>
            </div>
          )}
        </div>
      )}

      <div className="config-actions">
        <button
          type="button"
          onClick={handleSave}
          disabled={loading}
          className="btn-primary"
        >
          {loading ? 'Saving...' : 'Save'}
        </button>
        <button
          type="button"
          onClick={onCancel}
          className="btn-secondary"
        >
          Cancel
        </button>
      </div>
    </div>
  )
}

export default FieldDataSourceConfig

