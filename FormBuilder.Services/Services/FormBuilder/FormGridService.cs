using formBuilder.Domian.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormGridService : IFormGridService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormGridService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // ================================
        // GET ALL FORM GRIDS
        // ================================
        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var formGrids = await _unitOfWork.FormGridRepository.GetAllAsync();
                var formGridDtos = formGrids.Select(ToDto).ToList();
                return new ApiResponse(200, "All form grids retrieved successfully", formGridDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all form grids: {ex.Message}");
            }
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var formGrid = await _unitOfWork.FormGridRepository.GetByIdAsync(id);
                if (formGrid == null)
                    return new ApiResponse(404, "Form grid not found");

                var formGridDto = ToDto(formGrid);
                return new ApiResponse(200, "Form grid retrieved successfully", formGridDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form grid: {ex.Message}");
            }
        }

        // ================================
        // GET BY FORM BUILDER ID
        // ================================
        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var formGrids = await _unitOfWork.FormGridRepository.GetByFormBuilderIdAsync(formBuilderId);
                var formGridDtos = formGrids.Select(ToDto).ToList();
                return new ApiResponse(200, "Form grids by form builder retrieved successfully", formGridDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form grids by form builder: {ex.Message}");
            }
        }

        // ================================
        // GET BY TAB ID
        // ================================
        public async Task<ApiResponse> GetByTabIdAsync(int tabId)
        {
            try
            {
                var formGrids = await _unitOfWork.FormGridRepository.GetByTabIdAsync(tabId);
                var formGridDtos = formGrids.Select(ToDto).ToList();
                return new ApiResponse(200, "Form grids by tab retrieved successfully", formGridDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form grids by tab: {ex.Message}");
            }
        }

        // ================================
        // GET ACTIVE BY FORM BUILDER ID
        // ================================
        public async Task<ApiResponse> GetActiveByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var formGrids = await _unitOfWork.FormGridRepository.GetActiveByFormBuilderIdAsync(formBuilderId);
                var formGridDtos = formGrids.Select(ToDto).ToList();
                return new ApiResponse(200, "Active form grids by form builder retrieved successfully", formGridDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active form grids by form builder: {ex.Message}");
            }
        }

        // ================================
        // GET BY GRID CODE
        // ================================
        public async Task<ApiResponse> GetByGridCodeAsync(string gridCode, int formBuilderId)
        {
            try
            {
                var formGrid = await _unitOfWork.FormGridRepository.GetByGridCodeAsync(gridCode, formBuilderId);
                if (formGrid == null)
                    return new ApiResponse(404, "Form grid not found");

                var formGridDto = ToDto(formGrid);
                return new ApiResponse(200, "Form grid retrieved successfully", formGridDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form grid by code: {ex.Message}");
            }
        }

        // ================================
        // CREATE
        // ================================
        public async Task<ApiResponse> CreateAsync(CreateFormGridDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if grid code already exists
                var codeExists = await _unitOfWork.FormGridRepository.GridCodeExistsAsync(createDto.GridCode, createDto.FormBuilderId);
                if (codeExists)
                    return new ApiResponse(400, "Form grid code already exists for this form builder");

                // Get next grid order if not specified
                var gridOrder = createDto.GridOrder ??
                    await _unitOfWork.FormGridRepository.GetNextGridOrderAsync(createDto.FormBuilderId, createDto.TabId);

                var entity = ToEntity(createDto, gridOrder);
                _unitOfWork.FormGridRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form grid created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating form grid: {ex.Message}");
            }
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormGridDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormGridRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form grid not found");

                // Check if grid code already exists (excluding current record)
                if (!string.IsNullOrEmpty(updateDto.GridCode) && updateDto.GridCode != entity.GridCode)
                {
                    var codeExists = await _unitOfWork.FormGridRepository.GridCodeExistsAsync(updateDto.GridCode, entity.FormBuilderId, id);
                    if (codeExists)
                        return new ApiResponse(400, "Form grid code already exists for this form builder");
                }

                MapUpdate(updateDto, entity);
                _unitOfWork.FormGridRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form grid updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form grid: {ex.Message}");
            }
        }

        // ================================
        // DELETE
        // ================================
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormGridRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form grid not found");

                _unitOfWork.FormGridRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form grid deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting form grid: {ex.Message}");
            }
        }

        // ================================
        // TOGGLE ACTIVE
        // ================================
        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.FormGridRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form grid not found");

                entity.IsActive = isActive;
                entity.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormGridRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Form grid {message} successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling form grid active status: {ex.Message}");
            }
        }

        // ================================
        // EXISTS
        // ================================
        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormGridRepository.AnyAsync(g => g.Id == id);
                return new ApiResponse(200, "Form grid existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking form grid existence: {ex.Message}");
            }
        }

        // ================================
        // GRID CODE EXISTS
        // ================================
        public async Task<ApiResponse> GridCodeExistsAsync(string gridCode, int formBuilderId, int? excludeId = null)
        {
            try
            {
                var exists = await _unitOfWork.FormGridRepository.GridCodeExistsAsync(gridCode, formBuilderId, excludeId);
                return new ApiResponse(200, "Form grid code existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking form grid code existence: {ex.Message}");
            }
        }

        // ================================
        // GET NEXT GRID ORDER
        // ================================
        public async Task<ApiResponse> GetNextGridOrderAsync(int formBuilderId, int? tabId = null)
        {
            try
            {
                var nextOrder = await _unitOfWork.FormGridRepository.GetNextGridOrderAsync(formBuilderId, tabId);
                return new ApiResponse(200, "Next grid order retrieved successfully", nextOrder);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error getting next grid order: {ex.Message}");
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private FormGridDto ToDto(FORM_GRIDS entity)
        {
            if (entity == null) return null;

            return new FormGridDto
            {
                Id = entity.Id,
                FormBuilderId = entity.FormBuilderId,
                FormBuilderName = entity.FORM_BUILDER?.FormName,
                GridName = entity.GridName,
                GridCode = entity.GridCode,
                TabId = entity.TabId,
                TabName = entity.FORM_TABS?.TabName,
                GridOrder = entity.GridOrder,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        private FORM_GRIDS ToEntity(CreateFormGridDto dto, int gridOrder)
        {
            return new FORM_GRIDS
            {
                FormBuilderId = dto.FormBuilderId,
                GridName = dto.GridName,
                GridCode = dto.GridCode,
                TabId = dto.TabId,
                GridOrder = gridOrder,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.UtcNow
            };
        }

        private void MapUpdate(UpdateFormGridDto dto, FORM_GRIDS entity)
        {
            if (!string.IsNullOrEmpty(dto.GridName))
                entity.GridName = dto.GridName;

            if (!string.IsNullOrEmpty(dto.GridCode))
                entity.GridCode = dto.GridCode;

            if (dto.TabId.HasValue)
                entity.TabId = dto.TabId.Value;

            if (dto.GridOrder.HasValue)
                entity.GridOrder = dto.GridOrder.Value;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;

            entity.UpdatedDate = DateTime.UtcNow;
        }
    }
}