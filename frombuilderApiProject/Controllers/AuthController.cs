// Controllers/AuthController.cs

using FormBuilder.API.Models.FormBuilder.API.Models;
using FormBuilder.core.DTOS.Auth;
using FormBuilder.Core.DTOS.Auth;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResult>> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse>> Logout([FromBody] string userId)
    {
        var result = await _authService.LogoutAsync(userId);
        return Ok(result);
    }

    [HttpPost("revoke-token")]
    public async Task<ActionResult<ApiResponse>> RevokeToken([FromBody] string refreshToken)
    {
        var result = await _authService.RevokeTokenAsync(refreshToken);
        return Ok(result);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
        var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] string email)
    {
        var result = await _authService.ResetPasswordAsync(email);
        return Ok(result);
    }
}