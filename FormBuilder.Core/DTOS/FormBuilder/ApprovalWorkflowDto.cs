using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.Workflow
{
    // ================================
    // CREATE DTOs
    // ================================
    public class CreateApprovalWorkflowDto
    {
        [Required(ErrorMessage = "Document type ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid document type ID")]
        public int DocumentTypeId { get; set; }

        [Required(ErrorMessage = "Workflow name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Workflow name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
    }

    // ================================
    // UPDATE DTO
    // ================================
    public class UpdateApprovalWorkflowDto
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Workflow name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDefault { get; set; }
    }

    // ================================
    // RESPONSE DTOs
    // ================================
    public class ApprovalWorkflowDto
    {
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentTypeCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int StageCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<WorkflowStageDto> Stages { get; set; } = new List<WorkflowStageDto>();
    }

    public class WorkflowStageDto
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public string StageName { get; set; }
        public int StageOrder { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }

    public class ApprovalWorkflowBasicDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DocumentTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int StageCount { get; set; }
    }

    // ================================
    // SEARCH/FILTER DTOs
    // ================================
    public class WorkflowSearchDto
    {
        public string SearchTerm { get; set; }
        public int? DocumentTypeId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDefault { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
    }

    // ================================
    // BULK OPERATION DTOs
    // ================================
    public class BulkWorkflowStatusDto
    {
        [Required]
        public List<int> WorkflowIds { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

    public class BulkCreateWorkflowDto
    {
        [Required]
        public int DocumentTypeId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateApprovalWorkflowDto> Workflows { get; set; }
    }

    // ================================
    // VALIDATION DTOs
    // ================================
    public class WorkflowNameCheckDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int DocumentTypeId { get; set; }

        public int? ExcludeWorkflowId { get; set; }
    }

    // ================================
    // STATISTICS DTOs
    // ================================
    public class WorkflowStatisticsDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int TotalWorkflows { get; set; }
        public int ActiveWorkflows { get; set; }
        public int InactiveWorkflows { get; set; }
        public int WorkflowsWithStages { get; set; }
        public int WorkflowsWithoutStages { get; set; }
        public int DefaultWorkflows { get; set; }
        public DateTime? OldestWorkflowDate { get; set; }
        public DateTime? NewestWorkflowDate { get; set; }
    }

    public class OverallWorkflowStatisticsDto
    {
        public int TotalWorkflows { get; set; }
        public int ActiveWorkflows { get; set; }
        public int InactiveWorkflows { get; set; }
        public int TotalDocumentTypes { get; set; }
        public int DocumentTypesWithWorkflows { get; set; }
        public int DocumentTypesWithoutWorkflows { get; set; }
        public int WorkflowsWithStages { get; set; }
        public int WorkflowsWithoutStages { get; set; }
        public int DefaultWorkflows { get; set; }
        public List<DocumentTypeWorkflowCountDto> WorkflowCountByDocumentType { get; set; } = new List<DocumentTypeWorkflowCountDto>();
    }

    public class DocumentTypeWorkflowCountDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int WorkflowCount { get; set; }
        public int ActiveWorkflowCount { get; set; }
    }

    // ================================
    // RESPONSE DTOs FOR BULK OPERATIONS
    // ================================
    public class BulkCreateWorkflowResponseDto
    {
        public int DocumentTypeId { get; set; }
        public int TotalRequested { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<ApprovalWorkflowDto> CreatedWorkflows { get; set; } = new List<ApprovalWorkflowDto>();
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    }

    public class BulkStatusUpdateResponseDto
    {
        public int TotalRequested { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<int> FailedIds { get; set; } = new List<int>();
        public List<string> FailedReasons { get; set; } = new List<string>();
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    }

    // ================================
    // PAGINATED RESPONSE
    // ================================
    public class PaginatedWorkflowResponseDto
    {
        public List<ApprovalWorkflowDto> Workflows { get; set; } = new List<ApprovalWorkflowDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}