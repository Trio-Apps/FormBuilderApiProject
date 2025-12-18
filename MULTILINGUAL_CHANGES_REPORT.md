# تقرير التعديلات - دعم المحتوى ثنائي اللغة (Arabic/English)
## Multilingual Content Support Changes Report

---

## 📋 جدول المحتويات
1. [نظرة عامة](#نظرة-عامة)
2. [التعديلات على Database](#التعديلات-على-database)
3. [التعديلات على Entities](#التعديلات-على-entities)
4. [التعديلات على DTOs](#التعديلات-على-dtos)
5. [التعديلات على Services](#التعديلات-على-services)
6. [التعديلات على Controllers](#التعديلات-على-controllers)
7. [التعديلات على Validators](#التعديلات-على-validators)
8. [Migration](#migration)
9. [API Endpoints](#api-endpoints)
10. [أمثلة JSON Response](#أمثلة-json-response)
11. [خطوات التطبيق](#خطوات-التطبيق)

---

## 🎯 نظرة عامة

تم إضافة دعم كامل للمحتوى ثنائي اللغة (عربي/إنجليزي) لجميع البيانات التي يدخلها المستخدم في نظام Form Builder. التعديلات تشمل:

- ✅ **FORM_BUILDER**: اسم النموذج ووصفه
- ✅ **FORM_TABS**: اسم التبويب
- ✅ **FORM_FIELDS**: اسم الحقل، placeholder، hint text، validation message
- ✅ **FIELD_OPTIONS**: نص الخيار
- ✅ **FIELD_TYPES**: اسم النوع

---

## 🗄️ التعديلات على Database

### Migration File
**الملف**: `FormBuilder.Core/Migrations/20251218130256_AddMultilingualFields.cs`

### الجداول والحقول المضافة:

#### 1. FORM_BUILDER
```sql
ALTER TABLE FORM_BUILDER ADD ForeignFormName NVARCHAR(200) NULL;
ALTER TABLE FORM_BUILDER ADD ForeignDescription NVARCHAR(MAX) NULL;
```

#### 2. FORM_TABS
```sql
ALTER TABLE FORM_TABS ADD ForeignTabName NVARCHAR(200) NULL;
```

#### 3. FORM_FIELDS
```sql
ALTER TABLE FORM_FIELDS ADD ForeignFieldName NVARCHAR(200) NULL;
ALTER TABLE FORM_FIELDS ADD ForeignPlaceholder NVARCHAR(MAX) NULL;
ALTER TABLE FORM_FIELDS ADD ForeignHintText NVARCHAR(MAX) NULL;
ALTER TABLE FORM_FIELDS ADD ForeignValidationMessage NVARCHAR(MAX) NULL;
```

#### 4. FIELD_OPTIONS
```sql
ALTER TABLE FIELD_OPTIONS ADD ForeignOptionText NVARCHAR(200) NULL;
```

#### 5. FIELD_TYPES
```sql
ALTER TABLE FIELD_TYPES ADD ForeignTypeName NVARCHAR(100) NULL;
```

---

## 📦 التعديلات على Entities

### 1. FORM_BUILDER (`formBuilder.Domian/Entitys/FormBuilder/FormBuilder.cs`)
```csharp
[StringLength(200)]
public string? ForeignFormName { get; set; }

public string? ForeignDescription { get; set; }
```

### 2. FORM_TABS (`formBuilder.Domian/Entitys/FormBuilder/FormTab.cs`)
```csharp
[StringLength(200)]
public string? ForeignTabName { get; set; }
```

### 3. FORM_FIELDS (`formBuilder.Domian/Entitys/FormBuilder/FormField.cs`)
```csharp
[StringLength(200)]
public string? ForeignFieldName { get; set; }

public string? ForeignPlaceholder { get; set; }

public string? ForeignHintText { get; set; }

public string? ForeignValidationMessage { get; set; }
```

### 4. FIELD_OPTIONS (`formBuilder.Domian/Entitys/FormBuilder/FIELD_OPTIONS.cs`)
```csharp
[StringLength(200)]
public string? ForeignOptionText { get; set; }
```

### 5. FIELD_TYPES (`formBuilder.Domian/Entitys/FormBuilder/FieldType.cs`)
```csharp
[StringLength(100)]
public string? ForeignTypeName { get; set; }
```

---

## 📝 التعديلات على DTOs

### 1. FormBuilderDto (`FormBuilder.Core/DTOS/FormBuilder/FormBuilderDto.cs`)
```csharp
public string FormName { get; set; }
public string? ForeignFormName { get; set; }  // ✅ جديد
public string Description { get; set; }
public string? ForeignDescription { get; set; }  // ✅ جديد
```

### 2. CreateFormBuilderDto & UpdateFormBuilderDto
```csharp
[StringLength(200)]
public string? ForeignFormName { get; set; }  // ✅ جديد

public string? ForeignDescription { get; set; }  // ✅ جديد
```

### 3. FormTabDto (`FormBuilder.Core/DTOS/FormBuilder/FormTabDto.cs`)
```csharp
public string TabName { get; set; }
public string? ForeignTabName { get; set; }  // ✅ جديد

// Computed properties (للتوافق مع متطلبات Angular)
public string name_en => TabName;
public string? name_ar => ForeignTabName;
public int order => TabOrder;
public bool is_active => IsActive;
```

### 4. CreateFormTabDto & UpdateFormTabDto
```csharp
[StringLength(100)]
public string? ForeignTabName { get; set; }  // ✅ جديد
```

### 5. FormFieldDto (`FormBuilder.Core/DTOS/FormBuilder/FormFieldDto.cs`)
```csharp
public string FieldName { get; set; } = string.Empty;
public string? ForeignFieldName { get; set; }  // ✅ جديد
public string? Placeholder { get; set; }
public string? ForeignPlaceholder { get; set; }  // ✅ جديد
public string HintText { get; set; }
public string? ForeignHintText { get; set; }  // ✅ جديد
public string? ValidationMessage { get; set; }
public string? ForeignValidationMessage { get; set; }  // ✅ جديد

// Computed properties (للتوافق مع متطلبات Angular)
public string label_en => FieldName;
public string? label_ar => ForeignFieldName;
public string? placeholder_en => Placeholder;
public string? placeholder_ar => ForeignPlaceholder;
public string? type => FieldTypeName;
public bool is_required => IsMandatory ?? false;
public int tab_id => TabId;
```

### 6. CreateFormFieldDto & UpdateFormFieldDto
```csharp
[StringLength(200)]
public string? ForeignFieldName { get; set; }  // ✅ جديد

[StringLength(200)]
public string? ForeignPlaceholder { get; set; }  // ✅ جديد

[StringLength(500)]
public string? ForeignHintText { get; set; }  // ✅ جديد

[StringLength(500)]
public string? ForeignValidationMessage { get; set; }  // ✅ جديد
```

### 7. FieldOptionDto (`FormBuilder.Core/DTOS/FormBuilder/FieldOptionDto.cs`)
```csharp
public string OptionText { get; set; } = string.Empty;
public string? ForeignOptionText { get; set; }  // ✅ جديد
```

### 8. CreateFieldOptionDto & UpdateFieldOptionDto
```csharp
[StringLength(200)]
public string? ForeignOptionText { get; set; }  // ✅ جديد
```

### 9. FieldTypeDto (`FormBuilder.Core/DTOS/FormBuilder/FieldTypeDto.cs`)
```csharp
public string TypeName { get; set; }
public string? ForeignTypeName { get; set; }  // ✅ جديد

// Computed properties
public string type_name_en => TypeName;
public string? type_name_ar => ForeignTypeName;
```

### 10. FieldTypeCreateDto & FieldTypeUpdateDto
```csharp
[StringLength(100)]
public string? ForeignTypeName { get; set; }  // ✅ جديد
```

---

## ⚙️ التعديلات على Services

### 1. FormBuilderService (`FormBuilder.Services/Services/FormBuilder/FormBuilderService.cs`)

#### التعديل في `GetByCodeAsync()`:
تم تحديث الـ manual mapping ليشمل جميع الحقول ثنائية اللغة:

```csharp
dto.Tabs = entity.FORM_TABS
    .Where(t => t.IsActive)
    .OrderBy(t => t.TabOrder)
    .Select(t => new FormTabDto
    {
        // ... existing fields ...
        ForeignTabName = t.ForeignTabName,  // ✅ جديد
        Fields = t.FORM_FIELDS
            .Where(f => f.IsActive)
            .OrderBy(f => f.FieldOrder)
            .Select(f => new FormFieldDto
            {
                // ... existing fields ...
                ForeignFieldName = f.ForeignFieldName,  // ✅ جديد
                ForeignPlaceholder = f.ForeignPlaceholder,  // ✅ جديد
                ForeignHintText = f.ForeignHintText,  // ✅ جديد
                ForeignValidationMessage = f.ForeignValidationMessage,  // ✅ جديد
                FieldType = f.FIELD_TYPES != null ? new FieldTypeDto
                {
                    // ... existing fields ...
                    ForeignTypeName = f.FIELD_TYPES.ForeignTypeName,  // ✅ جديد
                } : null,
                FieldOptions = f.FIELD_OPTIONS?
                    .Where(fo => fo.IsActive)
                    .Select(fo => new FieldOptionDto
                    {
                        // ... existing fields ...
                        ForeignOptionText = fo.ForeignOptionText,  // ✅ جديد
                    }).ToList() ?? new List<FieldOptionDto>()
            }).ToList()
    })
```

#### إصلاح خطأ syntax:
```csharp
// قبل
if (string.IsNullOrWhiteSpace(formCode))
    var message = ...

// بعد
if (string.IsNullOrWhiteSpace(formCode))
{  // ✅ إضافة curly braces
    var message = ...
}
```

### 2. باقي Services
- ✅ **FormTabService**: يستخدم AutoMapper (لا يحتاج تعديل)
- ✅ **FormFieldService**: يستخدم AutoMapper (لا يحتاج تعديل)
- ✅ **FieldTypesService**: يستخدم AutoMapper (لا يحتاج تعديل)
- ✅ **FieldOptionsService**: يستخدم AutoMapper (لا يحتاج تعديل)

---

## 🎮 التعديلات على Controllers

### 1. FormBuilderController (`frombuilderApiProject/Controllers/FormBuilder/FormBuilderController.cs`)
- ✅ جاهز - لا يحتاج تعديلات
- ✅ يدعم `ForeignFormName` و `ForeignDescription` في Create/Update

### 2. FormTabsController (`frombuilderApiProject/Controllers/FormBuilder/FormTabsController.cs`)
- ✅ جاهز - لا يحتاج تعديلات
- ✅ يدعم `ForeignTabName` في Create/Update

### 3. FormFieldsController (`frombuilderApiProject/Controllers/FormBuilder/FormFieldsController.cs`)
- ✅ جاهز - لا يحتاج تعديلات
- ✅ يدعم جميع الحقول ثنائية اللغة في Create/Update

### 4. FieldOptionsController (`frombuilderApiProject/Controllers/FormBuilder/FieldOptionsController.cs`)
- ✅ جاهز - لا يحتاج تعديلات
- ✅ يدعم `ForeignOptionText` في Create/Update

### 5. FieldTypesController (`frombuilderApiProject/Controllers/FormBuilder/FieldTypesController.cs`)
- ✅ تم إضافة `ModelState.IsValid` validation في Create/Update
- ✅ يدعم `ForeignTypeName` في Create/Update

### 6. UserPermissionController (`frombuilderApiProject/Controllers/Auth/UserPermissionController.cs`)
- ✅ تم إصلاح خطأ: تغيير `IStringLocalizer<Shared>` إلى `IStringLocalizer<UserPermissionController>`

---

## ✅ التعديلات على Validators

### 1. CreateFormBuilderDtoValidator (`FormBuilder.Services/Validators/FormBuilder/CreateFormBuilderDtoValidator.cs`)
```csharp
RuleFor(x => x.ForeignFormName)
    .MaximumLength(200)
    .When(x => !string.IsNullOrWhiteSpace(x.ForeignFormName));  // ✅ جديد

RuleFor(x => x.ForeignDescription)
    .MaximumLength(1000)
    .When(x => !string.IsNullOrWhiteSpace(x.ForeignDescription));  // ✅ جديد
```

### 2. UpdateFormBuilderDtoValidator (`FormBuilder.Services/Validators/FormBuilder/UpdateFormBuilderDtoValidator.cs`)
```csharp
RuleFor(x => x.ForeignFormName)
    .MaximumLength(200)
    .When(x => !string.IsNullOrWhiteSpace(x.ForeignFormName));  // ✅ جديد

RuleFor(x => x.ForeignDescription)
    .MaximumLength(1000)
    .When(x => !string.IsNullOrWhiteSpace(x.ForeignDescription));  // ✅ جديد
```

---

## 🔄 Migration

### اسم الملف
`FormBuilder.Core/Migrations/20251218130256_AddMultilingualFields.cs`

### تشغيل Migration
```bash
dotnet ef database update --context FormBuilderDbContext --startup-project frombuilderApiProject
```

---

## 🌐 API Endpoints

جميع الـ endpoints التالية تدعم الآن الحقول ثنائية اللغة:

### Form Builder
- `GET /api/FormBuilder` - جلب جميع النماذج
- `GET /api/FormBuilder/{id}` - جلب نموذج محدد
- `GET /api/FormBuilder/code/{formCode}` - جلب نموذج بالكود (للعامة)
- `POST /api/FormBuilder` - إنشاء نموذج جديد
- `PUT /api/FormBuilder/{id}` - تحديث نموذج

### Form Tabs
- `GET /api/FormTabs` - جلب جميع التبويبات
- `GET /api/FormTabs/{id}` - جلب تبويب محدد
- `GET /api/FormTabs/form/{formBuilderId}` - جلب تبويبات نموذج
- `POST /api/FormTabs` - إنشاء تبويب جديد
- `PUT /api/FormTabs/{id}` - تحديث تبويب

### Form Fields
- `GET /api/FormFields` - جلب جميع الحقول
- `GET /api/FormFields/{id}` - جلب حقل محدد
- `GET /api/FormFields/tab/{tabId}` - جلب حقول تبويب
- `POST /api/FormFields` - إنشاء حقل جديد
- `PUT /api/FormFields/{id}` - تحديث حقل

### Field Options
- `GET /api/FieldOptions` - جلب جميع الخيارات
- `GET /api/FieldOptions/field/{fieldId}` - جلب خيارات حقل
- `POST /api/FieldOptions` - إنشاء خيار جديد
- `PUT /api/FieldOptions/{id}` - تحديث خيار

### Field Types
- `GET /api/FieldTypes` - جلب جميع الأنواع
- `GET /api/FieldTypes/{id}` - جلب نوع محدد
- `POST /api/FieldTypes` - إنشاء نوع جديد
- `PUT /api/FieldTypes/{id}` - تحديث نوع

---

## 📄 أمثلة JSON Response

### مثال 1: FormBuilderDto
```json
{
  "id": 1,
  "formName": "Employee Registration",
  "foreignFormName": "تسجيل الموظف",
  "formCode": "EMP_REG",
  "description": "Form for employee registration",
  "foreignDescription": "نموذج لتسجيل الموظفين",
  "version": 1,
  "isPublished": true,
  "isActive": true,
  "tabs": [
    {
      "id": 1,
      "formBuilderId": 1,
      "tabName": "Personal Information",
      "foreignTabName": "المعلومات الشخصية",
      "tabCode": "PERSONAL",
      "tabOrder": 1,
      "isActive": true,
      "name_en": "Personal Information",
      "name_ar": "المعلومات الشخصية",
      "order": 1,
      "is_active": true,
      "fields": [
        {
          "id": 1,
          "tabId": 1,
          "fieldTypeId": 1,
          "fieldTypeName": "Text",
          "fieldName": "First Name",
          "foreignFieldName": "الاسم الأول",
          "fieldCode": "FIRST_NAME",
          "fieldOrder": 1,
          "placeholder": "Enter first name",
          "foreignPlaceholder": "أدخل الاسم الأول",
          "hintText": "Your legal first name",
          "foreignHintText": "اسمك الأول القانوني",
          "isMandatory": true,
          "isEditable": true,
          "isVisible": true,
          "label_en": "First Name",
          "label_ar": "الاسم الأول",
          "placeholder_en": "Enter first name",
          "placeholder_ar": "أدخل الاسم الأول",
          "type": "Text",
          "is_required": true,
          "tab_id": 1,
          "fieldOptions": []
        }
      ]
    }
  ]
}
```

### مثال 2: CreateFormTabDto Request
```json
{
  "formBuilderId": 1,
  "tabName": "Contact Information",
  "foreignTabName": "معلومات الاتصال",
  "tabCode": "CONTACT",
  "tabOrder": 2,
  "isActive": true
}
```

### مثال 3: CreateFormFieldDto Request
```json
{
  "tabId": 1,
  "fieldTypeId": 2,
  "fieldName": "Email Address",
  "foreignFieldName": "عنوان البريد الإلكتروني",
  "fieldCode": "EMAIL",
  "fieldOrder": 1,
  "placeholder": "example@email.com",
  "foreignPlaceholder": "مثال@البريد.com",
  "hintText": "Enter a valid email address",
  "foreignHintText": "أدخل عنوان بريد إلكتروني صحيح",
  "validationMessage": "Invalid email format",
  "foreignValidationMessage": "تنسيق البريد الإلكتروني غير صحيح",
  "isMandatory": true,
  "isEditable": true,
  "isVisible": true
}
```

### مثال 4: FieldOptionDto
```json
{
  "id": 1,
  "fieldId": 5,
  "optionText": "Yes",
  "foreignOptionText": "نعم",
  "optionValue": "true",
  "optionOrder": 1,
  "isDefault": false,
  "isActive": true
}
```

### مثال 5: FieldTypeDto
```json
{
  "id": 1,
  "typeName": "Text",
  "foreignTypeName": "نص",
  "dataType": "string",
  "maxLength": 255,
  "hasOptions": false,
  "allowMultiple": false,
  "isActive": true,
  "type_name_en": "Text",
  "type_name_ar": "نص"
}
```

---

## 🚀 خطوات التطبيق

### 1. تشغيل Migration
```bash
cd D:\FormBuilderApiProject\frombuilderApiProject
dotnet ef database update --context FormBuilderDbContext --startup-project .
```

### 2. التحقق من Build
```bash
dotnet build
```

### 3. تشغيل المشروع
```bash
dotnet run
```

---

## 📌 ملاحظات مهمة للـ Frontend (Angular)

### 1. Naming Convention
الـ API يعيد البيانات بنمطين:

#### النمط الأول: Foreign* (الأصلي)
```typescript
{
  tabName: "Personal Info",
  foreignTabName: "المعلومات الشخصية"
}
```

#### النمط الثاني: Computed Properties (للتوافق)
```typescript
{
  name_en: "Personal Info",
  name_ar: "المعلومات الشخصية"
}
```

**الـ Frontend يمكنه استخدام أي من النمطين**، لكن يُنصح باستخدام النمط الثاني (`name_ar`/`name_en`) لأنه متوافق مع متطلبات المهمة.

### 2. الحقول المتاحة في FormTabDto
```typescript
interface FormTabDto {
  // الحقول الأساسية
  tabName: string;
  foreignTabName?: string;
  
  // Computed properties (موصى بها)
  name_en: string;
  name_ar?: string;
  order: number;
  is_active: boolean;
}
```

### 3. الحقول المتاحة في FormFieldDto
```typescript
interface FormFieldDto {
  // الحقول الأساسية
  fieldName: string;
  foreignFieldName?: string;
  placeholder?: string;
  foreignPlaceholder?: string;
  
  // Computed properties (موصى بها)
  label_en: string;
  label_ar?: string;
  placeholder_en?: string;
  placeholder_ar?: string;
  type?: string;
  is_required: boolean;
  tab_id: number;
}
```

### 4. FieldTypeDto
```typescript
interface FieldTypeDto {
  typeName: string;
  foreignTypeName?: string;
  
  // Computed properties
  type_name_en: string;
  type_name_ar?: string;
}
```

### 5. FieldOptionDto
```typescript
interface FieldOptionDto {
  optionText: string;
  foreignOptionText?: string;
}
```

---

## ✅ Checklist للـ Frontend

- [ ] تحديث TypeScript interfaces لتشمل الحقول الجديدة
- [ ] تحديث LanguageContext لدعم اللغة العربية/الإنجليزية
- [ ] تحديث FormViewer لعرض النصوص حسب اللغة المختارة
- [ ] تحديث TabNavigation لعرض أسماء التبويبات حسب اللغة
- [ ] تحديث BaseField لعرض labels و placeholders حسب اللغة
- [ ] تحديث SelectField و RadioField لعرض الخيارات حسب اللغة
- [ ] تحديث Admin Panel لإدخال البيانات ثنائية اللغة
- [ ] اختبار تبديل اللغة بدون reload للبيانات

---

## 📞 ملاحظات إضافية

1. **جميع الحقول ثنائية اللغة اختيارية (nullable)** - يمكن إنشاء سجلات بدونها
2. **JSON Serialization** يستخدم CamelCase تلقائياً
3. **Computed Properties** تظهر في JSON response تلقائياً
4. **لا حاجة لإعادة تحميل البيانات** عند تبديل اللغة - فقط تغيير العرض

---

**تاريخ التقرير**: 2025-01-18  
**الإصدار**: 1.0  
**الحالة**: ✅ مكتمل وجاهز للاستخدام
