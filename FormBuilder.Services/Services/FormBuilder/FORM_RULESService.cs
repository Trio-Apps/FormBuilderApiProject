using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.DTOS.FormRules;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Services.Services
{
    public class FORM_RULESService : IFORM_RULESService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly FormBuilderDbContext _dbContext;
        private readonly IFormRuleEvaluationService? _ruleEvaluationService;

        public FORM_RULESService(IunitOfwork unitOfWork, FormBuilderDbContext dbContext, IFormRuleEvaluationService? ruleEvaluationService = null)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _ruleEvaluationService = ruleEvaluationService;
        }

        public async Task<FORM_RULES> CreateRuleAsync(CreateFormRuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            // Validate form exists
            var formExists = await _unitOfWork.Repositary<FORM_BUILDER>()
                .SingleOrDefaultAsync(f => f.Id == ruleDto.FormBuilderId);
            if (formExists == null)
                throw new InvalidOperationException($"Form with ID '{ruleDto.FormBuilderId}' does not exist.");

            // Validate rule name uniqueness
            if (!await IsRuleNameUniqueAsync(ruleDto.FormBuilderId, ruleDto.RuleName))
                throw new InvalidOperationException($"Rule name '{ruleDto.RuleName}' is already in use for this form.");

            // Validate condition fields (required)
            if (string.IsNullOrWhiteSpace(ruleDto.ConditionField))
                throw new InvalidOperationException("Condition Field is required.");

            if (string.IsNullOrWhiteSpace(ruleDto.ConditionOperator))
                throw new InvalidOperationException("Condition Operator is required.");

            // Get actions directly from DTO (no JSON parsing needed)
            List<ActionDataDto>? actions = ruleDto.Actions;
            List<ActionDataDto>? elseActions = ruleDto.ElseActions;

            // Validate actions
            if (actions != null && actions.Any())
            {
                var validActionTypes = new[] { "SetVisible", "SetReadOnly", "SetMandatory", "SetDefault", "ClearValue", "Compute" };
                foreach (var action in actions)
                {
                    if (string.IsNullOrEmpty(action.Type) || string.IsNullOrEmpty(action.FieldCode))
                        throw new InvalidOperationException("Action Type and FieldCode are required.");
                    if (!validActionTypes.Contains(action.Type))
                        throw new InvalidOperationException($"Invalid action type: {action.Type}");
                    if (action.Type == "Compute" && string.IsNullOrEmpty(action.Expression))
                        throw new InvalidOperationException("Expression is required for Compute action.");
                }
            }

            // Validate else actions
            if (elseActions != null && elseActions.Any())
            {
                var validActionTypes = new[] { "SetVisible", "SetReadOnly", "SetMandatory", "SetDefault", "ClearValue", "Compute" };
                foreach (var action in elseActions)
                {
                    if (string.IsNullOrEmpty(action.Type) || string.IsNullOrEmpty(action.FieldCode))
                        throw new InvalidOperationException("Else Action Type and FieldCode are required.");
                    if (!validActionTypes.Contains(action.Type))
                        throw new InvalidOperationException($"Invalid else action type: {action.Type}");
                    if (action.Type == "Compute" && string.IsNullOrEmpty(action.Expression))
                        throw new InvalidOperationException("Expression is required for Compute action.");
                }
            }

            var ruleEntity = new FORM_RULES
            {
                FormBuilderId = ruleDto.FormBuilderId,
                RuleName = ruleDto.RuleName,
                ConditionField = ruleDto.ConditionField,
                ConditionOperator = ruleDto.ConditionOperator,
                ConditionValue = ruleDto.ConditionValue,
                ConditionValueType = ruleDto.ConditionValueType ?? "constant",
                IsActive = ruleDto.IsActive,
                ExecutionOrder = ruleDto.ExecutionOrder ?? 1
            };

            _unitOfWork.Repositary<FORM_RULES>().Add(ruleEntity);
            await _unitOfWork.CompleteAsyn();

            // Save Actions to separate table
            if (actions != null && actions.Any())
            {
                int actionOrder = 1;
                foreach (var action in actions)
                {
                    var ruleAction = new FORM_RULE_ACTIONS
                    {
                        RuleId = ruleEntity.Id,
                        ActionType = action.Type,
                        FieldCode = action.FieldCode,
                        Value = action.Value?.ToString(),
                        Expression = action.Expression,
                        IsElseAction = false,
                        ActionOrder = actionOrder++,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    _unitOfWork.Repositary<FORM_RULE_ACTIONS>().Add(ruleAction);
                }
            }

            // Save Else Actions to separate table
            if (elseActions != null && elseActions.Any())
            {
                int actionOrder = 1;
                foreach (var action in elseActions)
                {
                    var ruleAction = new FORM_RULE_ACTIONS
                    {
                        RuleId = ruleEntity.Id,
                        ActionType = action.Type,
                        FieldCode = action.FieldCode,
                        Value = action.Value?.ToString(),
                        Expression = action.Expression,
                        IsElseAction = true,
                        ActionOrder = actionOrder++,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    _unitOfWork.Repositary<FORM_RULE_ACTIONS>().Add(ruleAction);
                }
            }

            await _unitOfWork.CompleteAsyn();

            return ruleEntity;
        }

        public async Task<FORM_RULES> GetRuleByIdAsync(int id)
        {
            var rules = await _unitOfWork.Repositary<FORM_RULES>()
                .GetAllAsync(
                    filter: r => r.Id == id,
                    r => r.FORM_RULE_ACTIONS, r => r.FORM_BUILDER
                );
            return rules.FirstOrDefault();
        }

        public async Task<IEnumerable<FormRuleDto>> GetAllRulesAsync()
        {
            // 1. Update GetAllAsync to include the related FORM_BUILDERS entity.
            var rules = await _unitOfWork.Repositary<FORM_RULES>()
                .GetAllAsync(
                    filter: null, // No filter needed for getting all records
                    rule => rule.FORM_BUILDER, rule => rule.FORM_RULE_ACTIONS
                );

            // 2. Update the Select to map the new fields from the included entity.
            return rules.Select(rule => 
            {
                // Load actions from separate table
                var actions = rule.FORM_RULE_ACTIONS?
                    .Where(a => !a.IsElseAction && a.IsActive)
                    .OrderBy(a => a.ActionOrder)
                    .Select(a => new ActionDataDto
                    {
                        Type = a.ActionType,
                        FieldCode = a.FieldCode,
                        Value = a.Value,
                        Expression = a.Expression
                    })
                    .ToList() ?? new List<ActionDataDto>();

                var elseActions = rule.FORM_RULE_ACTIONS?
                    .Where(a => a.IsElseAction && a.IsActive)
                    .OrderBy(a => a.ActionOrder)
                    .Select(a => new ActionDataDto
                    {
                        Type = a.ActionType,
                        FieldCode = a.FieldCode,
                        Value = a.Value,
                        Expression = a.Expression
                    })
                    .ToList() ?? new List<ActionDataDto>();

                return new FormRuleDto
                {
                    Id = rule.Id,
                    FormBuilderId = rule.FormBuilderId,
                    RuleName = rule.RuleName,
                    ConditionField = rule.ConditionField,
                    ConditionOperator = rule.ConditionOperator,
                    ConditionValue = rule.ConditionValue,
                    ConditionValueType = rule.ConditionValueType,
                    Actions = actions.Any() ? actions : null, // Return as List, not JSON
                    ElseActions = elseActions.Any() ? elseActions : null, // Return as List, not JSON
                    IsActive = rule.IsActive,
                    ExecutionOrder = rule.ExecutionOrder ?? 1,
                    FormName = rule.FORM_BUILDER?.FormName ?? string.Empty,
                    FormCode = rule.FORM_BUILDER?.FormCode ?? string.Empty
                };
            }).ToList();
        }

        public async Task<bool> UpdateRuleAsync(UpdateFormRuleDto ruleDto,int id)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            var existingRule = await GetRuleByIdAsync(id);
            if (existingRule == null)
                throw new InvalidOperationException($"Rule with ID '{id}' does not exist.");

            // Validate rule name uniqueness (excluding current rule)
            if (!await IsRuleNameUniqueAsync(ruleDto.FormBuilderId, ruleDto.RuleName, id))
                throw new InvalidOperationException($"Rule name '{ruleDto.RuleName}' is already in use for this form.");

            // Validate condition fields (required)
            if (string.IsNullOrWhiteSpace(ruleDto.ConditionField))
                throw new InvalidOperationException("Condition Field is required.");

            if (string.IsNullOrWhiteSpace(ruleDto.ConditionOperator))
                throw new InvalidOperationException("Condition Operator is required.");

            // Get actions directly from DTO (no JSON parsing needed)
            List<ActionDataDto>? actions = ruleDto.Actions;
            List<ActionDataDto>? elseActions = ruleDto.ElseActions;

            // Validate actions
            if (actions != null && actions.Any())
            {
                var validActionTypes = new[] { "SetVisible", "SetReadOnly", "SetMandatory", "SetDefault", "ClearValue", "Compute" };
                foreach (var action in actions)
                {
                    if (string.IsNullOrEmpty(action.Type) || string.IsNullOrEmpty(action.FieldCode))
                        throw new InvalidOperationException("Action Type and FieldCode are required.");
                    if (!validActionTypes.Contains(action.Type))
                        throw new InvalidOperationException($"Invalid action type: {action.Type}");
                    if (action.Type == "Compute" && string.IsNullOrEmpty(action.Expression))
                        throw new InvalidOperationException("Expression is required for Compute action.");
                }
            }

            // Validate else actions
            if (elseActions != null && elseActions.Any())
            {
                var validActionTypes = new[] { "SetVisible", "SetReadOnly", "SetMandatory", "SetDefault", "ClearValue", "Compute" };
                foreach (var action in elseActions)
                {
                    if (string.IsNullOrEmpty(action.Type) || string.IsNullOrEmpty(action.FieldCode))
                        throw new InvalidOperationException("Else Action Type and FieldCode are required.");
                    if (!validActionTypes.Contains(action.Type))
                        throw new InvalidOperationException($"Invalid else action type: {action.Type}");
                    if (action.Type == "Compute" && string.IsNullOrEmpty(action.Expression))
                        throw new InvalidOperationException("Expression is required for Compute action.");
                }
            }

            // Update properties
            existingRule.FormBuilderId = ruleDto.FormBuilderId;
            existingRule.RuleName = ruleDto.RuleName;
            existingRule.ConditionField = ruleDto.ConditionField;
            existingRule.ConditionOperator = ruleDto.ConditionOperator;
            existingRule.ConditionValue = ruleDto.ConditionValue;
            existingRule.ConditionValueType = ruleDto.ConditionValueType ?? existingRule.ConditionValueType ?? "constant";
            existingRule.IsActive = ruleDto.IsActive;
            existingRule.ExecutionOrder = ruleDto.ExecutionOrder ?? existingRule.ExecutionOrder ?? 1;

            _unitOfWork.Repositary<FORM_RULES>().Update(existingRule);

            // Delete existing actions using DbContext directly to avoid tracking conflicts
            var actionsToDelete = await _dbContext.FORM_RULE_ACTIONS
                .Where(a => a.RuleId == existingRule.Id)
                .ToListAsync();
            
            if (actionsToDelete.Any())
            {
                _dbContext.FORM_RULE_ACTIONS.RemoveRange(actionsToDelete);
            }

            // Save new Actions to separate table
            if (actions != null && actions.Any())
            {
                int actionOrder = 1;
                foreach (var action in actions)
                {
                    var ruleAction = new FORM_RULE_ACTIONS
                    {
                        RuleId = existingRule.Id,
                        ActionType = action.Type,
                        FieldCode = action.FieldCode,
                        Value = action.Value?.ToString(),
                        Expression = action.Expression,
                        IsElseAction = false,
                        ActionOrder = actionOrder++,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    _unitOfWork.Repositary<FORM_RULE_ACTIONS>().Add(ruleAction);
                }
            }

            // Save new Else Actions to separate table
            if (elseActions != null && elseActions.Any())
            {
                int actionOrder = 1;
                foreach (var action in elseActions)
                {
                    var ruleAction = new FORM_RULE_ACTIONS
                    {
                        RuleId = existingRule.Id,
                        ActionType = action.Type,
                        FieldCode = action.FieldCode,
                        Value = action.Value?.ToString(),
                        Expression = action.Expression,
                        IsElseAction = true,
                        ActionOrder = actionOrder++,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    _unitOfWork.Repositary<FORM_RULE_ACTIONS>().Add(ruleAction);
                }
            }

            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

        public async Task<bool> DeleteRuleAsync(int id)
        {
            var ruleToDelete = await GetRuleByIdAsync(id);
            if (ruleToDelete == null)
                return false;

            _unitOfWork.Repositary<FORM_RULES>().Delete(ruleToDelete);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

        public async Task<bool> IsRuleNameUniqueAsync(int formBuilderId, string ruleName, int? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
                return false;

            var existingRule = await _unitOfWork.Repositary<FORM_RULES>()
                .SingleOrDefaultAsync(r => r.FormBuilderId == formBuilderId &&
                                          r.RuleName == ruleName.Trim() &&
                                          (!ignoreId.HasValue || r.Id != ignoreId.Value));

            return existingRule == null;
        }

        public async Task<bool> RuleExistsAsync(int id)
        {
            return await _unitOfWork.Repositary<FORM_RULES>()
                .AnyAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<FormRuleDto>> GetActiveRulesByFormIdAsync(int formBuilderId)
        {
            // Use filter directly instead of Where after GetAllAsync
            var activeRules = await _unitOfWork.Repositary<FORM_RULES>()
                .GetAllAsync(
                    filter: r => r.FormBuilderId == formBuilderId && r.IsActive,
                    rule => rule.FORM_BUILDER, rule => rule.FORM_RULE_ACTIONS
                );

            return activeRules
                .OrderBy(r => r.ExecutionOrder ?? 1)
                .Select(rule => 
                {
                    // Load actions from separate table
                    var actions = rule.FORM_RULE_ACTIONS?
                        .Where(a => !a.IsElseAction && a.IsActive)
                        .OrderBy(a => a.ActionOrder)
                        .Select(a => new ActionDataDto
                        {
                            Type = a.ActionType,
                            FieldCode = a.FieldCode,
                            Value = a.Value,
                            Expression = a.Expression
                        })
                        .ToList() ?? new List<ActionDataDto>();

                    var elseActions = rule.FORM_RULE_ACTIONS?
                        .Where(a => a.IsElseAction && a.IsActive)
                        .OrderBy(a => a.ActionOrder)
                        .Select(a => new ActionDataDto
                        {
                            Type = a.ActionType,
                            FieldCode = a.FieldCode,
                            Value = a.Value,
                            Expression = a.Expression
                        })
                        .ToList() ?? new List<ActionDataDto>();

                    return new FormRuleDto
                    {
                        Id = rule.Id,
                        FormBuilderId = rule.FormBuilderId,
                        RuleName = rule.RuleName,
                        ConditionField = rule.ConditionField,
                        ConditionOperator = rule.ConditionOperator,
                        ConditionValue = rule.ConditionValue,
                        ConditionValueType = rule.ConditionValueType,
                        Actions = actions.Any() ? actions : null, // Return as List, not JSON
                        ElseActions = elseActions.Any() ? elseActions : null, // Return as List, not JSON
                        IsActive = rule.IsActive,
                        ExecutionOrder = rule.ExecutionOrder ?? 1,
                        FormName = rule.FORM_BUILDER?.FormName ?? string.Empty,
                        FormCode = rule.FORM_BUILDER?.FormCode ?? string.Empty
                    };
                })
                .ToList();
        }

        // Private helper method - Validate ActionsJson structure
        private bool IsValidActionsJson(string actionsJson)
        {
            if (string.IsNullOrWhiteSpace(actionsJson))
                return false;

            try
            {
                var actions = JsonSerializer.Deserialize<List<ActionDataDto>>(
                    actionsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (actions == null || !actions.Any())
                    return false;

                var validActionTypes = new[] { "SetVisible", "SetReadOnly", "SetMandatory", "SetDefault", "ClearValue", "Compute" };
                foreach (var action in actions)
                {
                    if (string.IsNullOrEmpty(action.Type) || string.IsNullOrEmpty(action.FieldCode))
                        return false;

                    if (!validActionTypes.Contains(action.Type))
                        return false;

                    // For Compute action, Expression is required
                    if (action.Type == "Compute" && string.IsNullOrEmpty(action.Expression))
                        return false;
                }

                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Private helper method - Enhanced validation for RuleJson structure (for backward compatibility)
        private bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                return false;

            try
            {
                // First, check if it's valid JSON
                var ruleData = JsonSerializer.Deserialize<FormRuleDataDto>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (ruleData == null)
                    return false;

                // Validate Condition exists and has required fields
                if (ruleData.Condition == null || string.IsNullOrEmpty(ruleData.Condition.Field))
                    return false;

                // Validate Operator is valid
                var validOperators = new[] { "==", "!=", ">", "<", ">=", "<=", "contains", "isEmpty", "isNotEmpty" };
                if (!validOperators.Contains(ruleData.Condition.Operator))
                    return false;

                // Validate ValueType
                if (ruleData.Condition.ValueType != "constant" && ruleData.Condition.ValueType != "field")
                    return false;

                // Validate Actions exist and are valid
                if (ruleData.Actions == null || !ruleData.Actions.Any())
                    return false;

                var validActionTypes = new[] { "SetVisible", "SetReadOnly", "SetMandatory", "SetDefault", "ClearValue", "Compute" };
                foreach (var action in ruleData.Actions)
                {
                    if (string.IsNullOrEmpty(action.Type) || string.IsNullOrEmpty(action.FieldCode))
                        return false;

                    if (!validActionTypes.Contains(action.Type))
                        return false;

                    // For Compute action, Expression is required
                    if (action.Type == "Compute" && string.IsNullOrEmpty(action.Expression))
                        return false;
                }

                // Validate ElseActions if present (optional)
                if (ruleData.ElseActions != null)
                {
                    foreach (var action in ruleData.ElseActions)
                    {
                        if (string.IsNullOrEmpty(action.Type) || string.IsNullOrEmpty(action.FieldCode))
                            return false;

                        if (!validActionTypes.Contains(action.Type))
                            return false;

                        if (action.Type == "Compute" && string.IsNullOrEmpty(action.Expression))
                            return false;
                    }
                }

                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}