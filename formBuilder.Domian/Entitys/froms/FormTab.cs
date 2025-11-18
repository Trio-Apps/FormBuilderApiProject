// Models/FormTab.cs
using DocumentFormat.OpenXml.Wordprocessing;
using formBuilder.Domian.Entitys;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{


    namespace FormBuilder.API.Models
    {
        [Table("FORM_TABS")]
        public class FORM_TABS : BaseEntity
        {
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

            [ForeignKey("FormBuilderID")]
            public virtual FormBuilders FormBuilder { get; set; }

            public virtual ICollection<FormField> FormFields { get; set; } = new List<FormField>();
        }
    }

}