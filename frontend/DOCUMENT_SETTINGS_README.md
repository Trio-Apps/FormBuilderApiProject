# Document Settings - Frontend Implementation

## الملفات المضافة/المعدلة

### 1. Types (الأنواع)
- **`src/types/documentSettings.ts`**: تعريفات TypeScript للـ Document Settings

### 2. Components (المكونات)
- **`src/components/admin/DocumentSettings.tsx`**: مكون إدارة Document Settings
- **`src/components/admin/DocumentSettings.css`**: تنسيقات CSS للمكون

### 3. Pages (الصفحات)
- **`src/pages/FormBuilderAdmin.tsx`**: صفحة إدارة Form Builder مع تبويبات
- **`src/pages/FormBuilderAdmin.css`**: تنسيقات CSS للصفحة

### 4. Services (الخدمات)
- **`src/services/api.ts`**: تم إضافة دوال API للـ Document Settings:
  - `getDocumentSettings(formBuilderId)`
  - `saveDocumentSettings(settings)`
  - `deleteDocumentSettings(formBuilderId)`
  - `getActiveProjects()`

### 5. Routing (التوجيه)
- **`src/App.tsx`**: تم إضافة route جديد:
  - `/admin/form-builder/:id` - صفحة إدارة Form Builder

## كيفية الاستخدام

### 1. الوصول إلى صفحة Document Settings

افتح المتصفح وانتقل إلى:
```
http://localhost:3000/admin/form-builder/{formBuilderId}
```

مثال:
```
http://localhost:3000/admin/form-builder/1
```

### 2. استخدام المكون مباشرة

يمكنك أيضاً استخدام المكون مباشرة في أي صفحة:

```tsx
import DocumentSettingsComponent from './components/admin/DocumentSettings'

function MyPage() {
  return (
    <DocumentSettingsComponent formBuilderId={1} />
  )
}
```

## المميزات

### Document Type Section
- Document Name (اسم المستند)
- Document Code (كود المستند)
- Menu Caption (عنوان القائمة)
- Menu Order (ترتيب القائمة)
- Parent Menu ID (معرف القائمة الأب - اختياري)
- Is Active (نشط/غير نشط)

### Document Series Section
- إدارة سلاسل متعددة
- لكل سلسلة:
  - Project (المشروع)
  - Series Code / Prefix (كود السلسلة)
  - Next Number (الرقم التالي)
  - Is Default (افتراضي)
  - Is Active (نشط)
- إضافة/حذف سلاسل
- التحقق من أن سلسلة واحدة فقط هي الافتراضية لكل مشروع

## API Endpoints المستخدمة

### GET `/api/FormBuilderDocumentSettings/form/{formBuilderId}`
جلب إعدادات Document Settings لنموذج معين

### POST `/api/FormBuilderDocumentSettings`
حفظ إعدادات Document Settings

### DELETE `/api/FormBuilderDocumentSettings/form/{formBuilderId}`
حذف إعدادات Document Settings

### GET `/api/Projects/active`
جلب قائمة المشاريع النشطة

## Authentication (المصادقة)

جميع الـ API calls تتطلب:
- Token في `localStorage` تحت مفتاح `token` أو `accessToken`
- Header: `Authorization: Bearer {token}`

## ملاحظات

1. المكون يدعم حالة "لا توجد إعدادات" ويعرض نموذج فارغ
2. التحقق من البيانات يتم على الـ Frontend قبل الإرسال
3. رسائل النجاح/الخطأ تظهر للمستخدم
4. المكون يعيد تحميل البيانات بعد الحفظ بنجاح

