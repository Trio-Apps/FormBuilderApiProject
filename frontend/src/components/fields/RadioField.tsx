import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface RadioFieldProps {
  field: FormField
}

const RadioField = ({ field }: RadioFieldProps) => {
  const [value, setValue] = useState('')
  const sortedOptions = [...field.fieldOptions]
    .filter(opt => opt.isActive)
    .sort((a, b) => a.optionOrder - b.optionOrder)

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
            <span>{option.optionText}</span>
          </label>
        ))}
      </div>
    </BaseField>
  )
}

export default RadioField

