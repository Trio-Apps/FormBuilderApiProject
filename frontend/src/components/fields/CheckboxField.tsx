import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface CheckboxFieldProps {
  field: FormField
}

const CheckboxField = ({ field }: CheckboxFieldProps) => {
  const [selectedValues, setSelectedValues] = useState<string[]>([])
  const sortedOptions = [...field.fieldOptions]
    .filter(opt => opt.isActive)
    .sort((a, b) => a.optionOrder - b.optionOrder)

  const handleCheckboxChange = (optionValue: string, checked: boolean) => {
    if (checked) {
      setSelectedValues([...selectedValues, optionValue])
    } else {
      setSelectedValues(selectedValues.filter(v => v !== optionValue))
    }
  }

  // If there are options, render as multiple checkboxes
  if (sortedOptions.length > 0) {
    return (
      <BaseField field={field}>
        <div className="checkbox-group">
          {sortedOptions.map((option) => (
            <label key={option.id} className="checkbox-option">
              <input
                type="checkbox"
                name={`field-${field.id}`}
                value={option.optionValue}
                checked={selectedValues.includes(option.optionValue)}
                onChange={(e) => handleCheckboxChange(option.optionValue, e.target.checked)}
                disabled={!field.isEditable}
              />
              <span>{option.optionText}</span>
            </label>
          ))}
        </div>
      </BaseField>
    )
  }

  // Single checkbox (boolean field)
  const [checked, setChecked] = useState(false)
  return (
    <BaseField field={field}>
      <label className="checkbox-option">
        <input
          type="checkbox"
          name={`field-${field.id}`}
          checked={checked}
          onChange={(e) => setChecked(e.target.checked)}
          disabled={!field.isEditable}
        />
        <span>{field.placeholder || field.fieldName}</span>
      </label>
    </BaseField>
  )
}

export default CheckboxField

