
// Services/IAuthService.cs
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.core.DTOS.Auth;
using FormBuilder.Core.DTOS.Auth; // إما هذا أو core - اختر واحد

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginDto loginDto);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse> ResetPasswordAsync(string email);
    Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    Task<ApiResponse> LogoutAsync(string userId);
    Task<ApiResponse> RevokeTokenAsync(string refreshToken);
}