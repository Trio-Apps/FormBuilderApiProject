using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    using FormBuilder.API.Data;
    using FormBuilder.API.Models;
    using FormBuilder.Application.DTOS;
    using FormBuilder.Application.DTOS.Auth;
    using FormBuilder.Application.IServices;
    // Services/RoleService.cs
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class RoleService : IRoleService
    {
        private readonly AuthDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(AuthDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<RoleDto>> GetRoleByIdAsync(int roleId)
        {
            try
            {
                var role = await _context.CustomRoles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(r => r.RoleID == roleId);

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

        public async Task<ServiceResult<List<RoleDto>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _context.CustomRoles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Where(r => r.IsActive)
                    .ToListAsync();

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
                // Check if role name already exists
                var existingRole = await _context.CustomRoles
                    .FirstOrDefaultAsync(r => r.RoleName == createRoleDto.RoleName);

                if (existingRole != null)
                {
                    return new ServiceResult<RoleDto>
                    {
                        Success = false,
                        ErrorMessage = "Role with this name already exists",
                        StatusCode = 400
                    };
                }

                // Create new role
                var role = new Role
                {
                    RoleName = createRoleDto.RoleName,
                    Description = createRoleDto.Description,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                await _context.CustomRoles.AddAsync(role);
                await _context.SaveChangesAsync();

                // Assign permissions if any
                if (createRoleDto.PermissionIds.Any())
                {
                    await AssignPermissionsToRoleAsync(role.RoleID, createRoleDto.PermissionIds);
                }

                var roleDto = MapToRoleDto(role);
                _logger.LogInformation("Role created successfully: {RoleName}", role.RoleName);

                return new ServiceResult<RoleDto> { Success = true, Data = roleDto, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return new ServiceResult<RoleDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<RoleDto>> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto)
        {
            try
            {
                var role = await _context.CustomRoles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(r => r.RoleID == roleId);

                if (role == null)
                {
                    return new ServiceResult<RoleDto>
                    {
                        Success = false,
                        ErrorMessage = "Role not found",
                        StatusCode = 404
                    };
                }

                // Update properties if provided
                if (!string.IsNullOrEmpty(updateRoleDto.RoleName))
                {
                    // Check if new role name is taken by another role
                    var existingRole = await _context.CustomRoles
                        .FirstOrDefaultAsync(r => r.RoleName == updateRoleDto.RoleName && r.RoleID != roleId);

                    if (existingRole != null)
                    {
                        return new ServiceResult<RoleDto>
                        {
                            Success = false,
                            ErrorMessage = "Role name is already taken",
                            StatusCode = 400
                        };
                    }
                    role.RoleName = updateRoleDto.RoleName;
                }

                if (!string.IsNullOrEmpty(updateRoleDto.Description))
                    role.Description = updateRoleDto.Description;

                if (updateRoleDto.IsActive.HasValue)
                    role.IsActive = updateRoleDto.IsActive.Value;

                // Update permissions if provided
                if (updateRoleDto.PermissionIds != null && updateRoleDto.PermissionIds.Any())
                {
                    // Remove existing permissions
                    var existingPermissions = _context.RolePermissions.Where(rp => rp.RoleID == roleId);
                    _context.RolePermissions.RemoveRange(existingPermissions);

                    // Add new permissions
                    await AssignPermissionsToRoleAsync(roleId, updateRoleDto.PermissionIds);
                }

                await _context.SaveChangesAsync();

                var roleDto = MapToRoleDto(role);
                _logger.LogInformation("Role updated successfully: {RoleId}", roleId);

                return new ServiceResult<RoleDto> { Success = true, Data = roleDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {RoleId}", roleId);
                return new ServiceResult<RoleDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> DeleteRoleAsync(int roleId)
        {
            try
            {
                var role = await _context.CustomRoles
                    .Include(r => r.UserRoles)
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.RoleID == roleId);

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
                if (role.UserRoles.Any())
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "Cannot delete role that is assigned to users",
                        StatusCode = 400
                    };
                }

                // Remove role permissions
                _context.RolePermissions.RemoveRange(role.RolePermissions);

                // Remove role
                _context.CustomRoles.Remove(role);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Role deleted successfully: {RoleId}", roleId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role: {RoleId}", roleId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            try
            {
                var role = await _context.CustomRoles.FindAsync(roleId);
                if (role == null)
                {
                    return new ServiceResult<bool> { Success = false, ErrorMessage = "Role not found", StatusCode = 404 };
                }

                var permissions = await _context.Permissions
                    .Where(p => permissionIds.Contains(p.PermissionID))
                    .ToListAsync();

                var rolePermissions = permissions.Select(p => new RolePermission
                {
                    RoleID = roleId,
                    PermissionID = p.PermissionID,
                    AssignedDate = DateTime.UtcNow
                });

                await _context.RolePermissions.AddRangeAsync(rolePermissions);
                await _context.SaveChangesAsync();

                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role: {RoleId}", roleId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds)
        {
            try
            {
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleID == roleId && permissionIds.Contains(rp.PermissionID))
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(rolePermissions);
                await _context.SaveChangesAsync();

                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permissions from role: {RoleId}", roleId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        private RoleDto MapToRoleDto(Role role)
        {
            return new RoleDto
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedDate = role.CreatedDate,
                Permissions = role.RolePermissions?.Select(rp => rp.Permission.PermissionName).ToList() ?? new List<string>()
            };
        }
    }
}
