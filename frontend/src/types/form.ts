export interface FieldOption {
  id: number;
  fieldId: number;
  optionText: string;
  foreignOptionText?: string;
  optionValue: string;
  optionOrder: number;
  isDefault: boolean;
  isActive: boolean;
}

export interface FieldType {
  id: number;
  typeName: string;
  foreignTypeName?: string;
  dataType: string;
  maxLength?: number;
  hasOptions: boolean;
  allowMultiple: boolean;
  isActive: boolean;
  // Computed properties for task requirements
  type_name_en?: string;
  type_name_ar?: string;
}

export interface FormField {
  id: number;
  tabId: number;
  fieldTypeId: number;
  fieldTypeName?: string;
  fieldName: string;
  foreignFieldName?: string;
  fieldCode: string;
  fieldOrder: number;
  placeholder?: string;
  foreignPlaceholder?: string;
  hintText?: string;
  foreignHintText?: string;
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
  foreignValidationMessage?: string;
  visibilityRuleJson?: string;
  readOnlyRuleJson?: string;
  createdDate: string;
  createdByUserId?: string;
  createdByUserName?: string;
  isActive: boolean;
  fieldType?: FieldType;
  fieldOptions: FieldOption[];
  // Computed properties for task requirements
  label_en?: string;
  label_ar?: string;
  placeholder_en?: string;
  placeholder_ar?: string;
  type?: string;
  is_required?: boolean;
  tab_id?: number;
}

export interface FormTab {
  id: number;
  formBuilderId: number;
  tabName: string;
  foreignTabName?: string;
  tabCode: string;
  tabOrder: number;
  isActive: boolean;
  createdByUserId?: string;
  createdDate: string;
  fields: FormField[];
  // Computed properties for task requirements
  name_en?: string;
  name_ar?: string;
  order?: number;
  is_active?: boolean;
}

export interface FormBuilder {
  id: number;
  formName: string;
  foreignFormName?: string;
  formCode: string;
  description?: string;
  foreignDescription?: string;
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
  foreignFormName?: string;
  formCode: string;
  description?: string;
  foreignDescription?: string;
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







