using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    // Core DTOs
    public class CreateFormulaVariableDto
    {
        [Required]
        public int FormulaId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$", ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }

        [Required]
        public int SourceFieldId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class BulkCreateFormulaVariableItemDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$", ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }

        [Required]
        public int SourceFieldId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class BulkCreateFormulaVariablesDto
    {
        [Required]
        public int FormulaId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)] // Prevent too many variables at once
        public List<BulkCreateFormulaVariableItemDto> Variables { get; set; } = new List<BulkCreateFormulaVariableItemDto>();
    }

    public class UpdateFormulaVariableDto
    {
        [StringLength(100, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$", ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid SourceFieldId")]
        public int? SourceFieldId { get; set; }

        public bool? IsActive { get; set; }
    }

    // Main response DTO
    public class FormulaVariableDto
    {
        public int Id { get; set; }

        // Formula information
        public int FormulaId { get; set; }
        public string FormulaName { get; set; }
        public string FormulaCode { get; set; }

        // Variable information
        public string VariableName { get; set; }

        // Source field information
        public int SourceFieldId { get; set; }
        public string SourceFieldCode { get; set; }
        public string SourceFieldName { get; set; }
        public string SourceFieldType { get; set; }

        // Status
        public bool IsActive { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Extended information (optional)
        public FormulaInfoDto FormulaInfo { get; set; }
        public FieldInfoDto FieldInfo { get; set; }
    }

    // Support DTOs for nested information
    public class FormulaInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Expression { get; set; }
        public int FormBuilderId { get; set; }
        public bool IsActive { get; set; }
    }

    public class FieldInfoDto
    {
        public int Id { get; set; }
        public string FieldCode { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public int? FieldTypeId { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
    }

    // Filter/Search DTOs
    public class FormulaVariableFilterDto
    {
        public int? FormulaId { get; set; }
        public int? FormBuilderId { get; set; }
        public string VariableName { get; set; }
        public int? SourceFieldId { get; set; }
        public string FieldType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "VariableName";
        public bool SortDescending { get; set; } = false;
    }

    public class FormulaVariableSearchDto
    {
        [Required]
        public int FormulaId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string SearchTerm { get; set; }

        public bool IncludeInactive { get; set; } = false;
    }

    // Validation DTOs
    public class VariableNameCheckDto
    {
        [Required]
        public string VariableName { get; set; }

        [Required]
        public int FormulaId { get; set; }

        public int? ExcludeVariableId { get; set; }
    }

    public class SourceFieldUsageCheckDto
    {
        [Required]
        public int FieldId { get; set; }

        public int? ExcludeFormulaId { get; set; }
    }

    public class FormulaVariableValidationDto
    {
        [Required]
        public int FormulaId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$", ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SourceFieldId { get; set; }
    }

    // Batch operation DTOs
    public class BatchToggleActiveDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)] // Limit batch size
        public List<int> VariableIds { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

    public class BatchDeleteDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public List<int> VariableIds { get; set; }
    }

    // Response DTOs for specific queries
    public class VariableMappingDto
    {
        public string VariableName { get; set; }
        public int SourceFieldId { get; set; }
        public string SourceFieldCode { get; set; }
        public string SourceFieldName { get; set; }
        public string SourceFieldType { get; set; }
    }

    public class VariableCountDto
    {
        public int FormulaId { get; set; }
        public string FormulaName { get; set; }
        public int VariableCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
    }

    public class VariableNameDto
    {
        public string VariableName { get; set; }
        public int VariableId { get; set; }
    }

    public class VariableStatisticsDto
    {
        public int FormulaId { get; set; }
        public string FormulaName { get; set; }
        public int TotalVariables { get; set; }
        public int ActiveVariables { get; set; }
        public int InactiveVariables { get; set; }
        public bool HasVariables { get; set; }
        public bool HasActiveVariables { get; set; }
        public List<VariableTypeCountDto> TypeCounts { get; set; } = new List<VariableTypeCountDto>();
    }

    public class VariableTypeCountDto
    {
        public string FieldType { get; set; }
        public int Count { get; set; }
    }

    // Update specific properties
    public class UpdateVariableNameDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$", ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }
    }

    public class UpdateSourceFieldDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int SourceFieldId { get; set; }
    }

    public class UpdateStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }

    // Paginated response
    public class PagedFormulaVariablesDto
    {
        public List<FormulaVariableDto> Variables { get; set; } = new List<FormulaVariableDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    // Bulk operation responses
    public class BulkCreateResponseDto
    {
        public int FormulaId { get; set; }
        public int TotalRequested { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<FormulaVariableDto> CreatedVariables { get; set; } = new List<FormulaVariableDto>();
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    }

    public class BatchToggleResponseDto
    {
        public int TotalRequested { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<int> FailedIds { get; set; } = new List<int>();
        public List<string> FailedReasons { get; set; } = new List<string>();
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    }

    // Excel/Import DTOs
    public class ImportFormulaVariableDto
    {
        public string VariableName { get; set; }
        public string SourceFieldCode { get; set; }
        public string SourceFieldName { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ImportFormulaVariablesDto
    {
        [Required]
        public int FormulaId { get; set; }

        [Required]
        [MinLength(1)]
        public List<ImportFormulaVariableDto> Variables { get; set; } = new List<ImportFormulaVariableDto>();

        public bool OverwriteExisting { get; set; } = false;
        public bool SkipInvalidRows { get; set; } = true;
    }
}