public class FieldTypeDto
{
    public int Id { get; set; }
    public string TypeName { get; set; }
    public string? ForeignTypeName { get; set; }
    public string DataType { get; set; }
    public int? MaxLength { get; set; }
    public bool? HasOptions { get; set; }
    public bool? AllowMultiple { get; set; }
    public bool IsActive { get; set; }
    
    // Computed properties for task requirements
    public string type_name_en => TypeName;
    public string? type_name_ar => ForeignTypeName;
}
