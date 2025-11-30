using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentSeriesController : ControllerBase
    {
        private readonly IDocumentSeriesService _documentSeriesService;

        public DocumentSeriesController(IDocumentSeriesService documentSeriesService)
        {
            _documentSeriesService = documentSeriesService;
        }

        // ================================
        // GET ALL DOCUMENT SERIES
        // ================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _documentSeriesService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // GET BY ID
        // ================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _documentSeriesService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // GET BY SERIES CODE
        // ================================
        [HttpGet("code/{seriesCode}")]
        public async Task<IActionResult> GetBySeriesCode(string seriesCode)
        {
            var result = await _documentSeriesService.GetBySeriesCodeAsync(seriesCode);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // GET BY DOCUMENT TYPE ID
        // ================================
        [HttpGet("document-type/{documentTypeId}")]
        public async Task<IActionResult> GetByDocumentTypeId(int documentTypeId)
        {
            var result = await _documentSeriesService.GetByDocumentTypeIdAsync(documentTypeId);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // GET BY PROJECT ID
        // ================================
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProjectId(int projectId)
        {
            var result = await _documentSeriesService.GetByProjectIdAsync(projectId);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // GET ACTIVE SERIES
        // ================================
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _documentSeriesService.GetActiveAsync();
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // GET DEFAULT SERIES
        // ================================
        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultSeries([FromQuery] int documentTypeId, [FromQuery] int projectId)
        {
            var result = await _documentSeriesService.GetDefaultSeriesAsync(documentTypeId, projectId);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // CREATE NEW DOCUMENT SERIES
        // ================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentSeriesDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid data", ModelState));

            var result = await _documentSeriesService.CreateAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // UPDATE DOCUMENT SERIES
        // ================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentSeriesDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid data", ModelState));

            var result = await _documentSeriesService.UpdateAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // DELETE DOCUMENT SERIES
        // ================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _documentSeriesService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // TOGGLE ACTIVE STATUS
        // ================================
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id, [FromBody] bool isActive)
        {
            var result = await _documentSeriesService.ToggleActiveAsync(id, isActive);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // SET AS DEFAULT SERIES
        // ================================
        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetAsDefault(int id)
        {
            var result = await _documentSeriesService.SetAsDefaultAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // GET NEXT NUMBER
        // ================================
        [HttpGet("{id}/next-number")]
        public async Task<IActionResult> GetNextNumber(int id)
        {
            var result = await _documentSeriesService.GetNextNumberAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // ================================
        // CHECK EXISTS
        // ================================
        [HttpGet("{id}/exists")]
        public async Task<IActionResult> Exists(int id)
        {
            var result = await _documentSeriesService.ExistsAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}