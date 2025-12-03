using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormRules;
using FormBuilder.Domian.Entitys.froms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FORM_RULESService : IFORM_RULESService
    {
        private readonly IunitOfwork _unitOfWork;

        public FORM_RULESService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

            // Validate JSON structure
            if (!IsValidJson(ruleDto.RuleJson))
                throw new InvalidOperationException("Rule JSON is not valid.");

            var ruleEntity = new FORM_RULES
            {
                FormBuilderId = ruleDto.FormBuilderId,
                RuleName = ruleDto.RuleName,
                RuleJson = ruleDto.RuleJson,
                IsActive = ruleDto.IsActive
            };

            _unitOfWork.Repositary<FORM_RULES>().Add(ruleEntity);
            await _unitOfWork.CompleteAsyn();

            return ruleEntity;
        }

        public async Task<FORM_RULES> GetRuleByIdAsync(int id)
        {
            return await _unitOfWork.Repositary<FORM_RULES>()
                .SingleOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<FormRuleDto>> GetAllRulesAsync()
        {
            // 1. Update GetAllAsync to include the related FORM_BUILDERS entity.
            var rules = await _unitOfWork.Repositary<FORM_RULES>()
                .GetAllAsync(
                    filter: null, // No filter needed for getting all records
                    includes: rule => rule.FORM_BUILDER // Assuming navigation property is named FORM_BUILDERS
                );

            // 2. Update the Select to map the new fields from the included entity.
            return rules.Select(rule => new FormRuleDto
            {
                Id = rule.Id,
                FormBuilderId = rule.FormBuilderId,
                RuleName = rule.RuleName,
                RuleJson = rule.RuleJson,
                IsActive = rule.IsActive,

                // Map the new fields from the included FORM_BUILDERS entity
                FormName = rule.FORM_BUILDER.FormName,
                FormCode = rule.FORM_BUILDER.FormCode

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

            // Validate JSON structure
            if (!IsValidJson(ruleDto.RuleJson))
                throw new InvalidOperationException("Rule JSON is not valid.");

            // Update properties
            existingRule.FormBuilderId = ruleDto.FormBuilderId;
            existingRule.RuleName = ruleDto.RuleName;
            existingRule.RuleJson = ruleDto.RuleJson;
            existingRule.IsActive = ruleDto.IsActive;

            _unitOfWork.Repositary<FORM_RULES>().Update(existingRule);
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

        // Private helper method
        private bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                return false;

            try
            {
                JsonSerializer.Deserialize<object>(jsonString);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}