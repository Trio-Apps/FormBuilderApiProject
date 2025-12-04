using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormBuilder
{
    // Create DTOs
    public class CreateFormulaVariableDto
    {
        [Required(ErrorMessage = "Formula ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid formula ID")]
        public int FormulaId { get; set; }

        [Required(ErrorMessage = "Variable name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Variable name must be between 1 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$",
            ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }

        [Required(ErrorMessage = "Source field ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid source field ID")]
        public int SourceFieldId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class BulkCreateFormulaVariableItemDto
    {
        [Required(ErrorMessage = "Variable name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Variable name must be between 1 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$",
            ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }

        [Required(ErrorMessage = "Source field ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid source field ID")]
        public int SourceFieldId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class BulkCreateFormulaVariablesDto
    {
        [Required(ErrorMessage = "Formula ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid formula ID")]
        public int FormulaId { get; set; }

        [Required(ErrorMessage = "Variables list is required")]
        [MinLength(1, ErrorMessage = "At least one variable is required")]
        [MaxLength(100, ErrorMessage = "Cannot create more than 100 variables at once")]
        public List<BulkCreateFormulaVariableItemDto> Variables { get; set; } = new List<BulkCreateFormulaVariableItemDto>();
    }

    // Update DTO
    public class UpdateFormulaVariableDto
    {
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Variable name must be between 1 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$",
            ErrorMessage = "Variable name must start with a letter or underscore and contain only letters, numbers, and underscores")]
        public string VariableName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid source field ID")]
        public int? SourceFieldId { get; set; }

        public bool? IsActive { get; set; }
    }

    // Main Response DTO
    public class FormulaVariableDto
    {
        public int Id { get; set; }
        public int FormulaId { get; set; }
        public string FormulaName { get; set; }
        public string FormulaCode { get; set; }
        public string VariableName { get; set; }
        public int SourceFieldId { get; set; }
        public string SourceFieldCode { get; set; }
        public string SourceFieldName { get; set; }
        public string SourceFieldType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    // Response DTOs
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
        public int VariableCount { get; set; }
    }

    public class VariableStatisticsDto
    {
        public int FormulaId { get; set; }
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

    // Bulk Operation Response DTOs
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
}

