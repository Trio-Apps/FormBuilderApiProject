using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Models.DTOs;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IunitOfwork _unitOfWork;

        public ProjectService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var projects = await _unitOfWork.ProjectRepository.GetAllAsync();
                var dtos = projects.Select(ToDto).ToList();
                return new ApiResponse(200, "All projects retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all projects: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                if (project == null)
                    return new ApiResponse(404, "Project not found");

                var dto = ToDto(project);
                return new ApiResponse(200, "Project retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving project: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByCodeAsync(string code)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByCodeAsync(code);
                if (project == null)
                    return new ApiResponse(404, "Project not found");

                var dto = ToDto(project);
                return new ApiResponse(200, "Project retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving project: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveAsync()
        {
            try
            {
                var projects = await _unitOfWork.ProjectRepository.GetActiveAsync();
                var dtos = projects.Select(ToDto).ToList();
                return new ApiResponse(200, "Active projects retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active projects: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateProjectDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if code already exists
                var codeExists = await _unitOfWork.ProjectRepository.CodeExistsAsync(createDto.Code);
                if (codeExists)
                    return new ApiResponse(400, "Project code already exists");

                var entity = ToEntity(createDto);
                _unitOfWork.ProjectRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                var createdEntity = await _unitOfWork.ProjectRepository.GetByIdAsync(entity.Id);
                return new ApiResponse(200, "Project created successfully", ToDto(createdEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating project: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateProjectDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Project not found");

                // Check if code already exists (excluding current record)
                if (!string.IsNullOrEmpty(updateDto.Code) && updateDto.Code != entity.Code)
                {
                    var codeExists = await _unitOfWork.ProjectRepository.CodeExistsAsync(updateDto.Code, id);
                    if (codeExists)
                        return new ApiResponse(400, "Project code already exists");
                }

                MapUpdate(updateDto, entity);
                _unitOfWork.ProjectRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                return new ApiResponse(200, "Project updated successfully", ToDto(updatedEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating project: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Project not found");

                _unitOfWork.ProjectRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Project deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting project: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Project not found");

                entity.IsActive = isActive;
                _unitOfWork.ProjectRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Project {message} successfully", ToDto(updatedEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling project active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.ProjectRepository.AnyAsync(e => e.Id == id);
                return new ApiResponse(200, "Project existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking project existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CodeExistsAsync(string code, int? excludeId = null)
        {
            try
            {
                var exists = await _unitOfWork.ProjectRepository.CodeExistsAsync(code, excludeId);
                return new ApiResponse(200, "Project code existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking project code existence: {ex.Message}");
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private ProjectDto ToDto(PROJECTS entity)
        {
            if (entity == null) return null;

            return new ProjectDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        private PROJECTS ToEntity(CreateProjectDto dto)
        {
            return new PROJECTS
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                IsActive = dto.IsActive
            };
        }

        private void MapUpdate(UpdateProjectDto dto, PROJECTS entity)
        {
            if (!string.IsNullOrEmpty(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Code))
                entity.Code = dto.Code;

            if (!string.IsNullOrEmpty(dto.Description))
                entity.Description = dto.Description;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;
        }
    }
}