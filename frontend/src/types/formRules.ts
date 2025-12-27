// Form Rules Types
export interface Condition {
  field: string
  operator: string
  value: any
  valueType: 'constant' | 'field'
}

export interface Action {
  type: string
  fieldCode: string
  value?: any
  expression?: string
}

export interface FormRuleData {
  condition: Condition
  actions?: Action[]
  elseActions?: Action[]
}

export interface CreateFormRuleDto {
  formBuilderId: number
  ruleName: string
  conditionField?: string
  conditionOperator?: string
  conditionValue?: string
  conditionValueType?: string
  // Actions as List (not JSON) - matches .NET API
  actions?: Action[]
  elseActions?: Action[]
  isActive?: boolean
  executionOrder?: number
}

export interface UpdateFormRuleDto {
  formBuilderId: number
  ruleName: string
  conditionField?: string
  conditionOperator?: string
  conditionValue?: string
  conditionValueType?: string
  // Actions as List (not JSON) - matches .NET API
  actions?: Action[]
  elseActions?: Action[]
  isActive?: boolean
  executionOrder?: number
}

export interface FormRuleDto {
  id: number
  formBuilderId: number
  ruleName: string
  conditionField?: string
  conditionOperator?: string
  conditionValue?: string
  conditionValueType?: string
  // Actions as List (not JSON) - returned from .NET API
  actions?: Action[]
  elseActions?: Action[]
  isActive: boolean
  executionOrder?: number
  formName?: string
  formCode?: string
}

// Frontend Form Rule (for UI)
export interface FormRule {
  id?: number
  ruleName: string
  condition: Condition
  actions: Action[]
  elseActions?: Action[]
  isActive: boolean
  executionOrder: number
}

