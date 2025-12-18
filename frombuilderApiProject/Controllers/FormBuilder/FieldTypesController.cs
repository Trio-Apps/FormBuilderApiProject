using FormBuilder.Services.Services;
using FormBuilder.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administration")]

    public class FieldTypesController : ControllerBase
    {
        private readonly IFieldTypesService _fieldTypesService;

        public FieldTypesController(IFieldTypesService fieldTypesService)
        {
            _fieldTypesService = fieldTypesService;
        }

        // GET: api/FieldTypes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _fieldTypesService.GetAllAsync();
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _fieldTypesService.GetByIdAsync(id, asNoTracking: true);
            return result.ToActionResult();
        }

        // POST: api/FieldTypes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FieldTypeCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _fieldTypesService.CreateAsync(dto);
            if (result.Success && result.Data != null)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
            }
            return result.ToActionResult();
        }

        // PUT: api/FieldTypes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] FieldTypeUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _fieldTypesService.UpdateAsync(id, dto);
            if (result.Success) return NoContent();
            return result.ToActionResult();
        }

        // DELETE: api/FieldTypes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _fieldTypesService.DeleteAsync(id);
            if (result.Success) return NoContent();
            return result.ToActionResult();
        }

        // PATCH: api/FieldTypes/{id}/soft-delete
        [HttpPatch("{id:int}/soft-delete")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _fieldTypesService.SoftDeleteAsync(id);
            if (result.Success) return NoContent();
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _fieldTypesService.GetActiveAsync();
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/by-name/{typeName}
        [HttpGet("by-name/{typeName}")]
        public async Task<IActionResult> GetByTypeName(string typeName)
        {
            var result = await _fieldTypesService.GetByTypeNameAsync(typeName, asNoTracking: true);
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/with-options
        [HttpGet("with-options")]
        public async Task<IActionResult> GetWithOptions()
        {
            var result = await _fieldTypesService.GetWithOptionsAsync();
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/with-multiple
        [HttpGet("with-multiple")]
        public async Task<IActionResult> GetWithMultiple()
        {
            var result = await _fieldTypesService.GetWithMultipleValuesAsync();
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/dropdown
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetForDropdown()
        {
            var result = await _fieldTypesService.GetForDropdownAsync();
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/basic
        [HttpGet("basic")]
        public async Task<IActionResult> GetBasic()
        {
            var result = await _fieldTypesService.GetBasicAsync();
            return result.ToActionResult();
        }

        // GET: api/FieldTypes/advanced
        [HttpGet("advanced")]
        public async Task<IActionResult> GetAdvanced()
        {
            var result = await _fieldTypesService.GetAdvancedAsync();
            return result.ToActionResult();
        }
    }
}
