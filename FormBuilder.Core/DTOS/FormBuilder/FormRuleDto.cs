using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormRules
{
    public class CreateFormRuleDto
    {
        [Required]
        public int FormBuilderId { get; set; }

        [Required]
        [StringLength(200)]
        public string RuleName { get; set; }

        // Condition Fields
        [StringLength(100)]
        public string? ConditionField { get; set; }

        [StringLength(50)]
        public string? ConditionOperator { get; set; }

        [StringLength(500)]
        public string? ConditionValue { get; set; }

        [StringLength(20)]
        public string? ConditionValueType { get; set; } // "constant" or "field"

        // Actions (JSON string for array of actions)
        public string? ActionsJson { get; set; }

        // Else Actions (JSON string for array of actions)
        public string? ElseActionsJson { get; set; }

        // Keep RuleJson for backward compatibility (optional)
        public string? RuleJson { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Execution order - lower numbers execute first
        /// </summary>
        public int? ExecutionOrder { get; set; } = 1;
    }

    public class UpdateFormRuleDto
    {
        [Required]
        public int FormBuilderId { get; set; }

        [Required]
        [StringLength(200)]
        public string RuleName { get; set; }

        // Condition Fields
        [StringLength(100)]
        public string? ConditionField { get; set; }

        [StringLength(50)]
        public string? ConditionOperator { get; set; }

        [StringLength(500)]
        public string? ConditionValue { get; set; }

        [StringLength(20)]
        public string? ConditionValueType { get; set; } // "constant" or "field"

        // Actions (JSON string for array of actions)
        public string? ActionsJson { get; set; }

        // Else Actions (JSON string for array of actions)
        public string? ElseActionsJson { get; set; }

        // Keep RuleJson for backward compatibility (optional)
        public string? RuleJson { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Execution order - lower numbers execute first
        /// </summary>
        public int? ExecutionOrder { get; set; }
    }

    public class FormRuleDto
    {
        public int Id { get; set; }
        public int FormBuilderId { get; set; }
        public string RuleName { get; set; }
        
        // Condition Fields
        public string? ConditionField { get; set; }
        public string? ConditionOperator { get; set; }
        public string? ConditionValue { get; set; }
        public string? ConditionValueType { get; set; }

        // Actions
        public string? ActionsJson { get; set; }
        public string? ElseActionsJson { get; set; }

        // Keep RuleJson for backward compatibility
        public string? RuleJson { get; set; }
        
        public bool IsActive { get; set; }
        public int? ExecutionOrder { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
    }
}