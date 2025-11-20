using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.Application.DTOS;
using FormBuilder.Application.DTOS.Auth;
using FormBuilder.Application.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AuthDbContext context,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<UsersDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<UsersDto>
                    {
                        Success = false,
                        ErrorMessage = "User not found",
                        StatusCode = 404
                    };
                }

                var userDto = await MapToUsersDtoAsync(user);
                return new ServiceResult<UsersDto> { Success = true, Data = userDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                return new ServiceResult<UsersDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<UsersDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ServiceResult<UsersDto>
                    {
                        Success = false,
                        ErrorMessage = "User not found",
                        StatusCode = 404
                    };
                }

                var userDto = await MapToUsersDtoAsync(user);
                return new ServiceResult<UsersDto> { Success = true, Data = userDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return new ServiceResult<UsersDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }


        public async Task<ServiceResult<List<UsersDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UsersDto>();

                foreach (var user in users)
                {
                    var userDto = await MapToUsersDtoAsync(user);
                    userDtos.Add(userDto);
                }

                return new ServiceResult<List<UsersDto>> { Success = true, Data = userDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new ServiceResult<List<UsersDto>> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<List<UsersDto>>> GetActiveUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .Where(u => u.IsActive)
                    .ToListAsync();

                var userDtos = new List<UsersDto>();
                foreach (var user in users)
                {
                    var userDto = await MapToUsersDtoAsync(user);
                    userDtos.Add(userDto);
                }

                return new ServiceResult<List<UsersDto>> { Success = true, Data = userDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active users");
                return new ServiceResult<List<UsersDto>> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<UsersDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
                if (existingUser != null)
                {
                    return new ServiceResult<UsersDto>
                    {
                        Success = false,
                        ErrorMessage = "User with this email already exists",
                        StatusCode = 400
                    };
                }

                var user = new AppUser
                {
                    UserName = createUserDto.UserName,
                    Email = createUserDto.Email,
                    DisplayName = createUserDto.DisplayName,
                    PhoneNumber = createUserDto.PhoneNumber,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ServiceResult<UsersDto>
                    {
                        Success = false,
                        ErrorMessage = errors,
                        StatusCode = 400
                    };
                }

                // Assign roles if any
                if (createUserDto.Roles != null && createUserDto.Roles.Any())
                {
                    foreach (var roleName in createUserDto.Roles)
                    {
                        if (await _roleManager.RoleExistsAsync(roleName))
                        {
                            await _userManager.AddToRoleAsync(user, roleName);
                        }
                    }
                }

                var userDto = await MapToUsersDtoAsync(user);
                _logger.LogInformation("User created successfully: {Email}", createUserDto.Email);

                return new ServiceResult<UsersDto> { Success = true, Data = userDto, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", createUserDto.Email);
                return new ServiceResult<UsersDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<UsersDto>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<UsersDto>
                    {
                        Success = false,
                        ErrorMessage = "User not found",
                        StatusCode = 404
                    };
                }

                // Update basic properties
                if (!string.IsNullOrEmpty(updateUserDto.DisplayName))
                    user.DisplayName = updateUserDto.DisplayName;

                if (!string.IsNullOrEmpty(updateUserDto.Email))
                    user.Email = updateUserDto.Email;

                if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))
                    user.PhoneNumber = updateUserDto.PhoneNumber;

                if (updateUserDto.IsActive.HasValue)
                    user.IsActive = updateUserDto.IsActive.Value;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ServiceResult<UsersDto>
                    {
                        Success = false,
                        ErrorMessage = errors,
                        StatusCode = 400
                    };
                }

                var updatedUser = await _userManager.FindByIdAsync(userId);
                var userDto = await MapToUsersDtoAsync(updatedUser);

                _logger.LogInformation("User updated successfully: {UserId}", userId);
                return new ServiceResult<UsersDto> { Success = true, Data = userDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", userId);
                return new ServiceResult<UsersDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "User not found",
                        StatusCode = 404
                    };
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = errors,
                        StatusCode = 400
                    };
                }

                _logger.LogInformation("User deleted successfully: {UserId}", userId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "User not found",
                        StatusCode = 404
                    };
                }

                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = errors,
                        StatusCode = 400
                    };
                }

                _logger.LogInformation("User deactivated: {UserId}", userId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {UserId}", userId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "User not found",
                        StatusCode = 404
                    };
                }

                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = errors,
                        StatusCode = 400
                    };
                }

                _logger.LogInformation("User activated: {UserId}", userId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user: {UserId}", userId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        // Helper Methods
        private async Task<UsersDto> MapToUsersDtoAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UsersDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                Roles = roles.ToList()
            };
        }


        

       
       

       
      
    }
}