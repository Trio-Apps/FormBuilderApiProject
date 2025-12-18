using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims; // REQUIRED for accessing claims/user ID
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administration")]
    public class FormBuilderController : ControllerBase
    {
        private readonly IFormBuilderService _formBuilderService;

        public FormBuilderController(IFormBuilderService formBuilderService)
        {
            _formBuilderService = formBuilderService;
        }

        // --- GET Operations (Read) ---
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormBuilderDto>), 200)]
        public async Task<IActionResult> GetAllForms([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _formBuilderService.GetPagedAsync(page, pageSize);
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormBuilderDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFormById(int id)
        {
            var result = await _formBuilderService.GetByIdAsync(id, asNoTracking: true);
            return result.ToActionResult();
        }

        [HttpGet("code/{formCode}")]
        [ProducesResponseType(typeof(FormBuilderDto), 200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]

        public async Task<IActionResult> GetFormByCode(string formCode)
        {
            var result = await _formBuilderService.GetByCodeAsync(formCode, asNoTracking: true);
            return result.ToActionResult();
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

            createDto.CreatedByUserId = currentUserId;
            var result = await _formBuilderService.CreateAsync(createDto);
            if (result.Success && result.Data != null)
            {
                return CreatedAtAction(nameof(GetFormById), new { id = result.Data.Id }, result.Data);
            }

            return result.ToActionResult();
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

            var result = await _formBuilderService.UpdateAsync(id, updateDto);
            if (result.Success) return NoContent();
            return result.ToActionResult();
        }

        // --- DELETE Operation ---
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var result = await _formBuilderService.DeleteAsync(id);
            if (result.Success) return NoContent();
            return result.ToActionResult();
        }
    }
}