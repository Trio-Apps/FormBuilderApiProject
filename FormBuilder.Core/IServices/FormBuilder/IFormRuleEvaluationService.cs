using System.Collections.Generic;
using FormBuilder.Core.DTOS.FormRules;

namespace FormBuilder.Core.IServices.FormBuilder
{
    /// <summary>
    /// Service interface for evaluating form rules
    /// </summary>
    public interface IFormRuleEvaluationService
    {
        /// <summary>
        /// Evaluates a condition against form field values
        /// </summary>
        bool EvaluateCondition(ConditionDataDto condition, Dictionary<string, object> fieldValues);

        /// <summary>
        /// Evaluates a formula expression using field values
        /// </summary>
        object EvaluateFormula(string expression, Dictionary<string, object> fieldValues);

        /// <summary>
        /// Evaluates an expression (alias for EvaluateFormula for consistency)
        /// </summary>
        object EvaluateExpression(string expression, Dictionary<string, object> fieldValues);

        /// <summary>
        /// Validates actions and returns list of validation errors
        /// </summary>
        List<string> ValidateActions(
            List<ActionDataDto>? actions,
            Dictionary<string, object> fieldValues,
            string ruleName);

        /// <summary>
        /// Parses RuleJson string into FormRuleDataDto (for backward compatibility)
        /// </summary>
        FormRuleDataDto? ParseRuleJson(string ruleJson);

        /// <summary>
        /// Builds FormRuleDataDto from separate fields (new approach)
        /// </summary>
        FormRuleDataDto? BuildRuleDataFromFields(
            string? conditionField,
            string? conditionOperator,
            string? conditionValue,
            string? conditionValueType,
            string? actionsJson,
            string? elseActionsJson);
    }
}

