# تقرير خدمة رفع الملفات - File Upload Service
## File Upload Service Documentation

---

## 📋 نظرة عامة

المشروع يحتوي على خدمة كاملة لرفع الملفات (File Upload Service) جاهزة للاستخدام. الخدمة تدعم:

- ✅ رفع ملف واحد
- ✅ رفع عدة ملفات (Multiple Files)
- ✅ حفظ الملفات على Local File System
- ✅ إدارة الملفات المرفوعة (CRUD)
- ✅ إحصائيات الملفات
- ✅ تحميل الملفات

---

## 🏗️ البنية المعمارية

### 1. Service Layer
**الملف**: `FormBuilder.Services/Services/FormBuilder/FormSubmissionAttachmentsService.cs`

### 2. Storage Service
**الملف**: `FormBuilder.Services/Services/FileStorage/LocalFileStorageService.cs`

### 3. Controller
**الملف**: `frombuilderApiProject/Controllers/FormBuilder/FormSubmissionAttachmentsController.cs`

### 4. DTOs
**الملف**: `FormBuilder.Core/DTOS/FormBuilder/FormSubmissionAttachmentDto.cs`

---

## 🌐 API Endpoints

### 1. رفع ملف واحد
```http
POST /api/FormSubmissionAttachments/upload
Content-Type: multipart/form-data

Parameters:
- file: IFormFile (required)
- submissionId: int (required)
- fieldId: int (required)
- fieldCode: string (required)
```

**مثال Request (Angular/TypeScript)**:
```typescript
const formData = new FormData();
formData.append('file', file);
formData.append('submissionId', '1');
formData.append('fieldId', '5');
formData.append('fieldCode', 'DOCUMENT_FIELD');

const response = await fetch('/api/FormSubmissionAttachments/upload', {
  method: 'POST',
  body: formData
});
```

**Response**:
```json
{
  "statusCode": 200,
  "message": "Success",
  "data": {
    "id": 1,
    "submissionId": 1,
    "fieldId": 5,
    "fieldCode": "DOCUMENT_FIELD",
    "fileName": "document.pdf",
    "filePath": "submissions/1/guid_document.pdf",
    "fileSize": 1024000,
    "contentType": "application/pdf",
    "uploadedDate": "2025-01-18T10:30:00Z",
    "fileSizeFormatted": "1.00 MB",
    "downloadUrl": "/api/attachments/download/1"
  }
}
```

---

### 2. رفع عدة ملفات
```http
POST /api/FormSubmissionAttachments/upload-multiple
Content-Type: multipart/form-data

Parameters:
- files: List<IFormFile> (required)
- submissionId: int (required)
- fieldId: int (required)
- fieldCode: string (required)
```

**مثال Request (Angular/TypeScript)**:
```typescript
const formData = new FormData();
files.forEach(file => {
  formData.append('files', file);
});
formData.append('submissionId', '1');
formData.append('fieldId', '5');
formData.append('fieldCode', 'DOCUMENT_FIELD');

const response = await fetch('/api/FormSubmissionAttachments/upload-multiple', {
  method: 'POST',
  body: formData
});
```

**Response**:
```json
{
  "statusCode": 200,
  "message": "Multiple attachments upload completed",
  "data": [
    {
      "attachmentId": 1,
      "fileName": "document1.pdf",
      "filePath": "submissions/1/guid_document1.pdf",
      "fileSize": 1024000,
      "contentType": "application/pdf",
      "uploadedDate": "2025-01-18T10:30:00Z",
      "success": true,
      "message": "Upload successful"
    },
    {
      "attachmentId": 2,
      "fileName": "document2.pdf",
      "filePath": "submissions/1/guid_document2.pdf",
      "fileSize": 2048000,
      "contentType": "application/pdf",
      "uploadedDate": "2025-01-18T10:30:05Z",
      "success": true,
      "message": "Upload successful"
    }
  ]
}
```

---

### 3. جلب جميع الملفات
```http
GET /api/FormSubmissionAttachments
```

### 4. جلب ملف محدد
```http
GET /api/FormSubmissionAttachments/{id}
```

### 5. جلب ملفات Submission محدد
```http
GET /api/FormSubmissionAttachments/submission/{submissionId}
```

### 6. جلب ملفات Field محدد
```http
GET /api/FormSubmissionAttachments/field/{fieldId}
```

