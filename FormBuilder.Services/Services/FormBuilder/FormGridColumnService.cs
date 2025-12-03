using formBuilder.Domian.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Entitys.FromBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormGridColumnService : IFormGridColumnService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormGridColumnService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var columns = await _unitOfWork.FormGridColumnRepository.GetAllAsync();
                var columnDtos = columns.Select(ToDto).ToList();
                return new ApiResponse(200, "All grid columns retrieved successfully", columnDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all grid columns: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var column = await _unitOfWork.FormGridColumnRepository.GetByIdAsync(id);
                if (column == null)
                    return new ApiResponse(404, "Grid column not found");

                var columnDto = ToDto(column);
                return new ApiResponse(200, "Grid column retrieved successfully", columnDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid column: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByGridIdAsync(int gridId)
        {
            try
            {
                var columns = await _unitOfWork.FormGridColumnRepository.GetByGridIdAsync(gridId);
                var columnDtos = columns.Select(ToDto).ToList();
                return new ApiResponse(200, "Grid columns by grid retrieved successfully", columnDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid columns by grid: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveByGridIdAsync(int gridId)
        {
            try
            {
                var columns = await _unitOfWork.FormGridColumnRepository.GetActiveByGridIdAsync(gridId);
                var columnDtos = columns.Select(ToDto).ToList();
                return new ApiResponse(200, "Active grid columns retrieved successfully", columnDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active grid columns: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByColumnCodeAsync(string columnCode, int gridId)
        {
            try
            {
                var column = await _unitOfWork.FormGridColumnRepository.GetByColumnCodeAsync(columnCode, gridId);
                if (column == null)
                    return new ApiResponse(404, "Grid column not found");

                var columnDto = ToDto(column);
                return new ApiResponse(200, "Grid column retrieved successfully", columnDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid column by code: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var columns = await _unitOfWork.FormGridColumnRepository.GetByFormBuilderIdAsync(formBuilderId);
                var columnDtos = columns.Select(ToDto).ToList();
                return new ApiResponse(200, "Grid columns by form builder retrieved successfully", columnDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid columns by form builder: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByFieldTypeIdAsync(int fieldTypeId)
        {
            try
            {
                var columns = await _unitOfWork.FormGridColumnRepository.GetByFieldTypeIdAsync(fieldTypeId);
                var columnDtos = columns.Select(ToDto).ToList();
                return new ApiResponse(200, "Grid columns by field type retrieved successfully", columnDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid columns by field type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateFormGridColumnDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if column code already exists
                var codeExists = await _unitOfWork.FormGridColumnRepository.ColumnCodeExistsAsync(
                    createDto.ColumnCode, createDto.GridId);

                if (codeExists)
                    return new ApiResponse(400, "Grid column code already exists for this grid");

                // Get next column order if not specified
                var columnOrder = createDto.ColumnOrder ??
                    await _unitOfWork.FormGridColumnRepository.GetNextColumnOrderAsync(createDto.GridId);

                var entity = ToEntity(createDto, columnOrder);
                _unitOfWork.FormGridColumnRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Grid column created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating grid column: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormGridColumnDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormGridColumnRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid column not found");

                // Check if column code already exists (excluding current record)
                if (!string.IsNullOrEmpty(updateDto.ColumnCode) && updateDto.ColumnCode != entity.ColumnCode)
                {
                    var codeExists = await _unitOfWork.FormGridColumnRepository.ColumnCodeExistsAsync(
                        updateDto.ColumnCode, entity.GridId, id);

                    if (codeExists)
                        return new ApiResponse(400, "Grid column code already exists for this grid");
                }

                MapUpdate(updateDto, entity);
                _unitOfWork.FormGridColumnRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Grid column updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating grid column: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormGridColumnRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid column not found");

                _unitOfWork.FormGridColumnRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Grid column deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting grid column: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.FormGridColumnRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid column not found");

                entity.IsActive = isActive;
                entity.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormGridColumnRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Grid column {message} successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling grid column active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormGridColumnRepository.AnyAsync(c => c.Id == id);
                return new ApiResponse(200, "Grid column existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking grid column existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ColumnCodeExistsAsync(string columnCode, int gridId, int? excludeId = null)
        {
            try
            {
                var exists = await _unitOfWork.FormGridColumnRepository.ColumnCodeExistsAsync(
                    columnCode, gridId, excludeId);

                return new ApiResponse(200, "Grid column code existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking grid column code existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetNextColumnOrderAsync(int gridId)
        {
            try
            {
                var nextOrder = await _unitOfWork.FormGridColumnRepository.GetNextColumnOrderAsync(gridId);
                return new ApiResponse(200, "Next column order retrieved successfully", nextOrder);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error getting next column order: {ex.Message}");
            }
        }

        // Mapping methods
        private FormGridColumnDto ToDto(FORM_GRID_COLUMNS entity)
        {
            if (entity == null) return null;

            return new FormGridColumnDto
            {
                Id = entity.Id,
                GridId = entity.GridId,
                GridName = entity.FORM_GRIDS?.GridName,
                FormBuilderName = entity.FORM_GRIDS?.FORM_BUILDER?.FormName,
                FieldTypeId = entity.FieldTypeId,
                FieldTypeName = entity.FIELD_TYPES?.TypeName,
                ColumnName = entity.ColumnName,
                ColumnCode = entity.ColumnCode,
                ColumnOrder = entity.ColumnOrder,
                IsMandatory = entity.IsMandatory,
                DataType = entity.DataType,
                MaxLength = entity.MaxLength,
                DefaultValueJson = entity.DefaultValueJson,
                ValidationRuleJson = entity.ValidationRuleJson,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        private FORM_GRID_COLUMNS ToEntity(CreateFormGridColumnDto dto, int columnOrder)
        {
            return new FORM_GRID_COLUMNS
            {
                GridId = dto.GridId,
                FieldTypeId = dto.FieldTypeId,
                ColumnName = dto.ColumnName,
                ColumnCode = dto.ColumnCode,
                ColumnOrder = columnOrder,
                IsMandatory = dto.IsMandatory,
                DataType = dto.DataType,
                MaxLength = dto.MaxLength,
                DefaultValueJson = dto.DefaultValueJson,
                ValidationRuleJson = dto.ValidationRuleJson,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.UtcNow
            };
        }

        private void MapUpdate(UpdateFormGridColumnDto dto, FORM_GRID_COLUMNS entity)
        {
            if (dto.GridId.HasValue)
                entity.GridId = dto.GridId.Value;

            if (dto.FieldTypeId.HasValue)
                entity.FieldTypeId = dto.FieldTypeId.Value;

            if (!string.IsNullOrEmpty(dto.ColumnName))
                entity.ColumnName = dto.ColumnName;

            if (!string.IsNullOrEmpty(dto.ColumnCode))
                entity.ColumnCode = dto.ColumnCode;

            if (dto.ColumnOrder.HasValue)
                entity.ColumnOrder = dto.ColumnOrder.Value;

            if (dto.IsMandatory.HasValue)
                entity.IsMandatory = dto.IsMandatory.Value;

            if (!string.IsNullOrEmpty(dto.DataType))
                entity.DataType = dto.DataType;

            if (dto.MaxLength.HasValue)
                entity.MaxLength = dto.MaxLength.Value;

            if (dto.DefaultValueJson != null)
                entity.DefaultValueJson = dto.DefaultValueJson;

            if (dto.ValidationRuleJson != null)
                entity.ValidationRuleJson = dto.ValidationRuleJson;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;

            entity.UpdatedDate = DateTime.UtcNow;
        }
    }
}