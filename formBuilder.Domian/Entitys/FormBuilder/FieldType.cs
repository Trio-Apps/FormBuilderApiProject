
using formBuilder.Domian.Entitys;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("FIELD_TYPES")]
public class FIELD_TYPES :BaseEntity
{
    

    [Required, StringLength(100)]
    public string TypeName { get; set; }

    [StringLength(50)]
    public string DataType { get; set; }

    public int? MaxLength { get; set; }
    public bool HasOptions { get; set; }
    public bool AllowMultiple { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<FORM_FIELDS> FORM_FIELDS { get; set; }
    public virtual ICollection<FORM_GRID_COLUMNS> FORM_GRID_COLUMNS { get; set; }
}
