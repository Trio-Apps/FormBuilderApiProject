using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.API.Models;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administration")]
    public class FormulasController : ControllerBase
    {
        private readonly IFormulaService _formulaService;

        public FormulasController(IFormulaService formulaService)
        {
            _formulaService = formulaService;
        }

        #region Basic CRUD Operations

        [HttpGet("form-builder/{formBuilderId}")]
        public async Task<IActionResult> GetByFormBuilder(int formBuilderId)
        {
            var result = await _formulaService.GetAllAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _formulaService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("code/{code}/{formBuilderId}")]
        public async Task<IActionResult> GetByCode(string code, int formBuilderId)
        {
            var result = await _formulaService.GetByCodeAsync(code, formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormulaDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.CreateAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFormulaDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.UpdateAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _formulaService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("form-builder/{formBuilderId}")]
        public async Task<IActionResult> DeleteByFormBuilder(int formBuilderId)
        {
            var result = await _formulaService.DeleteByFormBuilderIdAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Status Management

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id, [FromBody] bool isActive)
        {
            var result = await _formulaService.ToggleActiveAsync(id, isActive);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form-builder/{formBuilderId}/active")]
        public async Task<IActionResult> GetActive(int formBuilderId)
        {
            var result = await _formulaService.GetActiveAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form-builder/{formBuilderId}/inactive")]
        public async Task<IActionResult> GetInactive(int formBuilderId)
        {
            var result = await _formulaService.GetInactiveAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Query Operations

        [HttpGet("with-details")]
        public async Task<IActionResult> GetFormulasWithDetails([FromQuery] int formBuilderId = 0)
        {
            var result = await _formulaService.GetFormulasWithDetailsAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/with-details")]
        public async Task<IActionResult> GetByIdWithDetails(int id)
        {
            var result = await _formulaService.GetByIdWithDetailsAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("result-field/{resultFieldId}")]
        public async Task<IActionResult> GetByResultField(int? resultFieldId)
        {
            var result = await _formulaService.GetByResultFieldAsync(resultFieldId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form-builder/{formBuilderId}/without-result-field")]
        public async Task<IActionResult> GetFormulasWithoutResultField(int formBuilderId)
        {
            var result = await _formulaService.GetFormulasWithoutResultFieldAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFormulas([FromQuery] string term, [FromQuery] int formBuilderId)
        {
            var result = await _formulaService.SearchFormulasAsync(term, formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Validation Operations

        [HttpPost("validate-expression")]
        public async Task<IActionResult> ValidateExpression([FromBody] ValidateExpressionDto validationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.ValidateExpressionAsync(validationDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("validate-expression-with-details")]
        public async Task<IActionResult> ValidateExpressionWithDetails([FromBody] ValidateExpressionDto validationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.ValidateExpressionWithDetailsAsync(validationDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("check-code-exists")]
        public async Task<IActionResult> CodeExists([FromQuery] string code, [FromQuery] int formBuilderId, [FromQuery] int? excludeId = null)
        {
            var result = await _formulaService.CodeExistsAsync(code, formBuilderId, excludeId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/is-active")]
        public async Task<IActionResult> IsActive(int id)
        {
            var result = await _formulaService.IsActiveAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form-builder/{formBuilderId}/has-active")]
        public async Task<IActionResult> HasActiveFormulas(int formBuilderId)
        {
            var result = await _formulaService.HasActiveFormulasAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Utility Operations

        [HttpGet("{id}/referenced-field-codes")]
        public async Task<IActionResult> GetReferencedFieldCodes(int id)
        {
            var result = await _formulaService.GetReferencedFieldCodesAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form-builder/{formBuilderId}/count")]
        public async Task<IActionResult> CountFormulas(int formBuilderId)
        {
            var result = await _formulaService.CountFormulasAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/expression")]
        public async Task<IActionResult> UpdateFormulaExpression(int id, [FromBody] string expressionText)
        {
            var result = await _formulaService.UpdateFormulaExpressionAsync(id, expressionText);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form-builder/{formBuilderId}/field-codes")]
        public async Task<IActionResult> GetFieldCodesForForm(int formBuilderId)
        {
            var result = await _formulaService.GetFieldCodesForFormAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region FORMULA_VARIABLES Operations

        [HttpGet("{formulaId}/variables")]
        public async Task<IActionResult> GetFormulaVariables(int formulaId)
        {
            var result = await _formulaService.GetFormulaVariablesAsync(formulaId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{formulaId}/variables/count")]
        public async Task<IActionResult> CountVariables(int formulaId)
        {
            var result = await _formulaService.CountVariablesAsync(formulaId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{formulaId}/has-variables")]
        public async Task<IActionResult> HasVariables(int formulaId)
        {
            var result = await _formulaService.HasVariablesAsync(formulaId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Additional Formula Operations

        [HttpGet("by-field/{fieldId}")]
        public async Task<IActionResult> GetFormulasByField(int fieldId)
        {
            var result = await _formulaService.GetFormulasByFieldAsync(fieldId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form-builder/{formBuilderId}/statistics")]
        public async Task<IActionResult> GetFormulaStatistics(int formBuilderId)
        {
            var result = await _formulaService.GetFormulaStatisticsAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("batch-update-status")]
        public async Task<IActionResult> BatchUpdateFormulaStatus([FromBody] BatchUpdateFormulaStatusDto batchDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.BatchUpdateFormulaStatusAsync(batchDto.FormulaIds, batchDto.IsActive);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{sourceFormulaId}/duplicate")]
        public async Task<IActionResult> DuplicateFormula(int sourceFormulaId, [FromBody] DuplicateFormulaDto duplicateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.DuplicateFormulaAsync(sourceFormulaId, duplicateDto);
            return StatusCode(result.StatusCode, result);
        }

        #endregion

        #region Formula Calculation Endpoints

        [HttpPost("{formulaId}/calculate")]
        public async Task<IActionResult> CalculateFormula(int formulaId, [FromBody] Dictionary<string, object> fieldValues)
        {
            var result = await _formulaService.CalculateFormulaAsync(formulaId, fieldValues);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("calculate-expression")]
        public async Task<IActionResult> CalculateExpression([FromBody] CalculateExpressionDto calculateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.CalculateExpressionAsync(calculateDto.ExpressionText, calculateDto.FieldValues);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("calculate-advanced")]
        public async Task<IActionResult> CalculateAdvanced([FromBody] CalculateAdvancedDto calculateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.CalculateWithAllOperationsAsync(
                calculateDto.ExpressionText,
                calculateDto.FieldValues);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("calculate-safe")]
        public async Task<IActionResult> CalculateSafe([FromBody] CalculateExpressionDto calculateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.SafeCalculateExpressionAsync(
                calculateDto.ExpressionText,
                calculateDto.FieldValues);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("form-builder/{formBuilderId}/batch-calculate")]
        public async Task<IActionResult> BatchCalculateFormulas(int formBuilderId, [FromBody] Dictionary<string, object> fieldValues)
        {
            var result = await _formulaService.BatchCalculateFormulasAsync(formBuilderId, fieldValues);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("preview-calculation")]
        public async Task<IActionResult> PreviewCalculation([FromBody] PreviewCalculationDto previewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid model state", ModelState));

            var result = await _formulaService.PreviewCalculationAsync(previewDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{formulaId}/test-with-samples")]
        public async Task<IActionResult> TestFormulaWithSampleData(int formulaId)
        {
            var result = await _formulaService.TestFormulaWithSampleDataAsync(formulaId);
            return StatusCode(result.StatusCode, result);
        }

        #endregion
    }

    // DTO classes for calculation
    public class BatchUpdateFormulaStatusDto
    {
        public List<int> FormulaIds { get; set; } = new List<int>();
        public bool IsActive { get; set; }
    }

    public class CalculateExpressionDto
    {
        public string ExpressionText { get; set; } = string.Empty;
        public Dictionary<string, object> FieldValues { get; set; } = new Dictionary<string, object>();
    }

    public class CalculateAdvancedDto
    {
        public string ExpressionText { get; set; } = string.Empty;
        public Dictionary<string, object> FieldValues { get; set; } = new Dictionary<string, object>();
    }
}