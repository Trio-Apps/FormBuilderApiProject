using formBuilder.Domian.Entitys;
using FormBuilder.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("APPROVAL_DELEGATIONS")]
public class APPROVAL_DELEGATIONS : BaseEntity
{
  

    public string FromUserId { get; set; }
    [ForeignKey("FromUserId")]
    public virtual AppUser FromUser { get; set; }

    public string ToUserId { get; set; }
    [ForeignKey("ToUserId")]
    public virtual AppUser ToUser { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
