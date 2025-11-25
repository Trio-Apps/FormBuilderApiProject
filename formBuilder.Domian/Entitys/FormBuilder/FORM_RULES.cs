using formBuilder.Domian.Entitys;
using FormBuilder.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Domian.Entitys.froms
{
    [Table("FORM_RULES")]
    public class FORM_RULES :BaseEntity
    {
    

        [ForeignKey("FORM_BUILDER")]
        public int FormBuilderId { get; set; }
        public virtual FORM_BUILDER FORM_BUILDER { get; set; }

        [Required, StringLength(200)]
        public string RuleName { get; set; }

        public string RuleJson { get; set; }
        public bool IsActive { get; set; }
    }
}
