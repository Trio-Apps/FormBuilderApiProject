using FormBuilder.API.Models;
using FormBuilder.Application.DTOS.Auth;
using FormBuilder.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.ApiProject.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Data);
        }

        // GET: api/roles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(string id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Data);
        }

        // GET: api/roles/name/{name}
        [HttpGet("name/{name}")]
        public async Task<ActionResult<RoleDto>> GetRoleByName(string name)
        {
            var result = await _roleService.GetRoleByNameAsync(name);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Data);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleService.CreateRoleAsync(createRoleDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return CreatedAtAction(nameof(GetRoleById), new { id = result.Data.Id }, result.Data);
        }

        // PUT: api/roles/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> UpdateRole(string id, UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Data);
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return NoContent();
        }

        // POST: api/roles/{id}/permissions
        [HttpPost("{id}/permissions")]
        public async Task<ActionResult> AssignPermissions(string id, [FromBody] List<int> permissionIds)
        {
            if (permissionIds == null || !permissionIds.Any())
                return BadRequest("Permission IDs are required");

            var result = await _roleService.AssignPermissionsToRoleAsync(id, permissionIds);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(new { message = "Permissions assigned successfully" });
        }
    }
}