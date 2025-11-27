
using formBuilder.Domian.Entitys;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("FORM_GRIDS")]
public class FORM_GRIDS : BaseEntity
{
 
    [ForeignKey("FORM_BUILDER")]
    public int FormBuilderId { get; set; }
    public virtual FORM_BUILDER FORM_BUILDER { get; set; }

    [Required, StringLength(200)]
    public string GridName { get; set; }

    [Required, StringLength(100)]
    public string GridCode { get; set; }

    [ForeignKey("FORM_TABS")]
    public int? TabId { get; set; }
    public virtual FORM_TABS FORM_TABS { get; set; }

    public int GridOrder { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<FORM_GRID_COLUMNS> FORM_GRID_COLUMNS { get; set; }
    public virtual ICollection<FORM_SUBMISSION_GRID_ROWS> FORM_SUBMISSION_GRID_ROWS { get; set; }
}
