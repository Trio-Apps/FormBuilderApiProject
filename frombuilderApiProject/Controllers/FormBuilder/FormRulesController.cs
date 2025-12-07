using FormBuilder.Core.DTOS.FormRules;
using FormBuilder.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administration")]
    public class FormRulesController : ControllerBase
    {
        private readonly IFORM_RULESService _formRulesService;

        public FormRulesController(IFORM_RULESService formRulesService)
        {
            _formRulesService = formRulesService ?? throw new ArgumentNullException(nameof(formRulesService));
        }

        // ----------------------------------------------------------------------
        // --- 1. GET Operations (Read) ---
        // ----------------------------------------------------------------------

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormRuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRules()
        {
            try
            {
                var rules = await _formRulesService.GetAllRulesAsync();
                return Ok(rules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormRuleDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRuleById(int id)
        {
            try
            {
                var rule = await _formRulesService.GetRuleByIdAsync(id);
                if (rule == null)
                {
                    return NotFound($"Rule with ID {id} not found.");
                }

                var ruleDto = new FormRuleDto
                {
                    Id = rule.Id,
                    FormBuilderId = rule.FormBuilderId,
                    RuleName = rule.RuleName,
                    RuleJson = rule.RuleJson,
                    IsActive = rule.IsActive
                };

                return Ok(ruleDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

            try
            {
                var createdRule = await _formRulesService.CreateRuleAsync(createDto);

                var createdRuleDto = new FormRuleDto
                {
                    Id = createdRule.Id,
                    FormBuilderId = createdRule.FormBuilderId,
                    RuleName = createdRule.RuleName,
                    RuleJson = createdRule.RuleJson,
                    IsActive = createdRule.IsActive
                };

                return CreatedAtAction(nameof(GetRuleById), new { id = createdRuleDto.Id }, createdRuleDto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("already in use") || ex.Message.Contains("not valid"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating rule: {ex.Message}");
            }
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

          

            try
            {
                var ruleExists = await _formRulesService.RuleExistsAsync(id);
                if (!ruleExists)
                {
                    return NotFound($"Rule with ID {id} not found.");
                }

                var isUpdated = await _formRulesService.UpdateRuleAsync(updateDto,id);

                if (!isUpdated)
                {
                    return BadRequest("Failed to update the rule.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already in use") || ex.Message.Contains("not valid"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating rule: {ex.Message}");
            }
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
            try
            {
                var isDeleted = await _formRulesService.DeleteRuleAsync(id);

                if (!isDeleted)
                {
                    return NotFound($"Rule with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting rule: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 5. Validation & Utility Operations ---
        // ----------------------------------------------------------------------

        [HttpGet("check-name/{ruleName}/form/{formBuilderId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckRuleNameUnique(int formBuilderId, string ruleName, [FromQuery] int? ignoreId = null)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RuleExists(int id)
        {
            try
            {
                var exists = await _formRulesService.RuleExistsAsync(id);
                return Ok(new
                {
                    id,
                    exists,
                    message = exists ? "Rule exists" : "Rule does not exist"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

            try
            {
                var results = new List<object>();
                var createdRules = new List<FormRuleDto>();

                foreach (var createDto in createDtos)
                {
                    try
                    {
                        var createdRule = await _formRulesService.CreateRuleAsync(createDto);
                        var ruleDto = new FormRuleDto
                        {
                            Id = createdRule.Id,
                            FormBuilderId = createdRule.FormBuilderId,
                            RuleName = createdRule.RuleName,
                            RuleJson = createdRule.RuleJson,
                            IsActive = createdRule.IsActive
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error in bulk operation: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 7. Additional Utility Endpoints ---
        // ----------------------------------------------------------------------

        [HttpGet("form/{formBuilderId}")]
        [ProducesResponseType(typeof(IEnumerable<FormRuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRulesByFormId(int formBuilderId)
        {
            try
            {
                var allRules = await _formRulesService.GetAllRulesAsync();
                var formRules = allRules.Where(r => r.FormBuilderId == formBuilderId).ToList();

                return Ok(formRules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("form/{formBuilderId}/active")]
        [ProducesResponseType(typeof(IEnumerable<FormRuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetActiveRulesByFormId(int formBuilderId)
        {
            try
            {
                var allRules = await _formRulesService.GetAllRulesAsync();
                var activeRules = allRules.Where(r => r.FormBuilderId == formBuilderId && r.IsActive).ToList();

                return Ok(activeRules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stats")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRulesStats()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting rules statistics: {ex.Message}");
            }
        }
    }
}