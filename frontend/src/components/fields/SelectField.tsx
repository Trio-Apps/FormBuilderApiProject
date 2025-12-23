import { useState } from 'react'
import BaseField, { useMultilingualField } from './BaseField'
import { FormField } from '../../types/form'
import { useLanguage } from '../../contexts/LanguageContext'
import './fields.css'

interface SelectFieldProps {
  field: FormField
}

const SelectField = ({ field }: SelectFieldProps) => {
  const [value, setValue] = useState('')
  const { currentLanguage } = useLanguage()
  const { placeholder } = useMultilingualField(field)
  const sortedOptions = [...field.fieldOptions]
    .filter(opt => opt.isActive)
    .sort((a, b) => a.optionOrder - b.optionOrder)

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
        value={value}
        onChange={(e) => setValue(e.target.value)}
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

