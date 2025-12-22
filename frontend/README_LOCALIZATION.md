# دليل الترجمة في React Frontend

## ✅ تم إعداد نظام الترجمة بنجاح!

---

## 📦 الخطوة الأولى: تثبيت المكتبات

```bash
cd frontend
npm install
```

سيتم تثبيت المكتبات التالية:
- `i18next`
- `react-i18next`
- `i18next-browser-languagedetector`

---

## 🚀 كيفية الاستخدام

### 1. استخدام الترجمة في Component

```tsx
import { useLanguage } from '../contexts/LanguageContext'

const MyComponent = () => {
  const { t, currentLanguage, changeLanguage, isRTL } = useLanguage()

  return (
    <div>
      <h1>{t('common.formBuilder')}</h1>
      <p>{t('common.fields')}</p>
    </div>
  )
}
```

### 2. إضافة Language Switcher

```tsx
import LanguageSwitcher from '../components/LanguageSwitcher'

<LanguageSwitcher />
```

### 3. استخدام مع Interpolation

```tsx
// في ملف الترجمة: "minValue": "الحد الأدنى للقيمة هو {{min}}"
const { t } = useLanguage()
<p>{t('errors.minValue', { min: 10 })}</p>
```

---

## 📁 الملفات المُنشأة

### ملفات الترجمة:
- ✅ `src/locales/en.json` - الإنجليزية
- ✅ `src/locales/ar.json` - العربية

### ملفات الإعداد:
- ✅ `src/i18n/config.ts` - إعداد i18next
- ✅ `src/contexts/LanguageContext.tsx` - Context للغة
- ✅ `src/components/LanguageSwitcher.tsx` - مكون تبديل اللغة
- ✅ `src/styles/rtl.css` - دعم RTL

### ملفات محدثة:
- ✅ `package.json` - إضافة المكتبات
- ✅ `src/main.tsx` - إضافة LanguageProvider
- ✅ `src/services/api.ts` - إضافة Accept-Language header
- ✅ `src/pages/FormViewer.tsx` - استخدام الترجمة

---

## 🔑 المفاتيح المتاحة

### Common (مشترك)
- `common.tabs` - "Tabs" / "التبويبات"
- `common.fields` - "Fields" / "الحقول"
- `common.formBuilder` - "Form Builder" / "منشئ النماذج"
- `common.fieldType` - "Field Type" / "نوع الحقل"
- `common.options` - "Options" / "الخيارات"
- `common.submit` - "Submit" / "إرسال"
- `common.loading` - "Loading..." / "جاري التحميل..."

### Errors (أخطاء)
- `errors.formNotFound` - "Form not found" / "النموذج غير موجود"
- `errors.requiredField` - "This field is required" / "هذا الحقل مطلوب"

---

## ✨ المميزات

1. **دعم RTL تلقائي**: يتم تغيير اتجاه النص تلقائياً عند اختيار العربية
2. **حفظ اللغة**: اللغة المحفوظة في localStorage
3. **API Integration**: يتم إرسال `Accept-Language` header تلقائياً
4. **Fallback**: في حالة عدم وجود ترجمة، يتم استخدام الإنجليزية

---

## 🎯 الخطوات التالية

1. **تشغيل npm install**:
   ```bash
   cd frontend
   npm install
   ```

2. **تشغيل المشروع**:
   ```bash
   npm run dev
   ```

3. **اختبار الترجمة**:
   - التحقق من Language Switcher
   - التحقق من تغيير اللغة
   - التحقق من RTL Support
   - التحقق من إرسال `Accept-Language` header في Network tab

---

## 📝 ملاحظات

- اللغة الافتراضية: الإنجليزية
- يتم اكتشاف اللغة تلقائياً من المتصفح
- اللغة المحفوظة في `localStorage` تحت مفتاح `i18nextLng`

---

## 🎉 الخلاصة

تم إعداد نظام الترجمة الكامل! جميع المكونات جاهزة للاستخدام! 🌍













