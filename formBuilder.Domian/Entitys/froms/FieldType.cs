// Models/FieldType.cs
using formBuilder.Domian.Entitys;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{



    namespace FormBuilder.API.Models
    {
        [Table("FIELD_TYPES")]
        public class FieldType : BaseEntity
        {
            [Required]
            [MaxLength(50)]
            [Column("TypeName")]
            public string TypeName { get; set; }

            [MaxLength(50)]
            [Column("DataType")]
            public string DataType { get; set; }

            [Column("MaxLength")]
            public int? MaxLength { get; set; }

            [Column("IsActive")]
            public bool IsActive { get; set; } = true;

            public virtual ICollection<FormField> FormFields { get; set; } = new List<FormField>();
        }
    }


}