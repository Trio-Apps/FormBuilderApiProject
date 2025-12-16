import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface EmailFieldProps {
  field: FormField
}

const EmailField = ({ field }: EmailFieldProps) => {
  const [value, setValue] = useState('')

  return (
    <BaseField field={field}>
      <input
        type="email"
        className="form-input"
        placeholder={field.placeholder || ''}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        disabled={!field.isEditable}
        readOnly={!field.isEditable}
        maxLength={field.maxLength}
      />
    </BaseField>
  )
}

export default EmailField

