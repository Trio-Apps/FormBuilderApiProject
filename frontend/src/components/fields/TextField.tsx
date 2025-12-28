import { useState } from 'react'
import BaseField, { useMultilingualField } from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface TextFieldProps {
  field: FormField
  value?: any
  onChange?: (value: any) => void
}

const TextField = ({ field, value: controlledValue, onChange }: TextFieldProps) => {
  const [internalValue, setInternalValue] = useState('')
  const { placeholder } = useMultilingualField(field)

  // Use controlled value if provided, otherwise use internal state
  const value = controlledValue !== undefined ? controlledValue : internalValue

  const handleChange = (newValue: string) => {
    if (onChange) {
      onChange(newValue)
    } else {
      setInternalValue(newValue)
    }
  }

  return (
    <BaseField field={field}>
      <input
        type="text"
        className="form-input"
        placeholder={placeholder || ''}
        value={value || ''}
        onChange={(e) => handleChange(e.target.value)}
        maxLength={field.maxLength}
        disabled={!field.isEditable}
        readOnly={!field.isEditable}
      />
    </BaseField>
  )
}

export default TextField

