import { useState } from 'react'
import BaseField from './BaseField'
import { FormField } from '../../types/form'
import './fields.css'

interface FileFieldProps {
  field: FormField
}

const FileField = ({ field }: FileFieldProps) => {
  const [fileName, setFileName] = useState('')

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      setFileName(file.name)
    }
  }

  return (
    <BaseField field={field}>
      <input
        type="file"
        className="form-input"
        onChange={handleFileChange}
        disabled={!field.isEditable}
        style={{ paddingTop: '8px', paddingBottom: '8px' }}
      />
      {fileName && (
        <div className="file-field-note" style={{ marginTop: '8px' }}>
          Selected: {fileName}
        </div>
      )}
    </BaseField>
  )
}

export default FileField

