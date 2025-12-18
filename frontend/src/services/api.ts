import { FormBuilder, ServiceResult } from '../types/form'

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
}












