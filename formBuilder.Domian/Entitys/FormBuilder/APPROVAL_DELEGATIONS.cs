using formBuilder.Domian.Entitys;
using FormBuilder.Domian.Entitys.FormBuilder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("APPROVAL_DELEGATIONS")]
public class APPROVAL_DELEGATIONS : BaseEntity
{
  

    public string FromUserId { get; set; }
    

    public string ToUserId { get; set; }


    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
