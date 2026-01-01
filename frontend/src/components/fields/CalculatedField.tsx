import { useEffect } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface CalculatedFieldProps {
  field: FormField
  value?: any
  onChange?: (value: any) => void
}

const CalculatedField = ({ field, value, onChange }: CalculatedFieldProps) => {
  // Calculated fields are always read-only
  const displayValue = value !== null && value !== undefined ? value : ''

  return (
    <BaseField field={field}>
      <input
        type="text"
        className="form-input calculated-field"
        placeholder={field.placeholder || 'Calculated automatically'}
        value={displayValue}
        readOnly={true}
        disabled={true}
        style={{
          backgroundColor: '#f5f5f5',
          cursor: 'not-allowed',
          fontWeight: '500'
        }}
      />
      {field.hintText && (
        <div className="field-hint" style={{ fontStyle: 'italic', color: '#666' }}>
          {field.hintText}
        </div>
      )}
    </BaseField>
  )
}

export default CalculatedField

