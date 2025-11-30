using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormSubmissionAttachmentsController : ControllerBase
    {
        private readonly IFormSubmissionAttachmentsService _formSubmissionAttachmentsService;

        public FormSubmissionAttachmentsController(IFormSubmissionAttachmentsService formSubmissionAttachmentsService)
        {
            _formSubmissionAttachmentsService = formSubmissionAttachmentsService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FormSubmissionAttachmentDto>>>> GetAll()
        {
            var result = await _formSubmissionAttachmentsService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FormSubmissionAttachmentDto>>> GetById(int id)
        {
            var result = await _formSubmissionAttachmentsService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("submission/{submissionId}")]
        public async Task<ActionResult<ApiResponse<List<FormSubmissionAttachmentDto>>>> GetBySubmissionId(int submissionId)
        {
            var result = await _formSubmissionAttachmentsService.GetBySubmissionIdAsync(submissionId);
            return StatusCode(result.StatusCode, result);
        }

        // ... باقي الـ endpoints بنفس النمط

        [HttpPost]
        public async Task<ActionResult<ApiResponse<FormSubmissionAttachmentDto>>> Create([FromBody] CreateFormSubmissionAttachmentDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<FormSubmissionAttachmentDto>(400, "Invalid data"));

            var result = await _formSubmissionAttachmentsService.CreateAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        // ... باقي الـ endpoints
    }
}