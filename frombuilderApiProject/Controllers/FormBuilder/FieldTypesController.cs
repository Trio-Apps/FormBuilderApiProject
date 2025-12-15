using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Entitys;
using FormBuilder.Domian.Interfaces;
using FormBuilder.Services.Services;
using FormBuilder.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.ApiProject.Controllers.FormBuilder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class FieldTypesController : ControllerBase
    {
        private readonly IFieldTypesService _fieldTypesService;

        public FieldTypesController(IFieldTypesService fieldTypesService)
        {
            _fieldTypesService = fieldTypesService;
        }

        // GET: api/FieldTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FieldTypeDto>>> GetAll()
        {
            var list = await _fieldTypesService.GetAllAsync();
            return Ok(list);
        }

        // GET: api/FieldTypes/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FieldTypeDto>> GetById(int id)
        {
            var dto = await _fieldTypesService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST: api/FieldTypes
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] FieldTypeCreateDto dto)
        {
            var response = await _fieldTypesService.CreateAsync(dto);
            if (response.StatusCode != 200)
                return BadRequest(response);

            // يمكنك هنا تمرير الـ ID الجديد إذا أردت
            return Ok(response);
        }

        // PUT: api/FieldTypes/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromBody] FieldTypeUpdateDto dto)
        {
            var response = await _fieldTypesService.UpdateAsync(dto, id);
            if (response.StatusCode != 200)
                return BadRequest(response);

            return Ok(response);
        }

        // DELETE: api/FieldTypes/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var response = await _fieldTypesService.DeleteAsync(id);
            if (response.StatusCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        // PATCH: api/FieldTypes/{id}/soft-delete
        [HttpPatch("{id:int}/soft-delete")]
        public async Task<ActionResult<ApiResponse>> SoftDelete(int id)
        {
            var response = await _fieldTypesService.SoftDeleteAsync(id);
            if (response.StatusCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        // GET: api/FieldTypes/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<FieldTypeDto>>> GetActive()
        {
            var list = await _fieldTypesService.GetActiveAsync();
            return Ok(list);
        }

        // GET: api/FieldTypes/by-name/{typeName}
        [HttpGet("by-name/{typeName}")]
        public async Task<ActionResult<FieldTypeDto>> GetByTypeName(string typeName)
        {
            var dto = await _fieldTypesService.GetByTypeNameAsync(typeName);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // GET: api/FieldTypes/with-options
        [HttpGet("with-options")]
        public async Task<ActionResult<IEnumerable<FieldTypeDto>>> GetWithOptions()
        {
            var list = await _fieldTypesService.GetWithOptionsAsync();
            return Ok(list);
        }

        // GET: api/FieldTypes/with-multiple
        [HttpGet("with-multiple")]
        public async Task<ActionResult<IEnumerable<FieldTypeDto>>> GetWithMultiple()
        {
            var list = await _fieldTypesService.GetWithMultipleValuesAsync();
            return Ok(list);
        }

        // GET: api/FieldTypes/dropdown
        [HttpGet("dropdown")]
        public async Task<ActionResult<IEnumerable<FieldTypeDropdownDto>>> GetForDropdown()
        {
            var list = await _fieldTypesService.GetForDropdownAsync();
            return Ok(list);
        }

        // GET: api/FieldTypes/basic
        [HttpGet("basic")]
        public async Task<ActionResult<IEnumerable<FieldTypeDto>>> GetBasic()
        {
            var list = await _fieldTypesService.GetBasicAsync();
            return Ok(list);
        }

        // GET: api/FieldTypes/advanced
        [HttpGet("advanced")]
        public async Task<ActionResult<IEnumerable<FieldTypeDto>>> GetAdvanced()
        {
            var list = await _fieldTypesService.GetAdvancedAsync();
            return Ok(list);
        }
    }
}
