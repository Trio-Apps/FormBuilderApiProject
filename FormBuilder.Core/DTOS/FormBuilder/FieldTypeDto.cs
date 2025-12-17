public class FieldTypeDto
{
    public int Id { get; set; }
    public string TypeName { get; set; }
    public string DataType { get; set; }
    public int? MaxLength { get; set; }
    public bool? HasOptions { get; set; }
    public bool? AllowMultiple { get; set; }
    public bool IsActive { get; set; }
}
