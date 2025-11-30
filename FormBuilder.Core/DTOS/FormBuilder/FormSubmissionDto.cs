using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    // ================================
    // MAIN SUBMISSION DTOs
    // ================================

    public class FormSubmissionDto
    {
        public int Id { get; set; }
        public int FormBuilderId { get; set; }
        public string FormName { get; set; }
        public int Version { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int SeriesId { get; set; }
        public string SeriesCode { get; set; }
        public string DocumentNumber { get; set; }
        public string SubmittedByUserId { get; set; }
        public string SubmittedByUserName { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }

    public class FormSubmissionDetailDto : FormSubmissionDto
    {
        public List<FormSubmissionValueDto> FieldValues { get; set; } = new();
        public List<FormSubmissionAttachmentDto> Attachments { get; set; } = new();
        public List<FormSubmissionGridDto> GridData { get; set; } = new();
    }

    public class CreateFormSubmissionDto
    {
        [Required]
        public int FormBuilderId { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }

        [Required]
        public int SeriesId { get; set; }

        [Required]
        public string SubmittedByUserId { get; set; }

        public string Status { get; set; } = "Draft";
    }

    public class UpdateFormSubmissionDto
    {
        public string DocumentNumber { get; set; }
        public string Status { get; set; }
        public DateTime? SubmittedDate { get; set; }
    }

    // ================================
    // FIELD VALUES DTOs (FORM_SUBMISSION_VALUES)
    // ================================

    public class FormSubmissionValueDto
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int FieldId { get; set; }
        public string FieldCode { get; set; }
        public string FieldName { get; set; }
        public string ValueString { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBool { get; set; }
        public string ValueJson { get; set; }
    }

    public class SaveFormSubmissionValueDto
    {
        [Required]
        public int FieldId { get; set; }

        [Required]
        public string FieldCode { get; set; }

        public string ValueString { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBool { get; set; }
        public string ValueJson { get; set; }
    }

    public class BulkSaveFieldValuesDto
    {
        [Required]
        public int SubmissionId { get; set; }

        public List<SaveFormSubmissionValueDto> FieldValues { get; set; } = new();
    }

    // ================================
    // ATTACHMENTS DTOs (FORM_SUBMISSION_ATTACHMENTS)
    // ================================


    public class SaveFormSubmissionAttachmentDto
    {
        [Required]
        public int FieldId { get; set; }

        [Required]
        public string FieldCode { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Required]
        public string ContentType { get; set; }
    }

    public class UploadAttachmentDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public int FieldId { get; set; }

        [Required]
        public string FieldCode { get; set; }
    }

    // ================================
    // GRID DATA DTOs (FORM_SUBMISSION_GRID_ROWS & FORM_SUBMISSION_GRID_CELLS)
    // ================================

    public class FormSubmissionGridDto
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public int GridId { get; set; }
        public string GridName { get; set; }
        public string GridCode { get; set; }
        public int RowIndex { get; set; }
        public List<FormSubmissionGridCellDto> Cells { get; set; } = new();
    }

    public class FormSubmissionGridCellDto
    {
        public int Id { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public string ColumnName { get; set; }
        public string ColumnCode { get; set; }
        public string ValueString { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBool { get; set; }
        public string ValueJson { get; set; }
    }

    public class SaveFormSubmissionGridDto
    {
        [Required]
        public int GridId { get; set; }

        [Required]
        public int RowIndex { get; set; }

        public List<SaveFormSubmissionGridCellDto> Cells { get; set; } = new();
    }

    public class SaveFormSubmissionGridCellDto
    {
        [Required]
        public int ColumnId { get; set; }

        [Required]
        public string ColumnCode { get; set; }

        public string ValueString { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBool { get; set; }
        public string ValueJson { get; set; }
    }

    public class BulkSaveGridDataDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public int GridId { get; set; }

        public List<SaveFormSubmissionGridDto> Rows { get; set; } = new();
    }

    // ================================
    // OPERATION DTOs
    // ================================

    public class SubmitFormDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public string SubmittedByUserId { get; set; }
    }

    public class ChangeStatusDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public string Status { get; set; }

        public string Comments { get; set; }
    }

    public class SaveFormSubmissionDataDto
    {
        [Required]
        public int SubmissionId { get; set; }

        public List<SaveFormSubmissionValueDto> FieldValues { get; set; } = new();
        public List<SaveFormSubmissionAttachmentDto> Attachments { get; set; } = new();
        public List<SaveFormSubmissionGridDto> GridData { get; set; } = new();
    }

    // ================================
    // FILTER & SEARCH DTOs
    // ================================

    public class FormSubmissionFilterDto
    {
        public int? FormBuilderId { get; set; }
        public int? DocumentTypeId { get; set; }
        public string Status { get; set; }
        public string SubmittedByUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DocumentNumber { get; set; }
        public string SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class FormSubmissionSearchResultDto
    {
        public List<FormSubmissionDto> Submissions { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    // ================================
    // STATISTICS & REPORTING DTOs
    // ================================

    public class FormSubmissionStatisticsDto
    {
        public int TotalSubmissions { get; set; }
        public int DraftCount { get; set; }
        public int SubmittedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int PendingApprovalCount { get; set; }
        public Dictionary<string, int> SubmissionsByDocumentType { get; set; } = new();
        public Dictionary<string, int> SubmissionsByStatus { get; set; } = new();
        public Dictionary<string, int> SubmissionsByMonth { get; set; } = new();
    }

    public class SubmissionTrendDto
    {
        public string Period { get; set; } // "2024-01", "2024-02", etc.
        public int Count { get; set; }
    }

    public class UserSubmissionStatsDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int TotalSubmissions { get; set; }
        public int DraftCount { get; set; }
        public int SubmittedCount { get; set; }
        public int ApprovedCount { get; set; }
    }

    // ================================
    // VALIDATION & WORKFLOW DTOs
    // ================================

    public class SubmissionValidationResultDto
    {
        public bool IsValid { get; set; }
        public List<ValidationErrorDto> Errors { get; set; } = new();
        public List<ValidationWarningDto> Warnings { get; set; } = new();
    }

    public class ValidationErrorDto
    {
        public string FieldCode { get; set; }
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; } // Required, Format, Range, etc.
    }

    public class ValidationWarningDto
    {
        public string FieldCode { get; set; }
        public string FieldName { get; set; }
        public string WarningMessage { get; set; }
    }

    public class WorkflowActionDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public string Action { get; set; } // Approve, Reject, Return, Cancel

        public string Comments { get; set; }
        public string PerformedByUserId { get; set; }
    }

    // ================================
    // EXPORT & IMPORT DTOs
    // ================================

    public class ExportSubmissionsDto
    {
        public FormSubmissionFilterDto Filter { get; set; }
        public string ExportFormat { get; set; } // Excel, PDF, CSV
        public List<string> IncludeFields { get; set; } = new();
    }

    public class ImportSubmissionDto
    {
        [Required]
        public int FormBuilderId { get; set; }

        [Required]
        public string FileContent { get; set; } // Base64 encoded file

        [Required]
        public string FileName { get; set; }

        public string ImportedByUserId { get; set; }
    }

    public class ImportResultDto
    {
        public bool Success { get; set; }
        public int ImportedCount { get; set; }
        public int FailedCount { get; set; }
        public List<ImportErrorDto> Errors { get; set; } = new();
        public string Message { get; set; }
    }

    public class ImportErrorDto
    {
        public int RowNumber { get; set; }
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
    }

    // ================================
    // HISTORY & AUDIT DTOs
    // ================================

    public class SubmissionHistoryDto
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string PerformedByUserId { get; set; }
        public string PerformedByUserName { get; set; }
        public DateTime PerformedDate { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }

    // ================================
    // DASHBOARD & SUMMARY DTOs
    // ================================

    public class SubmissionSummaryDto
    {
        public int TodayCount { get; set; }
        public int ThisWeekCount { get; set; }
        public int ThisMonthCount { get; set; }
        public int PendingApprovalCount { get; set; }
        public int MyDraftCount { get; set; }
        public List<RecentSubmissionDto> RecentSubmissions { get; set; } = new();
    }

    public class RecentSubmissionDto
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; }
        public string FormName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SubmittedByUserName { get; set; }
    }
}