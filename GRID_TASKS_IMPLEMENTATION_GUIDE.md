# Grid Support (Line Items Grid) - دليل التنفيذ الكامل

## 📊 نظرة عامة

**Grid** = جدول بيانات داخل الفورم (مثل Excel)
- كل صف = سجل فرعي مرتبط بالفورم الرئيسي
- مثال: فاتورة تحتوي على Grid للمنتجات (اسم المنتج، الكمية، السعر)

---

## ✅ المهام المكتملة (Backend)

### 1. Grid Schema Management ✅
**الملفات:**
- `FormGridsController.cs`
- `FormGridService.cs`
- `IFormGridService.cs`

**الـ Endpoints:**
- ✅ CRUD operations كاملة
- ✅ Get by FormBuilder, Tab, Code
- ✅ Toggle active, Exists, Code validation

### 2. Grid Columns Management ✅
**الملفات:**
- `FormGridColumnsController.cs`
- `FormGridColumnService.cs`
- `IFormGridColumnService.cs`

**الـ Endpoints:**
- ✅ CRUD operations كاملة
- ✅ Get by Grid, FieldType
- ✅ Toggle active, Exists, Code validation

### 3. Grid Data Persistence ✅
**الملفات:**
- `FormSubmissionGridRowsController.cs`
- `FormSubmissionGridCellsController.cs`
- `FormSubmissionGridRowService.cs`
- `FormSubmissionGridCellService.cs`

**الـ Endpoints:**
- ✅ CRUD operations للـ Rows
- ✅ CRUD operations للـ Cells
- ✅ Get by Submission, Grid, Row
- ✅ Bulk operations (CreateMultiple)

### 4. DTOs ✅
**الملفات:**
- `FormGridDto.cs`
- `FormGridColumnDto.cs`
- `FormSubmissionGridRowDto.cs`
- `FormSubmissionGridCellDto.cs`
- `BulkSaveGridDataDto.cs` (في FormSubmissionDto.cs)
- `SaveFormSubmissionDataDto.cs` (يحتوي على GridData)

---

## ⚠️ المهام المطلوبة (Backend)

### Task 1: Grid Field Type Integration ⚠️

#### المشكلة:
عند إنشاء Field من نوع "Grid"، لا يوجد مكان لحفظ Grid ID المرتبط به.

#### الحل المطلوب:

**1. تعديل `CreateFormFieldDto`:**
```csharp
// في FormBuilder.Core/DTOS/FormFields/CreateFormFieldDto.cs
public class CreateFormFieldDto
{
    // ... existing properties ...
    
    // إضافة خاصية GridId
    public int? GridId { get; set; } // Grid ID إذا كان FieldType = Grid
}
```

**2. تعديل `FormFieldDto`:**
```csharp
// في FormBuilder.Core/DTOS/FormBuilder/FormFieldDto.cs
public class FormFieldDto
{
    // ... existing properties ...
    
    public int? GridId { get; set; } // Grid ID المرتبط
    public FormGridDto? Grid { get; set; } // Navigation property
}
```

**3. تعديل Entity:**
```csharp
// في formBuilder.Domian/Entitys/FormBuilder/FormField.cs
public class FORM_FIELDS : BaseEntity
{
    // ... existing properties ...
    
    public int? GridId { get; set; }
    [ForeignKey("GridId")]
    public virtual FORM_GRIDS? Grid { get; set; }
}
```

**4. إضافة Migration:**
```bash
dotnet ef migrations add AddGridIdToFormFields --project FormBuilder.Core
```

**5. تعديل Service:**
```csharp
// في FormFieldService.cs
public async Task<ServiceResult<FormFieldDto>> CreateAsync(CreateFormFieldDto createDto)
{
    // التحقق من GridId إذا كان FieldType = Grid
    if (createDto.GridId.HasValue)
    {
        var fieldType = await _unitOfWork.FieldTypesRepository.GetByIdAsync(createDto.FieldTypeId);
        if (fieldType?.TypeName?.ToLower() == "grid")
        {
            // التحقق من وجود Grid
            var grid = await _unitOfWork.FormGridRepository.GetByIdAsync(createDto.GridId.Value);
            if (grid == null)
                return ServiceResult<FormFieldDto>.BadRequest("Grid not found");
            
            // التحقق من أن Grid ينتمي لنفس Tab
            if (grid.TabId.HasValue && grid.TabId != createDto.TabId)
                return ServiceResult<FormFieldDto>.BadRequest("Grid does not belong to this tab");
        }
    }
    
    // ... rest of create logic ...
}
```

