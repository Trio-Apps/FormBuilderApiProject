using formBuilder.Domian.Entitys;
using FormBuilder.Domian.Entitys.FormBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Domian.Entitys.FormBuilder
{
    [Table("FORM_GRID_COLUMNS")]
    public class FORM_GRID_COLUMNS : BaseEntity
    {
     

        [ForeignKey("FORM_GRIDS")]
        public int GridId { get; set; }
        public virtual FORM_GRIDS FORM_GRIDS { get; set; }

        public int? FieldTypeId { get; set; }

        [Required, StringLength(200)]
        public string ColumnName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ColumnCode { get; set; } = string.Empty;

        public int ColumnOrder { get; set; }
        public bool IsMandatory { get; set; }
        public string DataType { get; set; } = string.Empty;
        public int? MaxLength { get; set; }
        public string? DefaultValueJson { get; set; }
        public string? ValidationRuleJson { get; set; }
        public new bool IsActive { get; set; }

        public virtual ICollection<FORM_SUBMISSION_GRID_CELLS> FORM_SUBMISSION_GRID_CELLS { get; set; }
    }
}
