using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using FormBuilder.Core.DTOS.FormRules;
using FormBuilder.Core.IServices.FormBuilder;

namespace FormBuilder.Services.Services.FormBuilder
{
    /// <summary>
    /// Service for evaluating form rules (conditions and actions)
    /// </summary>
    public class FormRuleEvaluationService : IFormRuleEvaluationService
    {
        private readonly ILogger<FormRuleEvaluationService>? _logger;

        public FormRuleEvaluationService(ILogger<FormRuleEvaluationService>? logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Evaluates a condition against form field values
        /// </summary>
        public bool EvaluateCondition(ConditionDataDto condition, Dictionary<string, object> fieldValues)
        {
            if (condition == null)
            {
                _logger?.LogWarning("Condition is null");
                return false;
            }

            if (string.IsNullOrEmpty(condition.Field))
            {
                _logger?.LogWarning("Condition field is empty");
                return false;
            }

            if (fieldValues == null)
            {
                _logger?.LogWarning("FieldValues dictionary is null");
                return false;
            }

            if (!fieldValues.ContainsKey(condition.Field))
            {
                _logger?.LogDebug("Field '{Field}' not found in form values. Available fields: {AvailableFields}",
                    condition.Field, string.Join(", ", fieldValues.Keys));
                return false;
            }

            var fieldValue = fieldValues[condition.Field];
            object? compareValue;

            // Determine comparison value
            if (condition.ValueType == "field")
            {
                var compareField = condition.Value?.ToString() ?? "";
                if (string.IsNullOrEmpty(compareField) || !fieldValues.ContainsKey(compareField))
                {
                    _logger?.LogWarning("Comparison field '{Field}' not found in form values", compareField);
                    return false;
                }
                compareValue = fieldValues[compareField];
            }
            else
            {
                compareValue = condition.Value;
            }

            // Validate operator
            var validOperators = new[] { "==", "!=", ">", "<", ">=", "<=", "contains", "isEmpty", "isNotEmpty" };
            if (string.IsNullOrEmpty(condition.Operator) || !validOperators.Contains(condition.Operator))
            {
                _logger?.LogWarning("Invalid operator '{Operator}' for field '{Field}'", condition.Operator, condition.Field);
                return false;
            }

            // Evaluate based on operator
            try
            {
                bool result;
                switch (condition.Operator)
                {
                    case "==":
                        result = CompareValues(fieldValue, compareValue, (a, b) => a?.ToString() == b?.ToString());
                        break;
                    case "!=":
                        result = CompareValues(fieldValue, compareValue, (a, b) => a?.ToString() != b?.ToString());
                        break;
                    case ">":
                        result = CompareNumeric(fieldValue, compareValue, (a, b) => a > b);
                        break;
                    case "<":
                        result = CompareNumeric(fieldValue, compareValue, (a, b) => a < b);
                        break;
                    case ">=":
                        result = CompareNumeric(fieldValue, compareValue, (a, b) => a >= b);
                        break;
                    case "<=":
                        result = CompareNumeric(fieldValue, compareValue, (a, b) => a <= b);
                        break;
                    case "contains":
                        result = fieldValue?.ToString()?.Contains(compareValue?.ToString() ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
                        break;
                    case "isEmpty":
                        result = IsEmpty(fieldValue);
                        break;
                    case "isNotEmpty":
                        result = !IsEmpty(fieldValue);
                        break;
                    default:
                        _logger?.LogWarning("Unsupported operator '{Operator}' for field '{Field}'", condition.Operator, condition.Field);
                        return false;
                }

                _logger?.LogDebug("Condition evaluated: Field={Field}, Operator={Operator}, FieldValue={FieldValue}, CompareValue={CompareValue}, Result={Result}",
                    condition.Field, condition.Operator, fieldValue, compareValue, result);

                return result;
            }
            catch (FormatException ex)
            {
                _logger?.LogError(ex, "Format error evaluating condition: Field={Field}, Operator={Operator}, FieldValue={FieldValue}, CompareValue={CompareValue}",
                    condition.Field, condition.Operator, fieldValue, compareValue);
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error evaluating condition: Field={Field}, Operator={Operator}",
                    condition.Field, condition.Operator);
                return false;
            }
        }

        /// <summary>
        /// Evaluates a formula expression using field values
        /// </summary>
        public object EvaluateFormula(string expression, Dictionary<string, object> fieldValues)
        {
            return EvaluateExpression(expression, fieldValues);
        }

        /// <summary>
        /// Evaluates an expression using field values (for Compute actions)
        /// </summary>
        public object EvaluateExpression(string expression, Dictionary<string, object> fieldValues)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                _logger?.LogWarning("Expression is empty");
                return 0;
            }

            if (fieldValues == null)
            {
                _logger?.LogWarning("FieldValues dictionary is null");
                return 0;
            }

