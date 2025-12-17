# تقرير مراجعة قاعدة البيانات - Form Builder & Document Engine

## 📊 ملخص عام

تم مراجعة الجداول الموجودة ومقارنتها مع الـ Requirements المطلوبة في الـ Roadmap.

---

## ✅ Phase A - Authentication & Authorization

### المطلوب:
- Users, Roles, Permissions
- User-specific overrides
- JWT tokens
- Auditing

### الموجود:
- ✅ `REFRESH_TOKENS` - موجود
- ❌ **مفقود**: جدول Users (قد يكون في Identity)
- ❌ **مفقود**: جدول Roles (قد يكون في Identity)
- ❌ **مفقود**: جدول Permissions
- ❌ **مفقود**: جدول UserPermissions (ربط Users بالـ Permissions)

### التوصيات:
1. **التحقق من استخدام ASP.NET Identity** - إذا كان مستخدم، الجداول موجودة تلقائياً
2. **إضافة جدول Permissions** إذا لم يكن موجوداً:
   ```sql
   CREATE TABLE PERMISSIONS (
       Id INT PRIMARY KEY,
       PermissionCode VARCHAR(100) UNIQUE,
       PermissionName VARCHAR(200),
       Module VARCHAR(100),
       IsActive BIT
   )
   ```
3. **إضافة جدول UserPermissions**:
   ```sql
   CREATE TABLE USER_PERMISSIONS (
       Id INT PRIMARY KEY,
       UserId VARCHAR(450),
       PermissionId INT,
       IsGranted BIT,
       CreatedDate DATETIME
   )
   ```

---

## ✅ Phase B - Form Builder Core

### المطلوب:
- Form Builder (CRUD)
- Tabs and Fields
- Field Types, Rules, Data Sources, Options
- Grid support
- Validation rules
- Form versioning

### الموجود:
- ✅ `FORM_BUILDER` - موجود ✓
- ✅ `FORM_TABS` - موجود ✓
- ✅ `FORM_FIELDS` - موجود ✓
- ✅ `FIELD_TYPES` - موجود ✓
- ✅ `FIELD_OPTIONS` - موجود ✓
- ✅ `FIELD_DATA_SOURCES` - موجود ✓
- ✅ `FORM_RULES` - موجود ✓
- ✅ `FORM_VALIDATION_RULES` - موجود ✓
- ✅ `FORM_GRIDS` - موجود ✓
- ✅ `FORM_GRID_COLUMNS` - موجود ✓
- ✅ `FORMULAS` - موجود ✓
- ✅ `FORMULA_VARIABLES` - موجود ✓

### التعديلات المطلوبة:
1. **FORM_BUILDER**: يحتاج حقل `PreviewMode` (BIT) - للـ Preview Mode
2. **FORM_FIELDS**: تم إزالة `DataType`, `VisibilityRuleJson`, `ReadOnlyRuleJson` ✓ (تم التعديل)

---

## ✅ Phase C - Document Types & Number Series

### المطلوب:
- Document Types
- Projects
- Number Series per project & form
- Automated numbering

### الموجود:
- ✅ `DOCUMENT_TYPES` - موجود ✓
- ✅ `PROJECTS` - موجود ✓
- ✅ `DOCUMENT_SERIES` - موجود ✓

### التعديلات المطلوبة:
1. **DOCUMENT_SERIES**: يحتاج حقل `Prefix` (VARCHAR) - للبادئة في الرقم
2. **DOCUMENT_SERIES**: يحتاج حقل `Suffix` (VARCHAR) - للـ Suffix في الرقم
3. **DOCUMENT_SERIES**: يحتاج حقل `Format` (VARCHAR) - لتنسيق الرقم (مثل: {YYYY}-{MM}-{NUMBER})

---

## ✅ Phase D - Submission Engine

### المطلوب:
- Draft creation & submission
- Field values storage
- Attachments storage
- Grid data storage
- Validation engine
- Formula calculations

### الموجود:
- ✅ `FORM_SUBMISSIONS` - موجود ✓
- ✅ `FORM_SUBMISSION_VALUES` - موجود ✓
- ✅ `FORM_SUBMISSION_ATTACHMENTS` - موجود ✓
- ✅ `FORM_SUBMISSION_GRID_ROWS` - موجود ✓
- ✅ `FORM_SUBMISSION_GRID_CELLS` - موجود ✓
- ✅ `ATTACHMENT_TYPES` - موجود ✓
- ✅ `FORM_ATTACHMENT_TYPES` - موجود ✓

### التعديلات المطلوبة:
1. **FORM_SUBMISSIONS**: يحتاج حقل `DraftSavedDate` (DATETIME) - تاريخ حفظ المسودة
2. **FORM_SUBMISSIONS**: يحتاج حقل `LastModifiedDate` (DATETIME) - آخر تعديل
3. **FORM_SUBMISSIONS**: يحتاج حقل `ValidationErrorsJson` (NVARCHAR(MAX)) - لحفظ أخطاء التحقق

