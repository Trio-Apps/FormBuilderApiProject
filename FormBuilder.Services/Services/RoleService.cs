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
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly FormBuilderDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            RoleManager<IdentityRole> roleManager,
            FormBuilderDbContext context,
            ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<RoleDto>> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return new ServiceResult<RoleDto>
                    {
                        Success = false,
                        ErrorMessage = "Role not found",
                        StatusCode = 404
                    };
                }

                var roleDto = MapToRoleDto(role);
                return new ServiceResult<RoleDto> { Success = true, Data = roleDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by ID: {RoleId}", roleId);
                return new ServiceResult<RoleDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<RoleDto>> GetRoleByNameAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return new ServiceResult<RoleDto>
                    {
                        Success = false,
                        ErrorMessage = "Role not found",
                        StatusCode = 404
                    };
                }

                var roleDto = MapToRoleDto(role);
                return new ServiceResult<RoleDto> { Success = true, Data = roleDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by name: {RoleName}", roleName);
                return new ServiceResult<RoleDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<List<RoleDto>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var roleDtos = roles.Select(MapToRoleDto).ToList();

                return new ServiceResult<List<RoleDto>> { Success = true, Data = roleDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return new ServiceResult<List<RoleDto>> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            try
            {
                // Check if role already exists
                if (await _roleManager.RoleExistsAsync(createRoleDto.Name))
                {
                    return new ServiceResult<RoleDto>
                    {
                        Success = false,
                        ErrorMessage = "Role already exists",
                        StatusCode = 400
                    };
                }

                var role = new IdentityRole(createRoleDto.Name);
                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ServiceResult<RoleDto>
                    {
                        Success = false,
                        ErrorMessage = errors,
                        StatusCode = 400
                    };
                }

                var roleDto = MapToRoleDto(role);
                _logger.LogInformation("Role created successfully: {RoleName}", role.Name);

                return new ServiceResult<RoleDto> { Success = true, Data = roleDto, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role: {RoleName}", createRoleDto.Name);
                return new ServiceResult<RoleDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<RoleDto>> UpdateRoleAsync(string roleId, UpdateRoleDto updateRoleDto)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return new ServiceResult<RoleDto>
                    {
                        Success = false,
                        ErrorMessage = "Role not found",
                        StatusCode = 404
                    };
                }

                // Update role name if provided and different
                if (!string.IsNullOrEmpty(updateRoleDto.Name) && role.Name != updateRoleDto.Name)
                {
                    // Check if new name is taken
                    if (await _roleManager.RoleExistsAsync(updateRoleDto.Name))
                    {
                        return new ServiceResult<RoleDto>
                        {
                            Success = false,
                            ErrorMessage = "Role name already exists",
                            StatusCode = 400
                        };
                    }

                    role.Name = updateRoleDto.Name;
                    role.NormalizedName = updateRoleDto.Name.ToUpper();

                    var updateResult = await _roleManager.UpdateAsync(role);
                    if (!updateResult.Succeeded)
                    {
                        var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                        return new ServiceResult<RoleDto>
                        {
                            Success = false,
                            ErrorMessage = errors,
                            StatusCode = 400
                        };
                    }
                }

                var updatedRole = await _roleManager.FindByIdAsync(roleId);
                var roleDto = MapToRoleDto(updatedRole);

                _logger.LogInformation("Role updated successfully: {RoleId}", roleId);
                return new ServiceResult<RoleDto> { Success = true, Data = roleDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {RoleId}", roleId);
                return new ServiceResult<RoleDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> DeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "Role not found",
                        StatusCode = 404
                    };
                }

                // Check if role is assigned to any users
                var usersInRole = await _context.UserRoles.AnyAsync(ur => ur.RoleId == roleId);
                if (usersInRole)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "Cannot delete role that is assigned to users",
                        StatusCode = 400
                    };
                }

                // Delete the role
                var result = await _roleManager.DeleteAsync(role);
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

                _logger.LogInformation("Role deleted successfully: {RoleName}", role.Name);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role: {RoleId}", roleId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> AssignPermissionsToRoleAsync(string roleId, List<int> permissionIds)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "Role not found",
                        StatusCode = 404
                    };
                }

                // التحقق من وجود الصلاحيات
                var permissions = await _context.Permissions
                    .Where(p => permissionIds.Contains(p.PermissionID))
                    .ToListAsync();

                if (permissions.Count != permissionIds.Count)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "Some permissions were not found",
                        StatusCode = 400
                    };
                }

                // إزالة الصلاحيات الحالية لنفس الـ Role (لتجنب التكرار)
                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleID == roleId && permissionIds.Contains(rp.PermissionID))
                    .ToListAsync();

                if (existingPermissions.Any())
                {
                    _context.RolePermissions.RemoveRange(existingPermissions);
                }

                // إضافة الصلاحيات الجديدة
                var rolePermissions = permissions.Select(p => new RolePermission
                {
                    RoleID = roleId,
                    PermissionID = p.PermissionID,
                    AssignedDate = DateTime.UtcNow
                });

                await _context.RolePermissions.AddRangeAsync(rolePermissions);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Permissions assigned successfully to role: {RoleName}", role.Name);
                return new ServiceResult<bool>
                {
                    Success = true,
                    Data = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role: {RoleId}", roleId);
                return new ServiceResult<bool>
                {
                    Success = false,
                    ErrorMessage = "An error occurred while assigning permissions",
                    StatusCode = 500
                };
            }
        }

        // Helper Methods
        private RoleDto MapToRoleDto(IdentityRole role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }
    }
}