### 7. جلب ملفات Submission و Field
```http
GET /api/FormSubmissionAttachments/submission/{submissionId}/field/{fieldId}
```

### 8. إحصائيات الملفات
```http
GET /api/FormSubmissionAttachments/submission/{submissionId}/stats
```

**Response**:
```json
{
  "statusCode": 200,
  "message": "Attachment statistics retrieved successfully",
  "data": {
    "submissionId": 1,
    "totalAttachments": 5,
    "totalSize": 5242880,
    "totalSizeFormatted": "5.00 MB",
    "attachmentsByType": {
      "application/pdf": 3,
      "image/jpeg": 2
    }
  }
}
```

### 9. حذف ملف
```http
DELETE /api/FormSubmissionAttachments/{id}
```

### 10. حذف جميع ملفات Submission
```http
DELETE /api/FormSubmissionAttachments/submission/{submissionId}
```

### 11. حذف ملفات Submission و Field
```http
DELETE /api/FormSubmissionAttachments/submission/{submissionId}/field/{fieldId}
```

---

## 📦 DTOs

### 1. FormSubmissionAttachmentDto
```typescript
interface FormSubmissionAttachmentDto {
  id: number;
  submissionId: number;
  submissionDocumentNumber?: string;
  fieldId: number;
  fieldCode?: string;
  fieldName?: string;
  fileName: string;
  filePath: string;
  fileSize: number;
  contentType: string;
  uploadedDate: string;
  fileSizeFormatted?: string;  // Computed: "1.00 MB"
  downloadUrl?: string;  // Computed: "/api/attachments/download/{id}"
}
```

### 2. CreateFormSubmissionAttachmentDto
```typescript
interface CreateFormSubmissionAttachmentDto {
  submissionId: number;
  fieldId: number;
  fieldCode: string;
  fileName: string;
  filePath: string;
  fileSize: number;
  contentType: string;
}
```

### 3. UploadAttachmentDto
```typescript
interface UploadAttachmentDto {
  submissionId: number;
  fieldId: number;
  fieldCode: string;
}
```

### 4. AttachmentUploadResultDto
```typescript
interface AttachmentUploadResultDto {
  attachmentId: number;
  fileName: string;
  filePath: string;
  fileSize: number;
  contentType: string;
  uploadedDate: string;
  success: boolean;
  message?: string;
}
```

### 5. AttachmentStatsDto
```typescript
interface AttachmentStatsDto {
  submissionId: number;
  totalAttachments: number;
  totalSize: number;
  totalSizeFormatted: string;
  attachmentsByType: { [key: string]: number };
}
```

---

## ⚙️ Service Methods

### FormSubmissionAttachmentsService

#### 1. UploadAttachmentAsync
```csharp
Task<ApiResponse<FormSubmissionAttachmentDto>> UploadAttachmentAsync(
    IFormFile file, 
    UploadAttachmentDto uploadDto)
```

**المميزات**:
- ✅ التحقق من وجود الملف
- ✅ التحقق من حجم الملف (حد أقصى 10MB)
- ✅ حفظ الملف في مجلد `uploads/submissions/{submissionId}/`
- ✅ إنشاء اسم ملف فريد (GUID + اسم الملف الأصلي)
- ✅ حفظ معلومات الملف في Database

#### 2. UploadMultipleAttachmentsAsync
```csharp
Task<ApiResponse<List<AttachmentUploadResultDto>>> UploadMultipleAttachmentsAsync(
    List<IFormFile> files, 
    UploadAttachmentDto uploadDto)
```

**المميزات**:
- ✅ رفع عدة ملفات في طلب واحد
- ✅ إرجاع نتيجة لكل ملف (نجح/فشل)
- ✅ معالجة الأخطاء لكل ملف بشكل منفصل

---

## 💾 File Storage

### LocalFileStorageService

**المسار الافتراضي**: `uploads/` (في مجلد المشروع)

**التكوين** (appsettings.json):
```json
{
  "FileStorage": {
    "BasePath": "C:\\Uploads\\FormBuilder"
  }
}
```