**6. إضافة Endpoint:**
```csharp
// في FormFieldsController.cs
[HttpGet("by-grid/{gridId}")]
public async Task<IActionResult> GetFieldsByGridId(int gridId)
{
    var result = await _formFieldService.GetFieldsByGridIdAsync(gridId);
    return result.ToActionResult();
}
```

---

### Task 2: Bulk Grid Data Save Endpoint ⚠️

#### المشكلة:
لا يوجد endpoint لحفظ Grid data كاملة (rows + cells) عند submit الفورم.

#### الحل المطلوب:

**1. إضافة Method في Service:**
```csharp
// في FormSubmissionGridRowService.cs
public async Task<ApiResponse> SaveBulkGridDataAsync(BulkSaveGridDataDto bulkDto)
{
    // التحقق من Submission
    var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(bulkDto.SubmissionId);
    if (submission == null)
        return new ApiResponse(404, "Submission not found");
    
    // التحقق من Grid
    var grid = await _unitOfWork.FormGridRepository.GetByIdAsync(bulkDto.GridId);
    if (grid == null)
        return new ApiResponse(404, "Grid not found");
    
    // حذف البيانات القديمة
    await DeleteBySubmissionAndGridAsync(bulkDto.SubmissionId, bulkDto.GridId);
    
    // حفظ البيانات الجديدة
    var savedRows = new List<FormSubmissionGridRowDto>();
    
    foreach (var rowDto in bulkDto.Rows)
    {
        // إنشاء Row
        var createRowDto = new CreateFormSubmissionGridRowDto
        {
            SubmissionId = bulkDto.SubmissionId,
            GridId = bulkDto.GridId,
            RowIndex = rowDto.RowIndex,
            IsActive = true
        };
        
        var rowResult = await CreateAsync(createRowDto);
        if (rowResult.StatusCode != 200)
            return rowResult;
        
        var rowData = rowResult.Data as FormSubmissionGridRowDto;
        if (rowData == null) continue;
        
        // حفظ Cells
        foreach (var cellDto in rowDto.Cells)
        {
            var createCellDto = new CreateFormSubmissionGridCellDto
            {
                RowId = rowData.Id,
                ColumnId = cellDto.ColumnId,
                ValueString = cellDto.ValueString,
                ValueNumber = cellDto.ValueNumber,
                ValueDate = cellDto.ValueDate,
                ValueBool = cellDto.ValueBool,
                ValueJson = cellDto.ValueJson
            };
            
            await _formSubmissionGridCellService.CreateAsync(createCellDto);
        }
        
        savedRows.Add(rowData);
    }
    
    return new ApiResponse(200, "Grid data saved successfully", savedRows);
}
```

**2. إضافة Endpoint:**
```csharp
// في FormSubmissionGridRowsController.cs
[HttpPost("submission/{submissionId}/grid/{gridId}/bulk")]
[ProducesResponseType(typeof(ApiResponse<List<FormSubmissionGridRowDto>>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> SaveBulkGridData(
    int submissionId, 
    int gridId, 
    [FromBody] List<SaveFormSubmissionGridDto> rows)
{
    var bulkDto = new BulkSaveGridDataDto
    {
        SubmissionId = submissionId,
        GridId = gridId,
        Rows = rows
    };
    
    var result = await _formSubmissionGridRowService.SaveBulkGridDataAsync(bulkDto);
    return StatusCode(result.StatusCode, result);
}
```

---

### Task 3: Complete Grid Retrieval Endpoint ⚠️

#### المطلوب:
Endpoint لجلب Grid كامل مع جميع Rows و Cells.

**1. إضافة Method في Service:**
```csharp
// في FormSubmissionGridRowService.cs
public async Task<ApiResponse> GetCompleteGridDataAsync(int submissionId, int gridId)
{
    var rows = await _unitOfWork.FormSubmissionGridRowRepository
        .GetBySubmissionAndGridAsync(submissionId, gridId);
    
    var rowsWithCells = new List<FormSubmissionGridRowWithCellsDto>();
    
    foreach (var row in rows.Where(r => r.IsActive))
    {
        var rowDto = _mapper.Map<FormSubmissionGridRowDto>(row);
        var cells = await _unitOfWork.FormSubmissionGridCellRepository.GetByRowIdAsync(row.Id);
        var cellDtos = _mapper.Map<List<FormSubmissionGridCellDto>>(cells);
        
        rowsWithCells.Add(new FormSubmissionGridRowWithCellsDto
        {
            Id = rowDto.Id,
            SubmissionId = rowDto.SubmissionId,
            GridId = rowDto.GridId,
            RowIndex = rowDto.RowIndex,
            IsActive = rowDto.IsActive,
            Cells = cellDtos
        });
    }
    
    return new ApiResponse(200, "Complete grid data retrieved successfully", rowsWithCells);
}
```

