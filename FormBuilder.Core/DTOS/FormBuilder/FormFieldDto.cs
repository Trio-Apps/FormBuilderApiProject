using FormBuilder.Core.DTOS.FormFields;
using FormBuilder.Core.DTOS.FormTabs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FormBuilder.API.Models
{
    public class FormFieldDto
    {
        public int Id { get; set; }

        [Required]
        public int TabId { get; set; }

        [Required]
        public int FieldTypeId { get; set; }
        public string? FieldTypeName { get; set; }

        [Required, StringLength(200)]
        public string FieldName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FieldCode { get; set; } = string.Empty;

        [Required]
        public int FieldOrder { get; set; }

        public string? Placeholder { get; set; }
        public string HintText { get; set; }

        [Required]
        public bool ?IsMandatory { get; set; }

        [Required]
        public bool ?IsEditable { get; set; }

        [Required]
        public bool ?IsVisible { get; set; }

        public string? DefaultValueJson { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string? RegexPattern { get; set; }
        public string? ValidationMessage { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Navigation properties as DTOs
        // Ignored in JSON to prevent circular reference with FormTabDto.Fields (helps Swagger schema generation)
        [JsonIgnore]
        public FormTabDto? Tab { get; set; }
        public FieldTypeDto? FieldType { get; set; }
        public List<FieldOptionDto> FieldOptions { get; set; } = new List<FieldOptionDto>();
    }

    public class CreateFormFieldDto
    {
        [Required]
        public int TabId { get; set; }

        [Required]
        public int FieldTypeId { get; set; }

        [Required, StringLength(200)]
        public string FieldName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FieldCode { get; set; } = string.Empty;

        [Required]
        public int FieldOrder { get; set; }

        public string? Placeholder { get; set; }
        public string HintText { get; set; }

        public bool ?IsMandatory { get; set; } = true;
        public bool ?IsEditable { get; set; } = true;
        public bool ?IsVisible { get; set; } = true;

        public string? DefaultValueJson { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string? RegexPattern { get; set; }
        public string? ValidationMessage { get; set; }

        public string? CreatedByUserId { get; set; }
    }

    public class UpdateFormFieldDto
    {
        [Required]
        public int TabId { get; set; }

        [Required]
        public int FieldTypeId { get; set; }

        [Required, StringLength(200)]
        public string FieldName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FieldCode { get; set; } = string.Empty;

        [Required]
        public int FieldOrder { get; set; }

        public string? Placeholder { get; set; }
        public string HintText { get; set; }

        public bool? IsMandatory { get; set; }
        public bool? IsEditable { get; set; }
        public bool? IsVisible { get; set; }

        public string? DefaultValueJson { get; set; }
        public int? MaxLength { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string? RegexPattern { get; set; }
        public string? ValidationMessage { get; set; }
    }
}