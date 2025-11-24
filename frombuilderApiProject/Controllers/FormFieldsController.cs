using DocumentFormat.OpenXml.Wordprocessing;
using FormBuilder.Core.DTOS.FormFields;

using FormBuilder.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FormFieldsController : ControllerBase
    {
        private readonly IFormFieldService _formFieldService;

        public FormFieldsController(IFormFieldService formFieldService)
        {
            _formFieldService = formFieldService ?? throw new ArgumentNullException(nameof(formFieldService));
        }

        // --- Manual Mapping Helper ---
        private FormFieldDto MapToDto(FORM_FIELDS entity)
        {
            if (entity == null) return null;

            return new FormFieldDto
            {
                Id = entity.id,
                TabId = entity.TabId,
                FieldTypeId = entity.FieldTypeId,
                FieldName = entity.FieldName,
                FieldCode = entity.FieldCode,
                FieldOrder = entity.FieldOrder,
                Placeholder = entity.Placeholder,
                HintText = entity.HintText,
                IsMandatory = entity.IsMandatory,
                IsEditable = entity.IsEditable,
                IsVisible = entity.IsVisible,
                DefaultValueJson = entity.DefaultValueJson,
                DataType = entity.DataType,
                MaxLength = entity.MaxLength,
                MinValue = entity.MinValue,
                MaxValue = entity.MaxValue,
                RegexPattern = entity.RegexPattern,
                ValidationMessage = entity.ValidationMessage,
                VisibilityRuleJson = entity.VisibilityRuleJson,
                ReadOnlyRuleJson = entity.ReadOnlyRuleJson,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                IsActive = entity.IsActive,

                // Navigation Properties
                TabName = entity.FORM_TABS?.TabName,
                FieldTypeName = entity.FIELD_TYPES?.TypeName,
                FieldOptions = entity.FIELD_OPTIONS?.Select(o => new FieldOptionDto
                {
                   
                    OptionText = o.OptionText,
                    OptionValue = o.OptionValue,
                    SortOrder = o.OptionOrder,
                    IsDefault = o.IsDefault
                }).ToList() ?? new List<FieldOptionDto>()
            };
        }

        // ----------------------------------------------------------------------
        // --- 1. GET Operations (Read) ---
        // ----------------------------------------------------------------------

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormFieldDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFields()
        {
            try
            {
                var fields = await _formFieldService.GetAllFieldsAsync();
                var fieldsDto = fields.Select(f => MapToDto(f)).ToList();
                return Ok(fieldsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<FormFieldDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetActiveFields()
        {
            try
            {
                var fields = await _formFieldService.GetActiveFieldsAsync();
                var fieldsDto = fields.Select(f => MapToDto(f)).ToList();
                return Ok(fieldsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormFieldDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFieldById(int id)
        {
            try
            {
                var field = await _formFieldService.GetFieldByIdAsync(id, asNoTracking: true);
                if (field == null)
                {
                    return NotFound($"Field with ID {id} not found.");
                }
                return Ok(MapToDto(field));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/with-details")]
        [ProducesResponseType(typeof(FormFieldDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFieldWithDetails(int id)
        {
            try
            {
                var field = await _formFieldService.GetFieldWithDetailsAsync(id, asNoTracking: true);
                if (field == null)
                {
                    return NotFound($"Field with ID {id} not found.");
                }
                return Ok(MapToDto(field));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("tab/{tabId}")]
        [ProducesResponseType(typeof(IEnumerable<FormFieldDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFieldsByTabId(int tabId)
        {
            try
            {
                var fields = await _formFieldService.GetFieldsByTabIdAsync(tabId);
                if (fields == null || !fields.Any())
                {
                    return NotFound($"No fields found for tab with ID {tabId}.");
                }

                var fieldsDto = fields.Select(f => MapToDto(f)).ToList();
                return Ok(fieldsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("form/{formBuilderId}")]
        [ProducesResponseType(typeof(IEnumerable<FormFieldDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFieldsByFormId(int formBuilderId)
        {
            try
            {
                var fields = await _formFieldService.GetFieldsByFormIdAsync(formBuilderId);
                if (fields == null || !fields.Any())
                {
                    return NotFound($"No fields found for form with ID {formBuilderId}.");
                }

                var fieldsDto = fields.Select(f => MapToDto(f)).ToList();
                return Ok(fieldsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 2. POST Operation (Create) ---
        // ----------------------------------------------------------------------
        [HttpPost]
        [ProducesResponseType(typeof(FormFieldDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateField([FromBody] CreateFormFieldDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User ID not found in claims.");

            try
            {
                var fieldEntity = new FORM_FIELDS
                {
                    TabId = createDto.TabId,
                    FieldTypeId = createDto.FieldTypeId,
                    FieldName = createDto.FieldName,
                    FieldCode = createDto.FieldCode,
                    FieldOrder = createDto.FieldOrder,
                    Placeholder = createDto.Placeholder,
                    HintText = createDto.HintText,
                    IsMandatory = createDto.IsMandatory,
                    IsEditable = createDto.IsEditable,
                    IsVisible = createDto.IsVisible,
                    DefaultValueJson = createDto.DefaultValueJson,
                    DataType = createDto.DataType,
                    MaxLength = createDto.MaxLength,
                    MinValue = createDto.MinValue,
                    MaxValue = createDto.MaxValue,
                    RegexPattern = createDto.RegexPattern,
                    ValidationMessage = createDto.ValidationMessage,
                    VisibilityRuleJson = createDto.VisibilityRuleJson,
                    ReadOnlyRuleJson = createDto.ReadOnlyRuleJson,
                    IsActive = createDto.IsActive,
                    CreatedByUserId = currentUserId,
                    CreatedDate = DateTime.UtcNow
                };

                var createdField = await _formFieldService.CreateFieldAsync(fieldEntity);
                var createdFieldDto = MapToDto(createdField);

                return CreatedAtAction(nameof(GetFieldById), new { id = createdFieldDto.Id }, createdFieldDto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("already in use"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating field: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 3. PUT Operation (Update) ---
        // ----------------------------------------------------------------------
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateField(int id, [FromBody] UpdateFormFieldDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updateDto.Id)
                return BadRequest("ID mismatch");

            try
            {
                var existingField = await _formFieldService.GetFieldByIdAsync(id);
                if (existingField == null)
                {
                    return NotFound($"Field with ID {id} not found.");
                }

                // Update properties
                existingField.TabId = updateDto.TabId;
                existingField.FieldTypeId = updateDto.FieldTypeId;
                existingField.FieldName = updateDto.FieldName;
                existingField.FieldCode = updateDto.FieldCode;
                existingField.FieldOrder = updateDto.FieldOrder;
                existingField.Placeholder = updateDto.Placeholder;
                existingField.HintText = updateDto.HintText;
                existingField.IsMandatory = updateDto.IsMandatory;
                existingField.IsEditable = updateDto.IsEditable;
                existingField.IsVisible = updateDto.IsVisible;
                existingField.DefaultValueJson = updateDto.DefaultValueJson;
                existingField.DataType = updateDto.DataType;
                existingField.MaxLength = updateDto.MaxLength;
                existingField.MinValue = updateDto.MinValue;
                existingField.MaxValue = updateDto.MaxValue;
                existingField.RegexPattern = updateDto.RegexPattern;
                existingField.ValidationMessage = updateDto.ValidationMessage;
                existingField.VisibilityRuleJson = updateDto.VisibilityRuleJson;
                existingField.ReadOnlyRuleJson = updateDto.ReadOnlyRuleJson;
                existingField.IsActive = updateDto.IsActive;
                existingField.UpdatedDate = DateTime.UtcNow;

                var isUpdated = await _formFieldService.UpdateFieldAsync(existingField);

                if (!isUpdated)
                {
                    return BadRequest("Failed to update the field.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already in use"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating field: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 4. DELETE Operation ---
        // ----------------------------------------------------------------------
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteField(int id)
        {
            try
            {
                var isDeleted = await _formFieldService.DeleteFieldAsync(id);

                if (!isDeleted)
                {
                    return NotFound($"Field with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting field: {ex.Message}");
            }
        }

        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteField(int id)
        {
            try
            {
                var isSoftDeleted = await _formFieldService.SoftDeleteFieldAsync(id);

                if (!isSoftDeleted)
                {
                    return NotFound($"Field with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error soft deleting field: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 5. Validation & Utility Operations ---
        // ----------------------------------------------------------------------

        [HttpGet("check-code/{fieldCode}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckFieldCodeUnique(string fieldCode, [FromQuery] int? ignoreId = null)
        {
            try
            {
                var isUnique = await _formFieldService.IsFieldCodeUniqueAsync(fieldCode, ignoreId);
                return Ok(new
                {
                    fieldCode,
                    isUnique,
                    message = isUnique ? "Field code is available" : "Field code is already in use"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("check-name/{fieldName}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckFieldNameUnique(string fieldName, [FromQuery] int? ignoreId = null, [FromQuery] int? tabId = null)
        {
            try
            {
                var isUnique = await _formFieldService.IsFieldNameUniqueAsync(fieldName, ignoreId, tabId);
                return Ok(new
                {
                    fieldName,
                    tabId,
                    isUnique,
                    message = isUnique ? "Field name is available" : "Field name is already in use"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FieldExists(int id)
        {
            try
            {
                var exists = await _formFieldService.FieldExistsAsync(id);
                return Ok(new
                {
                    id,
                    exists,
                    message = exists ? "Field exists" : "Field does not exist"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("tab/{tabId}/count")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFieldsCount(int tabId)
        {
            try
            {
                var count = await _formFieldService.GetFieldsCountAsync(tabId);
                return Ok(new
                {
                    tabId,
                    count,
                    message = $"Found {count} fields for tab {tabId}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("tab/{tabId}/mandatory")]
        [ProducesResponseType(typeof(IEnumerable<FormFieldDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMandatoryFields(int tabId)
        {
            try
            {
                var fields = await _formFieldService.GetMandatoryFieldsAsync(tabId);
                if (fields == null || !fields.Any())
                {
                    return NotFound($"No mandatory fields found for tab with ID {tabId}.");
                }

                var fieldsDto = fields.Select(f => MapToDto(f)).ToList();
                return Ok(fieldsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("tab/{tabId}/visible")]
        [ProducesResponseType(typeof(IEnumerable<FormFieldDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetVisibleFields(int tabId)
        {
            try
            {
                var fields = await _formFieldService.GetVisibleFieldsAsync(tabId);
                if (fields == null || !fields.Any())
                {
                    return NotFound($"No visible fields found for tab with ID {tabId}.");
                }

                var fieldsDto = fields.Select(f => MapToDto(f)).ToList();
                return Ok(fieldsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 6. Bulk Operations ---
        // ----------------------------------------------------------------------

        [HttpPost("bulk")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFieldsBulk([FromBody] List<CreateFormFieldDto> createDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User ID not found in claims.");

            try
            {
                var results = new List<object>();
                var createdFields = new List<FORM_FIELDS>();

                foreach (var createDto in createDtos)
                {
                    try
                    {
                        var fieldEntity = new FORM_FIELDS
                        {
                            TabId = createDto.TabId,
                            FieldTypeId = createDto.FieldTypeId,
                            FieldName = createDto.FieldName,
                            FieldCode = createDto.FieldCode,
                            FieldOrder = createDto.FieldOrder,
                            Placeholder = createDto.Placeholder,
                            HintText = createDto.HintText,
                            IsMandatory = createDto.IsMandatory,
                            IsEditable = createDto.IsEditable,
                            IsVisible = createDto.IsVisible,
                            DefaultValueJson = createDto.DefaultValueJson,
                            DataType = createDto.DataType,
                            MaxLength = createDto.MaxLength,
                            MinValue = createDto.MinValue,
                            MaxValue = createDto.MaxValue,
                            RegexPattern = createDto.RegexPattern,
                            ValidationMessage = createDto.ValidationMessage,
                            VisibilityRuleJson = createDto.VisibilityRuleJson,
                            ReadOnlyRuleJson = createDto.ReadOnlyRuleJson,
                            IsActive = createDto.IsActive,
                            CreatedByUserId = currentUserId,
                            CreatedDate = DateTime.UtcNow
                        };

                        var createdField = await _formFieldService.CreateFieldAsync(fieldEntity);
                        createdFields.Add(createdField);

                        results.Add(new
                        {
                            success = true,
                            fieldCode = createDto.FieldCode,
                            message = "Created successfully"
                        });
                    }
                    catch (InvalidOperationException ex)
                    {
                        results.Add(new
                        {
                            success = false,
                            fieldCode = createDto.FieldCode,
                            message = ex.Message
                        });
                    }
                }

                return Ok(new
                {
                    total = createDtos.Count,
                    successful = createdFields.Count,
                    failed = createDtos.Count - createdFields.Count,
                    results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error in bulk operation: {ex.Message}");
            }
        }

        [HttpPut("update-orders")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateFieldsOrder([FromBody] Dictionary<int, int> fieldOrders)
        {
            if (fieldOrders == null || !fieldOrders.Any())
                return BadRequest("Field orders dictionary is required.");

            try
            {
                var isUpdated = await _formFieldService.UpdateFieldsOrderAsync(fieldOrders);

                if (!isUpdated)
                {
                    return BadRequest("Failed to update fields order.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating fields order: {ex.Message}");
            }
        }
    }
}