            try
            {
                // Replace field codes with their numeric values
                var formula = expression;
                var replacedFields = new List<string>();

                foreach (var kvp in fieldValues)
                {
                    try
                    {
                        var numValue = Convert.ToDouble(kvp.Value ?? 0);
                        // Replace field code in formula (with word boundaries to avoid partial matches)
                        var pattern = $@"\b{Regex.Escape(kvp.Key)}\b";
                        if (Regex.IsMatch(formula, pattern, RegexOptions.IgnoreCase))
                        {
                            formula = Regex.Replace(
                                formula,
                                pattern,
                                numValue.ToString(),
                                RegexOptions.IgnoreCase);
                            replacedFields.Add(kvp.Key);
                        }
                    }
                    catch (FormatException ex)
                    {
                        // If value cannot be converted to number, skip this field
                        _logger?.LogDebug(ex, "Field '{Field}' value '{Value}' cannot be converted to number, skipping",
                            kvp.Key, kvp.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Unexpected error processing field '{Field}' in expression", kvp.Key);
                    }
                }

                // Check if any field codes remain unreplaced (potential error)
                var remainingFieldPattern = @"\b[A-Za-z_][A-Za-z0-9_]*\b";
                var matches = Regex.Matches(formula, remainingFieldPattern);
                var unreplacedFields = matches
                    .Cast<Match>()
                    .Where(m => !double.TryParse(m.Value, out _) && 
                                !new[] { "true", "false", "null" }.Contains(m.Value.ToLower()))
                    .Select(m => m.Value)
                    .Distinct()
                    .ToList();

                if (unreplacedFields.Any())
                {
                    _logger?.LogWarning("Expression contains unreplaced field codes: {Fields}. Expression: {Expression}",
                        string.Join(", ", unreplacedFields), expression);
                }

                // Evaluate formula using DataTable.Compute (safer than eval)
                var dataTable = new DataTable();
                var result = dataTable.Compute(formula, null);
                
                if (result == null || result == DBNull.Value)
                {
                    _logger?.LogWarning("Expression evaluation returned null. Expression: {Expression}", expression);
                    return 0;
                }

                _logger?.LogDebug("Expression evaluated successfully: Expression={Expression}, Result={Result}, ReplacedFields={ReplacedFields}",
                    expression, result, string.Join(", ", replacedFields));
                
                return result;
            }
            catch (SyntaxErrorException ex)
            {
                _logger?.LogError(ex, "Syntax error in expression: {Expression}", expression);
                throw new InvalidOperationException($"Invalid expression syntax: {expression}", ex);
            }
            catch (EvaluateException ex)
            {
                _logger?.LogError(ex, "Evaluation error in expression: {Expression}", expression);
                throw new InvalidOperationException($"Expression evaluation failed: {expression}", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error evaluating expression: {Expression}", expression);
                throw new InvalidOperationException($"Error evaluating expression: {expression}", ex);
            }
        }

        /// <summary>
        /// Validates actions and returns list of validation errors
        /// </summary>
        public List<string> ValidateActions(
            List<ActionDataDto>? actions,
            Dictionary<string, object> fieldValues,
            string ruleName)
        {
            var errors = new List<string>();

            if (actions == null || !actions.Any())
            {
                _logger?.LogDebug("No actions to validate for rule: {RuleName}", ruleName);
                return errors;
            }

            foreach (var action in actions)
            {
                try
                {
                    if (action.Type == "SetMandatory" && 
                        (action.Value?.ToString() == "true" || action.Value?.ToString() == "True"))
                    {
                        if (!fieldValues.ContainsKey(action.FieldCode) ||
                            IsEmpty(fieldValues[action.FieldCode]))
                        {
                            var error = $"Field '{action.FieldCode}' is required based on rule '{ruleName}'";
                            errors.Add(error);
                            _logger?.LogWarning(error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error validating action: Type={Type}, FieldCode={FieldCode}",
                        action.Type, action.FieldCode);
                }
            }

            return errors;
        }

        /// <summary>
        /// Parses RuleJson string into FormRuleDataDto
        /// </summary>
        public FormRuleDataDto? ParseRuleJson(string ruleJson)
        {
            if (string.IsNullOrWhiteSpace(ruleJson))
            {
                _logger?.LogWarning("RuleJson is empty");
                return null;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var ruleData = JsonSerializer.Deserialize<FormRuleDataDto>(ruleJson, options);
                
                if (ruleData == null)
                {
                    _logger?.LogWarning("Failed to deserialize RuleJson");
                    return null;
                }

                return ruleData;
            }
            catch (JsonException ex)
            {
                _logger?.LogError(ex, "Error parsing RuleJson: {RuleJson}", ruleJson);
                return null;
            }
        }

        // Private helper methods

        private bool CompareValues(object? a, object? b, Func<object?, object?, bool> comparer)
        {
            try
            {
                return comparer(a, b);
            }
            catch
            {
                return false;
            }
        }

        private bool CompareNumeric(object? a, object? b, Func<double, double, bool> comparer)
        {
            try
            {
                var numA = Convert.ToDouble(a ?? 0);
                var numB = Convert.ToDouble(b ?? 0);
                return comparer(numA, numB);
            }
            catch
            {
                return false;
            }
        }

        private bool IsEmpty(object? value)
        {
            if (value == null)
                return true;

            var stringValue = value.ToString();
            return string.IsNullOrWhiteSpace(stringValue) ||
                   stringValue == "null" ||
                   stringValue == "";
        }
    }
}

