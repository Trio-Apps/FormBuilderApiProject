using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FormBuilder.API.Models.DTOs;
using FormBuilder.Domain.Interfaces.Services;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormSubmissionsController : ControllerBase
    {
        private readonly IFormSubmissionService _formSubmissionService;

        public FormSubmissionsController(IFormSubmissionService formSubmissionService)
        {
            _formSubmissionService = formSubmissionService;
        }

        // GET: api/formsubmissions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _formSubmissionService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/formsubmissions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _formSubmissionService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/formsubmissions/form-builder/5
        [HttpGet("form-builder/{formBuilderId}")]
        public async Task<IActionResult> GetByFormBuilderId(int formBuilderId)
        {
            var result = await _formSubmissionService.GetByFormBuilderIdAsync(formBuilderId);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/formsubmissions/user/user123
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var result = await _formSubmissionService.GetByUserIdAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/formsubmissions/status/draft
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _formSubmissionService.GetByStatusAsync(status);
            return StatusCode(result.StatusCode, result);
        }

        // POST: api/formsubmissions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFormSubmissionDto createDto)
        {
            var result = await _formSubmissionService.CreateAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        // PUT: api/formsubmissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFormSubmissionDto updateDto)
        {
            var result = await _formSubmissionService.UpdateAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        // PUT: api/formsubmissions/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] FormSubmissionStatusDto statusDto)
        {
            var result = await _formSubmissionService.UpdateStatusAsync(id, statusDto);
            return StatusCode(result.StatusCode, result);
        }

        // DELETE: api/formsubmissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _formSubmissionService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // POST: api/formsubmissions/filter
        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] FormSubmissionFilterDto filterDto)
        {
            var result = await _formSubmissionService.FilterSubmissionsAsync(filterDto);
            return StatusCode(result.StatusCode, result);
        }
    }
}