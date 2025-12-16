import { ReactNode } from 'react'
import { FormField } from '../../types/form'
import './BaseField.css'

interface BaseFieldProps {
  field: FormField
  children: ReactNode
}

const BaseField = ({ field, children }: BaseFieldProps) => {
  return (
    <div className="form-field">
      <label className="field-label">
        {field.fieldName}
        {field.isMandatory && <span className="required-indicator">*</span>}
      </label>
      {children}
    </div>
  )
}

export default BaseField







