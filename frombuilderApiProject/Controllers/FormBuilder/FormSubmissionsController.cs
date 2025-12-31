using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.API.Models;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class FormSubmissionsController : ControllerBase
    {
        private readonly IFormSubmissionsService _formSubmissionsService;

        public FormSubmissionsController(IFormSubmissionsService formSubmissionsService)
        {
            _formSubmissionsService = formSubmissionsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _formSubmissionsService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _formSubmissionsService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetByIdWithDetails(int id)
        {
            var result = await _formSubmissionsService.GetByIdWithDetailsAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("document/{documentNumber}")]
        public async Task<IActionResult> GetByDocumentNumber(string documentNumber)
        {
            var result = await _formSubmissionsService.GetByDocumentNumberAsync(documentNumber);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("form/{formBuilderId}")]
        public async Task<IActionResult> GetByFormBuilderId(int formBuilderId)
        {
            var result = await _formSubmissionsService.GetByFormBuilderIdAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("document-type/{documentTypeId}")]
        public async Task<IActionResult> GetByDocumentTypeId(int documentTypeId)
        {
            var result = await _formSubmissionsService.GetByDocumentTypeIdAsync(documentTypeId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var result = await _formSubmissionsService.GetByUserIdAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _formSubmissionsService.GetByStatusAsync(status);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormSubmissionDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid data", ModelState));

            var result = await _formSubmissionsService.CreateAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Create a new draft submission automatically determining Document Type and Series from FormBuilderId
        /// This endpoint implements the runtime flow: automatically loads Document Type and selects default Series
        /// </summary>
        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft([FromQuery] int formBuilderId, [FromQuery] int projectId, [FromQuery] string submittedByUserId)
        {
            if (formBuilderId <= 0)
                return BadRequest(new ApiResponse(400, "FormBuilderId is required"));
            if (projectId <= 0)
                return BadRequest(new ApiResponse(400, "ProjectId is required"));
            if (string.IsNullOrWhiteSpace(submittedByUserId))
                return BadRequest(new ApiResponse(400, "SubmittedByUserId is required"));

            var result = await _formSubmissionsService.CreateDraftAsync(formBuilderId, projectId, submittedByUserId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFormSubmissionDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid data", ModelState));

            var result = await _formSubmissionsService.UpdateAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _formSubmissionsService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitFormDto submitDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid data", ModelState));

            var result = await _formSubmissionsService.SubmitAsync(submitDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var result = await _formSubmissionsService.UpdateStatusAsync(id, status);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/exists")]
        public async Task<IActionResult> Exists(int id)
        {
            var result = await _formSubmissionsService.ExistsAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("save-data")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SaveFormSubmissionData([FromBody] SaveFormSubmissionDataDto saveDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid data", ModelState));

            var result = await _formSubmissionsService.SaveFormSubmissionDataAsync(saveDto);
            return StatusCode(result.StatusCode, result);
        }
    }
}