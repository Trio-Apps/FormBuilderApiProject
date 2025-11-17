// Models/FormField.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("FORM_FIELDS")]
    public class FormField
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FieldID")]
        public int FieldID { get; set; }

        [Required]
        [Column("TabID")]
        public int TabID { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("FieldName")]
        public string FieldName { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("FieldType")]
        public string FieldType { get; set; } // Text, Number, Date, Dropdown, etc.

        [Column("FieldOrder")]
        public int FieldOrder { get; set; } = 1;

        [Column("IsMandatory")]
        public bool IsMandatory { get; set; }

        [Column("IsEditable")]
        public bool IsEditable { get; set; } = true;

        [Column("DefaultValue")]
        public string DefaultValue { get; set; }

        [MaxLength(50)]
        [Column("DataType")]
        public string DataType { get; set; } // string, int, decimal, bool, DateTime

        [Column("MaxLength")]
        public int? MaxLength { get; set; }

        [Column("ValidationRules")]
        public string ValidationRules { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("TabID")]
        public virtual FormTab FormTab { get; set; }
    }
}