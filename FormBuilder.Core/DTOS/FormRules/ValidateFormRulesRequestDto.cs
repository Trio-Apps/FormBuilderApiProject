using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.DTOS.FormRules
{
    /// <summary>
    /// Request DTO for validating form rules
    /// </summary>
    public class ValidateFormRulesRequestDto
    {
        /// <summary>
        /// Form Builder ID
        /// </summary>
        [Required]
        public int FormBuilderId { get; set; }

        /// <summary>
        /// Dictionary of field codes and their values
        /// </summary>
        public Dictionary<string, object> FieldValues { get; set; } = new Dictionary<string, object>();
    }
}

