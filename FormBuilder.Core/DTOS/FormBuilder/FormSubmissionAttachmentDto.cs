using System;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    public class FormSubmissionAttachmentDto
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public string? SubmissionDocumentNumber { get; set; }
        public int FieldId { get; set; }
        public string? FieldCode { get; set; }
        public string? FieldName { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
        public string? FileSizeFormatted { get; set; }
        public string? DownloadUrl { get; set; }
    }

    public class CreateFormSubmissionAttachmentDto
    {
        [Required]
        public int SubmissionId { get; set; }

        [Required]
        public int FieldId { get; set; }

        [Required, StringLength(100)]
        public string FieldCode { get; set; } = string.Empty;

        [Required, StringLength(260)]
        public string FileName { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0")]
        public long FileSize { get; set; }

        [Required, StringLength(100)]
        public string ContentType { get; set; } = string.Empty;
    }

    public class UpdateFormSubmissionAttachmentDto
    {
        [StringLength(260)]
        public string? FileName { get; set; }

        [StringLength(500)]
        public string? FilePath { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0")]
        public long? FileSize { get; set; }

        [StringLength(100)]
        public string? ContentType { get; set; }
    }

   

    public class AttachmentUploadResultDto
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class BulkAttachmentsDto
    {
        [Required]
        public int SubmissionId { get; set; }

        public List<CreateFormSubmissionAttachmentDto> Attachments { get; set; } = new();
    }

    public class AttachmentStatsDto
    {
        public int SubmissionId { get; set; }
        public int TotalAttachments { get; set; }
        public long TotalSize { get; set; }
        public string TotalSizeFormatted { get; set; } = string.Empty;
        public Dictionary<string, int> AttachmentsByType { get; set; } = new();
    }
}