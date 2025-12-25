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
  actionsJson?: string
  elseActionsJson?: string
  ruleJson?: string // Send this to let backend parse and split automatically
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
  actionsJson?: string
  elseActionsJson?: string
  ruleJson?: string // Send this to let backend parse and split automatically
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
  actionsJson?: string // Generated from FORM_RULE_ACTIONS table
  elseActionsJson?: string // Generated from FORM_RULE_ACTIONS table
  ruleJson?: string
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

