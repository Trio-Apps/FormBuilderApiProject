
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.core.DTOS.Auth;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginDto loginDto);
    Task<ApiResponse> ResetPasswordAsync(string email);
    Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);

}

