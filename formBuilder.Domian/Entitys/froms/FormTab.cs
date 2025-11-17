// Models/FormTab.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("FORM_TABS")]
    public class FormTab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TabID")]
        public int TabID { get; set; }

        [Required]
        [Column("FormBuilderID")]
        public int FormBuilderID { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("TabName")]
        public string TabName { get; set; }

        [Column("TabOrder")]
        public int TabOrder { get; set; } = 1;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("FormBuilderID")]
        public virtual FormBuilder FormBuilder { get; set; }

        public virtual ICollection<FormField> FormFields { get; set; } = new List<FormField>();
    }
}