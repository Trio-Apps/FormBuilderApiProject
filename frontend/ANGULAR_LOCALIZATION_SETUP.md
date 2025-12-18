# إعداد الترجمة في React (Angular-like) - Localization Setup

## ✅ ما تم إنجازه

تم إعداد نظام الترجمة الكامل في React Frontend:

---

## 1. المكتبات المضافة

تم إضافة المكتبات التالية في `package.json`:
- `i18next`: ^23.7.6
- `react-i18next`: ^14.0.0
- `i18next-browser-languagedetector`: ^7.2.0

**ملاحظة**: تحتاج إلى تشغيل `npm install` لتثبيت المكتبات.

---

## 2. ملفات الترجمة

تم إنشاء ملفات الترجمة في `src/locales/`:

### `en.json` (الإنجليزية)
- مصطلحات أساسية (Tabs, Fields, Form Builder, Field Type, Options)
- رسائل الأخطاء
- رسائل النماذج
- أنواع الحقول

### `ar.json` (العربية)
- نفس البنية مع الترجمات العربية

---

## 3. إعداد i18n

تم إنشاء `src/i18n/config.ts`:
- إعداد i18next
- اكتشاف اللغة تلقائياً من localStorage/navigator
- دعم العربية والإنجليزية
- تغيير اتجاه النص تلقائياً (RTL/LTR)

---

## 4. Language Context

تم إنشاء `src/contexts/LanguageContext.tsx`:
- `LanguageProvider`: Provider للغة
- `useLanguage`: Hook للوصول للغة
- تغيير اتجاه النص تلقائياً
- حفظ اللغة في localStorage

---

## 5. Language Switcher Component

تم إنشاء `src/components/LanguageSwitcher.tsx`:
- زر للتبديل بين الإنجليزية والعربية
- تصميم responsive
- دعم RTL

---

## 6. تحديث API Service

تم تحديث `src/services/api.ts`:
- إضافة `Accept-Language` header تلقائياً
- استخدام اللغة الحالية من localStorage

---

## 7. تحديث Components

تم تحديث:
- `main.tsx`: إضافة LanguageProvider
- `FormViewer.tsx`: استخدام الترجمة وإضافة LanguageSwitcher

---

## كيفية الاستخدام

### 1. تثبيت المكتبات

```bash
cd frontend
npm install
```

### 2. استخدام الترجمة في Component

```tsx
import { useLanguage } from '../contexts/LanguageContext'

const MyComponent = () => {
  const { t, currentLanguage, changeLanguage, isRTL } = useLanguage()

  return (
    <div>
      <h1>{t('common.formBuilder')}</h1>
      <p>{t('common.fields')}</p>
      <button onClick={() => changeLanguage('ar')}>العربية</button>
      <button onClick={() => changeLanguage('en')}>English</button>
    </div>
  )
}
```

### 3. استخدام مع Interpolation

```tsx
const { t } = useLanguage()

// في ملف الترجمة: "minValue": "الحد الأدنى للقيمة هو {{min}}"
<p>{t('errors.minValue', { min: 10 })}</p>
```

### 4. إضافة Language Switcher

```tsx
import LanguageSwitcher from '../components/LanguageSwitcher'

<LanguageSwitcher />
```

---

## المفاتيح المتاحة في الترجمة

### Common (مشترك)
- `common.tabs` - "Tabs" / "التبويبات"
- `common.field` - "Field" / "حقل"
- `common.fields` - "Fields" / "الحقول"
- `common.formBuilder` - "Form Builder" / "منشئ النماذج"
- `common.fieldType` - "Field Type" / "نوع الحقل"
- `common.options` - "Options" / "الخيارات"
- `common.submit` - "Submit" / "إرسال"
- `common.loading` - "Loading..." / "جاري التحميل..."

### Errors (أخطاء)
- `errors.formNotFound` - "Form not found" / "النموذج غير موجود"
- `errors.failedToFetchForm` - "Failed to fetch form" / "فشل تحميل النموذج"
- `errors.requiredField` - "This field is required" / "هذا الحقل مطلوب"
- `errors.invalidEmail` - "Invalid email address" / "عنوان بريد إلكتروني غير صالح"

### Form (نموذج)
- `form.noFields` - "No fields available" / "لا توجد حقول متاحة"
- `form.submitSuccess` - "Form submitted successfully" / "تم إرسال النموذج بنجاح"

---

## الملفات المُنشأة/المُحدثة

### ملفات جديدة:
1. ✅ `src/locales/en.json`
2. ✅ `src/locales/ar.json`
3. ✅ `src/i18n/config.ts`
4. ✅ `src/contexts/LanguageContext.tsx`
5. ✅ `src/components/LanguageSwitcher.tsx`
6. ✅ `src/components/LanguageSwitcher.css`

### ملفات محدثة:
1. ✅ `package.json` - إضافة المكتبات
2. ✅ `src/main.tsx` - إضافة LanguageProvider
3. ✅ `src/services/api.ts` - إضافة Accept-Language header
4. ✅ `src/pages/FormViewer.tsx` - استخدام الترجمة

---

## الخطوات التالية

1. **تشغيل npm install**:
   ```bash
   cd frontend
   npm install
   ```

2. **اختبار الترجمة**:
   - تشغيل المشروع: `npm run dev`
   - التحقق من Language Switcher
   - التحقق من تغيير اللغة
   - التحقق من إرسال `Accept-Language` header

3. **إضافة المزيد من الترجمات**:
   - إضافة مفاتيح جديدة في `en.json` و `ar.json`
   - استخدامها في Components

---

## ملاحظات مهمة

1. **RTL Support**: يتم تغيير اتجاه النص تلقائياً عند اختيار العربية
2. **Language Persistence**: اللغة المحفوظة في localStorage
3. **API Integration**: يتم إرسال `Accept-Language` header تلقائياً مع كل طلب API
4. **Fallback**: في حالة عدم وجود ترجمة، يتم استخدام الإنجليزية

---

## الخلاصة

تم إعداد نظام الترجمة الكامل في React Frontend! 🎉

- ✅ دعم العربية والإنجليزية
- ✅ Language Switcher Component
- ✅ RTL Support
- ✅ API Integration (Accept-Language header)
- ✅ Language Persistence

جميع المكونات جاهزة للاستخدام! 🌍

