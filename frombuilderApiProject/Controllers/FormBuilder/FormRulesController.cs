using FormBuilder.Core.DTOS.FormRules;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administration")]
    public class FormRulesController : ControllerBase
    {
        private readonly IFORM_RULESService _formRulesService;
        private readonly IFormRuleEvaluationService _ruleEvaluationService;
        private readonly ILogger<FormRulesController>? _logger;

        public FormRulesController(
            IFORM_RULESService formRulesService,
            IFormRuleEvaluationService ruleEvaluationService,
            ILogger<FormRulesController>? logger = null)
        {
            _formRulesService = formRulesService ?? throw new ArgumentNullException(nameof(formRulesService));
            _ruleEvaluationService = ruleEvaluationService ?? throw new ArgumentNullException(nameof(ruleEvaluationService));
            _logger = logger;
        }

        // ----------------------------------------------------------------------
        // --- 1. GET Operations (Read) ---
        // ----------------------------------------------------------------------

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormRuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRules()
        {
            var rules = await _formRulesService.GetAllRulesAsync();
            return Ok(rules);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormRuleDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRuleById(int id)
        {
            var rule = await _formRulesService.GetRuleByIdAsync(id);
            if (rule == null)
            {
                return NotFound($"Rule with ID {id} not found.");
            }

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

            var ruleDto = new FormRuleDto
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
                ExecutionOrder = rule.ExecutionOrder ?? 1
            };

            return Ok(ruleDto);
        }

        // ----------------------------------------------------------------------
        // --- 2. POST Operation (Create) ---
        // ----------------------------------------------------------------------
        [HttpPost]
        [ProducesResponseType(typeof(FormRuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRule([FromBody] CreateFormRuleDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRule = await _formRulesService.CreateRuleAsync(createDto);

            // Reload rule with actions to get the complete data
            var ruleWithActions = await _formRulesService.GetRuleByIdAsync(createdRule.Id);

            // Load actions from separate table
            var actions = ruleWithActions?.FORM_RULE_ACTIONS?
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

            var elseActions = ruleWithActions?.FORM_RULE_ACTIONS?
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

            var createdRuleDto = new FormRuleDto
            {
                Id = createdRule.Id,
                FormBuilderId = createdRule.FormBuilderId,
                RuleName = createdRule.RuleName,
                ConditionField = createdRule.ConditionField,
                ConditionOperator = createdRule.ConditionOperator,
                ConditionValue = createdRule.ConditionValue,
                ConditionValueType = createdRule.ConditionValueType,
                Actions = actions.Any() ? actions : null, // Return as List, not JSON
                ElseActions = elseActions.Any() ? elseActions : null, // Return as List, not JSON
                IsActive = createdRule.IsActive,
                ExecutionOrder = createdRule.ExecutionOrder ?? 1
            };

            return CreatedAtAction(nameof(GetRuleById), new { id = createdRuleDto.Id }, createdRuleDto);
        }

        // ----------------------------------------------------------------------
        // --- 3. PUT Operation (Update) ---
        // ----------------------------------------------------------------------
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRule(int id, [FromBody] UpdateFormRuleDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          

            var ruleExists = await _formRulesService.RuleExistsAsync(id);
            if (!ruleExists)
            {
                return NotFound($"Rule with ID {id} not found.");
            }

            var isUpdated = await _formRulesService.UpdateRuleAsync(updateDto, id);

            if (!isUpdated)
            {
                return BadRequest("Failed to update the rule.");
            }

            return NoContent();
        }

        // ----------------------------------------------------------------------
        // --- 4. DELETE Operation ---
        // ----------------------------------------------------------------------
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRule(int id)
        {
            var isDeleted = await _formRulesService.DeleteRuleAsync(id);

            if (!isDeleted)
            {
                return NotFound($"Rule with ID {id} not found.");
            }

            return NoContent();
        }

        // ----------------------------------------------------------------------
        // --- 5. Validation & Utility Operations ---
        // ----------------------------------------------------------------------

        [HttpGet("check-name/{ruleName}/form/{formBuilderId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckRuleNameUnique(int formBuilderId, string ruleName, [FromQuery] int? ignoreId = null)
        {
            var isUnique = await _formRulesService.IsRuleNameUniqueAsync(formBuilderId, ruleName, ignoreId);
            return Ok(new
            {
                formBuilderId,
                ruleName,
                isUnique,
                message = isUnique ? "Rule name is available" : "Rule name is already in use"
            });
        }

        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RuleExists(int id)
        {
            var exists = await _formRulesService.RuleExistsAsync(id);
            return Ok(new
            {
                id,
                exists,
                message = exists ? "Rule exists" : "Rule does not exist"
            });
        }

        // ----------------------------------------------------------------------
        // --- 6. Bulk Operations ---
        // ----------------------------------------------------------------------

        [HttpPost("bulk")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRulesBulk([FromBody] List<CreateFormRuleDto> createDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var results = new List<object>();
            var createdRules = new List<FormRuleDto>();

            foreach (var createDto in createDtos)
            {
                try
                {
                    var createdRule = await _formRulesService.CreateRuleAsync(createDto);
                    // Reload rule to get complete data with actions
                    var fullRule = await _formRulesService.GetRuleByIdAsync(createdRule.Id);
                    var ruleDto = new FormRuleDto
                    {
                        Id = fullRule.Id,
                        FormBuilderId = fullRule.FormBuilderId,
                        RuleName = fullRule.RuleName,
                        ConditionField = fullRule.ConditionField,
                        ConditionOperator = fullRule.ConditionOperator,
                        ConditionValue = fullRule.ConditionValue,
                        ConditionValueType = fullRule.ConditionValueType,
                        Actions = fullRule.FORM_RULE_ACTIONS?
                            .Where(a => !a.IsElseAction && a.IsActive)
                            .OrderBy(a => a.ActionOrder)
                            .Select(a => new ActionDataDto
                            {
                                Type = a.ActionType,
                                FieldCode = a.FieldCode,
                                Value = a.Value,
                                Expression = a.Expression
                            })
                            .ToList(),
                        ElseActions = fullRule.FORM_RULE_ACTIONS?
                            .Where(a => a.IsElseAction && a.IsActive)
                            .OrderBy(a => a.ActionOrder)
                            .Select(a => new ActionDataDto
                            {
                                Type = a.ActionType,
                                FieldCode = a.FieldCode,
                                Value = a.Value,
                                Expression = a.Expression
                            })
                            .ToList(),
                        IsActive = fullRule.IsActive,
                        ExecutionOrder = fullRule.ExecutionOrder ?? 1,
                        FormName = fullRule.FORM_BUILDER?.FormName ?? string.Empty,
                        FormCode = fullRule.FORM_BUILDER?.FormCode ?? string.Empty
                    };
                    createdRules.Add(ruleDto);

                    results.Add(new
                    {
                        success = true,
                        ruleName = createDto.RuleName,
                        message = "Created successfully"
                    });
                }
                catch (InvalidOperationException ex)
                {
                    results.Add(new
                    {
                        success = false,
                        ruleName = createDto.RuleName,
                        message = ex.Message
                    });
                }
            }

            return Ok(new
            {
                total = createDtos.Count,
                successful = createdRules.Count,
                failed = createDtos.Count - createdRules.Count,
                results,
                createdRules
            });
        }

        // ----------------------------------------------------------------------
        // --- 7. Additional Utility Endpoints ---
        // ----------------------------------------------------------------------

        [HttpGet("form/{formBuilderId}")]
        [ProducesResponseType(typeof(IEnumerable<FormRuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRulesByFormId(int formBuilderId)
        {
            var allRules = await _formRulesService.GetAllRulesAsync();
            var formRules = allRules.Where(r => r.FormBuilderId == formBuilderId).ToList();

            return Ok(formRules);
        }

        [HttpGet("form/{formBuilderId}/active")]
        [AllowAnonymous] // Allow anonymous access for public form viewing
        [ProducesResponseType(typeof(IEnumerable<FormRuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetActiveRulesByFormId(int formBuilderId)
        {
            var activeRules = await _formRulesService.GetActiveRulesByFormIdAsync(formBuilderId);
            return Ok(activeRules);
        }

        [HttpGet("stats")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRulesStats()
        {
            var allRules = await _formRulesService.GetAllRulesAsync();
            var totalRules = allRules.Count();
            var activeRules = allRules.Count(r => r.IsActive);
            var inactiveRules = totalRules - activeRules;

            return Ok(new
            {
                totalRules,
                activeRules,
                inactiveRules,
                rulesByForm = allRules.GroupBy(r => r.FormBuilderId)
                    .Select(g => new
                    {
                        formBuilderId = g.Key,
                        count = g.Count(),
                        activeCount = g.Count(r => r.IsActive)
                    })
            });
        }

        // ----------------------------------------------------------------------
        // --- 8. Validation Endpoint ---
        // ----------------------------------------------------------------------

        /// <summary>
        /// Validates form rules against field values (used when submitting form)
        /// POST /api/FormRules/validate
        /// </summary>
        [HttpPost("validate")]
        [AllowAnonymous] // Allow anonymous for public form submissions
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ValidateFormRules([FromBody] ValidateFormRulesRequestDto request)
        {
            if (request == null || request.FormBuilderId <= 0)
            {
                return BadRequest(new { message = "Invalid request. FormBuilderId is required." });
            }

            if (request.FieldValues == null)
            {
                request.FieldValues = new Dictionary<string, object>();
            }

            try
            {
                // Get active rules for the form
                var allRules = await _formRulesService.GetAllRulesAsync();
                var activeRules = allRules
                    .Where(r => r.FormBuilderId == request.FormBuilderId && r.IsActive)
                    .OrderBy(r => r.ExecutionOrder ?? 1)
                    .ToList();

                var validationErrors = new List<string>();

                foreach (var rule in activeRules)
                {
                    try
                    {
                        // Build rule data from fields (new approach) or parse RuleJson (backward compatibility)
                        FormRuleDataDto? ruleData = null;
                        
                        if (!string.IsNullOrWhiteSpace(rule.ConditionField) && !string.IsNullOrWhiteSpace(rule.ConditionOperator))
                        {
                            // Use new approach with separate fields
                            // Convert Actions and ElseActions Lists to JSON strings
                            string? actionsJson = null;
                            string? elseActionsJson = null;

                            if (rule.Actions != null && rule.Actions.Any())
                            {
                                actionsJson = System.Text.Json.JsonSerializer.Serialize(rule.Actions);
                            }

                            if (rule.ElseActions != null && rule.ElseActions.Any())
                            {
                                elseActionsJson = System.Text.Json.JsonSerializer.Serialize(rule.ElseActions);
                            }

                            ruleData = _ruleEvaluationService.BuildRuleDataFromFields(
                                rule.ConditionField,
                                rule.ConditionOperator,
                                rule.ConditionValue,
                                rule.ConditionValueType,
                                actionsJson,
                                elseActionsJson);
                        }

                        if (ruleData == null || ruleData.Condition == null)
                        {
                            _logger?.LogWarning("Invalid rule structure for rule {RuleId}: {RuleName}", rule.Id, rule.RuleName);
                            continue;
                        }

                        // Evaluate condition
                        bool conditionMet = _ruleEvaluationService.EvaluateCondition(
                            ruleData.Condition,
                            request.FieldValues);

                        if (conditionMet)
                        {
                            // Validate actions (check if mandatory fields are filled)
                            if (ruleData.Actions != null && ruleData.Actions.Any())
                            {
                                var actionErrors = _ruleEvaluationService.ValidateActions(
                                    ruleData.Actions,
                                    request.FieldValues,
                                    rule.RuleName);
                                validationErrors.AddRange(actionErrors);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error evaluating rule {RuleId}: {RuleName}", rule.Id, rule.RuleName);
                        // Continue with other rules - don't fail entire validation
                    }
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        valid = false,
                        errors = validationErrors,
                        message = "Form validation failed based on rules"
                    });
                }

                return Ok(new
                {
                    valid = true,
                    message = "Form validation passed"
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error validating form rules");
                return StatusCode(500, new { message = "Internal server error during validation", error = ex.Message });
            }
        }

        // ----------------------------------------------------------------------
        // --- 9. Evaluate Single Rule Endpoint ---
        // ----------------------------------------------------------------------

        /// <summary>
        /// Evaluates a single rule with provided field values (for testing/debugging)
        /// POST /api/FormRules/evaluate
        /// </summary>
        [HttpPost("evaluate")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EvaluateRule([FromBody] EvaluateRuleRequestDto request)
        {
            if (request == null || request.RuleId <= 0)
            {
                return BadRequest(new { message = "Invalid request. RuleId is required." });
            }

            if (request.FieldValues == null)
            {
                request.FieldValues = new Dictionary<string, object>();
            }

            try
            {
                // Get the rule
                var rule = await _formRulesService.GetRuleByIdAsync(request.RuleId);
                if (rule == null)
                {
                    return NotFound(new { message = $"Rule with ID {request.RuleId} not found." });
                }

                // Build rule data from fields (new approach) or parse RuleJson (backward compatibility)
                FormRuleDataDto? ruleData = null;
                
                if (!string.IsNullOrWhiteSpace(rule.ConditionField) && !string.IsNullOrWhiteSpace(rule.ConditionOperator))
                {
                    // Build ActionsJson and ElseActionsJson from FORM_RULE_ACTIONS
                    string? actionsJson = null;
                    string? elseActionsJson = null;

                    if (rule.FORM_RULE_ACTIONS != null && rule.FORM_RULE_ACTIONS.Any())
                    {
                        var actions = rule.FORM_RULE_ACTIONS
                            .Where(a => !a.IsElseAction && a.IsActive)
                            .OrderBy(a => a.ActionOrder)
                            .Select(a => new
                            {
                                Type = a.ActionType,
                                FieldCode = a.FieldCode,
                                Value = a.Value,
                                Expression = a.Expression
                            })
                            .ToList();

                        var elseActionsList = rule.FORM_RULE_ACTIONS
                            .Where(a => a.IsElseAction && a.IsActive)
                            .OrderBy(a => a.ActionOrder)
                            .Select(a => new
                            {
                                Type = a.ActionType,
                                FieldCode = a.FieldCode,
                                Value = a.Value,
                                Expression = a.Expression
                            })
                            .ToList();

                        if (actions.Any())
                            actionsJson = System.Text.Json.JsonSerializer.Serialize(actions);
                        
                        if (elseActionsList.Any())
                            elseActionsJson = System.Text.Json.JsonSerializer.Serialize(elseActionsList);
                    }

                    // Use new approach with separate fields
                    ruleData = _ruleEvaluationService.BuildRuleDataFromFields(
                        rule.ConditionField,
                        rule.ConditionOperator,
                        rule.ConditionValue,
                        rule.ConditionValueType,
                        actionsJson,
                        elseActionsJson);
                }
                else if (!string.IsNullOrWhiteSpace(rule.RuleJson))
                {
                    // Fallback to old RuleJson approach for backward compatibility
                    ruleData = _ruleEvaluationService.ParseRuleJson(rule.RuleJson);
                }

                if (ruleData == null || ruleData.Condition == null)
                {
                    return BadRequest(new { message = "Invalid rule structure. Condition fields are required." });
                }

                // Evaluate condition
                bool conditionMet = _ruleEvaluationService.EvaluateCondition(
                    ruleData.Condition,
                    request.FieldValues);

                // Get actions that would be applied
                var appliedActions = new List<object>();
                var elseActions = new List<object>();

                if (conditionMet)
                {
                    if (ruleData.Actions != null && ruleData.Actions.Any())
                    {
                        foreach (var action in ruleData.Actions)
                        {
                            appliedActions.Add(new
                            {
                                type = action.Type,
                                fieldCode = action.FieldCode,
                                value = action.Value,
                                expression = action.Expression
                            });
                        }
                    }
                }
                else
                {
                    if (ruleData.ElseActions != null && ruleData.ElseActions.Any())
                    {
                        foreach (var action in ruleData.ElseActions)
                        {
                            elseActions.Add(new
                            {
                                type = action.Type,
                                fieldCode = action.FieldCode,
                                value = action.Value,
                                expression = action.Expression
                            });
                        }
                    }
                }

                // Simulate field state after applying actions
                var simulatedFieldStates = new Dictionary<string, object>(request.FieldValues);
                
                if (conditionMet && ruleData.Actions != null)
                {
                    foreach (var action in ruleData.Actions)
                    {
                        switch (action.Type)
                        {
                            case "SetDefault":
                                if (!simulatedFieldStates.ContainsKey(action.FieldCode))
                                {
                                    simulatedFieldStates[action.FieldCode] = action.Value ?? "";
                                }
                                break;
                            case "ClearValue":
                                if (simulatedFieldStates.ContainsKey(action.FieldCode))
                                {
                                    simulatedFieldStates[action.FieldCode] = "";
                                }
                                break;
                            case "Compute":
                                if (!string.IsNullOrEmpty(action.Expression))
                                {
                                    try
                                    {
                                        var computedValue = _ruleEvaluationService.EvaluateExpression(
                                            action.Expression,
                                            request.FieldValues);
                                        simulatedFieldStates[action.FieldCode] = computedValue;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger?.LogWarning(ex, "Error computing expression for field {FieldCode}", action.FieldCode);
                                    }
                                }
                                break;
                        }
                    }
                }

                return Ok(new
                {
                    ruleId = rule.Id,
                    ruleName = rule.RuleName,
                    conditionMet,
                    condition = new
                    {
                        field = ruleData.Condition.Field,
                        @operator = ruleData.Condition.Operator,
                        value = ruleData.Condition.Value,
                        valueType = ruleData.Condition.ValueType
                    },
                    appliedActions,
                    elseActions,
                    simulatedFieldStates,
                    message = conditionMet ? "Condition is met - THEN actions would be applied" : "Condition is not met - ELSE actions would be applied (if any)"
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error evaluating rule {RuleId}", request.RuleId);
                return StatusCode(500, new { message = "Internal server error during rule evaluation", error = ex.Message });
            }
        }
    }
}