using FormBuilder.API.Models.DTOs;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Administration")]

    public class DocumentTypesController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypesController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _documentTypeService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _documentTypeService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

       
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _documentTypeService.GetByCodeAsync(code);
            return StatusCode(result.StatusCode, result);
        }

      
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _documentTypeService.GetActiveAsync();
            return StatusCode(result.StatusCode, result);
        }

      
        [HttpGet("parent-menu/{parentMenuId}")]
        public async Task<IActionResult> GetByParentMenuId(int parentMenuId)
        {
            var result = await _documentTypeService.GetByParentMenuIdAsync(parentMenuId);
            return StatusCode(result.StatusCode, result);
        }

        
        [HttpGet("parent-menu/null")]
        public async Task<IActionResult> GetRootMenuItems()
        {
            var result = await _documentTypeService.GetByParentMenuIdAsync(null);
            return StatusCode(result.StatusCode, result);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTypeDto createDto)
        {
            var result = await _documentTypeService.CreateAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentTypeDto updateDto)
        {
            var result = await _documentTypeService.UpdateAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

     

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _documentTypeService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}