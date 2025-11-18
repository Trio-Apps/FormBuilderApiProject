// Models/FormField.cs
using formBuilder.Domian.Entitys;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace FormBuilder.API.Models
    {
        [Table("FORM_FIELDS")]
        public class FormField : BaseEntity
        {
            [Required]
            [Column("TabID")]
            public int TabID { get; set; }

            [Required]
            [MaxLength(255)]
            [Column("FieldName")]
            public string FieldName { get; set; }

            [Required]
            [Column("FieldTypeID")]
            public int FieldTypeID { get; set; }  // FK to FieldType

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
            public string DataType { get; set; }

            [Column("MaxLength")]
            public int? MaxLength { get; set; }

            [Column("ValidationRules")]
            public string ValidationRules { get; set; }

            [Column("CreatedDate")]
            public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

            [ForeignKey("TabID")]
            public virtual FORM_TABS FormTab { get; set; }

            [ForeignKey("FieldTypeID")]
            public virtual FieldType FieldType { get; set; }
        }
    }


}