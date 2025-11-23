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
    // Services/PermissionService.cs
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class PermissionService : IPermissionService
    {
        private readonly FormBuilderDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(FormBuilderDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<PermissionDto>> GetPermissionByIdAsync(int permissionId)
        {
            try
            {
                var permission = await _context.Permissions.FindAsync(permissionId);
                if (permission == null)
                {
                    return new ServiceResult<PermissionDto> { Success = false, ErrorMessage = "Permission not found", StatusCode = 404 };
                }

                var permissionDto = MapToPermissionDto(permission);
                return new ServiceResult<PermissionDto> { Success = true, Data = permissionDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission by ID: {PermissionId}", permissionId);
                return new ServiceResult<PermissionDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<List<PermissionDto>>> GetAllPermissionsAsync()
        {
            try
            {
                var permissions = await _context.Permissions.ToListAsync();
                var permissionDtos = permissions.Select(MapToPermissionDto).ToList();
                return new ServiceResult<List<PermissionDto>> { Success = true, Data = permissionDtos, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all permissions");
                return new ServiceResult<List<PermissionDto>> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }


        public async Task<ServiceResult<PermissionDto>> CreatePermissionAsync(CreatePermissionDto createPermissionDto)
        {
            try
            {
                // Check if permission name already exists
                var existingPermission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.PermissionName == createPermissionDto.PermissionName);

                if (existingPermission != null)
                {
                    return new ServiceResult<PermissionDto>
                    {
                        Success = false,
                        ErrorMessage = "Permission with this name already exists",
                        StatusCode = 400
                    };
                }

                var permission = new Permission
                {
                    PermissionName = createPermissionDto.PermissionName,
                    Description = createPermissionDto.Description,
                   
                    CreatedDate = DateTime.UtcNow
                };

                await _context.Permissions.AddAsync(permission);
                await _context.SaveChangesAsync();

                var permissionDto = MapToPermissionDto(permission);
                _logger.LogInformation("Permission created successfully: {PermissionName}", permission.PermissionName);

                return new ServiceResult<PermissionDto> { Success = true, Data = permissionDto, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission");
                return new ServiceResult<PermissionDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<PermissionDto>> UpdatePermissionAsync(int permissionId, UpdatePermissionDto updatePermissionDto)
        {
            try
            {
                var permission = await _context.Permissions.FindAsync(permissionId);
                if (permission == null)
                {
                    return new ServiceResult<PermissionDto> { Success = false, ErrorMessage = "Permission not found", StatusCode = 404 };
                }

                if (!string.IsNullOrEmpty(updatePermissionDto.PermissionName))
                {
                    // Check if new permission name is taken by another permission
                    var existingPermission = await _context.Permissions
                        .FirstOrDefaultAsync(p => p.PermissionName == updatePermissionDto.PermissionName && p.PermissionID != permissionId);

                    if (existingPermission != null)
                    {
                        return new ServiceResult<PermissionDto>
                        {
                            Success = false,
                            ErrorMessage = "Permission name is already taken",
                            StatusCode = 400
                        };
                    }
                    permission.PermissionName = updatePermissionDto.PermissionName;
                }

                if (!string.IsNullOrEmpty(updatePermissionDto.Description))
                    permission.Description = updatePermissionDto.Description;


                await _context.SaveChangesAsync();

                var permissionDto = MapToPermissionDto(permission);
                _logger.LogInformation("Permission updated successfully: {PermissionId}", permissionId);

                return new ServiceResult<PermissionDto> { Success = true, Data = permissionDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission: {PermissionId}", permissionId);
                return new ServiceResult<PermissionDto> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        public async Task<ServiceResult<bool>> DeletePermissionAsync(int permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .Include(p => p.RolePermissions)
                    .FirstOrDefaultAsync(p => p.PermissionID == permissionId);

                if (permission == null)
                {
                    return new ServiceResult<bool> { Success = false, ErrorMessage = "Permission not found", StatusCode = 404 };
                }

                // Check if permission is assigned to any roles
                if (permission.RolePermissions.Any())
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        ErrorMessage = "Cannot delete permission that is assigned to roles",
                        StatusCode = 400
                    };
                }

                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Permission deleted successfully: {PermissionId}", permissionId);
                return new ServiceResult<bool> { Success = true, Data = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission: {PermissionId}", permissionId);
                return new ServiceResult<bool> { Success = false, ErrorMessage = "An error occurred", StatusCode = 500 };
            }
        }

        private PermissionDto MapToPermissionDto(Permission permission)
        {
            return new PermissionDto
            {
                PermissionID = permission.PermissionID,
                PermissionName = permission.PermissionName,
                Description = permission.Description,
                CreatedDate = permission.CreatedDate
            };
        }
    }
}
