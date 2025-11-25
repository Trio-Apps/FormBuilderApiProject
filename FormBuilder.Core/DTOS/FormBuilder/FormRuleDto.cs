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

        [Required]
        public string RuleJson { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateFormRuleDto
    {

        [Required]
        public int FormBuilderId { get; set; }

        [Required]
        [StringLength(200)]
        public string RuleName { get; set; }

        [Required]
        public string RuleJson { get; set; }

        public bool IsActive { get; set; }
    }

    public class FormRuleDto
    {
        public int Id { get; set; }
        public int FormBuilderId { get; set; }
        public string RuleName { get; set; }
        public string RuleJson { get; set; }
        public bool IsActive { get; set; }
        public string FormName { get; set; }
        public string FormCode { get; set; }
    }
}