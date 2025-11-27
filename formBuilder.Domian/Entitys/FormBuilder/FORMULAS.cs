using formBuilder.Domian.Entitys;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.froms;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("FORMULAS")]
public class FORMULAS : BaseEntity
{
   

    [ForeignKey("FormBuilderId")]
    public virtual FORM_BUILDER FORM_BUILDER { get; set; }
    public int FormBuilderId { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; }

    [Required, StringLength(100)]
    public string Code { get; set; }

    public string ExpressionText { get; set; }

    public int? ResultFieldId { get; set; } // Nullable لو مش دايمًا مرتبط
    [ForeignKey("ResultFieldId")]
    public virtual FORM_FIELDS RESULT_FIELD { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<FORMULA_VARIABLES> FORMULA_VARIABLES { get; set; }
}
