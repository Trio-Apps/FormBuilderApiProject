using System.ComponentModel.DataAnnotations;

public class FieldTypeCreateDto
{
    [Required, StringLength(100)]
    public string TypeName { get; set; }

    [StringLength(50)]
    public string DataType { get; set; }

    public int? MaxLength { get; set; }
    public bool HasOptions { get; set; }
    public bool AllowMultiple { get; set; }
}
