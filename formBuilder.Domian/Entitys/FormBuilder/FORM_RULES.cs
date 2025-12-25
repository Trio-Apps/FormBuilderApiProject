using formBuilder.Domian.Entitys;
using FormBuilder.Domian.Entitys.FormBuilder;
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

        // Condition Fields (بدلاً من RuleJson)
        [StringLength(100)]
        public string? ConditionField { get; set; }

        [StringLength(50)]
        public string? ConditionOperator { get; set; }

        [StringLength(500)]
        public string? ConditionValue { get; set; }

        [StringLength(20)]
        public string? ConditionValueType { get; set; } // "constant" or "field"

        // Navigation property for Actions (stored in separate table)
        public virtual ICollection<FORM_RULE_ACTIONS> FORM_RULE_ACTIONS { get; set; }

        // Keep RuleJson for backward compatibility (nullable)
        public string? RuleJson { get; set; }

        public new bool IsActive { get; set; }

        /// <summary>
        /// Execution order - lower numbers execute first
        /// </summary>
        public int? ExecutionOrder { get; set; } = 1;

        // Constructor
        public FORM_RULES()
        {
            FORM_RULE_ACTIONS = new HashSet<FORM_RULE_ACTIONS>();
        }
    }
}