**ميزات Storage Service**:
- ✅ حفظ الملفات في مجلدات منظمة (`submissions/{submissionId}/`)
- ✅ تنظيف أسماء الملفات (إزالة الأحرف غير المسموحة)
- ✅ إنشاء أسماء ملفات فريدة (GUID)
- ✅ دعم Stream للقراءة/الكتابة
- ✅ دعم Content Type detection
- ✅ دعم حذف الملفات

**مثال مسار الملف**:
```
uploads/
  └── submissions/
      └── 1/
          ├── {guid}_document1.pdf
          ├── {guid}_document2.pdf
          └── {guid}_image.jpg
```

---

## 🔒 الأمان والتحقق

### 1. Authorization
جميع الـ endpoints تتطلب:
```csharp
[Authorize(Roles = "Administration")]
```

### 2. File Size Validation
- ✅ الحد الأقصى: **10MB** لكل ملف
- ✅ يمكن تعديله في `FormSubmissionAttachmentsService.cs` (السطر 179)

### 3. File Name Validation
- ✅ التحقق من عدم تكرار اسم الملف لنفس Submission
- ✅ تنظيف أسماء الملفات (إزالة الأحرف الخطرة)

### 4. Content Type Detection
يدعم الأنواع التالية:
- PDF: `application/pdf`
- Word: `application/msword`, `application/vnd.openxmlformats-officedocument.wordprocessingml.document`
- Excel: `application/vnd.ms-excel`, `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- Images: `image/jpeg`, `image/png`, `image/gif`
- Text: `text/plain`
- JSON/XML: `application/json`, `application/xml`

---

## 📝 أمثلة استخدام (Angular/TypeScript)

### 1. رفع ملف واحد
```typescript
async uploadFile(file: File, submissionId: number, fieldId: number, fieldCode: string) {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('submissionId', submissionId.toString());
  formData.append('fieldId', fieldId.toString());
  formData.append('fieldCode', fieldCode);

  try {
    const response = await this.http.post<ApiResponse<FormSubmissionAttachmentDto>>(
      '/api/FormSubmissionAttachments/upload',
      formData
    ).toPromise();
    
    if (response?.statusCode === 200) {
      console.log('File uploaded successfully:', response.data);
      return response.data;
    }
  } catch (error) {
    console.error('Upload error:', error);
    throw error;
  }
}
```

### 2. رفع عدة ملفات
```typescript
async uploadMultipleFiles(
  files: File[], 
  submissionId: number, 
  fieldId: number, 
  fieldCode: string
) {
  const formData = new FormData();
  files.forEach(file => {
    formData.append('files', file);
  });
  formData.append('submissionId', submissionId.toString());
  formData.append('fieldId', fieldId.toString());
  formData.append('fieldCode', fieldCode);

  try {
    const response = await this.http.post<ApiResponse<AttachmentUploadResultDto[]>>(
      '/api/FormSubmissionAttachments/upload-multiple',
      formData
    ).toPromise();
    
    if (response?.statusCode === 200) {
      const successful = response.data.filter(r => r.success);
      const failed = response.data.filter(r => !r.success);
      
      console.log('Successful uploads:', successful);
      if (failed.length > 0) {
        console.warn('Failed uploads:', failed);
      }
      
      return response.data;
    }
  } catch (error) {
    console.error('Upload error:', error);
    throw error;
  }
}
```

### 3. جلب ملفات Submission
```typescript
async getSubmissionAttachments(submissionId: number) {
  try {
    const response = await this.http.get<ApiResponse<FormSubmissionAttachmentDto[]>>(
      `/api/FormSubmissionAttachments/submission/${submissionId}`
    ).toPromise();
    
    return response?.data || [];
  } catch (error) {
    console.error('Error fetching attachments:', error);
    return [];
  }
}
```

### 4. حذف ملف
```typescript
async deleteAttachment(attachmentId: number) {
  try {
    const response = await this.http.delete<ApiResponse<boolean>>(
      `/api/FormSubmissionAttachments/${attachmentId}`
    ).toPromise();
    
    return response?.data || false;
  } catch (error) {
    console.error('Error deleting attachment:', error);
    throw error;
  }
}
```

### 5. إحصائيات الملفات
```typescript
async getAttachmentStats(submissionId: number) {
  try {
    const response = await this.http.get<ApiResponse<AttachmentStatsDto>>(
      `/api/FormSubmissionAttachments/submission/${submissionId}/stats`
    ).toPromise();
    
    return response?.data;
  } catch (error) {
    console.error('Error fetching stats:', error);
    return null;
  }
}
```

---

## 🎨 مثال Component (Angular)

```typescript
import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-file-upload',
  template: `
    <div>
      <input 
        type="file" 
        #fileInput 
        (change)="onFileSelected($event)"
        multiple
      />
      <button (click)="uploadFiles()" [disabled]="!selectedFiles.length">
        Upload Files
      </button>
      <div *ngIf="uploadProgress">
        Uploading... {{ uploadProgress }}%
      </div>
      <div *ngFor="let result of uploadResults">
        <p [class.success]="result.success" [class.error]="!result.success">
          {{ result.fileName }}: {{ result.message }}
        </p>
      </div>
    </div>
  `
})
export class FileUploadComponent {
  selectedFiles: File[] = [];
  uploadResults: AttachmentUploadResultDto[] = [];
  uploadProgress = 0;

