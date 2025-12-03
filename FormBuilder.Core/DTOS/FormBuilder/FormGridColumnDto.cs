using System;

namespace FormBuilder.API.DTOs
{
    public class FormGridColumnDto
    {
        public int Id { get; set; }
        public int GridId { get; set; }
        public string GridName { get; set; } = string.Empty;
        public string FormBuilderName { get; set; } = string.Empty;
        public int FieldTypeId { get; set; }
        public string FieldTypeName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string ColumnCode { get; set; } = string.Empty;
        public int ColumnOrder { get; set; }
        public bool IsMandatory { get; set; }
        public string DataType { get; set; } = string.Empty;
        public int? MaxLength { get; set; }
        public string? DefaultValueJson { get; set; }
        public string? ValidationRuleJson { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateFormGridColumnDto
    {
        public int GridId { get; set; }
        public int FieldTypeId { get; set; }
        public string ColumnName { get; set; } = string.Empty;
        public string ColumnCode { get; set; } = string.Empty;
        public int? ColumnOrder { get; set; }
        public bool IsMandatory { get; set; }
        public string DataType { get; set; } = string.Empty;
        public int? MaxLength { get; set; }
        public string? DefaultValueJson { get; set; }
        public string? ValidationRuleJson { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateFormGridColumnDto
    {
        public int? GridId { get; set; }
        public int? FieldTypeId { get; set; }
        public string? ColumnName { get; set; }
        public string? ColumnCode { get; set; }
        public int? ColumnOrder { get; set; }
        public bool? IsMandatory { get; set; }
        public string? DataType { get; set; }
        public int? MaxLength { get; set; }
        public string? DefaultValueJson { get; set; }
        public string? ValidationRuleJson { get; set; }
        public bool? IsActive { get; set; }
    }
}