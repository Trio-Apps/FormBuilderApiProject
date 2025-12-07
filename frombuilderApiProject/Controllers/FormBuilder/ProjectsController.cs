using FormBuilder.API.Models.DTOs;
using FormBuilder.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administration")]

    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _projectService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _projectService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/projects/code/PROJ001
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _projectService.GetByCodeAsync(code);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/projects/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _projectService.GetActiveAsync();
            return StatusCode(result.StatusCode, result);
        }

        // POST: api/projects
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto createDto)
        {
            var result = await _projectService.CreateAsync(createDto);
            return StatusCode(result.StatusCode, result);
        }

        // PUT: api/projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto updateDto)
        {
            var result = await _projectService.UpdateAsync(id, updateDto);
            return StatusCode(result.StatusCode, result);
        }

        // DELETE: api/projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _projectService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }


        // GET: api/projects/5/exists
        [HttpGet("{id}/exists")]
        public async Task<IActionResult> Exists(int id)
        {
            var result = await _projectService.ExistsAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        // GET: api/projects/code/PROJ001/exists
        [HttpGet("code/{code}/exists")]
        public async Task<IActionResult> CodeExists(string code, [FromQuery] int? excludeId = null)
        {
            var result = await _projectService.CodeExistsAsync(code, excludeId);
            return StatusCode(result.StatusCode, result);
        }
    }
}