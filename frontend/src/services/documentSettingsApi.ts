import { DocumentSettings, SaveDocumentSettings, Project } from '../types/documentSettings'

const API_BASE_URL = '/api'

// Get current language from localStorage or default to 'en'
const getCurrentLanguage = (): string => {
  return localStorage.getItem('i18nextLng') || 'en'
}

/**
 * Get authorization headers (includes token if available)
 */
const getAuthHeaders = (): HeadersInit => {
  const language = getCurrentLanguage()
  const token = localStorage.getItem('token') || localStorage.getItem('accessToken')
  
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    'Accept-Language': language,
  }
  
  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }
  
  return headers
}

export class DocumentSettingsApi {
  /**
   * Get Document Settings for a Form Builder
   * GET /api/FormBuilderDocumentSettings/form/{formBuilderId}
   */
  static async getDocumentSettings(formBuilderId: number): Promise<DocumentSettings> {
    const response = await fetch(
      `${API_BASE_URL}/FormBuilderDocumentSettings/form/${formBuilderId}`,
      {
        method: 'GET',
        headers: getAuthHeaders(),
      }
    )

    if (!response.ok) {
      if (response.status === 404) {
        // Return empty settings if not found
        return {
          formBuilderId,
          formBuilderName: '',
          documentName: '',
          documentCode: '',
          menuCaption: '',
          menuOrder: 0,
          isActive: true,
          documentSeries: []
        }
      }
      throw new Error(`Failed to fetch document settings: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper if present
    if (data.success !== undefined) {
      if (!data.success || !data.data) {
        throw new Error(data.errorMessage || data.message || 'Failed to fetch document settings')
      }
      return data.data
    }

    return data as DocumentSettings
  }

  /**
   * Save Document Settings for a Form Builder
   * POST /api/FormBuilderDocumentSettings
   */
  static async saveDocumentSettings(settings: SaveDocumentSettings): Promise<DocumentSettings> {
    const response = await fetch(
      `${API_BASE_URL}/FormBuilderDocumentSettings`,
      {
        method: 'POST',
        headers: getAuthHeaders(),
        body: JSON.stringify(settings)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      const errorMessage = errorData.errorMessage || errorData.message || errorData.error || 'Failed to save document settings'
      
      // Provide more specific error messages
      if (errorMessage.includes('code already exists') || errorMessage.includes('CodeExists')) {
        throw new Error('Document code or series code already exists. Please use a different code.')
      }
      
      if (errorMessage.includes('Invalid') || errorMessage.includes('not found')) {
        throw new Error(errorMessage)
      }
      
      throw new Error(errorMessage)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper if present
    if (data.success !== undefined) {
      if (!data.success || !data.data) {
        throw new Error(data.errorMessage || data.message || 'Failed to save document settings')
      }
      return data.data
    }

    return data as DocumentSettings
  }

  /**
   * Delete Document Settings for a Form Builder
   * DELETE /api/FormBuilderDocumentSettings/form/{formBuilderId}
   */
  static async deleteDocumentSettings(formBuilderId: number): Promise<void> {
    const response = await fetch(
      `${API_BASE_URL}/FormBuilderDocumentSettings/form/${formBuilderId}`,
      {
        method: 'DELETE',
        headers: getAuthHeaders()
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(errorData.errorMessage || errorData.message || 'Failed to delete document settings')
    }
  }

  /**
   * Get active projects
   * GET /api/Projects/active
   */
  static async getActiveProjects(): Promise<Project[]> {
    const response = await fetch(
      `${API_BASE_URL}/Projects/active`,
      {
        method: 'GET',
        headers: getAuthHeaders()
      }
    )

    if (!response.ok) {
      throw new Error(`Failed to fetch projects: ${response.statusText}`)
    }

    const data = await response.json()
    // Handle ServiceResult wrapper if present
    if (data.success !== undefined) {
      return data.data || []
    }
    return data || []
  }
}

