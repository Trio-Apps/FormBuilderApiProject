namespace FormBuilder.Core.DTOS.FormBuilder
{
    /// <summary>
    /// Unified response format for field options from any data source
    /// </summary>
    public class FieldOptionResponseDto
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for getting field options
    /// </summary>
    public class GetFieldOptionsRequestDto
    {
        public int FieldId { get; set; }
        public Dictionary<string, object>? Context { get; set; }
        public string? RequestBodyJson { get; set; }
    }

    /// <summary>
    /// Request DTO for previewing data source
    /// </summary>
    public class PreviewDataSourceRequestDto
    {
        public int? FieldId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string? ApiUrl { get; set; }
        public string? HttpMethod { get; set; }
        public string? RequestBodyJson { get; set; }
        public string? ValuePath { get; set; }
        public string? TextPath { get; set; }
    }
}

