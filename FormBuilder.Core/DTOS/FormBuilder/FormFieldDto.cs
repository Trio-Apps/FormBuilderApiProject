using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormFields
{
    public class UpdateFormFieldDto
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "TabId is required")]
        public int TabId { get; set; }

        [Required(ErrorMessage = "FieldTypeId is required")]
        public int FieldTypeId { get; set; }

        [Required(ErrorMessage = "FieldName is required")]
        [StringLength(200, ErrorMessage = "FieldName cannot exceed 200 characters")]
        public string FieldName { get; set; }

        [Required(ErrorMessage = "FieldCode is required")]
        [StringLength(100, ErrorMessage = "FieldCode cannot exceed 100 characters")]
        public string FieldCode { get; set; }

        [Required(ErrorMessage = "DataType is required")]
        [StringLength(50, ErrorMessage = "DataType cannot exceed 50 characters")]
        public string DataType { get; set; }

        public int FieldOrder { get; set; }

        [StringLength(200, ErrorMessage = "Placeholder cannot exceed 200 characters")]
        public string Placeholder { get; set; }

        [StringLength(500, ErrorMessage = "HintText cannot exceed 500 characters")]
        public string HintText { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsEditable { get; set; }

        public bool IsVisible { get; set; }

        public string DefaultValueJson { get; set; }

        public int? MaxLength { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MinValue must be a positive number")]
        public decimal? MinValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MaxValue must be a positive number")]
        public decimal? MaxValue { get; set; }

        [StringLength(500, ErrorMessage = "RegexPattern cannot exceed 500 characters")]
        public string RegexPattern { get; set; }

        [StringLength(500, ErrorMessage = "ValidationMessage cannot exceed 500 characters")]
        public string ValidationMessage { get; set; }

        public string VisibilityRuleJson { get; set; }

        public string ReadOnlyRuleJson { get; set; }

        public bool IsActive { get; set; }

        // خصائص إضافية
        public bool ShowInList { get; set; }

        public bool IsSearchable { get; set; }

        public string CssClass { get; set; }

        public string Style { get; set; }

        public List<FieldOptionDto> FieldOptions { get; set; } = new List<FieldOptionDto>();
    }

    public class FormFieldDto
    {
        public int Id { get; set; }
        public int TabId { get; set; }
        public int FieldTypeId { get; set; }
        public string FieldName { get; set; }
        public string FieldCode { get; set; }
        public string DataType { get; set; }
        public int FieldOrder { get; set; }
        public string Placeholder { get; set; }
        public string HintText { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
        public string DefaultValueJson { get; set; }
        public int? MaxLength { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string RegexPattern { get; set; }
        public string ValidationMessage { get; set; }
        public string VisibilityRuleJson { get; set; }
        public string ReadOnlyRuleJson { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        // خصائص إضافية
        public bool ShowInList { get; set; }
        public bool IsSearchable { get; set; }
        public string CssClass { get; set; }
        public string Style { get; set; }

        // Navigation Properties للعرض
        public string TabName { get; set; }
        public string FieldTypeName { get; set; }
        public string FieldTypeCode { get; set; }
        public List<FieldOptionDto> FieldOptions { get; set; } = new List<FieldOptionDto>();

        // معلومات إضافية للعرض
        public string Status => IsActive ? "Active" : "Inactive";
        public string MandatoryStatus => IsMandatory ? "Required" : "Optional";
        public string VisibilityStatus => IsVisible ? "Visible" : "Hidden";
        public string EditableStatus => IsEditable ? "Editable" : "Readonly";
    }
}