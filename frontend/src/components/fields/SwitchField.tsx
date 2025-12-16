import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface SwitchFieldProps {
  field: FormField
}

const SwitchField = ({ field }: SwitchFieldProps) => {
  const [checked, setChecked] = useState(false)

  return (
    <BaseField field={field}>
      <label className="switch-wrapper" style={{ cursor: field.isEditable ? 'pointer' : 'default' }}>
        <input
          type="checkbox"
          className="switch-input"
          checked={checked}
          onChange={(e) => setChecked(e.target.checked)}
          disabled={!field.isEditable}
        />
        <span className="switch-slider" style={{ opacity: field.isEditable ? 1 : 0.54 }}></span>
      </label>
    </BaseField>
  )
}

export default SwitchField