---

## ✅ Phase E - Workflow & Approvals

### المطلوب:
- Workflow definitions
- Approval stages with rules
- Stage assignees (roles/users)
- Delegation with date ranges
- Approval actions (Approve, Reject, Return)
- Approval history

### الموجود:
- ✅ `APPROVAL_WORKFLOWS` - موجود ✓
- ✅ `APPROVAL_STAGES` - موجود ✓
- ✅ `APPROVAL_STAGE_ASSIGNEES` - موجود ✓
- ✅ `APPROVAL_DELEGATIONS` - موجود ✓
- ✅ `DOCUMENT_APPROVAL_HISTORY` - موجود ✓

### التعديلات المطلوبة:
1. **APPROVAL_STAGES**: 
   - ✅ `MinAmount` (DECIMAL) - موجود ✓
   - ✅ `MaxAmount` (DECIMAL) - موجود ✓
   - ❌ يحتاج: `ConditionJson` (NVARCHAR(MAX)) - شروط إضافية
2. **APPROVAL_DELEGATIONS**: 
   - ✅ `StartDate` (DATETIME) - موجود ✓
   - ✅ `EndDate` (DATETIME) - موجود ✓
   - ✅ `IsActive` (BIT) - موجود ✓
3. **DOCUMENT_APPROVAL_HISTORY**: 
   - ✅ `ActionType` (VARCHAR) - موجود ✓
   - ✅ `Comments` (NVARCHAR(MAX)) - موجود ✓
   - ❌ يحتاج: `PerformedByUserId` (VARCHAR) - من قام بالإجراء (يستخدم CreatedByUserId من BaseEntity)

---

## ✅ Phase F - Notifications & Email Engine

### المطلوب:
- SMTP configuration
- Email templates with placeholders
- Event-based triggers
- Internal & email notifications

### الموجود:
- ✅ `SMTP_CONFIGS` - موجود ✓
- ✅ `EMAIL_TEMPLATES` - موجود ✓
- ✅ `ALERT_RULES` - موجود ✓

### التعديلات المطلوبة:
1. **EMAIL_TEMPLATES**: 
   - ✅ `SubjectTemplate`, `BodyTemplateHtml` - موجود ✓
   - ✅ `IsActive` (BIT) - موجود ✓
   - ❌ يحتاج: `EventType` (VARCHAR) - نوع الحدث (Submit, Approve, Reject, DueDate)
   - ❌ يحتاج: `PlaceholdersJson` (NVARCHAR(MAX)) - قائمة الـ Placeholders المتاحة
2. **ALERT_RULES**: 
   - ✅ `TriggerType` (VARCHAR) - موجود ✓
   - ✅ `NotificationType` (VARCHAR) - موجود ✓
   - ✅ `TargetRoleId` (VARCHAR) - موجود ✓
   - ✅ `TargetUserId` (VARCHAR) - موجود ✓
   - ✅ `ConditionJson` (NVARCHAR(MAX)) - موجود ✓

---

## ✅ Phase G - Buttons, Actions & Layout Integrations

### المطلوب:
- Custom button definitions
- Actions (CopyToDocument, SendEmail, OpenLayout, Custom)
- Crystal Reports integration

### الموجود:
- ✅ `FORM_BUTTONS` - موجود ✓
- ✅ `CRYSTAL_LAYOUTS` - موجود ✓

### التعديلات المطلوبة:
1. **FORM_BUTTONS**: التحقق من وجود حقول:
   - `ActionType` (VARCHAR) - موجود ✓
   - `ActionConfigJson` (NVARCHAR(MAX)) - موجود ✓
   - يحتاج إضافة: `ConditionJson` (NVARCHAR(MAX)) - شروط إظهار الزر
2. **CRYSTAL_LAYOUTS**: التحقق من وجود حقول:
   - `LayoutPath` (VARCHAR) - مسار الـ Layout
   - `ParametersJson` (NVARCHAR(MAX)) - معاملات الـ Layout

---

## ✅ Phase H - SAP B1 Integration

### المطلوب:
- SAP object mapping
- Field mapping
- Draft document creation
- Error logging

### الموجود:
- ✅ `SAP_OBJECT_MAPPINGS` - موجود ✓
- ✅ `SAP_FIELD_MAPPINGS` - موجود ✓

### التعديلات المطلوبة:
1. **SAP_OBJECT_MAPPINGS**: يحتاج حقول:
   - `SapObjectType` (VARCHAR) - نوع الـ SAP Object
   - `MappingConfigJson` (NVARCHAR(MAX)) - إعدادات الربط
   - `IsActive` (BIT) - حالة التفعيل
