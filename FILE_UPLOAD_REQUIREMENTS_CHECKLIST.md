# ✅ قائمة التحقق - متطلبات رفع الملفات
## File Upload Requirements Checklist

---

## 📋 المتطلبات الوظيفية (Functional Requirements)

### ✅ 1. File Type Support
- [x] **File Type موجود في Database**
  - TypeName: "File"
  - ForeignTypeName: "ملف"
  - AllowMultiple: true (يدعم رفع عدة ملفات)
  - Location: `DataSeeder.cs`

### ✅ 2. Single or Multiple File Upload
- [x] **رفع ملف واحد**
  - Endpoint: `POST /api/FormSubmissionAttachments/upload`
  - Status: ✅ جاهز

- [x] **رفع عدة ملفات**
  - Endpoint: `POST /api/FormSubmissionAttachments/upload-multiple`
  - Status: ✅ جاهز

### ✅ 3. Validation (Type, Size)

#### ✅ File Type Validation
- [x] **الأنواع المدعومة**:
  - PDF (`.pdf`)
  - Images: JPG (`.jpg`, `.jpeg`), PNG (`.png`)
  - Excel: XLS (`.xls`), XLSX (`.xlsx`)
  - Word: DOC (`.doc`), DOCX (`.docx`)
- [x] **Location**: `FormSubmissionAttachmentsService.cs` (السطر 183-189)
- [x] **Error Message**: واضح ومفيد

#### ✅ File Size Validation
- [x] **الحد الأقصى**: 10MB
- [x] **Location**: `FormSubmissionAttachmentsService.cs` (السطر 179)
- [x] **Error Message**: "File size exceeds maximum allowed size (10MB)"

### ✅ 4. Preview and Download Support

#### ✅ Download Endpoint
- [x] **Endpoint**: `GET /api/FormSubmissionAttachments/{id}/download`
- [x] **Status**: ✅ جاهز
- [x] **Location**: `FormSubmissionAttachmentsController.cs` (السطر 170-188)
- [x] **Returns**: File stream with correct Content-Type

#### ⚪ Preview Support
- [ ] **Frontend**: يحتاج implementation في Angular
- [x] **Backend**: جاهز (DownloadUrl موجود في DTO)

---

## 🏗️ Backend Scope

### ✅ 1. Multipart Upload Endpoint
- [x] **Endpoint**: `POST /api/FormSubmissionAttachments/upload`
- [x] **Content-Type**: `multipart/form-data`
- [x] **Parameters**: file, submissionId, fieldId, fieldCode
- [x] **Status**: ✅ جاهز

### ✅ 2. File Storage (Local or Cloud)
- [x] **Storage Service**: `LocalFileStorageService`
- [x] **Interface**: `IFileStorageService` (يمكن الترقية لـ Cloud)
- [x] **Path**: `uploads/submissions/{submissionId}/`
- [x] **Unique Names**: GUID + filename
- [x] **Status**: ✅ جاهز

### ✅ 3. Metadata Storage
- [x] **Table**: `FORM_SUBMISSION_ATTACHMENTS`
- [x] **Fields**:
  - `Id` (fileId)
  - `FileName` (name)
  - `FileSize` (size)
  - `ContentType` (mime)
  - `FilePath`
  - `SubmissionId`
  - `FieldId`
  - `UploadedDate`
- [x] **Status**: ✅ جاهز

---

## 🎨 Frontend Scope

### ⚪ 1. Upload UI
- [ ] **Component**: يحتاج implementation في Angular
- [x] **API**: جاهز

### ⚪ 2. File Preview (Image/PDF)
- [ ] **Image Preview**: يحتاج implementation في Angular
- [ ] **PDF Preview**: يحتاج implementation في Angular
- [x] **Backend**: DownloadUrl موجود في Response

### ⚪ 3. Download Option
- [ ] **Download Button**: يحتاج implementation في Angular
- [x] **Backend**: Download endpoint جاهز

---

## ✅ Acceptance Criteria

### ✅ 1. Files Upload Successfully
- [x] **Single File**: ✅ جاهز
- [x] **Multiple Files**: ✅ جاهز
- [x] **Validation**: ✅ جاهز
- [x] **Storage**: ✅ جاهز
- [x] **Database**: ✅ جاهز

### ✅ 2. Validation Enforced
- [x] **File Type**: ✅ جاهز (PDF, Images, Excel, Word)
- [x] **File Size**: ✅ جاهز (10MB max)
- [x] **Error Messages**: ✅ واضحة

### ✅ 3. Files Persist in Draft and Final Submission
- [x] **Database Storage**: ✅ جاهز
- [x] **File System Storage**: ✅ جاهز
- [x] **Metadata**: ✅ محفوظة في Database

---

## 📊 Summary

### ✅ Backend: جاهز 100%

| Requirement | Status | Location |
|------------|--------|----------|
| File Type | ✅ | DataSeeder.cs |
| Single Upload | ✅ | Controller + Service |
| Multiple Upload | ✅ | Controller + Service |
| Type Validation | ✅ | Service (Line 183-189) |
| Size Validation | ✅ | Service (Line 179) |
| File Storage | ✅ | LocalFileStorageService |
| Metadata Storage | ✅ | FORM_SUBMISSION_ATTACHMENTS |
| Download Endpoint | ✅ | Controller (Line 170) |

### ⚪ Frontend: يحتاج Implementation

