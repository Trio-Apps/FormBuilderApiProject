using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormFields;
using CreateFormFieldDto = FormBuilder.Core.DTOS.FormFields.CreateFormFieldDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administration")]

    public class FormFieldsController : ControllerBase
    {
        private readonly IFormFieldService _formFieldService;

        public FormFieldsController(IFormFieldService formFieldService)
        {
            _formFieldService = formFieldService;
        }

        // ================================
        // GET ALL FORM FIELDS
        // ================================
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllFormFields()
        {
            try
            {
                var formFields = await _formFieldService.GetAllAsync();
                return Ok(new ApiResponse(200, "Form fields retrieved successfully", formFields));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving form fields: {ex.Message}"));
            }
        }

        // ================================
        // GET FORM FIELD BY ID
        // ================================
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetFormFieldById(int id)
        {
            try
            {
                var formField = await _formFieldService.GetByIdAsync(id);
                if (formField == null)
                {
                    return NotFound(new ApiResponse(404, "Form field not found"));
                }

                return Ok(new ApiResponse(200, "Form field retrieved successfully", formField));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving form field: {ex.Message}"));
            }
        }

        // ================================
        // CREATE FORM FIELD
        // ================================
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateFormField([FromBody] CreateFormFieldDto createFormFieldDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(400, "Invalid form field data", ModelState));
                }

                var result = await _formFieldService.CreateAsync(createFormFieldDto);

                if (result.StatusCode == 200 || result.StatusCode == 201)
                {
                    // Extract the ID from the result to use in CreatedAtAction
                    var createdId = ExtractIdFromResult(result);
                    if (createdId.HasValue)
                    {
                        return CreatedAtAction(nameof(GetFormFieldById), new { id = createdId.Value }, result);
                    }
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error creating form field: {ex.Message}"));
            }
        }

        // ================================
        // UPDATE FORM FIELD
        // ================================
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateFormField(int id, [FromBody] UpdateFormFieldDto updateFormFieldDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse(400, "Invalid form field data", ModelState));
                }

                var result = await _formFieldService.UpdateAsync(updateFormFieldDto, id);

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
                return StatusCode(500, new ApiResponse(500, $"Error updating form field: {ex.Message}"));
            }
        }

        // ================================
        // DELETE FORM FIELD (HARD DELETE)
        // ================================
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteFormField(int id)
        {
            try
            {
                var result = await _formFieldService.DeleteAsync(id);

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
                return StatusCode(500, new ApiResponse(500, $"Error deleting form field: {ex.Message}"));
            }
        }

        // ================================
        // SOFT DELETE FORM FIELD
        // ================================
        [HttpDelete("soft-delete/{id}")]
        public async Task<ActionResult<ApiResponse>> SoftDeleteFormField(int id)
        {
            try
            {
                var result = await _formFieldService.SoftDeleteAsync(id);

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
                return StatusCode(500, new ApiResponse(500, $"Error soft deleting form field: {ex.Message}"));
            }
        }

        // ================================
        // SPECIAL QUERIES
        // ================================

        // GET ACTIVE FORM FIELDS
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse>> GetActiveFormFields()
        {
            try
            {
                var formFields = await _formFieldService.GetActiveAsync();
                return Ok(new ApiResponse(200, "Active form fields retrieved successfully", formFields));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving active form fields: {ex.Message}"));
            }
        }

        // GET FORM FIELDS BY TAB ID
        [HttpGet("tab/{tabId}")]
        public async Task<ActionResult<ApiResponse>> GetFormFieldsByTabId(int tabId)
        {
            try
            {
                var formFields = await _formFieldService.GetByTabIdAsync(tabId);
                return Ok(new ApiResponse(200, "Form fields retrieved successfully", formFields));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving form fields by tab: {ex.Message}"));
            }
        }

        // GET FORM FIELDS BY FORM ID
        [HttpGet("form/{formBuilderId}")]
        public async Task<ActionResult<ApiResponse>> GetFormFieldsByFormId(int formBuilderId)
        {
            try
            {
                var formFields = await _formFieldService.GetByFormIdAsync(formBuilderId);
                return Ok(new ApiResponse(200, "Form fields retrieved successfully", formFields));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving form fields by form: {ex.Message}"));
            }
        }

        // GET MANDATORY FIELDS BY TAB ID
        [HttpGet("tab/{tabId}/mandatory")]
        public async Task<ActionResult<ApiResponse>> GetMandatoryFields(int tabId)
        {
            try
            {
                var formFields = await _formFieldService.GetMandatoryFieldsAsync(tabId);
                return Ok(new ApiResponse(200, "Mandatory form fields retrieved successfully", formFields));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving mandatory form fields: {ex.Message}"));
            }
        }

        // GET VISIBLE FIELDS BY TAB ID
        [HttpGet("tab/{tabId}/visible")]
        public async Task<ActionResult<ApiResponse>> GetVisibleFields(int tabId)
        {
            try
            {
                var formFields = await _formFieldService.GetVisibleFieldsAsync(tabId);
                return Ok(new ApiResponse(200, "Visible form fields retrieved successfully", formFields));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving visible form fields: {ex.Message}"));
            }
        }

        // GET EDITABLE FIELDS BY TAB ID
        [HttpGet("tab/{tabId}/editable")]
        public async Task<ActionResult<ApiResponse>> GetEditableFields(int tabId)
        {
            try
            {
                var formFields = await _formFieldService.GetEditableFieldsAsync(tabId);
                return Ok(new ApiResponse(200, "Editable form fields retrieved successfully", formFields));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving editable form fields: {ex.Message}"));
            }
        }

        // GET FORM FIELD BY FIELD CODE
        [HttpGet("code/{fieldCode}")]
        public async Task<ActionResult<ApiResponse>> GetFormFieldByCode(string fieldCode)
        {
            try
            {
                var formField = await _formFieldService.GetByFieldCodeAsync(fieldCode);
                if (formField == null)
                {
                    return NotFound(new ApiResponse(404, "Form field not found"));
                }

                return Ok(new ApiResponse(200, "Form field retrieved successfully", formField));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving form field by code: {ex.Message}"));
            }
        }

        // GET USAGE COUNT
        [HttpGet("{id}/usage-count")]
        public async Task<ActionResult<ApiResponse>> GetFormFieldUsageCount(int id)
        {
            try
            {
                var usageCount = await _formFieldService.GetUsageCountAsync(id);
                return Ok(new ApiResponse(200, "Form field usage count retrieved successfully", usageCount));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Error retrieving form field usage count: {ex.Message}"));
            }
        }

        // Helper method to extract ID from service result
        private int? ExtractIdFromResult(ApiResponse result)
        {
            if (result.Data is FormFieldDto formFieldDto)
            {
                return formFieldDto.Id;
            }

            // If Data is a dynamic object with Id property
            if (result.Data != null && result.Data.GetType().GetProperty("Id") != null)
            {
                return (int?)result.Data.GetType().GetProperty("Id")?.GetValue(result.Data);
            }

            return null;
        }
    }
}