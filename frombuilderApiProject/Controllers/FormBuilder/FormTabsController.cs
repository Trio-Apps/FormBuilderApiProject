using FormBuilder.Core.DTOS.FormTabs;
using FormBuilder.Core.DTOS.FormTabs.FormBuilder.Core.DTOS.FormTabs;
using FormBuilder.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FormTabsController : ControllerBase
    {
        private readonly IFormTabService _formTabService;

        public FormTabsController(IFormTabService formTabService)
        {
            _formTabService = formTabService ?? throw new ArgumentNullException(nameof(formTabService));
        }

        // --- Manual Mapping Helper ---
        private FormTabDto MapToDto(FORM_TABS entity)
        {
            if (entity == null) return null;

            return new FormTabDto
            {
                Id = entity.id,
                FormBuilderId = entity.FormBuilderId,
                TabName = entity.TabName,
                TabCode = entity.TabCode,
                TabOrder = entity.TabOrder,
                IsActive = entity.IsActive,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate,
            };
        }

        private FORM_TABS MapToEntity(CreateFormTabDto dto, string currentUserId)
        {
            return new FORM_TABS
            {
                FormBuilderId = dto.FormBuilderId,
                TabName = dto.TabName,
                TabCode = dto.TabCode,
                TabOrder = dto.TabOrder,
                IsActive = dto.IsActive,
                CreatedByUserId = currentUserId,
                CreatedDate = DateTime.UtcNow
            };
        }

        // ----------------------------------------------------------------------
        // --- 1. GET Operations (Read) ---
        // ----------------------------------------------------------------------

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormTabDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllTabs()
        {
            try
            {
                var tabs = await _formTabService.GetAllTabsAsync();
                var tabsDto = tabs.Select(t => MapToDto(t)).ToList();
                return Ok(tabsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

  

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormTabDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTabById(int id)
        {
            try
            {
                var tab = await _formTabService.GetTabByIdAsync(id, asNoTracking: true);
                if (tab == null)
                {
                    return NotFound($"Tab with ID {id} not found.");
                }
                return Ok(MapToDto(tab));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("form/{formBuilderId}")]
        [ProducesResponseType(typeof(IEnumerable<FormTabDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTabsByFormId(int formBuilderId)
        {
            try
            {
                var tabs = await _formTabService.GetTabsByFormIdAsync(formBuilderId);
                if (tabs == null || !tabs.Any())
                {
                    return NotFound($"No tabs found for form with ID {formBuilderId}.");
                }

                var tabsDto = tabs.Select(t => MapToDto(t)).ToList();
                return Ok(tabsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/with-details")]
        [ProducesResponseType(typeof(FormTabDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTabWithDetails(int id)
        {
            try
            {
                var tab = await _formTabService.GetTabWithDetailsAsync(id, asNoTracking: true);
                if (tab == null)
                {
                    return NotFound($"Tab with ID {id} not found.");
                }
                return Ok(MapToDto(tab));
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
        [ProducesResponseType(typeof(FormTabDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateTab([FromBody] CreateFormTabDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User ID not found in claims.");

            try
            {
                var tabEntity = new FORM_TABS
                {
                    FormBuilderId = createDto.FormBuilderId,
                    TabName = createDto.TabName,
                    TabCode = createDto.TabCode,
                    TabOrder = createDto.TabOrder,
                    IsActive = createDto.IsActive,
                    CreatedByUserId = currentUserId,
                    CreatedDate = DateTime.UtcNow
                };

                var createdTab = await _formTabService.CreateTabAsync(tabEntity);
                var createdTabDto = MapToDto(createdTab);

                return CreatedAtAction(nameof(GetTabById), new { id = createdTabDto.Id }, createdTabDto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("already in use"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating tab: {ex.Message}");
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
        public async Task<IActionResult> UpdateTab(int id, [FromBody] UpdateFormTabDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            try
            {
                var existingTab = await _formTabService.GetTabByIdAsync(id);
                if (existingTab == null)
                {
                    return NotFound($"Tab with ID {id} not found.");
                }

                // Update properties
                existingTab.TabName = updateDto.TabName;
                existingTab.TabCode = updateDto.TabCode;
                existingTab.TabOrder = updateDto.TabOrder;
                existingTab.IsActive = updateDto.IsActive;
                existingTab.UpdatedDate = DateTime.UtcNow;

                var isUpdated = await _formTabService.UpdateTabAsync(existingTab);

                if (!isUpdated)
                {
                    return BadRequest("Failed to update the tab.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already in use"))
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating tab: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------------
        // --- 4. DELETE Operation ---
        // ----------------------------------------------------------------------
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTab(int id)
        {
            try
            {
                var isDeleted = await _formTabService.DeleteTabAsync(id);

                if (!isDeleted)
                {
                    return NotFound($"Tab with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting tab: {ex.Message}");
            }
        }



        [HttpGet("check-code/{tabCode}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CheckTabCodeUnique(string tabCode, [FromQuery] int? ignoreId = null)
        {
            try
            {
                var isUnique = await _formTabService.IsTabCodeUniqueAsync(tabCode, ignoreId);
                return Ok(new
                {
                    tabCode,
                    isUnique,
                    message = isUnique ? "Tab code is available" : "Tab code is already in use"
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
        public async Task<IActionResult> TabExists(int id)
        {
            try
            {
                var exists = await _formTabService.TabExistsAsync(id);
                return Ok(new
                {
                    id,
                    exists,
                    message = exists ? "Tab exists" : "Tab does not exist"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("bulk")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateTabsBulk([FromBody] List<CreateFormTabDto> createDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User ID not found in claims.");

            try
            {
                var results = new List<object>();
                var createdTabs = new List<FORM_TABS>();

                foreach (var createDto in createDtos)
                {
                    try
                    {
                        var tabEntity = new FORM_TABS
                        {
                            FormBuilderId = createDto.FormBuilderId,
                            TabName = createDto.TabName,
                            TabCode = createDto.TabCode,
                            TabOrder = createDto.TabOrder,
                            IsActive = createDto.IsActive,
                            CreatedByUserId = currentUserId,
                            CreatedDate = DateTime.UtcNow
                        };

                        var createdTab = await _formTabService.CreateTabAsync(tabEntity);
                        createdTabs.Add(createdTab);

                        results.Add(new
                        {
                            success = true,
                            tabCode = createDto.TabCode,
                            message = "Created successfully"
                        });
                    }
                    catch (InvalidOperationException ex)
                    {
                        results.Add(new
                        {
                            success = false,
                            tabCode = createDto.TabCode,
                            message = ex.Message
                        });
                    }
                }

                return Ok(new
                {
                    total = createDtos.Count,
                    successful = createdTabs.Count,
                    failed = createDtos.Count - createdTabs.Count,
                    results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error in bulk operation: {ex.Message}");
            }
        }
    }
}