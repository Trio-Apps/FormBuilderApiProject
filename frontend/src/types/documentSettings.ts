export interface DocumentSettings {
  formBuilderId: number
  formBuilderName: string
  documentTypeId?: number
  documentName: string
  documentCode: string
  menuCaption: string
  menuOrder: number
  parentMenuId?: number
  isActive: boolean
  documentSeries: DocumentSeries[]
}

export interface DocumentSeries {
  id?: number
  documentTypeId: number
  documentTypeName?: string
  projectId: number
  projectName?: string
  seriesCode: string
  nextNumber: number
  isDefault: boolean
  isActive: boolean
}

export interface SaveDocumentSettings {
  formBuilderId: number
  documentName: string
  documentCode: string
  menuCaption: string
  menuOrder: number
  parentMenuId?: number
  isActive: boolean
  documentSeries: SaveDocumentSeries[]
}

export interface SaveDocumentSeries {
  id?: number
  projectId: number
  seriesCode: string
  nextNumber: number
  isDefault: boolean
  isActive: boolean
}

export interface Project {
  id: number
  name: string
  code: string
  description?: string
  isActive: boolean
}

