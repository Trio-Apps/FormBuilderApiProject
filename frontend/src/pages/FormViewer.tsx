import { useEffect, useState, useCallback } from 'react'
import { useParams } from 'react-router-dom'
import { FormBuilder, FormField } from '../types/form'
import { ApiService } from '../services/api'
import { FormRuleDto } from '../types/formRules'
import TabNavigation from '../components/TabNavigation'
import FormFieldRenderer from '../components/FormFieldRenderer'
import LanguageSwitcher from '../components/LanguageSwitcher'
import { useLanguage } from '../contexts/LanguageContext'
import { evaluateExpression, extractFieldCodes } from '../utils/calculationEngine'
import './FormViewer.css'

const FormViewer = () => {
  const { formPublicId } = useParams<{ formPublicId: string }>()
  const { t } = useLanguage()
  const [form, setForm] = useState<FormBuilder | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [activeTabIndex, setActiveTabIndex] = useState(0)
  const [rules, setRules] = useState<FormRuleDto[]>([])
  const [fieldValues, setFieldValues] = useState<Record<string, any>>({})

  // Load rules for the form
  const loadRules = useCallback(async (formBuilderId: number) => {
    try {
      console.log(`[FormView] Loading rules from API for form: ${formBuilderId}`)
      const result = await ApiService.getActiveRulesByFormId(formBuilderId)
      if (result.success && result.data) {
        console.log(`[FormView] Loaded rules from API: ${result.data.length}`)
        setRules(result.data)
        return result.data
      } else {
        console.log(`[FormView] Loaded rules from API: 0`)
        setRules([])
        return []
      }
    } catch (err) {
      console.error('[FormView] Error loading rules:', err)
      setRules([])
      return []
    }
  }, [])

  // Evaluate rules and apply actions
  const evaluateRules = useCallback((currentValues: Record<string, any>) => {
    if (rules.length === 0) {
      console.log('[FormView] No rules to evaluate')
      return
    }

    if (!form) return

    console.log(`[FormView] Evaluating ${rules.length} rules`)

    // Sort rules by execution order
    const sortedRules = [...rules].sort((a, b) => (a.executionOrder || 1) - (b.executionOrder || 1))

    // Create a deep copy of form to update field visibility
    const updatedForm: FormBuilder = {
      ...form,
      tabs: form.tabs.map(tab => ({
        ...tab,
        fields: tab.fields.map(field => ({ ...field }))
      }))
    }
    
    const updatedValues = { ...currentValues }
    let formChanged = false

    sortedRules.forEach((rule) => {
      if (!rule.isActive || !rule.conditionField || !rule.conditionOperator) return

      const fieldValue = updatedValues[rule.conditionField]
      const conditionValue = rule.conditionValue
      let conditionMet = false

      // Evaluate condition
      switch (rule.conditionOperator.toLowerCase()) {
        case 'equals':
        case '==':
        case '===':
          conditionMet = String(fieldValue) === String(conditionValue)
          break
        case 'notequals':
        case '!=':
        case '!==':
          conditionMet = String(fieldValue) !== String(conditionValue)
          break
        case 'contains':
          conditionMet = String(fieldValue || '').includes(String(conditionValue))
          break
        case 'greaterthan':
        case '>':
          conditionMet = Number(fieldValue) > Number(conditionValue)
          break
        case 'lessthan':
        case '<':
          conditionMet = Number(fieldValue) < Number(conditionValue)
          break
        default:
          conditionMet = String(fieldValue) === String(conditionValue)
      }

      // Apply actions
      const actionsToApply = conditionMet ? rule.actions : rule.elseActions
      if (actionsToApply && actionsToApply.length > 0) {
        actionsToApply.forEach((action) => {
          // Find the field by code
          updatedForm.tabs.forEach((tab) => {
            tab.fields.forEach((field) => {
              if (field.fieldCode === action.fieldCode) {
                switch (action.type) {
                  case 'SetVisible':
                    if (field.isVisible !== true) {
                      field.isVisible = true
                      formChanged = true
                    }
                    break
                  case 'SetHidden':
                  case 'Hide':
                    if (field.isVisible !== false) {
                      field.isVisible = false
                      formChanged = true
                    }
                    break
                  case 'SetDefault':
                    if (action.value !== undefined) {
                      updatedValues[action.fieldCode] = action.value
                      formChanged = true
                    }
                    break
                  case 'ClearValue':
                    updatedValues[action.fieldCode] = ''
                    formChanged = true
                    break
                }
              }
            })
          })
        })
      }
    })

    if (formChanged) {
      setForm(updatedForm)
      setFieldValues(updatedValues)
    }
  }, [rules, form])

  useEffect(() => {
    const fetchForm = async () => {
      if (!formPublicId) {
        setError('Form ID is required')
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const formData = await ApiService.getFormByCode(formPublicId)
        
        // Load field options for fields with data sources (API/LookupTable)
        if (formData.tabs) {
          for (const tab of formData.tabs) {
            if (tab.fields) {
              for (const field of tab.fields) {
                // If field has a data source (API or LookupTable), load options
                if (field.fieldDataSource && 
                    (field.fieldDataSource.sourceType === 'Api' || 
                     field.fieldDataSource.sourceType === 'LookupTable')) {
                  try {
                    const options = await ApiService.getFieldOptions(field.id)
                    // Backend returns FieldOption[] directly, so we can use it as-is
                    field.fieldOptions = options || []
                  } catch (err) {
                    console.error(`Failed to load options for field ${field.id}:`, err)
                    // Keep empty array if loading fails
                    field.fieldOptions = []
                  }
                }
              }
            }
          }
        }
        
        setForm(formData)

        // Load rules after form is loaded
        await loadRules(formData.id)
      } catch (err) {
        setError(err instanceof Error ? err.message : t('errors.failedToFetchForm'))
      } finally {
        setLoading(false)
      }
    }

    fetchForm()
  }, [formPublicId, t, loadRules])

  // Calculate calculated fields when fieldValues change
  const calculateFields = useCallback((currentValues: Record<string, any>) => {
    if (!form) return currentValues

    const updatedValues = { ...currentValues }
    let hasChanges = false

    // Find all calculated fields across all tabs
    form.tabs.forEach((tab) => {
      tab.fields.forEach((field) => {
        const fieldTypeName = field.fieldTypeName || field.fieldType?.typeName || ''
        const isCalculated = fieldTypeName.toLowerCase() === 'calculated'

        if (isCalculated && field.expressionText) {
          // Check if we should recalculate based on RecalculateOn setting
          const recalculateOn = field.recalculateOn || 'OnFieldChange'
          
          // For now, always recalculate on field change (we'll enhance this later)
          if (recalculateOn === 'OnFieldChange' || recalculateOn === 'OnLoad') {
            const resultType = field.resultType || 'decimal'
            
            // Create a copy of values without the current calculated field to prevent circular reference
            const valuesForCalculation = { ...currentValues }
            // Remove the current field's value to prevent self-reference
            delete valuesForCalculation[field.fieldCode]
            delete valuesForCalculation[field.fieldCode.toLowerCase()]
            
            const calculatedValue = evaluateExpression(
              field.expressionText,
              valuesForCalculation,
              resultType
            )

            // Only update if value changed or is null/undefined
            const currentValue = updatedValues[field.fieldCode]
            if (currentValue !== calculatedValue && calculatedValue !== null && calculatedValue !== undefined) {
              updatedValues[field.fieldCode] = calculatedValue
              hasChanges = true
              console.log(`[FormView] Calculated field ${field.fieldCode}: ${calculatedValue} (Expression: ${field.expressionText})`)
            }
          }
        }
      })
    })

    return hasChanges ? updatedValues : currentValues
  }, [form])

  // Evaluate rules when rules or fieldValues change
  useEffect(() => {
    if (rules.length > 0 && form) {
      evaluateRules(fieldValues)
    }
  }, [rules, fieldValues, form, evaluateRules])

  // Calculate calculated fields when fieldValues change
  useEffect(() => {
    if (form) {
      const updatedValues = calculateFields(fieldValues)
      if (updatedValues !== fieldValues) {
        setFieldValues(updatedValues)
      }
    }
  }, [form, fieldValues, calculateFields])

  if (loading) {
    return (
      <div className="form-viewer-container">
        <div className="form-viewer-loading">{t('common.loading')}</div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="form-viewer-container">
        <LanguageSwitcher />
        <div className="form-viewer-error">
          <h2>{t('errors.formNotFound')}</h2>
          <p>{error}</p>
        </div>
      </div>
    )
  }

  if (!form) {
    return (
      <div className="form-viewer-container">
        <LanguageSwitcher />
        <div className="form-viewer-error">
          <h2>{t('errors.formNotFound')}</h2>
          <p>{t('errors.formNotFound')}</p>
        </div>
      </div>
    )
  }

  const { currentLanguage } = useLanguage()
  const activeTabs = form.tabs.filter(tab => tab.isActive).sort((a, b) => a.tabOrder - b.tabOrder)
  const activeTab = activeTabs[activeTabIndex]

  // Get multilingual form name and description
  const getFormName = () => {
    if (currentLanguage === 'ar' && form.foreignFormName) {
      return form.foreignFormName
    }
    return form.formName
  }

  const getFormDescription = () => {
    if (currentLanguage === 'ar' && form.foreignDescription) {
      return form.foreignDescription
    }
    return form.description
  }

  return (
    <div className="form-viewer-container">
      <div className="form-viewer-content">
        {/* Language Switcher */}
        <div style={{ position: 'absolute', top: '20px', right: '20px' }}>
          <LanguageSwitcher />
        </div>

        {/* Form Header */}
        <div className="form-header">
          <h1 className="form-title">{getFormName()}</h1>
          {getFormDescription() && (
            <p className="form-description">{getFormDescription()}</p>
          )}
        </div>

        {/* Tab Navigation */}
        {activeTabs.length > 1 && (
          <TabNavigation
            tabs={activeTabs}
            activeIndex={activeTabIndex}
            onTabChange={setActiveTabIndex}
          />
        )}

        {/* Form Fields */}
        {activeTab && (
          <div className="form-tab-content">
            <div className="form-fields-container">
              {activeTab.fields
                .filter(field => field.isVisible)
                .sort((a, b) => a.fieldOrder - b.fieldOrder)
                .map((field) => (
                  <FormFieldRenderer 
                    key={field.id} 
                    field={field}
                    value={fieldValues[field.fieldCode]}
                    onChange={(value: any) => {
                      const newValues = { ...fieldValues, [field.fieldCode]: value }
                      setFieldValues(newValues)
                      console.log(`[FormView] Value change: ID=${field.id}, Code=${field.fieldCode} -> ${value}`)
                      // Evaluate rules when field value changes
                      evaluateRules(newValues)
                    }}
                  />
                ))}
            </div>
            
            {/* Submit Button - Only show on last tab */}
            {activeTabIndex === activeTabs.length - 1 && (
              <div className="form-submit-container">
                <button type="button" className="form-submit-button">
                  {t('common.submit')}
                </button>
              </div>
            )}
          </div>
        )}

        {activeTabs.length === 0 && (
          <div className="form-empty">
            <p>{t('form.noFields')}</p>
          </div>
        )}
      </div>
    </div>
  )
}

export default FormViewer





