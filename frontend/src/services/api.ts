import { FormBuilder, ServiceResult, FieldDataSource, FieldOptionResponse, FieldOption } from '../types/form'
import { FormRuleDto, CreateFormRuleDto, UpdateFormRuleDto, FormRule, Condition, Action } from '../types/formRules'

const API_BASE_URL = '/api'

// Get current language from localStorage or default to 'en'
const getCurrentLanguage = (): string => {
  return localStorage.getItem('i18nextLng') || 'en'
}

export class ApiService {
  /**
   * Get default headers with Accept-Language
   */
  static getHeaders(): HeadersInit {
    const language = getCurrentLanguage()
    return {
      'Content-Type': 'application/json',
      'Accept-Language': language,
    }
  }

  /**
   * Fetch form by code (public identifier)
   */
  static async getFormByCode(formCode: string): Promise<FormBuilder> {
    const response = await fetch(`${API_BASE_URL}/FormBuilder/code/${encodeURIComponent(formCode)}`, {
      method: 'GET',
      headers: this.getHeaders(),
    })

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Form not found')
      }
      throw new Error(`Failed to fetch form: ${response.statusText}`)
    }

    const data = await response.json()
    
    // Handle ServiceResult wrapper if present
    if (data.success !== undefined) {
      const result = data as ServiceResult<FormBuilder>
      if (!result.success || !result.data) {
        throw new Error(result.message || 'Failed to fetch form')
      }
      return result.data
    }

    return data as FormBuilder
  }

  /**
   * Get authorization headers (includes token if available)
   */
  static getAuthHeaders(): HeadersInit {
    const headers = this.getHeaders()
    const token = localStorage.getItem('token') || localStorage.getItem('accessToken')
    if (token) {
      return {
        ...headers,
        'Authorization': `Bearer ${token}`
      }
    }
    return headers
  }

  // ================================
  // FIELD DATA SOURCES - Public Endpoints
  // ================================

  /**
   * Get field options (Public - No auth required)
   * GET /api/FieldDataSources/field-options?fieldId={id}&context={json}
   * 
   * Backend returns FieldOptionResponse[] (with 'text' and 'value') for Api/LookupTable,
   * or FieldOptionDto[] (with 'optionText' and 'optionValue') for Static.
   * This method converts FieldOptionResponse[] to FieldOption[] format for consistency.
   */
  static async getFieldOptions(
    fieldId: number,
    context?: Record<string, any>
  ): Promise<FieldOption[]> {
    const contextParam = context
      ? `&context=${encodeURIComponent(JSON.stringify(context))}`
      : ''

    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/field-options?fieldId=${fieldId}${contextParam}`,
      {
        method: 'GET',
        headers: this.getHeaders(),
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch field options: ${response.statusText}`)
    }

    const data = await response.json()
    let options: any[] = []
    
    // Handle ApiResponse wrapper
    if (data.statusCode !== undefined) {
      options = data.data || []
    }
    // Handle ServiceResult wrapper
    else if (data.success !== undefined) {
      options = data.data || []
    }
    else {
      options = data || []
    }

    // Convert FieldOptionResponse[] (with 'text' and 'value') to FieldOption[] format
    // If options have 'text' and 'value' properties, convert them
    if (options.length > 0 && options[0].text !== undefined && options[0].value !== undefined) {
      return options.map((opt: FieldOptionResponse, index: number) => ({
        id: opt.value as number || index + 1, // Use value as id if it's a number, otherwise use index
        fieldId: fieldId,
        optionText: opt.text,
        foreignOptionText: undefined, // Api/LookupTable options don't have foreign text
        optionValue: String(opt.value),
        optionOrder: index + 1,
        isDefault: false,
        isActive: true
      }))
    }

    // If options already have FieldOption format (optionText, optionValue), return as-is
    return options as FieldOption[]
  }

  /**
   * Get field options with POST (Public - No auth required)
   * POST /api/FieldDataSources/field-options
   * 
   * Backend returns FieldOptionResponse[] (with 'text' and 'value') for Api/LookupTable,
   * or FieldOptionDto[] (with 'optionText' and 'optionValue') for Static.
   * This method converts FieldOptionResponse[] to FieldOption[] format for consistency.
   */
  static async getFieldOptionsPost(
    fieldId: number,
    context?: Record<string, any>,
    requestBodyJson?: string
  ): Promise<FieldOption[]> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/field-options`,
      {
        method: 'POST',
        headers: this.getHeaders(),
        body: JSON.stringify({
          fieldId,
          context,
          requestBodyJson
        })
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch field options: ${response.statusText}`)
    }

    const data = await response.json()
    let options: any[] = []
    
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      options = data.data || []
    }
    // Handle ApiResponse wrapper
    else if (data.statusCode !== undefined) {
      options = data.data || []
    }
    else {
      options = data || []
    }

    // Convert FieldOptionResponse[] (with 'text' and 'value') to FieldOption[] format
    // If options have 'text' and 'value' properties, convert them
    if (options.length > 0 && options[0].text !== undefined && options[0].value !== undefined) {
      return options.map((opt: FieldOptionResponse, index: number) => ({
        id: typeof opt.value === 'number' ? opt.value : index + 1, // Use value as id if it's a number, otherwise use index
        fieldId: fieldId,
        optionText: opt.text,
        foreignOptionText: undefined, // Api/LookupTable options don't have foreign text
        optionValue: String(opt.value),
        optionOrder: index + 1,
        isDefault: false,
        isActive: true
      }))
    }

    // If options already have FieldOption format (optionText, optionValue), return as-is
    return options as FieldOption[]
  }

  // ================================
  // FIELD DATA SOURCES - Admin Endpoints
  // ================================

  /**
   * Inspect API structure - Get available fields (Admin)
   * POST /api/FieldDataSources/inspect-api
   */
  static async inspectApi(request: {
    apiUrl: string
    apiPath?: string
    httpMethod?: string
    requestBodyJson?: string
    arrayPropertyNames?: string[]
  }): Promise<{
    fullUrl: string
    success: boolean
    errorMessage?: string
    itemsCount?: number
    availableFields: string[]
    nestedFields: string[]
    sampleItem?: any
    rawResponse?: string
    suggestedValuePaths: string[]
    suggestedTextPaths: string[]
  }> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/inspect-api`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(request)
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to inspect API: ${response.statusText}`)
    }

    const result = await response.json()
    // Handle ApiResponse wrapper
    if (result.data) {
      return result.data
    }
    return result
  }

  /**
   * Preview data source (Admin)
   * POST /api/FieldDataSources/preview
   * valuePath and textPath are OPTIONAL - will be auto-detected if not provided
   */
  static async previewDataSource(
    request: {
      fieldId?: number
      sourceType: string
      apiUrl?: string
      apiPath?: string
      httpMethod?: string
      requestBodyJson?: string
      valuePath?: string
      textPath?: string
      arrayPropertyNames?: string[]
    }
  ): Promise<FieldOptionResponse[]> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/preview`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(request)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(errorData.message || `Failed to preview data source: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ApiResponse wrapper
    if (data.data) {
      return Array.isArray(data.data) ? data.data : []
    }
    return Array.isArray(data) ? data : []
  }

  /**
   * Get available lookup tables (Admin)
   * GET /api/FieldDataSources/lookup-tables
   */
  static async getAvailableLookupTables(): Promise<string[]> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/lookup-tables`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch lookup tables: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
  }

  /**
   * Get all field data sources (Admin)
   * GET /api/FieldDataSources
   */
  static async getAllFieldDataSources(): Promise<FieldDataSource[]> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch data sources: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
  }

  /**
   * Get field data source by ID (Admin)
   * GET /api/FieldDataSources/{id}
   */
  static async getFieldDataSourceById(id: number): Promise<FieldDataSource> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/${id}`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Data source not found')
      }
      throw new Error(`Failed to fetch data source: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      if (!data.data) {
        throw new Error(data.message || 'Data source not found')
      }
      return data.data
    }
    return data
  }

  /**
   * Get field data sources by field ID (Admin)
   * GET /api/FieldDataSources/field/{fieldId}
   */
  static async getFieldDataSourcesByFieldId(fieldId: number): Promise<FieldDataSource[]> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/field/${fieldId}`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch data sources: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
  }

  /**
   * Get active field data sources by field ID (Admin)
   * GET /api/FieldDataSources/field/{fieldId}/active
   */
  static async getActiveFieldDataSourcesByFieldId(fieldId: number): Promise<FieldDataSource[]> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/field/${fieldId}/active`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch active data sources: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
  }

  /**
   * Get field data source by field ID and type (Admin)
   * GET /api/FieldDataSources/field/{fieldId}/type/{sourceType}
   */
  static async getFieldDataSourceByFieldIdAndType(
    fieldId: number,
    sourceType: string
  ): Promise<FieldDataSource> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/field/${fieldId}/type/${encodeURIComponent(sourceType)}`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Data source not found')
      }
      throw new Error(`Failed to fetch data source: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      if (!data.data) {
        throw new Error(data.message || 'Data source not found')
      }
      return data.data
    }
    return data
  }

  /**
   * Get data sources count for field (Admin)
   * GET /api/FieldDataSources/field/{fieldId}/count
   */
  static async getDataSourcesCount(fieldId: number): Promise<number> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/field/${fieldId}/count`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch data sources count: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || 0
    }
    return data || 0
  }

  /**
   * Create field data source (Admin)
   * POST /api/FieldDataSources
   */
  static async createFieldDataSource(
    dataSource: Omit<FieldDataSource, 'id'>
  ): Promise<FieldDataSource> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(dataSource)
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to create data source: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      if (!data.data) {
        throw new Error(data.message || 'Failed to create data source')
      }
      return data.data
    }
    return data
  }

  /**
   * Create bulk field data sources (Admin)
   * POST /api/FieldDataSources/bulk
   */
  static async createBulkFieldDataSources(
    dataSources: Omit<FieldDataSource, 'id'>[]
  ): Promise<FieldDataSource[]> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/bulk`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(dataSources)
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to create data sources: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
  }

  /**
   * Update field data source (Admin)
   * PUT /api/FieldDataSources/{id}
   */
  static async updateFieldDataSource(
    id: number,
    dataSource: Partial<Omit<FieldDataSource, 'id' | 'fieldId'>>
  ): Promise<FieldDataSource> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/${id}`,
      {
        method: 'PUT',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(dataSource)
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Data source not found')
      }
      throw new Error(`Failed to update data source: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      if (!data.data) {
        throw new Error(data.message || 'Failed to update data source')
      }
      return data.data
    }
    return data
  }

  /**
   * Delete field data source (Hard Delete) (Admin)
   * DELETE /api/FieldDataSources/{id}
   */
  static async deleteFieldDataSource(id: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/${id}`,
      {
        method: 'DELETE',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Data source not found')
      }
      throw new Error(`Failed to delete data source: ${response.statusText}`)
    }
  }

  // ================================
  // DOCUMENT SETTINGS - Admin Endpoints
  // ================================

  /**
   * Get Document Settings for a Form Builder (Admin)
   * GET /api/FormBuilderDocumentSettings/form/{formBuilderId}
   */
  static async getDocumentSettings(formBuilderId: number): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/FormBuilderDocumentSettings/form/${formBuilderId}`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        return null // No settings exist yet
      }
      throw new Error(`Failed to fetch document settings: ${response.statusText}`)
    }

    const data = await response.json()
    return data.success !== undefined ? data.data : data
  }

  /**
   * Save Document Settings for a Form Builder (Admin)
   * POST /api/FormBuilderDocumentSettings
   */
  static async saveDocumentSettings(settings: any): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/FormBuilderDocumentSettings`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(settings)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(errorData.errorMessage || errorData.message || 'Failed to save document settings')
    }

    const data = await response.json()
    return data.success !== undefined ? data.data : data
  }

  /**
   * Delete Document Settings for a Form Builder (Admin)
   * DELETE /api/FormBuilderDocumentSettings/form/{formBuilderId}
   */
  static async deleteDocumentSettings(formBuilderId: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/FormBuilderDocumentSettings/form/${formBuilderId}`,
      {
        method: 'DELETE',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to delete document settings: ${response.statusText}`)
    }
  }

  /**
   * Get active projects (Admin)
   * GET /api/Projects/active
   */
  static async getActiveProjects(): Promise<any[]> {
    const response = await fetch(
      `${API_BASE_URL}/Projects/active`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch projects: ${response.statusText}`)
    }

    const data = await response.json()
    const projects = data.success !== undefined ? data.data : data
    return Array.isArray(projects) ? projects : []
  }

  // ================================
  // DOCUMENT TYPES - CRUD Endpoints
  // ================================

  /**
   * Get all Document Types
   * GET /api/DocumentTypes
   */
  static async getAllDocumentTypes(): Promise<any[]> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentTypes`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch document types: ${response.statusText}`)
    }

    const data = await response.json()
    const documentTypes = data.success !== undefined ? data.data : data
    return Array.isArray(documentTypes) ? documentTypes : []
  }

  /**
   * Get Document Type by ID
   * GET /api/DocumentTypes/{id}
   */
  static async getDocumentTypeById(id: number): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentTypes/${id}`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Document type not found')
      }
      throw new Error(`Failed to fetch document type: ${response.statusText}`)
    }

    const data = await response.json()
    return data.success !== undefined ? data.data : data
  }

  /**
   * Get active Document Types
   * GET /api/DocumentTypes/active
   */
  static async getActiveDocumentTypes(): Promise<any[]> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentTypes/active`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch active document types: ${response.statusText}`)
    }

    const data = await response.json()
    const documentTypes = data.success !== undefined ? data.data : data
    return Array.isArray(documentTypes) ? documentTypes : []
  }

  /**
   * Create Document Type
   * POST /api/DocumentTypes
   */
  static async createDocumentType(documentType: any): Promise<any> {
    // Clean up the DTO - remove undefined/null values for optional fields
    const cleanDto: any = {
      name: documentType.name,
      code: documentType.code,
      menuCaption: documentType.menuCaption,
      menuOrder: documentType.menuOrder ?? 0,
      isActive: documentType.isActive ?? true
    }

    // Only include formBuilderId if it has a valid value
    if (documentType.formBuilderId != null && documentType.formBuilderId > 0) {
      cleanDto.formBuilderId = documentType.formBuilderId
    }

    // Include parentMenuId if it's explicitly set (including 0, which will be normalized to null in backend)
    if (documentType.parentMenuId !== undefined && documentType.parentMenuId !== null) {
      // Send 0 as-is; backend will normalize it to null
      cleanDto.parentMenuId = documentType.parentMenuId > 0 ? documentType.parentMenuId : 0
    }

    const response = await fetch(
      `${API_BASE_URL}/DocumentTypes`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(cleanDto)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error || `Failed to create document type (${response.status})`
      throw new Error(errorMessage)
    }

    const data = await response.json()
    return data.success !== undefined ? data.data : data
  }

  /**
   * Update Document Type
   * PUT /api/DocumentTypes/{id}
   */
  static async updateDocumentType(id: number, documentType: any): Promise<void> {
    // Clean up the DTO - only include fields that are provided
    const cleanDto: any = {}

    if (documentType.name != null) cleanDto.name = documentType.name
    if (documentType.code != null) cleanDto.code = documentType.code
    if (documentType.menuCaption != null) cleanDto.menuCaption = documentType.menuCaption
    if (documentType.menuOrder != null) cleanDto.menuOrder = documentType.menuOrder
    if (documentType.isActive != null) cleanDto.isActive = documentType.isActive

    // Only include formBuilderId if it has a valid value or is explicitly null
    if (documentType.formBuilderId !== undefined) {
      cleanDto.formBuilderId = documentType.formBuilderId > 0 ? documentType.formBuilderId : null
    }

    // Include parentMenuId if it's explicitly set (including 0, which will be normalized to null in backend)
    if (documentType.parentMenuId !== undefined) {
      // Send 0 as-is; backend will normalize it to null
      cleanDto.parentMenuId = documentType.parentMenuId > 0 ? documentType.parentMenuId : 0
    }

    const response = await fetch(
      `${API_BASE_URL}/DocumentTypes/${id}`,
      {
        method: 'PUT',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(cleanDto)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error || `Failed to update document type (${response.status})`
      throw new Error(errorMessage)
    }
  }

  /**
   * Delete Document Type
   * DELETE /api/DocumentTypes/{id}
   */
  static async deleteDocumentType(id: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentTypes/${id}`,
      {
        method: 'DELETE',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      
      if (response.status === 404) {
        throw new Error(errorData.message || errorData.errorMessage || 'Document type not found')
      }
      
      // Check for foreign key constraint errors
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error
      if (errorMessage && (
        errorMessage.includes('foreign key') || 
        errorMessage.includes('constraint') ||
        errorMessage.includes('reference') ||
        errorMessage.includes('children') ||
        errorMessage.includes('submissions')
      )) {
        throw new Error('Cannot delete document type: It has related records (children or submissions). Please remove or reassign them first.')
      }
      
      throw new Error(errorMessage || `Failed to delete document type (${response.status})`)
    }
  }

  /**
   * Soft delete field data source (Admin)
   * DELETE /api/FieldDataSources/soft-delete/{id}
   */
  static async softDeleteFieldDataSource(id: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/FieldDataSources/soft-delete/${id}`,
      {
        method: 'DELETE',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Data source not found')
      }
      throw new Error(`Failed to soft delete data source: ${response.statusText}`)
    }
  }

  // ================================
  // DOCUMENT SERIES - CRUD Endpoints
  // ================================

  /**
   * Get all Document Series
   * GET /api/DocumentSeries
   */
  static async getAllDocumentSeries(): Promise<any[]> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch document series: ${response.statusText}`)
    }

    const data = await response.json()
    return data.data || []
  }

  /**
   * Get Document Series by ID
   * GET /api/DocumentSeries/{id}
   */
  static async getDocumentSeriesById(id: number): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries/${id}`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Document series not found')
      }
      throw new Error(`Failed to fetch document series: ${response.statusText}`)
    }

    const data = await response.json()
    return data.data
  }

  /**
   * Get Document Series by Document Type ID
   * GET /api/DocumentSeries/document-type/{documentTypeId}
   */
  static async getDocumentSeriesByDocumentTypeId(documentTypeId: number): Promise<any[]> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries/document-type/${documentTypeId}`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch document series: ${response.statusText}`)
    }

    const data = await response.json()
    return data.data || []
  }

  /**
   * Create Document Series
   * POST /api/DocumentSeries
   */
  static async createDocumentSeries(series: any): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries`,
      {
        method: 'POST',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(series)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error || `Failed to create document series (${response.status})`
      throw new Error(errorMessage)
    }

    const data = await response.json()
    return data.data || data
  }

  /**
   * Update Document Series
   * PUT /api/DocumentSeries/{id}
   */
  static async updateDocumentSeries(id: number, series: any): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries/${id}`,
      {
        method: 'PUT',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(series)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error || `Failed to update document series (${response.status})`
      throw new Error(errorMessage)
    }

    const data = await response.json()
    return data.data || data
  }

  /**
   * Delete Document Series
   * DELETE /api/DocumentSeries/{id}
   */
  static async deleteDocumentSeries(id: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries/${id}`,
      {
        method: 'DELETE',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      
      if (response.status === 404) {
        throw new Error(errorData.message || 'Document series not found')
      }
      
      // Check for foreign key constraint errors
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error
      if (errorMessage && (
        errorMessage.includes('foreign key') || 
        errorMessage.includes('constraint') ||
        errorMessage.includes('reference') ||
        errorMessage.includes('submissions') ||
        errorMessage.includes('form submissions')
      )) {
        throw new Error('Cannot delete document series: There are form submissions associated with this series. Please delete or reassign the submissions first.')
      }
      
      throw new Error(errorMessage || `Failed to delete document series (${response.status})`)
    }
  }

  /**
   * Toggle Document Series Active Status
   * PATCH /api/DocumentSeries/{id}/toggle-active
   */
  static async toggleDocumentSeriesActive(id: number, isActive: boolean): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries/${id}/toggle-active`,
      {
        method: 'PATCH',
        headers: this.getAuthHeaders(),
        body: JSON.stringify(isActive)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error || `Failed to toggle document series status (${response.status})`
      throw new Error(errorMessage)
    }

    const data = await response.json()
    return data.data || data
  }

  /**
   * Set Document Series as Default
   * PATCH /api/DocumentSeries/{id}/set-default
   */
  static async setDocumentSeriesAsDefault(id: number): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries/${id}/set-default`,
      {
        method: 'PATCH',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      const errorMessage = errorData.message || errorData.errorMessage || errorData.error || `Failed to set document series as default (${response.status})`
      throw new Error(errorMessage)
    }

    const data = await response.json()
    return data.data || data
  }

  /**
   * Get Next Number for Document Series
   * GET /api/DocumentSeries/{id}/next-number
   */
  static async getDocumentSeriesNextNumber(id: number): Promise<any> {
    const response = await fetch(
      `${API_BASE_URL}/DocumentSeries/${id}/next-number`,
      {
        method: 'GET',
        headers: this.getAuthHeaders()
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(errorData.message || errorData.errorMessage || `Failed to get next number (${response.status})`)
    }

    const data = await response.json()
    return data.data || data
  }

  // ================================ 
  // FORM RULES - Admin Endpoints
  // ================================

  /**
   * Get all form rules
   * GET /api/FormRules
   */
  static async getAllFormRules(): Promise<ServiceResult<FormRuleDto[]>> {
    const response = await fetch(`${API_BASE_URL}/FormRules`, {
      method: 'GET',
      headers: this.getAuthHeaders(),
    })
    return response.json()
  }

  /**
   * Get form rule by ID
   * GET /api/FormRules/{id}
   */
  static async getFormRuleById(id: number): Promise<ServiceResult<FormRuleDto>> {
    const response = await fetch(`${API_BASE_URL}/FormRules/${id}`, {
      method: 'GET',
      headers: this.getAuthHeaders(),
    })
    return response.json()
  }

  /**
   * Get active rules by form builder ID
   * GET /api/FormRules/form/{formBuilderId}/active
   */
  static async getActiveRulesByFormId(formBuilderId: number): Promise<ServiceResult<FormRuleDto[]>> {
    const response = await fetch(`${API_BASE_URL}/FormRules/form/${formBuilderId}/active`, {
      method: 'GET',
      headers: this.getHeaders(), // Use getHeaders() instead of getAuthHeaders() for public access
    })
    return response.json()
  }

  /**
   * Create form rule
   * POST /api/FormRules
   */
  static async createFormRule(rule: CreateFormRuleDto): Promise<ServiceResult<FormRuleDto>> {
    const response = await fetch(`${API_BASE_URL}/FormRules`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(rule),
    })
    return response.json()
  }

  /**
   * Update form rule
   * PUT /api/FormRules/{id}
   */
  static async updateFormRule(id: number, rule: UpdateFormRuleDto): Promise<ServiceResult<boolean>> {
    const response = await fetch(`${API_BASE_URL}/FormRules/${id}`, {
      method: 'PUT',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(rule),
    })
    return response.json()
  }

  /**
   * Delete form rule
   * DELETE /api/FormRules/{id}
   */
  static async deleteFormRule(id: number): Promise<ServiceResult<boolean>> {
    const response = await fetch(`${API_BASE_URL}/FormRules/${id}`, {
      method: 'DELETE',
      headers: this.getAuthHeaders(),
    })
    return response.json()
  }

  /**
   * Helper: Convert FormRule (UI) to CreateFormRuleDto
   */
  static convertFormRuleToDto(formRule: FormRule, formBuilderId: number): CreateFormRuleDto {
    return {
      formBuilderId,
      ruleName: formRule.ruleName,
      conditionField: formRule.condition.field,
      conditionOperator: formRule.condition.operator,
      conditionValue: formRule.condition.value?.toString(),
      conditionValueType: formRule.condition.valueType,
      // Send Actions and ElseActions as arrays (not JSON strings)
      actions: formRule.actions && formRule.actions.length > 0 ? formRule.actions : undefined,
      elseActions: formRule.elseActions && formRule.elseActions.length > 0 ? formRule.elseActions : undefined,
      isActive: formRule.isActive,
      executionOrder: formRule.executionOrder
    }
  }

  /**
   * Helper: Convert FormRuleDto to FormRule (UI)
   */
  static convertDtoToFormRule(dto: FormRuleDto): FormRule {
    let condition: Condition = {
      field: dto.conditionField || '',
      operator: dto.conditionOperator || '==',
      value: dto.conditionValue || '',
      valueType: (dto.conditionValueType as 'constant' | 'field') || 'constant'
    }

    // Actions and ElseActions are now returned as arrays directly from .NET API
    let actions: Action[] = dto.actions || []
    let elseActions: Action[] = dto.elseActions || []

    return {
      id: dto.id,
      ruleName: dto.ruleName,
      condition,
      actions,
      elseActions,
      isActive: dto.isActive,
      executionOrder: dto.executionOrder || 1
    }
  }
}












