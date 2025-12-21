# 📋 ملخص الـ Endpoints المطلوبة لرفع الملفات
## File Upload Endpoints Summary

---

## 🎯 الـ Endpoints الأساسية المطلوبة

### 1. ✅ رفع ملف واحد (الأهم)
```http
POST /api/FormSubmissionAttachments/upload
Content-Type: multipart/form-data
Authorization: Bearer {token}

Form Data:
- file: IFormFile (required)
- submissionId: int (required)
- fieldId: int (required)
- fieldCode: string (required)
```

**مثال استخدام (Angular)**:
```typescript
const formData = new FormData();
formData.append('file', file);
formData.append('submissionId', submissionId.toString());
formData.append('fieldId', fieldId.toString());
formData.append('fieldCode', fieldCode);

this.http.post('/api/FormSubmissionAttachments/upload', formData)
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
    "uploadedDate": "2025-01-18T10:30:00Z"
  }
}
```

---

### 2. ✅ رفع عدة ملفات (اختياري)
```http
POST /api/FormSubmissionAttachments/upload-multiple
Content-Type: multipart/form-data
Authorization: Bearer {token}

Form Data:
- files: List<IFormFile> (required)
- submissionId: int (required)
- fieldId: int (required)
- fieldCode: string (required)
```

**مثال استخدام (Angular)**:
```typescript
const formData = new FormData();
files.forEach(file => {
  formData.append('files', file);
});
formData.append('submissionId', submissionId.toString());
formData.append('fieldId', fieldId.toString());
formData.append('fieldCode', fieldCode);

this.http.post('/api/FormSubmissionAttachments/upload-multiple', formData)
```

---

### 3. ✅ جلب ملفات Field محدد (للعرض)
```http
GET /api/FormSubmissionAttachments/field/{fieldId}
Authorization: Bearer {token}
```

**مثال استخدام (Angular)**:
```typescript
this.http.get(`/api/FormSubmissionAttachments/field/${fieldId}`)
```

**Response**:
```json
{
  "statusCode": 200,
  "message": "Success",
  "data": [
    {
      "id": 1,
      "fileName": "document1.pdf",
      "filePath": "submissions/1/guid_document1.pdf",
      "fileSize": 1024000,
      "contentType": "application/pdf",
      "uploadedDate": "2025-01-18T10:30:00Z",
      "downloadUrl": "/api/attachments/download/1"
    }
  ]
}
```

---

### 4. ✅ حذف ملف (اختياري)
```http
DELETE /api/FormSubmissionAttachments/{id}
Authorization: Bearer {token}
```

**مثال استخدام (Angular)**:
```typescript
this.http.delete(`/api/FormSubmissionAttachments/${attachmentId}`)
```

---

## 📌 الـ Endpoints المطلوبة فقط (الأساسية)

### ✅ المطلوبة للعمل الأساسي:

1. **POST /api/FormSubmissionAttachments/upload** ⭐ (الأهم)
   - لرفع ملف واحد
   - **مطلوبة 100%**

2. **GET /api/FormSubmissionAttachments/field/{fieldId}** ⭐
   - لجلب الملفات المرفوعة لحقل محدد
   - **مطلوبة للعرض**

### ⚪ اختيارية (يمكن إضافتها لاحقاً):

3. **POST /api/FormSubmissionAttachments/upload-multiple**
   - لرفع عدة ملفات دفعة واحدة
   - **اختيارية** (يمكن استخدام upload عدة مرات)

4. **DELETE /api/FormSubmissionAttachments/{id}**
   - لحذف ملف
   - **اختيارية**

5. **GET /api/FormSubmissionAttachments/submission/{submissionId}**
   - لجلب جميع ملفات Submission
   - **اختيارية**

---

## 🎯 السيناريو الكامل

### عند اختيار File Type في Angular:

```
1. المستخدم يختار Field Type = "File"
   ↓
2. يظهر File Input
   ↓
3. المستخدم يرفع ملف
   ↓
4. POST /api/FormSubmissionAttachments/upload
   Body: {
     file: File,
     submissionId: 1,
     fieldId: 5,
     fieldCode: "DOCUMENT_FIELD"
   }
   ↓
5. الملف يُحفظ في Database ✅
   ↓
6. Response بالبيانات المحفوظة
   ↓
7. (اختياري) GET /api/FormSubmissionAttachments/field/5
   لعرض الملفات المرفوعة
```

---

## 📝 مثال كود Angular كامل

```typescript
// Service
export class FileUploadService {
  constructor(private http: HttpClient) {}

  // رفع ملف واحد
  uploadFile(file: File, submissionId: number, fieldId: number, fieldCode: string) {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('submissionId', submissionId.toString());
    formData.append('fieldId', fieldId.toString());
    formData.append('fieldCode', fieldCode);

    return this.http.post<ApiResponse<FormSubmissionAttachmentDto>>(
      '/api/FormSubmissionAttachments/upload',
      formData
    );
  }

  // جلب ملفات Field
  getFieldAttachments(fieldId: number) {
    return this.http.get<ApiResponse<FormSubmissionAttachmentDto[]>>(
      `/api/FormSubmissionAttachments/field/${fieldId}`
    );
  }

  // حذف ملف
  deleteAttachment(attachmentId: number) {
    return this.http.delete<ApiResponse<boolean>>(
      `/api/FormSubmissionAttachments/${attachmentId}`
    );
  }
}
```

---

## ✅ الخلاصة

### الـ Endpoints المطلوبة فقط:

1. ✅ **POST /api/FormSubmissionAttachments/upload** (مطلوبة)
2. ✅ **GET /api/FormSubmissionAttachments/field/{fieldId}** (مطلوبة للعرض)

### الباقي اختياري:
- upload-multiple (يمكن استخدام upload عدة مرات)
- delete (يمكن إضافتها لاحقاً)
- stats (لإحصائيات - اختياري)

---

**ملاحظة**: جميع الـ endpoints موجودة وجاهزة في Controller ✅

