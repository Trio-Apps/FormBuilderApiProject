using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{// Services/UserRoleService.cs
    using FormBuilder.API.Data;
    using FormBuilder.API.Models;
    using FormBuilder.Application.DTOS;
    using FormBuilder.Application.DTOS.Auth;
    using FormBuilder.Application.IServices;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class UserRoleService : IUserRoleService
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserRoleService> _logger;

        public UserRoleService(AuthDbContext context, UserManager<AppUser> userManager, ILogger<UserRoleService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ServiceResult<bool>> AssignRoleToUserAsync(AssignRoleToUserDto assignRoleDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
                if (user == null)
                {
                    return new ServiceResult<bool> { Success = false, ErrorMessage = "User not found", StatusCode = 404 };
                }

                var role = await _context.CustomRoles.FindAsync(assignRoleDto.RoleId);
                if (role == null)
                {
                    return new ServiceResult<bool> { Success = false, ErrorMessage = "Role not found", StatusCode = 404 };
                }

                // Check if user already has this role
                var existingUserRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserID == assignRoleDto.UserId && ur.RoleID == assignRoleDto.RoleId);

                if (existingUserRole != null)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "User already has this role",
                        StatusCode = 400
                    };
                }

                var userRole = new UserRole
                {
                    UserID = assignRoleDto.UserId,
                    RoleID = assignRoleDto.RoleId,
                    StartDate = DateTime.UtcNow,
                    EndDate = assignRoleDto.EndDate
                };

                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role {RoleId} assigned to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to user");
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> RemoveRoleFromUserAsync(string userId, int roleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserID == userId && ur.RoleID == roleId);

                if (userRole == null)
                {
                    return new ServiceResult<bool> { Success = false, ErrorMessage = "User role assignment not found", StatusCode = 404 };
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role {RoleId} removed from user {UserId}", roleId, userId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role from user");
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<List<RoleDto>>> GetUserRolesAsync(string userId)
        {
            try
            {
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserID == userId)
                    .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .ToListAsync();

                var roleDtos = userRoles.Select(ur => new RoleDto
                {
                    RoleID = ur.Role.RoleID,
                    RoleName = ur.Role.RoleName,
                    Description = ur.Role.Description,
                    IsActive = ur.Role.IsActive,
                    CreatedDate = ur.Role.CreatedDate,
                    Permissions = ur.Role.RolePermissions?.Select(rp => rp.Permission.PermissionName).ToList() ?? new List<string>()
                }).ToList();

                return new ServiceResult<List<RoleDto>> { Success = true, Data = roleDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user roles for user: {UserId}", userId);
                return new ServiceResult<List<RoleDto>> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<List<UserWithRolesDto>>> GetUsersWithRolesAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserWithRolesDto>();

                foreach (var user in users)
                {
                    var userRolesResult = await GetUserRolesAsync(user.Id);
                    var userDto = new UserWithRolesDto
                    {
                        Id = user.Id,
                        DisplayName = user.DisplayName,
                        Email = user.Email,
                        IsActive = user.IsActive,
                        Roles = userRolesResult.Success ? userRolesResult.Data : new List<RoleDto>()
                    };
                    userDtos.Add(userDto);
                }

                return new ServiceResult<List<UserWithRolesDto>> { Success = true, Data = userDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users with roles");
                return new ServiceResult<List<UserWithRolesDto>> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> UpdateUserRoleAsync(string userId, int roleId, DateTime? endDate)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserID == userId && ur.RoleID == roleId);

                if (userRole == null)
                {
                    return new ServiceResult<bool> { Success = false, ErrorMessage = "User role assignment not found", StatusCode = 404 };
                }

                userRole.EndDate = endDate;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User role updated for user {UserId}, role {RoleId}", userId, roleId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role");
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }
    }
}
