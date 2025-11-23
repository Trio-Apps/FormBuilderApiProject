using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Domian.Entitys.FromBuilder
{


    [Table("DOCUMENT_TYPES")]
    public class DOCUMENT_TYPES
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public string Code { get; set; }

        [ForeignKey("FORM_BUILDER")]
        public int FormBuilderId { get; set; }
        public virtual FORM_BUILDER FORM_BUILDER { get; set; }

        [Required, StringLength(200)]
        public string MenuCaption { get; set; }

        public int MenuOrder { get; set; }
        public int? ParentMenuId { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<FORM_SUBMISSIONS> FORM_SUBMISSIONS { get; set; }
        public virtual ICollection<DOCUMENT_SERIES> DOCUMENT_SERIES { get; set; }
        public virtual ICollection<APPROVAL_WORKFLOWS> APPROVAL_WORKFLOWS { get; set; }
        public virtual ICollection<EMAIL_TEMPLATES> EMAIL_TEMPLATES { get; set; }
        public virtual ICollection<ALERT_RULES> ALERT_RULES { get; set; }
        public virtual ICollection<CRYSTAL_LAYOUTS> CRYSTAL_LAYOUTS { get; set; }
        public virtual ICollection<ADOBE_SIGNATURE_CONFIG> ADOBE_SIGNATURE_CONFIG { get; set; }
        public virtual ICollection<OUTLOOK_APPROVAL_CONFIG> OUTLOOK_APPROVAL_CONFIG { get; set; }
        public virtual ICollection<SAP_OBJECT_MAPPINGS> SAP_OBJECT_MAPPINGS { get; set; }
    }

}
