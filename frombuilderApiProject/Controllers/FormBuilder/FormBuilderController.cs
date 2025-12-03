using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Core.IServices.FormBuilder.FormBuilder.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // REQUIRED for accessing claims/user ID
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FormBuilderController : ControllerBase
    {
        private readonly IFormBuilderService _formBuilderService;

        public FormBuilderController(IFormBuilderService formBuilderService)
        {
            _formBuilderService = formBuilderService;
        }

        // --- Manual Mapping Helper ---
        private FormBuilderDto MapToDto(FORM_BUILDER entity)
        {
            if (entity == null) return null;
            return new FormBuilderDto
            {
                
                Id = entity.Id,
                FormName = entity.FormName,
                FormCode = entity.FormCode,
                Description = entity.Description,
                Version = entity.Version,
                IsPublished = entity.IsPublished,
                IsActive = entity.IsActive,

                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        // --- GET Operations (Read) ---
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormBuilderDto>), 200)]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _formBuilderService.GetAllFormsAsync();
            var formsDto = forms.Select(f => MapToDto(f)).ToList();
            return Ok(formsDto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormBuilderDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFormById(int id)
        {
            var form = await _formBuilderService.GetFormByIdAsync(id, asNoTracking: true);
            if (form == null) return NotFound();
            return Ok(MapToDto(form));
        }

        [HttpGet("code/{formCode}")]
        [ProducesResponseType(typeof(FormBuilderDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFormByCode(string formCode)
        {
            var form = await _formBuilderService.GetFormByCodeAsync(formCode, asNoTracking: true);
            if (form == null) return NotFound();
            return Ok(MapToDto(form));
        }

        // --- POST Operation (Create) ---
        [HttpPost]
        [ProducesResponseType(typeof(FormBuilderDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CreateForm([FromBody] CreateFormBuilderDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Successfully retrieving authenticated user's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User ID not found in claims. Ensure the request is authenticated.");
            }

            try
            {
                var formEntity = new FORM_BUILDER
                {
                    FormName = createDto.FormName,
                    FormCode = createDto.FormCode,
                    Description = createDto.Description,

                    // Initializing non-nullable fields
                    Version = 1,
                    IsPublished = false,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,

                    // FOREIGN KEY FIX: Uses the authenticated user's ID to satisfy the database constraint.
                    CreatedByUserId = currentUserId
                };

                var createdForm = await _formBuilderService.CreateFormAsync(formEntity);
                var createdFormDto = MapToDto(createdForm);

                return CreatedAtAction(nameof(GetFormById), new { id = createdFormDto.Id }, createdFormDto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Form code"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // --- PUT Operation (Update) ---
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] UpdateFormBuilderDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingForm = await _formBuilderService.GetFormByIdAsync(id);
            if (existingForm == null)
            {
                return NotFound();
            }

            try
            {
                existingForm.FormName = updateDto.FormName;
                existingForm.FormCode = updateDto.FormCode;
                existingForm.Description = updateDto.Description;
                existingForm.IsPublished = updateDto.IsPublished;
                existingForm.IsActive = updateDto.IsActive;
                // 🛑 FIX: Changed 'existingForm.id' (lowercase) to 'existingForm.Id' (PascalCase)
                existingForm.Id = id;

                await _formBuilderService.UpdateFormAsync(existingForm);

                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Form code"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // --- DELETE Operation ---
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var isDeleted = await _formBuilderService.DeleteFormAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}