**2. إضافة DTO:**
```csharp
// في FormSubmissionGridRowDto.cs
public class FormSubmissionGridRowWithCellsDto : FormSubmissionGridRowDto
{
    public List<FormSubmissionGridCellDto> Cells { get; set; } = new();
}
```

**3. إضافة Endpoint:**
```csharp
// في FormSubmissionGridRowsController.cs
[HttpGet("submission/{submissionId}/grid/{gridId}/complete")]
[ProducesResponseType(typeof(ApiResponse<List<FormSubmissionGridRowWithCellsDto>>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetCompleteGridData(int submissionId, int gridId)
{
    var result = await _formSubmissionGridRowService.GetCompleteGridDataAsync(submissionId, gridId);
    return StatusCode(result.StatusCode, result);
}
```

---

### Task 4: Grid Validation ⚠️

#### المطلوب:
Endpoint للتحقق من صحة بيانات Grid.

**1. إضافة Validation Service:**
```csharp
// في FormSubmissionGridRowService.cs
public async Task<ApiResponse> ValidateGridDataAsync(BulkSaveGridDataDto bulkDto)
{
    var errors = new List<GridValidationErrorDto>();
    var warnings = new List<GridValidationWarningDto>();
    
    // التحقق من Grid
    var grid = await _unitOfWork.FormGridRepository.GetByIdAsync(bulkDto.GridId);
    if (grid == null)
    {
        errors.Add(new GridValidationErrorDto
        {
            Field = "GridId",
            Message = "Grid not found"
        });
        return new ApiResponse(400, "Validation failed", new GridValidationResultDto
        {
            IsValid = false,
            Errors = errors,
            Warnings = warnings
        });
    }
    
    // جلب Columns
    var columns = await _unitOfWork.FormGridColumnRepository.GetByGridIdAsync(bulkDto.GridId);
    var requiredColumns = columns.Where(c => c.IsMandatory && c.IsActive).ToList();
    
    // التحقق من كل Row
    foreach (var row in bulkDto.Rows)
    {
        var rowIndex = bulkDto.Rows.IndexOf(row);
        
        // التحقق من Required Columns
        foreach (var column in requiredColumns)
        {
            var cell = row.Cells.FirstOrDefault(c => c.ColumnId == column.Id);
            if (cell == null || IsCellEmpty(cell, column))
            {
                errors.Add(new GridValidationErrorDto
                {
                    Field = column.ColumnName,
                    Message = $"{column.ColumnName} is required",
                    RowIndex = rowIndex,
                    ColumnId = column.Id
                });
            }
        }
        
        // التحقق من نوع البيانات
        foreach (var cell in row.Cells)
        {
            var column = columns.FirstOrDefault(c => c.Id == cell.ColumnId);
            if (column != null)
            {
                var validationResult = ValidateCellValue(cell, column);
                if (!validationResult.IsValid)
                {
                    errors.AddRange(validationResult.Errors.Select(e => new GridValidationErrorDto
                    {
                        Field = column.ColumnName,
                        Message = e,
                        RowIndex = rowIndex,
                        ColumnId = column.Id
                    }));
                }
            }
        }
    }
    
    return new ApiResponse(200, "Validation completed", new GridValidationResultDto
    {
        IsValid = errors.Count == 0,
        Errors = errors,
        Warnings = warnings
    });
}

private bool IsCellEmpty(SaveFormSubmissionGridCellDto cell, FORM_GRID_COLUMNS column)
{
    if (column.FieldTypeId == null) return true;
    
    // التحقق حسب نوع البيانات
    if (column.DataType == "string" || column.DataType == "text")
        return string.IsNullOrWhiteSpace(cell.ValueString);
    
    if (column.DataType == "number" || column.DataType == "decimal")
        return !cell.ValueNumber.HasValue;
    
    if (column.DataType == "date" || column.DataType == "datetime")
        return !cell.ValueDate.HasValue;
    
    if (column.DataType == "boolean")
        return !cell.ValueBool.HasValue;
    
    return true;
}
```

**2. إضافة DTOs:**
```csharp
// في FormSubmissionGridRowDto.cs
public class GridValidationResultDto
{
    public bool IsValid { get; set; }
    public List<GridValidationErrorDto> Errors { get; set; } = new();
    public List<GridValidationWarningDto> Warnings { get; set; } = new();
}

public class GridValidationErrorDto
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? RowIndex { get; set; }
    public int? ColumnId { get; set; }
}

public class GridValidationWarningDto
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? RowIndex { get; set; }
}
```