  constructor(private http: HttpClient) {}

  onFileSelected(event: any) {
    this.selectedFiles = Array.from(event.target.files);
  }

  async uploadFiles() {
    if (!this.selectedFiles.length) return;

    const formData = new FormData();
    this.selectedFiles.forEach(file => {
      formData.append('files', file);
    });
    formData.append('submissionId', '1');
    formData.append('fieldId', '5');
    formData.append('fieldCode', 'DOCUMENT_FIELD');

    try {
      const response = await this.http.post<ApiResponse<AttachmentUploadResultDto[]>>(
        '/api/FormSubmissionAttachments/upload-multiple',
        formData,
        {
          reportProgress: true,
          observe: 'events'
        }
      ).toPromise();
      
      // Handle response
      if (response?.data) {
        this.uploadResults = response.data;
      }
    } catch (error) {
      console.error('Upload failed:', error);
    }
  }
}
```

---

## ⚠️ ملاحظات مهمة

### 1. حجم الملف
- الحد الأقصى الحالي: **10MB**
- يمكن تعديله في `FormSubmissionAttachmentsService.cs`:
```csharp
if (file.Length > 10 * 1024 * 1024)  // 10MB
```

### 2. مسار الملفات
- الملفات تُحفظ في: `uploads/submissions/{submissionId}/`
- يمكن تغيير المسار في `appsettings.json`:
```json
{
  "FileStorage": {
    "BasePath": "C:\\YourCustomPath"
  }
}
```

### 3. أسماء الملفات
- يتم إضافة GUID قبل اسم الملف لتجنب التكرار
- مثال: `{guid}_original-filename.pdf`

### 4. Content Type
- يتم اكتشاف Content Type تلقائياً من امتداد الملف
- يمكن إرسال Content Type مخصص في الـ request

### 5. Authorization
- جميع الـ endpoints تتطلب `Administration` role
- يمكن تعديله في Controller حسب الحاجة

---

## 🔧 التكوين المطلوب

### 1. appsettings.json
```json
{
  "FileStorage": {
    "BasePath": "uploads"
  }
}
```

### 2. Program.cs
يجب تسجيل الخدمات:
```csharp
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IFormSubmissionAttachmentsService, FormSubmissionAttachmentsService>();
```

---

## ✅ Checklist للـ Frontend

- [ ] إنشاء File Upload Component
- [ ] إضافة File Input مع دعم Multiple Files
- [ ] إضافة Progress Bar للرفع
- [ ] إضافة Validation لحجم الملف
- [ ] إضافة Preview للملفات قبل الرفع
- [ ] إضافة قائمة الملفات المرفوعة
- [ ] إضافة زر حذف للملفات
- [ ] إضافة Download Link للملفات
- [ ] إضافة Error Handling
- [ ] إضافة Loading States

---

## 📞 ملاحظات إضافية

1. **الملفات تُحفظ على Local File System** - يمكن الترقية لاحقاً لـ Cloud Storage (Azure Blob, AWS S3)
2. **الخدمة جاهزة للاستخدام** - لا تحتاج تعديلات إضافية
3. **يدعم جميع أنواع الملفات** - PDF, Images, Documents, etc.
4. **آمن** - التحقق من حجم الملف وأسماء الملفات

---

**تاريخ التقرير**: 2025-01-18  
**الإصدار**: 1.0  
**الحالة**: ✅ جاهز للاستخدام
