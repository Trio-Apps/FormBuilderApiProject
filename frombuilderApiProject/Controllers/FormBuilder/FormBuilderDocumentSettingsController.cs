using FormBuilder.API.Extensions;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Core.DTOS.FormBuilder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administration")]
    [Produces("application/json")]
    public class FormBuilderDocumentSettingsController : ControllerBase
    {
        private readonly IFormBuilderDocumentSettingsService _documentSettingsService;
        private readonly IStringLocalizer<FormBuilderDocumentSettingsController> _localizer;

        public FormBuilderDocumentSettingsController(
            IFormBuilderDocumentSettingsService documentSettingsService,
            IStringLocalizer<FormBuilderDocumentSettingsController> localizer)
        {
            _documentSettingsService = documentSettingsService;
            _localizer = localizer;
        }

        /// <summary>
        /// Get Document Settings (Document Type + Series) for a specific Form Builder
        /// </summary>
        [HttpGet("form/{formBuilderId}")]
        [ProducesResponseType(typeof(DocumentSettingsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDocumentSettings(int formBuilderId)
        {
            var result = await _documentSettingsService.GetDocumentSettingsAsync(formBuilderId);
            return result.ToActionResult();
        }

        /// <summary>
        /// Save Document Settings (Document Type + Series) for a Form Builder
        /// Creates or updates Document Type and manages Document Series
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(DocumentSettingsDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SaveDocumentSettings([FromBody] SaveDocumentSettingsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _documentSettingsService.SaveDocumentSettingsAsync(dto);
            return result.ToActionResult();
        }

        /// <summary>
        /// Delete Document Settings for a Form Builder (removes Document Type and all Series)
        /// </summary>
        [HttpDelete("form/{formBuilderId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDocumentSettings(int formBuilderId)
        {
            var result = await _documentSettingsService.DeleteDocumentSettingsAsync(formBuilderId);
            if (result.Success) return NoContent();
            return result.ToActionResult();
        }
    }
}