2. **إضافة جدول SAP_SYNC_LOGS**:
   ```sql
   CREATE TABLE SAP_SYNC_LOGS (
       Id INT PRIMARY KEY,
       SubmissionId INT,
       SapObjectType VARCHAR(50),
       Status VARCHAR(50),
       ErrorMessage NVARCHAR(MAX),
       SyncDate DATETIME,
       CreatedDate DATETIME
   )
   ```

---

## 📋 الجداول الإضافية المطلوبة

### 1. جدول NOTIFICATIONS (للإشعارات الداخلية)
```sql
CREATE TABLE NOTIFICATIONS (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId VARCHAR(450),
    Title NVARCHAR(200),
    Message NVARCHAR(MAX),
    NotificationType VARCHAR(50),
    RelatedSubmissionId INT,
    IsRead BIT DEFAULT 0,
    ReadDate DATETIME,
    CreatedDate DATETIME,
    IsActive BIT DEFAULT 1
)
```

### 2. جدول FORM_VERSIONS (لإدارة إصدارات النماذج)
```sql
CREATE TABLE FORM_VERSIONS (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FormBuilderId INT,
    VersionNumber INT,
    VersionNotes NVARCHAR(MAX),
    PublishedDate DATETIME,
    PublishedByUserId VARCHAR(450),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME
)
```

### 3. جدول FORM_PREVIEW_SESSIONS (لجلسات المعاينة)
```sql
CREATE TABLE FORM_PREVIEW_SESSIONS (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FormBuilderId INT,
    SessionToken VARCHAR(200),
    PreviewDataJson NVARCHAR(MAX),
    CreatedDate DATETIME,
    ExpiresDate DATETIME,
    IsActive BIT DEFAULT 1
)
```

---

## 🔧 التعديلات المطلوبة على الجداول الموجودة

### FORM_BUILDER
```sql
ALTER TABLE FORM_BUILDER
ADD PreviewMode BIT DEFAULT 0;
```

### DOCUMENT_SERIES
```sql
ALTER TABLE DOCUMENT_SERIES
ADD Prefix VARCHAR(50) NULL,
    Suffix VARCHAR(50) NULL,
    Format VARCHAR(100) NULL DEFAULT '{NUMBER}';
```

### FORM_SUBMISSIONS
```sql
ALTER TABLE FORM_SUBMISSIONS
ADD DraftSavedDate DATETIME NULL,
    LastModifiedDate DATETIME NULL,
    ValidationErrorsJson NVARCHAR(MAX) NULL;
```

### EMAIL_TEMPLATES
```sql
ALTER TABLE EMAIL_TEMPLATES
ADD EventType VARCHAR(50) NULL,  -- Submit, Approve, Reject, DueDate, etc.
    PlaceholdersJson NVARCHAR(MAX) NULL;  -- JSON array of available placeholders
```

### APPROVAL_STAGES
```sql
ALTER TABLE APPROVAL_STAGES
ADD ConditionJson NVARCHAR(MAX) NULL;  -- Additional conditions for stage activation
```

### FORM_BUTTONS
```sql
ALTER TABLE FORM_BUTTONS
ADD ConditionJson NVARCHAR(MAX) NULL;
```

---

## ✅ الخلاصة

### الجداول الموجودة: 38 جدول
### الجداول المطلوبة: 3 جداول جديدة
### التعديلات المطلوبة: 5 جداول تحتاج تعديل

### الأولويات:
1. **عاجل**: إضافة جداول Permissions و UserPermissions (Phase A)
2. **مهم**: إضافة جدول NOTIFICATIONS (Phase F)
3. **مهم**: إضافة حقول في DOCUMENT_SERIES (Phase C)
4. **متوسط**: إضافة حقول في FORM_SUBMISSIONS (Phase D)
5. **منخفض**: إضافة جداول FORM_VERSIONS و FORM_PREVIEW_SESSIONS

---

## 📝 ملاحظات إضافية

1. **BaseEntity**: جميع الجداول ترث من BaseEntity الذي يحتوي على:
   - Id, CreatedByUserId, CreatedDate, UpdatedDate, IsActive ✓

2. **Relationships**: جميع العلاقات محددة بشكل صحيح في DbContext ✓

3. **Indexes**: هناك indexes على:
   - FORM_BUILDER.FormCode ✓
   - FORM_SUBMISSIONS.DocumentNumber ✓
   - DOCUMENT_TYPES.Code ✓
   - PROJECTS.Code ✓
   - DOCUMENT_SERIES.SeriesCode ✓

4. **Cascade Deletes**: تم تحديدها بشكل صحيح في معظم الحالات ✓

---

**تاريخ المراجعة**: $(Get-Date -Format "yyyy-MM-dd")
**المراجع**: AI Assistant
**الحالة**: ✅ جاهز للتطبيق

