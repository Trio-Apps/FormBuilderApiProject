// Services/AuthService.cs
using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.API.Services;
using FormBuilder.core.DTOS.Auth;
using FormBuilder.Core.DTOS.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    private readonly FormBuilderDbContext _context;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        FormBuilderDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
        _context = context;
    }

    public async Task<AuthResult> LoginAsync(LoginDto loginDto)
    {
        try
        {
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

            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var (token, jwtId) = await _tokenService.CreateTokenWithIdAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenModel = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = jwtId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenModel);
            await _context.SaveChangesAsync();

            var userDto = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken
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

    public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Token and refresh token are required",
                    StatusCode = 400
                };
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Invalid access token",
                    StatusCode = 401
                };
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Invalid token claims",
                    StatusCode = 401
                };
            }

            var storedRefreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == request.RefreshToken &&
                    rt.UserId == userId &&
                    !rt.IsUsed &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);

            if (storedRefreshToken == null)
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Invalid or expired refresh token",
                    StatusCode = 401
                };
            }

            storedRefreshToken.IsUsed = true;

            var (newToken, newJwtId) = await _tokenService.CreateTokenWithIdAsync(storedRefreshToken.User);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var newRefreshTokenModel = new RefreshToken
            {
                UserId = storedRefreshToken.UserId,
                Token = newRefreshToken,
                JwtId = newJwtId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newRefreshTokenModel);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                DisplayName = storedRefreshToken.User.DisplayName,
                Email = storedRefreshToken.User.Email,
                Token = newToken,
                RefreshToken = newRefreshToken
            };

            _logger.LogInformation($"Token refreshed successfully for user: {storedRefreshToken.User.Email}");

            return new AuthResult
            {
                Success = true,
                User = userDto,
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "An error occurred while refreshing token",
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

            await RevokeAllUserRefreshTokens(userId);

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

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            _logger.LogInformation($"Password reset token for {email}: {token}");

            return new ApiResponse(200, "Password reset instructions sent to your email");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return new ApiResponse(500, "An error occurred while resetting password");
        }
    }

    public async Task<ApiResponse> LogoutAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse(404, "User not found");

            await RevokeAllUserRefreshTokens(userId);
            await _signInManager.SignOutAsync();

            _logger.LogInformation($"User {user.Email} logged out successfully");
            return new ApiResponse(200, "Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout process");
            return new ApiResponse(500, "An error occurred during logout");
        }
    }

    public async Task<ApiResponse> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Refresh token revoked for user: {token.UserId}");
            }

            return new ApiResponse(200, "Token revoked successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token revocation");
            return new ApiResponse(500, "An error occurred while revoking token");
        }
    }

    private async Task RevokeAllUserRefreshTokens(string userId)
    {
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsUsed && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }
}