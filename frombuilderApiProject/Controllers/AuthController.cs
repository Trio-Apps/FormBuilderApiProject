// Controllers/AuthController.cs
using FormBuilder.API.Models;
using FormBuilder.API.Models.FormBuilder.API.Models;
using FormBuilder.core.DTOS.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (!result.Success)
            return StatusCode(result.StatusCode, new ApiResponse(result.StatusCode, result.ErrorMessage));

        return Ok(result.User);
    }
    [HttpPost("ResetPassword")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] string email)
    {
        var result = await _authService.ResetPasswordAsync(email);

        return StatusCode(result.StatusCode, result);
    }
    [HttpPost("ChangePassword")]
    public async Task<ActionResult<ApiResponse>> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        // Get current user ID from claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ApiResponse(401, "User not authenticated"));

        var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

        return StatusCode(result.StatusCode, result);
    }
    // Controllers/AuthController.cs
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _authService.LogoutAsync(userId);

        if (result.StatusCode == 200)
            return Ok(result);

        return StatusCode(result.StatusCode, result);
    }

}

