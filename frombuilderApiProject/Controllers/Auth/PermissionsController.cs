// Controllers/PermissionsController.cs
using FormBuilder.API.Models;
using FormBuilder.Application.DTOS.Auth;
using FormBuilder.Core.IServices.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<PermissionDto>>> GetAllPermissions()
    {
        var result = await _permissionService.GetAllPermissionsAsync();
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.Data);
    }

    

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionDto createPermissionDto)
    {
        var result = await _permissionService.CreatePermissionAsync(createPermissionDto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return StatusCode(201, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PermissionDto>> UpdatePermission(int id, UpdatePermissionDto updatePermissionDto)
    {
        var result = await _permissionService.UpdatePermissionAsync(id, updatePermissionDto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePermission(int id)
    {
        var result = await _permissionService.DeletePermissionAsync(id);
        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(new ApiResponse(200, "Permission deleted successfully"));
    }
}