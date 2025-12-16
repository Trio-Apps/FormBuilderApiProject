import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface NumberFieldProps {
  field: FormField
}

const NumberField = ({ field }: NumberFieldProps) => {
  const [value, setValue] = useState('')

  return (
    <BaseField field={field}>
      <input
        type="number"
        className="form-input"
        placeholder={field.placeholder || ''}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        disabled={!field.isEditable}
        readOnly={!field.isEditable}
        min={field.minValue}
        max={field.maxValue}
      />
    </BaseField>
  )
}

export default NumberField

