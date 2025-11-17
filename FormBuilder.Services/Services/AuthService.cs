// Services/AuthService.cs
using FormBuilder.API.Data;
using FormBuilder.API.DTOs;
using FormBuilder.API.DTOs.FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FormBuilder.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService; // أضف هذا
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AuthDbContext context,
            UserManager<User> userManager,
            ITokenService tokenService, // أضف هذا
            ILogger<AuthService> logger)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService; // أضف هذا
            _logger = logger;
        }

        public async Task<SingleApiResponse> RegisterAsync(UserRegisterDto userRegister, string origin)
        {
            try
            {
                _logger.LogInformation("Registration attempt for user: {Username}", userRegister.Username);

                

             

                // Create new user
                var user = new User
                {
                    Username = userRegister.Username,
                    Email = userRegister.Email,

                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                // Use UserManager to create user with password
                var result = await _userManager.CreateAsync(user, userRegister.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return SingleApiResponse.ErrorResult($"User creation failed: {errors}");
                }

                // Assign default role (User)
                var userRole = await _context.ROLES
                    .FirstOrDefaultAsync(r => r.RoleName == "User" && r.IsActive);

                if (userRole != null)
                {
                    var userRoleEntity = new UserRole
                    {
                        UserID = user.Id,
                        RoleID = userRole.RoleID,
                        StartDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _context.USER_ROLES.AddAsync(userRoleEntity);
                    await _context.SaveChangesAsync();
                }

                // Get user roles for token
                var userWithRoles = await _context.USERS
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                var roles = userWithRoles?.UserRoles
                    .Where(ur => ur.IsActive && ur.Role.IsActive)
                    .Select(ur => ur.Role.RoleName)
                    .ToList() ?? new List<string> { "User" };

                // Generate JWT Token
                var token = await _tokenService.CreateTokenAsync(user);

                // Create UserInfoDto
                var userInfo = new UserInfoDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
               
                    Roles = roles
                };

                // Create AuthResponseDto
                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    TokenExpiration = DateTime.UtcNow.AddHours(1), // أو الوقت الذي تحدده في الإعدادات
                    User = userInfo
                };

                _logger.LogInformation("User registered successfully: {Username}", userRegister.Username);

                return SingleApiResponse.SuccessResult(authResponse, "User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", userRegister.Username);
                return SingleApiResponse.ErrorResult("An error occurred during registration");
            }
        }

        public async Task<SingleApiResponse> Login(UserLoginDto userLoginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", userLoginDto.email);

                // Find user by username or email
                var user = await _userManager.FindByEmailAsync(userLoginDto.email)
                          ?? await _userManager.FindByNameAsync(userLoginDto.email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Username}", userLoginDto.email);
                    return SingleApiResponse.UnauthorizedResult("Invalid credentials");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed: User inactive - {email}", userLoginDto.email);
                    return SingleApiResponse.UnauthorizedResult("Account is deactivated");
                }

                // Validate password using UserManager
                if (!await _userManager.CheckPasswordAsync(user, userLoginDto.Password))
                {
                    _logger.LogWarning("Login failed: Invalid password for user - {email}", userLoginDto.email);
                    return SingleApiResponse.UnauthorizedResult("Invalid credentials");
                }

                // Update last login
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Get user roles
                var userWithRoles = await _context.USERS
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                var roles = userWithRoles?.UserRoles
                    .Where(ur => ur.IsActive && ur.Role.IsActive)
                    .Select(ur => ur.Role.RoleName)
                    .ToList() ?? new List<string>();

                // Generate JWT Token
                var token = await _tokenService.CreateTokenAsync(user);

                // Create UserInfoDto
                var userInfo = new UserInfoDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                
                    Roles = roles
                };

                // Create AuthResponseDto
                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    TokenExpiration = DateTime.UtcNow.AddHours(1), // أو الوقت الذي تحدده في الإعدادات
                    User = userInfo
                };

                _logger.LogInformation("Login successful for user: {email}", userLoginDto.email);

                return SingleApiResponse.SuccessResult(authResponse, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {email}", userLoginDto.email);
                return SingleApiResponse.ErrorResult("An error occurred during login");
            }
        }

        public Task<SingleApiResponse> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<SingleApiResponse> LogoutAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}