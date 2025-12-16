import { FormBuilder, ServiceResult } from '../types/form'

const API_BASE_URL = '/api'

export class ApiService {
  /**
   * Fetch form by code (public identifier)
   */
  static async getFormByCode(formCode: string): Promise<FormBuilder> {
    const response = await fetch(`${API_BASE_URL}/FormBuilder/code/${encodeURIComponent(formCode)}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
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







