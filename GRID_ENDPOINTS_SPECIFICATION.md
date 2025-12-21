# Grid Support (Line Items Grid) - Backend Endpoints Specification

## نظرة عامة
الـ Grid هو field type خاص يمثل مجموعة قابلة للتكرار من الحقول (Line Items) داخل الـ Form. كل row يعامل كـ sub-record مرتب بالـ parent form.

---

## 1. Grid Schema Management (Form Grids)

### Base Route: `/api/FormGrids`

#### ✅ موجودة بالفعل:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/FormGrids` | Get all grids |
| GET | `/api/FormGrids/{id}` | Get grid by ID |
| GET | `/api/FormGrids/by-form-builder/{formBuilderId}` | Get grids by form builder |
| GET | `/api/FormGrids/by-tab/{tabId}` | Get grids by tab |
| GET | `/api/FormGrids/active-by-form-builder/{formBuilderId}` | Get active grids |
| GET | `/api/FormGrids/by-code/{gridCode}/{formBuilderId}` | Get grid by code |
| POST | `/api/FormGrids` | Create new grid |
| PUT | `/api/FormGrids/{id}` | Update grid |
| DELETE | `/api/FormGrids/{id}` | Delete grid |
| PATCH | `/api/FormGrids/{id}/toggle-active` | Toggle active status |
| GET | `/api/FormGrids/exists/{id}` | Check if grid exists |
| GET | `/api/FormGrids/code-exists/{gridCode}/{formBuilderId}` | Check if code exists |
| GET | `/api/FormGrids/next-order/{formBuilderId}` | Get next order |

---

## 2. Grid Columns Management

### Base Route: `/api/FormGridColumns`

#### ✅ موجودة بالفعل:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/FormGridColumns` | Get all columns |
| GET | `/api/FormGridColumns/{id}` | Get column by ID |
| GET | `/api/FormGridColumns/by-grid/{gridId}` | Get columns by grid ID |
| GET | `/api/FormGridColumns/active-by-grid/{gridId}` | Get active columns |
| GET | `/api/FormGridColumns/by-code/{columnCode}/{gridId}` | Get column by code |
| POST | `/api/FormGridColumns` | Create column |
| PUT | `/api/FormGridColumns/{id}` | Update column |
| DELETE | `/api/FormGridColumns/{id}` | Delete column |
| PATCH | `/api/FormGridColumns/{id}/toggle-active` | Toggle active |
| GET | `/api/FormGridColumns/exists/{id}` | Check if exists |
| GET | `/api/FormGridColumns/code-exists/{columnCode}/{gridId}` | Check code exists |
| GET | `/api/FormGridColumns/next-order/{gridId}` | Get next order |

---

## 3. Grid Rows Management (Submission Data)

### Base Route: `/api/FormSubmissionGridRows`

#### ✅ موجودة بالفعل:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/FormSubmissionGridRows` | Get all rows |
| GET | `/api/FormSubmissionGridRows/{id}` | Get row by ID |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}` | Get rows by submission |
| GET | `/api/FormSubmissionGridRows/grid/{gridId}` | Get rows by grid |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}` | Get rows by submission & grid |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/active` | Get active rows |
| POST | `/api/FormSubmissionGridRows` | Create single row |
| POST | `/api/FormSubmissionGridRows/multiple` | Create multiple rows |
| PUT | `/api/FormSubmissionGridRows/{id}` | Update row |
| DELETE | `/api/FormSubmissionGridRows/{id}` | Delete single row |
| DELETE | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}` | Delete all rows for grid |
| PATCH | `/api/FormSubmissionGridRows/{id}/toggle-active` | Toggle active |
| GET | `/api/FormSubmissionGridRows/exists/{id}` | Check if exists |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/row-index/{rowIndex}/exists` | Check row index exists |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/next-index` | Get next row index |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/count` | Get row count by submission |
| GET | `/api/FormSubmissionGridRows/grid/{gridId}/count` | Get row count by grid |
| GET | `/api/FormSubmissionGridRows/form-builder/{formBuilderId}` | Get by form builder |
| POST | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/reorder` | Reorder rows |

---

## 4. Grid Cells Management (Cell Data)

### Base Route: `/api/FormSubmissionGridCells`

#### ✅ موجودة بالفعل:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/FormSubmissionGridCells` | Get all cells |
| GET | `/api/FormSubmissionGridCells/{id}` | Get cell by ID |
| GET | `/api/FormSubmissionGridCells/row/{rowId}` | Get cells by row |
| GET | `/api/FormSubmissionGridCells/row/{rowId}/column/{columnId}` | Get cell by row & column |
| POST | `/api/FormSubmissionGridCells` | Create cell |
| PUT | `/api/FormSubmissionGridCells/{id}` | Update cell |
| DELETE | `/api/FormSubmissionGridCells/{id}` | Delete cell |
| DELETE | `/api/FormSubmissionGridCells/row/{rowId}` | Delete all cells in row |

---

## 5. ⚠️ Endpoints المطلوبة إضافتها (Missing/Enhancements)

### 5.1 Bulk Operations for Grid Data

