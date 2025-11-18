// Services/AuthService.cs
using FormBuilder.API.Models;
using FormBuilder.API.Services;
using FormBuilder.core.DTOS.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Input validation
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Email and password are required",
                    StatusCode = 400
                };

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                _logger.LogWarning($"Login failed for email: {loginDto.Email} - User not found");
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password",
                    StatusCode = 401
                };
            }

            // Check if user is active
            if (!user.IsActive)
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Account is deactivated",
                    StatusCode = 401
                };

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Login failed for user: {user.Email} - Invalid password");
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password",
                    StatusCode = 401
                };
            }

            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var userDto = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user)
            };

            _logger.LogInformation($"User {user.Email} logged in successfully");

            return new AuthResult
            {
                Success = true,
                User = userDto,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login process");
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "An error occurred during login",
                StatusCode = 500
            };
        }
    }


    public async Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse(404, "User not found");

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ApiResponse(400, errors);
            }

            _logger.LogInformation($"Password changed successfully for user: {user.Email}");
            return new ApiResponse(200, "Password changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return new ApiResponse(500, "An error occurred while changing password");
        }
    }


    public async Task<ApiResponse> ResetPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ApiResponse(404, "User not found");

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Here you would typically send the token via email
            // For now, we'll just log it (in production, send email)
            _logger.LogInformation($"Password reset token for {email}: {token}");

            return new ApiResponse(200, "Password reset instructions sent to your email");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return new ApiResponse(500, "An error occurred while resetting password");
        }
    }
}