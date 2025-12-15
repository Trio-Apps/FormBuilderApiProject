using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administration")]

    public class FieldOptionsController : ControllerBase
    {
        private readonly IFieldOptionsService _fieldOptionsService;

        public FieldOptionsController(IFieldOptionsService fieldOptionsService)
        {
            _fieldOptionsService = fieldOptionsService;
        }

        // ================================
        // GET ALL FIELD OPTIONS
        // ================================
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllFieldOptions()
        {
            try
            {
                // Note: You'll need to add this method to your IFieldOptionsService interface
                var result = await _fieldOptionsService.GetAllAsync();
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving field options: {ex.Message}"));
            }
        }

        // ================================
        // GET FIELD OPTION BY ID
        // ================================
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetFieldOptionById(int id)
        {
            try
            {
                var result = await _fieldOptionsService.GetByIdAsync(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving field option: {ex.Message}"));
            }
        }

        // ================================
        // CREATE FIELD OPTION
        // ================================
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateFieldOption([FromBody] CreateFieldOptionDto createFieldOptionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(400, "Invalid field option data", ModelState));
                }

                var result = await _fieldOptionsService.CreateAsync(createFieldOptionDto);

                if (result.StatusCode == 200)
                {
                    return CreatedAtAction(nameof(GetFieldOptionById), new { id = result.Data?.GetType().GetProperty("Id")?.GetValue(result.Data) }, result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error creating field option: {ex.Message}"));
            }
        }

        // ================================
        // UPDATE FIELD OPTION
        // ================================
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateFieldOption(int id, [FromBody] UpdateFieldOptionDto updateFieldOptionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(400, "Invalid field option data", ModelState));
                }

                var result = await _fieldOptionsService.UpdateAsync(id, updateFieldOptionDto);

                if (result.StatusCode == 200)
                {
                    return Ok(result);
                }
                else if (result.StatusCode == 404)
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error updating field option: {ex.Message}"));
            }
        }

        // ================================
        // DELETE FIELD OPTION
        // ================================
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteFieldOption(int id)
        {
            try
            {
                var result = await _fieldOptionsService.DeleteAsync(id);

                if (result.StatusCode == 200)
                {
                    return Ok(result);
                }
                else if (result.StatusCode == 404)
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error deleting field option: {ex.Message}"));
            }
        }
    }
}