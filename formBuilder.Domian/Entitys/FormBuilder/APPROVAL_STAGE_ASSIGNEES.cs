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
    [Table("APPROVAL_STAGE_ASSIGNEES")]
    public class APPROVAL_STAGE_ASSIGNEES
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("APPROVAL_STAGES")]
        public int StageId { get; set; }
        public virtual APPROVAL_STAGES APPROVAL_STAGES { get; set; }

        [StringLength(450)]
        public string RoleId { get; set; }

        [ForeignKey("AppUser")]
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}
