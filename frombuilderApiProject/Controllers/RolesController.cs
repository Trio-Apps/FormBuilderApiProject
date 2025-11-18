// Controllers/RolesController.cs
using FormBuilder.API.Models;
using FormBuilder.Application.DTOS.Auth;
using FormBuilder.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleService roleService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
    {
        var result = await _roleService.GetAllRolesAsync();
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(int id)
    {
        var result = await _roleService.GetRoleByIdAsync(id);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto createRoleDto)
    {
        var result = await _roleService.CreateRoleAsync(createRoleDto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return StatusCode(201, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, UpdateRoleDto updateRoleDto)
    {
        var result = await _roleService.UpdateRoleAsync(id, updateRoleDto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(new ApiResponse(200, "Role deleted successfully"));
    }

    [HttpPost("{id}/permissions")]
    public async Task<ActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds)
    {
        var result = await _roleService.AssignPermissionsToRoleAsync(id, permissionIds);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(new ApiResponse(200, "Permissions assigned successfully"));
    }

    [HttpDelete("{id}/permissions")]
    public async Task<ActionResult> RemovePermissions(int id, [FromBody] List<int> permissionIds)
    {
        var result = await _roleService.RemovePermissionsFromRoleAsync(id, permissionIds);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(new ApiResponse(200, "Permissions removed successfully"));
    }
}