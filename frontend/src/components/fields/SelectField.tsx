import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface SelectFieldProps {
  field: FormField
}

const SelectField = ({ field }: SelectFieldProps) => {
  const [value, setValue] = useState('')
  const sortedOptions = [...field.fieldOptions]
    .filter(opt => opt.isActive)
    .sort((a, b) => a.optionOrder - b.optionOrder)

  return (
    <BaseField field={field}>
      <select 
        className="form-select" 
        value={value}
        onChange={(e) => setValue(e.target.value)}
        disabled={!field.isEditable}
      >
        <option value="">{field.placeholder || 'Select an option'}</option>
        {sortedOptions.map((option) => (
          <option key={option.id} value={option.optionValue}>
            {option.optionText}
          </option>
        ))}
      </select>
    </BaseField>
  )
}

export default SelectField

