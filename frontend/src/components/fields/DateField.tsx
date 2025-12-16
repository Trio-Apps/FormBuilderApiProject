import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface DateFieldProps {
  field: FormField
}

const DateField = ({ field }: DateFieldProps) => {
  const [value, setValue] = useState('')

  return (
    <BaseField field={field}>
      <input
        type="date"
        className="form-input"
        placeholder={field.placeholder || ''}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        disabled={!field.isEditable}
        readOnly={!field.isEditable}
      />
    </BaseField>
  )
}

export default DateField

