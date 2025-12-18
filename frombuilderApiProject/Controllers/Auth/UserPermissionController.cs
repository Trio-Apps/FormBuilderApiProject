using FormBuilder.Application.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserPermissionController : ControllerBase
{
    private readonly IUserPermissionService _permissionService;
    private readonly IStringLocalizer<UserPermissionController> _localizer;

    public UserPermissionController(IUserPermissionService permissionService, IStringLocalizer<UserPermissionController> localizer)
    {
        _permissionService = permissionService;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize(Roles = "Administration")]
    public async Task<IActionResult> GetAll()
    {
        var permissions = await _permissionService.GetAllAsync();
        return Ok(permissions);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Administration")]
    public async Task<IActionResult> GetUserPermissions(int userId)
    {
        var permissions = await _permissionService.GetUserPermissionsAsync(userId);
        return Ok(permissions);
    }

    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUserPermissions()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { message = _localizer["Common_InvalidUserToken"] });

        var permissions = await _permissionService.GetUserPermissionsAsync(userId);
        return Ok(permissions);
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckPermission([FromBody] CheckPermissionRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { message = _localizer["Common_InvalidUserToken"] });

        var hasPermission = await _permissionService.HasPermissionAsync(userId, request.PermissionName);
        return Ok(new { hasPermission, permissionName = request.PermissionName });
    }

    [HttpPost("check-multiple")]
    public async Task<IActionResult> CheckMultiplePermissions([FromBody] CheckPermissionsRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { message = _localizer["Common_InvalidUserToken"] });

        var results = await _permissionService.CheckMultiplePermissionsAsync(userId, request.PermissionNames);
        return Ok(results);
    }

    [HttpGet("role/{roleId}")]
    [Authorize(Roles = "Administration")]
    public async Task<IActionResult> GetRolePermissions(int roleId)
    {
        var permissions = await _permissionService.GetRolePermissionsAsync(roleId);
        return Ok(permissions);
    }

    [HttpGet("matrix")]
    [Authorize(Roles = "Administration")]
    public async Task<IActionResult> GetPermissionMatrix()
    {
        var matrix = await _permissionService.GetPermissionMatrixAsync();
        return Ok(matrix);
    }
}
