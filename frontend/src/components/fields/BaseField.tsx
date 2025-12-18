import { ReactNode } from 'react'
import { FormField } from '../../types/form'
import { useLanguage } from '../../contexts/LanguageContext'
import './BaseField.css'

interface BaseFieldProps {
  field: FormField
  children: ReactNode
}

const BaseField = ({ field, children }: BaseFieldProps) => {
  const { currentLanguage } = useLanguage()
  
  // Get multilingual content based on current language
  // Support both naming patterns: label_ar/label_en and foreignFieldName/FieldName
  const getFieldName = () => {
    if (currentLanguage === 'ar') {
      return field.label_ar || field.foreignFieldName || field.fieldName
    }
    return field.label_en || field.fieldName
  }

  const getPlaceholder = () => {
    if (currentLanguage === 'ar') {
      return field.placeholder_ar || field.foreignPlaceholder || field.placeholder
    }
    return field.placeholder_en || field.placeholder
  }

  const getHintText = () => {
    if (currentLanguage === 'ar' && field.foreignHintText) {
      return field.foreignHintText
    }
    return field.hintText
  }

  const getValidationMessage = () => {
    if (currentLanguage === 'ar' && field.foreignValidationMessage) {
      return field.foreignValidationMessage
    }
    return field.validationMessage
  }

  return (
    <div className="form-field">
      <label className="field-label">
        {getFieldName()}
        {field.isMandatory && <span className="required-indicator">*</span>}
      </label>
      {children}
      {getHintText() && (
        <div className="field-hint">{getHintText()}</div>
      )}
    </div>
  )
}

// Export helper functions for use in field components
export const useMultilingualField = (field: FormField) => {
  const { currentLanguage } = useLanguage()
  
  return {
    fieldName: currentLanguage === 'ar' 
      ? (field.label_ar || field.foreignFieldName || field.fieldName)
      : (field.label_en || field.fieldName),
    placeholder: currentLanguage === 'ar'
      ? (field.placeholder_ar || field.foreignPlaceholder || field.placeholder)
      : (field.placeholder_en || field.placeholder),
    hintText: currentLanguage === 'ar' && field.foreignHintText ? field.foreignHintText : field.hintText,
    validationMessage: currentLanguage === 'ar' && field.foreignValidationMessage ? field.foreignValidationMessage : field.validationMessage,
  }
}

export default BaseField










