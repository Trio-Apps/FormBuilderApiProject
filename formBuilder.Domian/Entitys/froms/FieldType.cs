// Models/FieldType.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("FIELD_TYPES")]
    public class FieldType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FieldTypeID")]
        public int FieldTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("TypeName")]
        public string TypeName { get; set; } // Text, Number, Date, Dropdown, etc.

        [MaxLength(50)]
        [Column("DataType")]
        public string DataType { get; set; } // string, int, decimal, bool, DateTime

        [Column("MaxLength")]
        public int? MaxLength { get; set; }


        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<FormField> FormFields { get; set; } = new List<FormField>();
    }
}