using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Domian.Entitys.FromBuilder
{
    [Table("DOCUMENT_SERIES")]
    public class DOCUMENT_SERIES
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("DOCUMENT_TYPES")]
        public int DocumentTypeId { get; set; }
        public virtual DOCUMENT_TYPES DOCUMENT_TYPES { get; set; }

        [ForeignKey("PROJECTS")]
        public int ProjectId { get; set; }
        public virtual PROJECTS PROJECTS { get; set; }

        [Required, StringLength(50)]
        public string SeriesCode { get; set; }

        public int NextNumber { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<FORM_SUBMISSIONS> FORM_SUBMISSIONS { get; set; }
    }

}
