// Models/FormBuilder.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("FORM_BUILDER")]
    public class FormBuilder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FormBuilderID")]
        public int FormBuilderID { get; set; }

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

        // Navigation Properties
        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; }

        public virtual ICollection<FormTab> FormTabs { get; set; } = new List<FormTab>();
    }
}