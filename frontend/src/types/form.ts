export interface FieldOption {
  id: number;
  fieldId: number;
  optionText: string;
  optionValue: string;
  optionOrder: number;
  isDefault: boolean;
  isActive: boolean;
}

export interface FieldType {
  id: number;
  typeName: string;
  dataType: string;
  maxLength?: number;
  hasOptions: boolean;
  allowMultiple: boolean;
  isActive: boolean;
}

export interface FormField {
  id: number;
  tabId: number;
  fieldTypeId: number;
  fieldTypeName?: string;
  fieldName: string;
  fieldCode: string;
  fieldOrder: number;
  placeholder?: string;
  hintText?: string;
  isMandatory: boolean;
  isEditable: boolean;
  isVisible: boolean;
  defaultValueJson?: string;
  dataType?: string;
  maxLength?: number;
  minValue?: number;
  maxValue?: number;
  regexPattern?: string;
  validationMessage?: string;
  visibilityRuleJson?: string;
  readOnlyRuleJson?: string;
  createdDate: string;
  createdByUserId?: string;
  createdByUserName?: string;
  isActive: boolean;
  fieldType?: FieldType;
  fieldOptions: FieldOption[];
}

export interface FormTab {
  id: number;
  formBuilderId: number;
  tabName: string;
  tabCode: string;
  tabOrder: number;
  isActive: boolean;
  createdByUserId?: string;
  createdDate: string;
  fields: FormField[];
}

export interface FormBuilder {
  id: number;
  formName: string;
  formCode: string;
  description?: string;
  version: number;
  isPublished: boolean;
  isActive: boolean;
  createdByUserId?: string;
  createdDate: string;
  updatedDate?: string;
  tabs: FormTab[];
}

export interface UpdateFormBuilderDto {
  formName: string;
  formCode: string;
  description?: string;
  isPublished?: boolean;
  isActive?: boolean;
}

export interface ServiceResult<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
  statusCode?: number;
}