| Requirement | Status | Notes |
|------------|--------|-------|
| Upload UI | ⚪ | يحتاج Angular Component |
| File Preview | ⚪ | يحتاج Angular Component |
| Download Button | ⚪ | يحتاج Angular Component |

---

## 🔧 Code Changes Made

### 1. File Type Validation Added
**File**: `FormSubmissionAttachmentsService.cs`
```csharp
// Validate file type (PDF, Images, Excel, Word)
var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".xls", ".xlsx", ".doc", ".docx" };
var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
{
    return new ApiResponse<FormSubmissionAttachmentDto>(400, 
        $"File type not allowed. Allowed types: PDF, Images (JPG, PNG), Excel (XLS, XLSX), Word (DOC, DOCX)");
}
```

### 2. Download Endpoint Added
**File**: `FormSubmissionAttachmentsController.cs`
```csharp
[HttpGet("{id}/download")]
public async Task<IActionResult> DownloadFile(int id)
{
    var attachmentResult = await _formSubmissionAttachmentsService.GetByIdAsync(id);
    if (!attachmentResult.Success || attachmentResult.Data == null)
        return NotFound(new { message = "File not found" });

    var attachment = attachmentResult.Data;
    var fileStorageService = HttpContext.RequestServices.GetRequiredService<IFileStorageService>();
    
    var fileStream = await fileStorageService.GetFileAsync(attachment.FilePath);
    if (fileStream == null)
        return NotFound(new { message = "File not found on disk" });

    return File(fileStream, attachment.ContentType, attachment.FileName);
}
```

---

## 🌐 Complete API Endpoints List

### Upload
1. ✅ `POST /api/FormSubmissionAttachments/upload` - رفع ملف واحد
2. ✅ `POST /api/FormSubmissionAttachments/upload-multiple` - رفع عدة ملفات

### Retrieve
3. ✅ `GET /api/FormSubmissionAttachments/{id}` - جلب ملف محدد
4. ✅ `GET /api/FormSubmissionAttachments/field/{fieldId}` - جلب ملفات Field
5. ✅ `GET /api/FormSubmissionAttachments/submission/{submissionId}` - جلب ملفات Submission

### Download
6. ✅ `GET /api/FormSubmissionAttachments/{id}/download` - تحميل ملف ⭐ جديد

### Delete
7. ✅ `DELETE /api/FormSubmissionAttachments/{id}` - حذف ملف

---

## 📝 Example: Complete Upload Flow

### 1. Upload File
```typescript
// Angular
const formData = new FormData();
formData.append('file', file);
formData.append('submissionId', '1');
formData.append('fieldId', '5');
formData.append('fieldCode', 'DOCUMENT_FIELD');

this.http.post('/api/FormSubmissionAttachments/upload', formData)
  .subscribe(response => {
    if (response.statusCode === 200) {
      console.log('File uploaded:', response.data);
      // response.data.downloadUrl = "/api/FormSubmissionAttachments/1/download"
    }
  });
```

### 2. Download File
```typescript
// Angular
downloadFile(attachmentId: number, fileName: string) {
  window.open(`/api/FormSubmissionAttachments/${attachmentId}/download`, '_blank');
}
```

### 3. Preview File (Frontend)
```typescript
// Angular - Image Preview
previewImage(attachmentId: number) {
  return `/api/FormSubmissionAttachments/${attachmentId}/download`;
}

// Angular - PDF Preview
previewPDF(attachmentId: number) {
  return `/api/FormSubmissionAttachments/${attachmentId}/download`;
}
```

---

## ✅ Final Checklist

### Backend ✅
- [x] File Type موجود
- [x] Upload endpoint (single)
- [x] Upload endpoint (multiple)
- [x] File type validation
- [x] File size validation
- [x] File storage
- [x] Metadata storage
- [x] Download endpoint
- [x] Error handling

### Frontend ⚪
- [ ] Upload UI component
- [ ] File type validation (client-side)
- [ ] File size validation (client-side)
- [ ] Progress indicator
- [ ] Image preview
- [ ] PDF preview
- [ ] Download button
- [ ] Error messages display

---

## 🎯 Supported File Types

| Type | Extensions | MIME Types |
|------|-----------|------------|
| PDF | `.pdf` | `application/pdf` |
| Images | `.jpg`, `.jpeg`, `.png` | `image/jpeg`, `image/png` |
| Excel | `.xls`, `.xlsx` | `application/vnd.ms-excel`, `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
| Word | `.doc`, `.docx` | `application/msword`, `application/vnd.openxmlformats-officedocument.wordprocessingml.document` |

---

## 📏 File Size Limits

- **Maximum Size**: 10MB per file
- **Location**: `FormSubmissionAttachmentsService.cs` (Line 179)
- **Can be modified**: نعم، يمكن تغيير القيمة

---

## 🔒 Security

- ✅ **File Type Validation**: يمنع رفع أنواع غير مسموحة
- ✅ **File Size Validation**: يمنع رفع ملفات كبيرة
- ✅ **File Name Sanitization**: تنظيف أسماء الملفات
- ✅ **Unique File Names**: GUID لتجنب التكرار
- ✅ **Authorization**: `[Authorize(Roles = "Administration")]` (يمكن تعديله)

---

**تاريخ التقرير**: 2025-01-18  
**الحالة**: ✅ Backend جاهز 100%  
**Frontend**: ⚪ يحتاج Implementation
