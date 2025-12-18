using formBuilder.Domian.Entitys;
using FormBuilder.Domian.Entitys.FormBuilder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("APPROVAL_DELEGATIONS")]
public class APPROVAL_DELEGATIONS : BaseEntity
{
  

    public string FromUserId { get; set; } = string.Empty;
    

    public string ToUserId { get; set; } = string.Empty;


    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public new bool IsActive { get; set; }
}
