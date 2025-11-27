using System;

namespace FormBuilder.API.Models.DTOs
{
    public class FormSubmissionDto
    {
        public int Id { get; set; }
        public int FormBuilderId { get; set; }
        public int Version { get; set; }
        public int DocumentTypeId { get; set; }
        public int SeriesId { get; set; }
        public string DocumentNumber { get; set; }
        public string SubmittedByUserId { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        // Navigation properties (اختياري)
        public string FormBuilderName { get; set; }
        public string DocumentTypeName { get; set; }
        public string SeriesName { get; set; }
        public string SubmittedByUserName { get; set; }
    }

    public class CreateFormSubmissionDto
    {
        public int FormBuilderId { get; set; }
        public int Version { get; set; } = 1;
        public int DocumentTypeId { get; set; }
        public int SeriesId { get; set; }
        public string DocumentNumber { get; set; }
        public string SubmittedByUserId { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string Status { get; set; } = "Draft";

        // يمكن إضافة بيانات النموذج الديناميكية
        public object FormData { get; set; }
    }

    public class UpdateFormSubmissionDto
    {
        public string Status { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? SubmittedDate { get; set; }

        // بيانات النموذج المحدثة
        public object FormData { get; set; }
    }

    public class FormSubmissionStatusDto
    {
        public string Status { get; set; }
    }

    public class FormSubmissionFilterDto
    {
        public int? FormBuilderId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string DocumentNumber { get; set; }
    }

    public class FormSubmissionResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<FormSubmissionDto> Submissions { get; set; }
    }
}