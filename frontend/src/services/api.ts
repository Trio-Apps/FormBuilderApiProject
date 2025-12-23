import { FormBuilder, ServiceResult, FieldDataSource, FieldOptionResponse, FieldOption } from '../types/form'

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
    // Handle ApiResponse wrapper
    if (data.statusCode !== undefined) {
      return data.data || []
    }
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
  }

  /**
   * Get field options with POST (Public - No auth required)
   * POST /api/FieldDataSources/field-options
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
    // Handle ServiceResult wrapper
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
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
}












