// Controllers/UserRolesController.cs
using FormBuilder.API.Models;
using FormBuilder.Application.DTOS.Auth;
using FormBuilder.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;
    private readonly ILogger<UserRolesController> _logger;

    public UserRolesController(IUserRoleService userRoleService, ILogger<UserRolesController> logger)
    {
        _userRoleService = userRoleService;
        _logger = logger;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserWithRolesDto>>> GetUsersWithRoles()
    {
        var result = await _userRoleService.GetUsersWithRolesAsync();
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.Data);
    }

    [HttpGet("users/{userId}/roles")]
    public async Task<ActionResult<List<RoleDto>>> GetUserRoles(string userId)
    {
        var result = await _userRoleService.GetUserRolesAsync(userId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.Data);
    }

    [HttpPost("assign")]
    public async Task<ActionResult> AssignRoleToUser(AssignRoleToUserDto assignRoleDto)
    {
        var result = await _userRoleService.AssignRoleToUserAsync(assignRoleDto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(new ApiResponse(200, "Role assigned to user successfully"));
    }

    [HttpDelete("users/{userId}/roles/{roleId}")]
    public async Task<ActionResult> RemoveRoleFromUser(string userId, int roleId)
    {
        var result = await _userRoleService.RemoveRoleFromUserAsync(userId, roleId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(new ApiResponse(200, "Role removed from user successfully"));
    }

    [HttpPut("users/{userId}/roles/{roleId}")]
    public async Task<ActionResult> UpdateUserRole(string userId, int roleId, [FromBody] DateTime? endDate)
    {
        var result = await _userRoleService.UpdateUserRoleAsync(userId, roleId, endDate);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(new ApiResponse(200, "User role updated successfully"));
    }
}