using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]

    public class FormGridColumnsController : ControllerBase
    {
        private readonly IFormGridColumnService _formGridColumnService;

        public FormGridColumnsController(IFormGridColumnService formGridColumnService)
        {
            _formGridColumnService = formGridColumnService;
        }

        /// <summary>
        /// Get all grid columns
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var response = await _formGridColumnService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get grid column by ID
        /// </summary>
        /// <param name="id">Grid column ID</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            var response = await _formGridColumnService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get grid columns by grid ID
        /// </summary>
        /// <param name="gridId">Grid ID</param>
        [HttpGet("by-grid/{gridId}")]
        public async Task<ActionResult<ApiResponse>> GetByGridId(int gridId)
        {
            var response = await _formGridColumnService.GetByGridIdAsync(gridId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get active grid columns by grid ID
        /// </summary>
        /// <param name="gridId">Grid ID</param>
        [HttpGet("active-by-grid/{gridId}")]
        public async Task<ActionResult<ApiResponse>> GetActiveByGridId(int gridId)
        {
            var response = await _formGridColumnService.GetActiveByGridIdAsync(gridId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get grid column by column code
        /// </summary>
        /// <param name="columnCode">Column code</param>
        /// <param name="gridId">Grid ID</param>
        [HttpGet("by-code/{columnCode}/{gridId}")]
        public async Task<ActionResult<ApiResponse>> GetByColumnCode(string columnCode, int gridId)
        {
            var response = await _formGridColumnService.GetByColumnCodeAsync(columnCode, gridId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get grid columns by form builder ID
        /// </summary>
        /// <param name="formBuilderId">Form builder ID</param>
        [HttpGet("by-form-builder/{formBuilderId}")]
        public async Task<ActionResult<ApiResponse>> GetByFormBuilderId(int formBuilderId)
        {
            var response = await _formGridColumnService.GetByFormBuilderIdAsync(formBuilderId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get grid columns by field type ID
        /// </summary>
        /// <param name="fieldTypeId">Field type ID</param>
        [HttpGet("by-field-type/{fieldTypeId}")]
        public async Task<ActionResult<ApiResponse>> GetByFieldTypeId(int fieldTypeId)
        {
            var response = await _formGridColumnService.GetByFieldTypeIdAsync(fieldTypeId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create new grid column
        /// </summary>
        /// <param name="createDto">Grid column data</param>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] CreateFormGridColumnDto createDto)
        {
            var response = await _formGridColumnService.CreateAsync(createDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update existing grid column
        /// </summary>
        /// <param name="id">Grid column ID</param>
        /// <param name="updateDto">Updated grid column data</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromBody] UpdateFormGridColumnDto updateDto)
        {
            var response = await _formGridColumnService.UpdateAsync(id, updateDto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete grid column
        /// </summary>
        /// <param name="id">Grid column ID</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var response = await _formGridColumnService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Toggle grid column active status
        /// </summary>
        /// <param name="id">Grid column ID</param>
        /// <param name="isActive">Active status</param>
        [HttpPatch("{id}/toggle-active")]
        public async Task<ActionResult<ApiResponse>> ToggleActive(int id, [FromBody] bool isActive)
        {
            var response = await _formGridColumnService.ToggleActiveAsync(id, isActive);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Check if grid column exists
        /// </summary>
        /// <param name="id">Grid column ID</param>
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<ApiResponse>> Exists(int id)
        {
            var response = await _formGridColumnService.ExistsAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Check if column code exists
        /// </summary>
        /// <param name="columnCode">Column code</param>
        /// <param name="gridId">Grid ID</param>
        /// <param name="excludeId">Exclude ID (optional)</param>
        [HttpGet("code-exists/{columnCode}/{gridId}")]
        public async Task<ActionResult<ApiResponse>> ColumnCodeExists(string columnCode, int gridId, [FromQuery] int? excludeId = null)
        {
            var response = await _formGridColumnService.ColumnCodeExistsAsync(columnCode, gridId, excludeId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get next column order for a grid
        /// </summary>
        /// <param name="gridId">Grid ID</param>
        [HttpGet("next-order/{gridId}")]
        public async Task<ActionResult<ApiResponse>> GetNextColumnOrder(int gridId)
        {
            var response = await _formGridColumnService.GetNextColumnOrderAsync(gridId);
            return StatusCode(response.StatusCode, response);
        }
    }
}