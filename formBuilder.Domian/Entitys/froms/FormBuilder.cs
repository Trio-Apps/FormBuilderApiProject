// Models/FormBuilder.cs
using formBuilder.Domian.Entitys;
using FormBuilder.API.Models.FormBuilder.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("FORM_BUILDERS")]
    public class FormBuilders : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        [Column("FormName")]
        public string FormName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("FormCode")]
        public string FormCode { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Required]
        [Column("CreatedBy")]
        public int CreatedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<FORM_TABS> FormTabs { get; set; } = new List<FORM_TABS>();
    }
}