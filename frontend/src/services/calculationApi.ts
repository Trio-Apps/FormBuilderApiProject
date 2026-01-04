import { ApiService } from './api'

/**
 * Request interface for calculation endpoints
 */
export interface CalculateExpressionRequest {
  expressionText: string
  fieldValues: Record<string, any>
}

/**
 * Response interface for calculation endpoints
 */
export interface CalculateResponse {
  success: boolean
  data: number | string | null
  message?: string
  statusCode?: number
}

/**
 * Batch calculation response
 */
export interface BatchCalculateResponse {
  success: boolean
  data: Record<number, number | string>
  message?: string
}

/**
 * Calculation API Service
 * Handles all calculation-related API calls
 */
export class CalculationApiService {
  private static readonly BASE_URL = '/api/Formulas'

  /**
   * حساب تعبير مباشر
   * POST /api/Formulas/calculate-expression
   * 
   * @param request - التعبير والقيم
   * @returns النتيجة المحسوبة
   */
  static async calculateExpression(
    request: CalculateExpressionRequest
  ): Promise<CalculateResponse> {
    const response = await fetch(
      `${this.BASE_URL}/calculate-expression`,
      {
        method: 'POST',
        headers: ApiService.getAuthHeaders(),
        body: JSON.stringify(request)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(
        errorData.message || 
        `Failed to calculate expression: ${response.statusText}`
      )
    }

    const data = await response.json()
    return data as CalculateResponse
  }

  /**
   * حساب آمن (مُوصى به) ✅
   * POST /api/Formulas/calculate-safe
   * 
   * يستخدم SafeCalculateExpressionAsync الذي يدعم جميع العمليات المتقدمة
   * 
   * @param request - التعبير والقيم
   * @returns النتيجة المحسوبة
   */
  static async calculateSafe(
    request: CalculateExpressionRequest
  ): Promise<CalculateResponse> {
    const response = await fetch(
      `${this.BASE_URL}/calculate-safe`,
      {
        method: 'POST',
        headers: ApiService.getAuthHeaders(),
        body: JSON.stringify(request)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(
        errorData.message || 
        `Failed to calculate expression: ${response.statusText}`
      )
    }

    const data = await response.json()
    return data as CalculateResponse
  }

  /**
   * حساب متقدم (مع جميع العمليات)
   * POST /api/Formulas/calculate-advanced
   * 
   * @param request - التعبير والقيم
   * @returns النتيجة المحسوبة
   */
  static async calculateAdvanced(
    request: CalculateExpressionRequest
  ): Promise<CalculateResponse> {
    const response = await fetch(
      `${this.BASE_URL}/calculate-advanced`,
      {
        method: 'POST',
        headers: ApiService.getAuthHeaders(),
        body: JSON.stringify(request)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(
        errorData.message || 
        `Failed to calculate expression: ${response.statusText}`
      )
    }

    const data = await response.json()
    return data as CalculateResponse
  }

  /**
   * حساب باستخدام Formula ID
   * POST /api/Formulas/{formulaId}/calculate
   * 
   * @param formulaId - معرف الصيغة
   * @param fieldValues - قيم الحقول
   * @returns النتيجة المحسوبة
   */
  static async calculateByFormulaId(
    formulaId: number,
    fieldValues: Record<string, any>
  ): Promise<CalculateResponse> {
    const response = await fetch(
      `${this.BASE_URL}/${formulaId}/calculate`,
      {
        method: 'POST',
        headers: ApiService.getAuthHeaders(),
        body: JSON.stringify(fieldValues)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(
        errorData.message || 
        `Failed to calculate formula: ${response.statusText}`
      )
    }

    const data = await response.json()
    return data as CalculateResponse
  }

  /**
   * حساب مجمع لجميع الصيغ في نموذج
   * POST /api/Formulas/form-builder/{formBuilderId}/batch-calculate
   * 
   * يحسب جميع الصيغ النشطة في النموذج دفعة واحدة
   * 
   * @param formBuilderId - معرف النموذج
   * @param fieldValues - قيم الحقول
   * @returns Dictionary<formulaId, result>
   */
  static async batchCalculate(
    formBuilderId: number,
    fieldValues: Record<string, any>
  ): Promise<BatchCalculateResponse> {
    const response = await fetch(
      `${this.BASE_URL}/form-builder/${formBuilderId}/batch-calculate`,
      {
        method: 'POST',
        headers: ApiService.getAuthHeaders(),
        body: JSON.stringify(fieldValues)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(
        errorData.message || 
        `Failed to batch calculate: ${response.statusText}`
      )
    }

    const data = await response.json()
    return data as BatchCalculateResponse
  }

  /**
   * معاينة الحساب (للاختبار)
   * POST /api/Formulas/preview-calculation
   * 
   * @param request - التعبير والقيم
   * @returns النتيجة المحسوبة
   */
  static async previewCalculation(
    request: CalculateExpressionRequest
  ): Promise<CalculateResponse> {
    const response = await fetch(
      `${this.BASE_URL}/preview-calculation`,
      {
        method: 'POST',
        headers: ApiService.getAuthHeaders(),
        body: JSON.stringify(request)
      }
    )

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}))
      throw new Error(
        errorData.message || 
        `Failed to preview calculation: ${response.statusText}`
      )
    }

    const data = await response.json()
    return data as CalculateResponse
  }

  /**
   * Helper: تحويل النتيجة إلى رقم
   * 
   * @param response - استجابة الحساب
   * @param defaultValue - القيمة الافتراضية في حالة الخطأ
   * @returns رقم أو القيمة الافتراضية
   */
  static getNumericResult(
    response: CalculateResponse,
    defaultValue: number = 0
  ): number {
    if (!response.success || response.data === null || response.data === undefined) {
      return defaultValue
    }

    const numValue = Number(response.data)
    return isNaN(numValue) ? defaultValue : numValue
  }

  /**
   * Helper: تحويل النتيجة إلى نص
   * 
   * @param response - استجابة الحساب
   * @param defaultValue - القيمة الافتراضية في حالة الخطأ
   * @returns نص أو القيمة الافتراضية
   */
  static getStringResult(
    response: CalculateResponse,
    defaultValue: string = ''
  ): string {
    if (!response.success || response.data === null || response.data === undefined) {
      return defaultValue
    }

    return String(response.data)
  }
}


