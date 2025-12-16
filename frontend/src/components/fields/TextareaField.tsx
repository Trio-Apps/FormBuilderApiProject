import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface TextareaFieldProps {
  field: FormField
}

const TextareaField = ({ field }: TextareaFieldProps) => {
  const [value, setValue] = useState('')

  return (
    <BaseField field={field}>
      <textarea
        className="form-textarea"
        placeholder={field.placeholder || ''}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        disabled={!field.isEditable}
        readOnly={!field.isEditable}
        maxLength={field.maxLength}
        rows={4}
      />
    </BaseField>
  )
}

export default TextareaField

