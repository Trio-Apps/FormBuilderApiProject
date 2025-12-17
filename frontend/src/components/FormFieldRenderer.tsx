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
import './FormFieldRenderer.css'

interface FormFieldRendererProps {
  field: FormField
}

const FormFieldRenderer = ({ field }: FormFieldRendererProps) => {
  const fieldTypeName = field.fieldTypeName || field.fieldType?.typeName || 'Text'

  // Normalize field type name to match our components
  const normalizedType = fieldTypeName.toLowerCase().trim()

  const renderField = () => {
    switch (normalizedType) {
      case 'text':
        return <TextField field={field} />
      case 'number':
        return <NumberField field={field} />
      case 'email':
        return <EmailField field={field} />
      case 'textarea':
        return <TextareaField field={field} />
      case 'dropdown':
      case 'select':
        return <SelectField field={field} />
      case 'radio':
        return <RadioField field={field} />
      case 'checkbox':
        return <CheckboxField field={field} />
      case 'date':
        return <DateField field={field} />
      case 'file':
      case 'file upload':
        return <FileField field={field} />
      case 'switch':
      case 'toggle':
        return <SwitchField field={field} />
      default:
        // Default to text field for unknown types
        return <TextField field={field} />
    }
  }

  return (
    <div className="form-field-wrapper">
      {renderField()}
      {field.hintText && (
        <div className="field-hint">{field.hintText}</div>
      )}
    </div>
  )
}

export default FormFieldRenderer









