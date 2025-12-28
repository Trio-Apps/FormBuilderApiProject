import { useState } from 'react'
import BaseField, { useMultilingualField } from './BaseField'
import { FormField } from '../../types/form'
import { useLanguage } from '../../contexts/LanguageContext'
import './fields.css'

interface SelectFieldProps {
  field: FormField
  value?: any
  onChange?: (value: any) => void
}

const SelectField = ({ field, value: controlledValue, onChange }: SelectFieldProps) => {
  const [internalValue, setInternalValue] = useState('')
  const { currentLanguage } = useLanguage()
  const { placeholder } = useMultilingualField(field)
  const sortedOptions = [...field.fieldOptions]
    .filter(opt => opt.isActive)
    .sort((a, b) => a.optionOrder - b.optionOrder)

  // Use controlled value if provided, otherwise use internal state
  const value = controlledValue !== undefined ? controlledValue : internalValue

  const handleChange = (newValue: string) => {
    if (onChange) {
      onChange(newValue)
    } else {
      setInternalValue(newValue)
    }
  }

  const getOptionText = (option: { optionText: string; foreignOptionText?: string }) => {
    if (currentLanguage === 'ar' && option.foreignOptionText) {
      return option.foreignOptionText
    }
    return option.optionText
  }

  // Show error message if no options available
  if (sortedOptions.length === 0) {
    return (
      <BaseField field={field}>
        <div className="field-error-message" style={{
          backgroundColor: '#fee',
          borderLeft: '4px solid #f00',
          padding: '8px 12px',
          borderRadius: '4px',
          display: 'flex',
          alignItems: 'center',
          gap: '8px'
        }}>
          <span style={{ color: '#f00' }}>⚠</span>
          <span style={{ color: '#f00' }}>No options available</span>
        </div>
      </BaseField>
    )
  }

  return (
    <BaseField field={field}>
      <select 
        className="form-select" 
        value={value || ''}
        onChange={(e) => handleChange(e.target.value)}
        disabled={!field.isEditable}
      >
        <option value="">{placeholder || 'Select an option'}</option>
        {sortedOptions.map((option) => (
          <option key={option.id} value={option.optionValue}>
            {getOptionText(option)}
          </option>
        ))}
      </select>
    </BaseField>
  )
}

export default SelectField

