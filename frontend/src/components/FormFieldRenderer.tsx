import { FormField } from '../types/form'
import TextField from './fields/TextField'
import NumberField from './fields/NumberField'
import EmailField from './fields/EmailField'
import TextareaField from './fields/TextareaField'
import SelectField from './fields/SelectField'
import RadioField from './fields/RadioField'
import CheckboxField from './fields/CheckboxField'
import DateField from './fields/DateField'
import FileField from './fields/FileField'
import SwitchField from './fields/SwitchField'
import CalculatedField from './fields/CalculatedField'
import './FormFieldRenderer.css'

interface FormFieldRendererProps {
  field: FormField
  value?: any
  onChange?: (value: any) => void
}

const FormFieldRenderer = ({ field, value, onChange }: FormFieldRendererProps) => {
  const fieldTypeName = field.fieldTypeName || field.fieldType?.typeName || 'Text'

  // Normalize field type name to match our components
  const normalizedType = fieldTypeName.toLowerCase().trim()

  const renderField = () => {
    switch (normalizedType) {
      case 'text':
        return <TextField field={field} value={value} onChange={onChange} />
      case 'number':
        return <NumberField field={field} value={value} onChange={onChange} />
      case 'email':
        return <EmailField field={field} value={value} onChange={onChange} />
      case 'textarea':
        return <TextareaField field={field} value={value} onChange={onChange} />
      case 'dropdown':
      case 'select':
        return <SelectField field={field} value={value} onChange={onChange} />
      case 'radio':
        return <RadioField field={field} value={value} onChange={onChange} />
      case 'checkbox':
        return <CheckboxField field={field} value={value} onChange={onChange} />
      case 'date':
        return <DateField field={field} value={value} onChange={onChange} />
      case 'file':
      case 'file upload':
        return <FileField field={field} value={value} onChange={onChange} />
      case 'switch':
      case 'toggle':
        return <SwitchField field={field} value={value} onChange={onChange} />
      case 'calculated':
        return <CalculatedField field={field} value={value} onChange={onChange} />
      default:
        // Default to text field for unknown types
        return <TextField field={field} value={value} onChange={onChange} />
    }
  }

  return (
    <div className="form-field-wrapper">
      {renderField()}
    </div>
  )
}

export default FormFieldRenderer










