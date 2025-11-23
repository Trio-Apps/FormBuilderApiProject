using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FromBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Domian.Entitys.FormBuilder
{
    [Table("APPROVAL_STAGES")]
    public class APPROVAL_STAGES
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("APPROVAL_WORKFLOWS")]
        public int WorkflowId { get; set; }
        public virtual APPROVAL_WORKFLOWS APPROVAL_WORKFLOWS { get; set; }

        public int StageOrder { get; set; }

        [Required, StringLength(200)]
        public string StageName { get; set; }

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool IsFinalStage { get; set; }

        public virtual ICollection<APPROVAL_STAGE_ASSIGNEES> APPROVAL_STAGE_ASSIGNEES { get; set; }
        public virtual ICollection<DOCUMENT_APPROVAL_HISTORY> DOCUMENT_APPROVAL_HISTORY { get; set; }
    }
}
