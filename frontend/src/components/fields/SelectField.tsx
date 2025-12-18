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

