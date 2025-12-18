import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import { useLanguage } from '../../contexts/LanguageContext'
import './fields.css'

interface RadioFieldProps {
  field: FormField
}

const RadioField = ({ field }: RadioFieldProps) => {
  const [value, setValue] = useState('')
  const { currentLanguage } = useLanguage()
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
      <div className="radio-group">
        {sortedOptions.map((option) => (
          <label key={option.id} className="radio-option">
            <input
              type="radio"
              name={`field-${field.id}`}
              value={option.optionValue}
              checked={value === option.optionValue}
              onChange={(e) => setValue(e.target.value)}
              disabled={!field.isEditable}
            />
            <span>{getOptionText(option)}</span>
          </label>
        ))}
      </div>
    </BaseField>
  )
}

export default RadioField

