# ترجمة المصطلحات الأساسية - Terms Localization

## المصطلحات المترجمة

تم إضافة ترجمات للمصطلحات الأساسية التالية:

### 1. TABS (التبويبات)
- **English**: `Term_Tabs` = "Tabs"
- **Arabic**: `Term_Tabs` = "التبويبات"

### 2. FIELD / FIELDS (حقل / الحقول)
- **English**: 
  - `Term_Field` = "Field"
  - `Term_Fields` = "Fields"
- **Arabic**: 
  - `Term_Field` = "حقل"
  - `Term_Fields` = "الحقول"

### 3. FORMBUILDER (منشئ النماذج)
- **English**: 
  - `Term_FormBuilder` = "Form Builder"
  - `Term_FormBuilders` = "Form Builders"
- **Arabic**: 
  - `Term_FormBuilder` = "منشئ النماذج"
  - `Term_FormBuilders` = "منشئو النماذج"

### 4. FIELD TYPE (نوع الحقل)
- **English**: 
  - `Term_FieldType` = "Field Type"
  - `Term_FieldTypes` = "Field Types"
- **Arabic**: 
  - `Term_FieldType` = "نوع الحقل"
  - `Term_FieldTypes` = "أنواع الحقول"

### 5. OPTION / OPTIONS (خيار / الخيارات)
- **English**: 
  - `Term_Option` = "Option"
  - `Term_Options` = "Options"
- **Arabic**: 
  - `Term_Option` = "خيار"
  - `Term_Options` = "الخيارات"

---

## الملفات المُحدثة

### في مشروع API (`frombuilderApiProject/Resources/`):
- ✅ `Shared.en.resx` - تم إضافة 8 مفاتيح جديدة
- ✅ `Shared.ar.resx` - تم إضافة 8 مفاتيح جديدة

### في مشروع Services (`FormBuilder.Services/Resources/`):
- ✅ `Shared.en.resx` - تم إضافة 8 مفاتيح جديدة
- ✅ `Shared.ar.resx` - تم إضافة 8 مفاتيح جديدة

---

## كيفية الاستخدام

يمكن استخدام هذه المصطلحات في أي Controller أو Service:

```csharp
// في Controller
private readonly IStringLocalizer<Shared> _localizer;

public MyController(IStringLocalizer<Shared> localizer)
{
    _localizer = localizer;
}

// استخدام
var tabsLabel = _localizer["Term_Tabs"]; // "Tabs" أو "التبويبات"
var fieldsLabel = _localizer["Term_Fields"]; // "Fields" أو "الحقول"
var formBuilderLabel = _localizer["Term_FormBuilder"]; // "Form Builder" أو "منشئ النماذج"
var fieldTypeLabel = _localizer["Term_FieldType"]; // "Field Type" أو "نوع الحقل"
var optionsLabel = _localizer["Term_Options"]; // "Options" أو "الخيارات"
```

---

## الخلاصة

تم إضافة ترجمات لجميع المصطلحات الأساسية المطلوبة:
- ✅ TABS
- ✅ FIELD / FIELDS
- ✅ FORMBUILDER / FORMBUILDERS
- ✅ FIELD TYPE / FIELD TYPES
- ✅ OPTION / OPTIONS

جميع المصطلحات متاحة الآن في ملفات `Shared.en.resx` و `Shared.ar.resx` في كلا المشروعين! 🎉