#### 🔴 مطلوب إضافتها:

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| POST | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/bulk` | Save entire grid data (rows + cells) | `BulkSaveGridDataDto` |
| PUT | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/bulk` | Update entire grid data | `BulkSaveGridDataDto` |
| POST | `/api/FormSubmissionGridCells/row/{rowId}/bulk` | Save all cells for a row | `List<CreateFormSubmissionGridCellDto>` |
| PUT | `/api/FormSubmissionGridCells/row/{rowId}/bulk` | Update all cells for a row | `List<UpdateFormSubmissionGridCellDto>` |

### 5.2 Grid Data Retrieval (Complete Grid with Cells)

#### 🔴 مطلوب إضافتها:

| Method | Endpoint | Description | Response |
|--------|----------|-------------|----------|
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/complete` | Get complete grid data (rows + cells) | `List<FormSubmissionGridRowDto>` with nested cells |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/complete/active` | Get active complete grid data | Same as above |

### 5.3 Grid Validation

#### 🔴 مطلوب إضافتها:

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| POST | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/validate` | Validate grid data | `BulkSaveGridDataDto` |
| POST | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/validate-row` | Validate single row | `CreateFormSubmissionGridRowDto` + cells |

### 5.4 Grid Statistics/Analytics

#### 🔴 مطلوب إضافتها:

| Method | Endpoint | Description | Response |
|--------|----------|-------------|----------|
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/stats` | Get grid statistics | `GridStatsDto` (total rows, total cells, etc.) |
| GET | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/summary` | Get grid summary | Summary of grid data |

### 5.5 Grid Row Operations

#### 🔴 مطلوب إضافتها:

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| POST | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/duplicate/{rowId}` | Duplicate a row with all cells | - |
| POST | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/move` | Move row to new index | `{ rowId: int, newIndex: int }` |
| POST | `/api/FormSubmissionGridRows/submission/{submissionId}/grid/{gridId}/swap` | Swap two rows | `{ rowId1: int, rowId2: int }` |

---

## 6. DTOs المطلوبة

### 6.1 ✅ موجودة بالفعل:
- `FormGridDto`, `CreateFormGridDto`, `UpdateFormGridDto`
- `FormGridColumnDto`, `CreateFormGridColumnDto`, `UpdateFormGridColumnDto`
- `FormSubmissionGridRowDto`, `CreateFormSubmissionGridRowDto`, `UpdateFormSubmissionGridRowDto`
- `FormSubmissionGridCellDto`, `CreateFormSubmissionGridCellDto`, `UpdateFormSubmissionGridCellDto`
- `BulkSaveGridDataDto` (في FormSubmissionDto.cs)

### 6.2 🔴 مطلوب إضافتها:

```csharp
// Grid Statistics DTO
public class GridStatsDto
{
    public int GridId { get; set; }
    public string GridName { get; set; }
    public int TotalRows { get; set; }
    public int ActiveRows { get; set; }
    public int TotalCells { get; set; }
    public Dictionary<string, int> RowsBySubmission { get; set; }
}

// Grid Summary DTO
public class GridSummaryDto
{
    public int GridId { get; set; }
    public string GridName { get; set; }
    public int RowCount { get; set; }
    public List<ColumnSummaryDto> ColumnSummaries { get; set; }
}

public class ColumnSummaryDto
{
    public int ColumnId { get; set; }
    public string ColumnName { get; set; }
    public int FilledCells { get; set; }
    public int EmptyCells { get; set; }
}

// Validation Result DTO
public class GridValidationResultDto
{
    public bool IsValid { get; set; }
    public List<ValidationErrorDto> Errors { get; set; }
    public List<ValidationWarningDto> Warnings { get; set; }
}

public class ValidationErrorDto
{
    public string Field { get; set; }
    public string Message { get; set; }
    public int? RowIndex { get; set; }
    public int? ColumnId { get; set; }
}
```

---

## 7. ملخص الـ Endpoints المطلوبة

### ✅ موجودة (90%):
- Grid Schema Management: ✅ كاملة
- Grid Columns Management: ✅ كاملة
- Grid Rows CRUD: ✅ كاملة
- Grid Cells CRUD: ✅ كاملة

### 🔴 مطلوب إضافتها (10%):
1. **Bulk Operations** (4 endpoints)
2. **Complete Grid Retrieval** (2 endpoints)
3. **Validation** (2 endpoints)
4. **Statistics/Analytics** (2 endpoints)
5. **Row Operations** (3 endpoints)

**إجمالي المطلوب: 13 endpoint جديد**

---

## 8. ملاحظات مهمة

1. **Authorization**: جميع الـ endpoints تحتاج `[Authorize(Roles = "Administration")]`
2. **Response Format**: جميع الـ responses تستخدم `ApiResponse<T>`
3. **Error Handling**: يجب استخدام `StatusCode(result.StatusCode, result)`
4. **Validation**: يجب إضافة FluentValidation للـ DTOs الجديدة
5. **Documentation**: يجب إضافة XML comments و `ProducesResponseType` attributes

---

## 9. أولويات التنفيذ

### Priority 1 (Critical):
- ✅ Bulk save/update operations
- ✅ Complete grid retrieval with cells

### Priority 2 (Important):
- ✅ Validation endpoints
- ✅ Row operations (duplicate, move, swap)

### Priority 3 (Nice to have):
- ✅ Statistics/Analytics endpoints


