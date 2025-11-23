using FormBuilder.API.Models;
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
    public class FORM_GRID_COLUMNS
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("FORM_GRIDS")]
        public int GridId { get; set; }
        public virtual FORM_GRIDS FORM_GRIDS { get; set; }

        [ForeignKey("FIELD_TYPES")]
        public int FieldTypeId { get; set; }
        public virtual FIELD_TYPES FIELD_TYPES { get; set; }

        [Required, StringLength(200)]
        public string ColumnName { get; set; }

        [Required, StringLength(100)]
        public string ColumnCode { get; set; }

        public int ColumnOrder { get; set; }
        public bool IsMandatory { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }
        public string DefaultValueJson { get; set; }
        public string ValidationRuleJson { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<FORM_SUBMISSION_GRID_CELLS> FORM_SUBMISSION_GRID_CELLS { get; set; }
    }
}