**3. إضافة Endpoint:**
```csharp
// في FormSubmissionGridRowsController.cs
[HttpPost("submission/{submissionId}/grid/{gridId}/validate")]
[ProducesResponseType(typeof(ApiResponse<GridValidationResultDto>), StatusCodes.Status200OK)]
public async Task<IActionResult> ValidateGridData(
    int submissionId, 
    int gridId, 
    [FromBody] List<SaveFormSubmissionGridDto> rows)
{
    var bulkDto = new BulkSaveGridDataDto
    {
        SubmissionId = submissionId,
        GridId = gridId,
        Rows = rows
    };
    
    var result = await _formSubmissionGridRowService.ValidateGridDataAsync(bulkDto);
    return StatusCode(result.StatusCode, result);
}
```

---

### Task 5: Integration with Form Submission ⚠️

#### المطلوب:
عند حفظ/إرسال الفورم، يجب حفظ Grid data تلقائياً.

**1. تعديل FormSubmissionService:**
```csharp
// في FormSubmissionService.cs
public async Task<ApiResponse> SaveFormSubmissionDataAsync(SaveFormSubmissionDataDto saveDto)
{
    // حفظ Submission
    var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(saveDto.SubmissionId);
    if (submission == null)
        return new ApiResponse(404, "Submission not found");
    
    // حفظ Field Values
    foreach (var fieldValue in saveDto.FieldValues)
    {
        // ... existing logic ...
    }
    
    // حفظ Attachments
    foreach (var attachment in saveDto.Attachments)
    {
        // ... existing logic ...
    }
    
    // حفظ Grid Data
    foreach (var gridData in saveDto.GridData)
    {
        var bulkDto = new BulkSaveGridDataDto
        {
            SubmissionId = saveDto.SubmissionId,
            GridId = gridData.GridId,
            Rows = new List<SaveFormSubmissionGridDto> { gridData }
        };
        
        // التحقق من البيانات أولاً
        var validationResult = await _formSubmissionGridRowService.ValidateGridDataAsync(bulkDto);
        if (validationResult.StatusCode == 200)
        {
            var validationData = validationResult.Data as GridValidationResultDto;
            if (validationData != null && !validationData.IsValid)
            {
                return new ApiResponse(400, "Grid validation failed", validationData);
            }
        }
        
        // حفظ البيانات
        await _formSubmissionGridRowService.SaveBulkGridDataAsync(bulkDto);
    }
    
    return new ApiResponse(200, "Form submission data saved successfully");
}
```

---

## 📋 ملخص المهام المطلوبة (Backend)

### Priority 1 (Critical):
1. ✅ **Grid Field Type Integration** - ربط Grid مع Field
2. ✅ **Bulk Grid Data Save** - حفظ Grid data كاملة
3. ✅ **Complete Grid Retrieval** - جلب Grid كامل مع cells
4. ✅ **Grid Validation** - التحقق من صحة البيانات
5. ✅ **Form Submission Integration** - دمج Grid مع Form Submission

### Priority 2 (Important):
6. ⚠️ **Grid Statistics** - إحصائيات Grid
7. ⚠️ **Row Operations** - نسخ، نقل، تبديل rows

### Priority 3 (Nice to have):
8. ⚠️ **Grid Export** - تصدير Grid إلى Excel
9. ⚠️ **Grid Import** - استيراد Grid من Excel

---

## 🔧 خطوات التنفيذ

### Step 1: Database Changes
```bash
# إضافة GridId إلى FORM_FIELDS
dotnet ef migrations add AddGridIdToFormFields --project FormBuilder.Core
dotnet ef database update --project FormBuilder.Core
```

### Step 2: Backend Implementation
1. تعديل DTOs
2. تعديل Entities
3. تعديل Services
4. إضافة Endpoints
5. إضافة Validation

### Step 3: Testing
1. Unit Tests
2. Integration Tests
3. Manual Testing

---

## 📝 ملاحظات مهمة

1. **DefaultValueJson**: يمكن استخدامه كـ fallback لحفظ Grid ID إذا لم نضف GridId column
2. **Validation**: يجب التحقق من Required columns و Data types
3. **Performance**: عند جلب Grid كبير، يجب استخدام pagination
4. **Transactions**: يجب استخدام transactions عند حفظ Grid data

---

## ❓ أسئلة شائعة

**س: ماذا لو كان Grid كبير جداً (1000+ row)؟**
ج: يجب إضافة pagination و lazy loading

**س: هل يمكن حذف Grid إذا كان مستخدم في Field؟**
ج: يجب التحقق من وجود Fields تستخدم Grid قبل الحذف

**س: ماذا عن Grid في Draft mode؟**
ج: يجب حفظ Grid data حتى في Draft mode

