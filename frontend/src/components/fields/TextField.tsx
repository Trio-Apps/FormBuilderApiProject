import { useState } from 'react'
import BaseField, { useMultilingualField } from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface TextFieldProps {
  field: FormField
}

const TextField = ({ field }: TextFieldProps) => {
  const [value, setValue] = useState('')
  const { placeholder } = useMultilingualField(field)

  return (
    <BaseField field={field}>
      <input
        type="text"
        className="form-input"
        placeholder={placeholder || ''}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        maxLength={field.maxLength}
        disabled={!field.isEditable}
        readOnly={!field.isEditable}
      />
    </BaseField>
  )
}

export default TextField

