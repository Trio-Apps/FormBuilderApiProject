using formBuilder.Domian.Entitys;

using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FieldOptionsService : IFieldOptionsService
    {
        private readonly IunitOfwork _unitOfWork;

        public FieldOptionsService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        // ================================
        // GET ALL FIELD OPTIONS
        // ================================
        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var options = await _unitOfWork.Repositary<FIELD_OPTIONS>().GetAllAsync();
                var optionDtos = options.Select(ToDto).ToList();
                return new ApiResponse(200, "Field options retrieved successfully", optionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving field options: {ex.Message}");
            }
        }
        // ================================
        // GET BY FIELD ID
        // ================================
        public async Task<ApiResponse> GetByFieldIdAsync(int fieldId)
        {
            try
            {
                var options = await _unitOfWork.FieldOptionsRepository.GetByFieldIdAsync(fieldId);
                var optionDtos = options.Select(ToDto).ToList();
                return new ApiResponse(200, "Field options retrieved successfully", optionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving field options: {ex.Message}");
            }
        }

        // ================================
        // GET ACTIVE BY FIELD ID
        // ================================
        public async Task<ApiResponse> GetActiveByFieldIdAsync(int fieldId)
        {
            try
            {
                var options = await _unitOfWork.FieldOptionsRepository.GetActiveByFieldIdAsync(fieldId);
                var optionDtos = options.Select(ToDto).ToList();
                return new ApiResponse(200, "Active field options retrieved successfully", optionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active field options: {ex.Message}");
            }
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var option = await _unitOfWork.FieldOptionsRepository.GetByIdAsync(id);
                if (option == null)
                    return new ApiResponse(404, "Field option not found");

                var optionDto = ToDto(option);
                return new ApiResponse(200, "Field option retrieved successfully", optionDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving field option: {ex.Message}");
            }
        }

        // ================================
        // CREATE
        // ================================
        public async Task<ApiResponse> CreateAsync(CreateFieldOptionDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Validate if field exists
                var fieldExists = await _unitOfWork.FormFieldRepository.ExistsAsync(createDto.FieldId);
                if (!fieldExists)
                    return new ApiResponse(400, "Invalid field ID");

                var entity = ToEntity(createDto);
                _unitOfWork.Repositary<FIELD_OPTIONS>().Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field option created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating field option: {ex.Message}");
            }
        }

        // ================================
        // CREATE BULK
        // ================================
        public async Task<ApiResponse> CreateBulkAsync(List<CreateFieldOptionDto> createDtos)
        {
            try
            {
                if (createDtos == null || !createDtos.Any())
                    return new ApiResponse(400, "No field options provided");

                var entities = createDtos.Select(ToEntity).ToList();
                _unitOfWork.Repositary<FIELD_OPTIONS>().AddRange(entities);
                await _unitOfWork.CompleteAsyn();

                var resultDtos = entities.Select(ToDto).ToList();
                return new ApiResponse(200, "Field options created successfully", resultDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating field options: {ex.Message}");
            }
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<ApiResponse> UpdateAsync(int id, UpdateFieldOptionDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FieldOptionsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Field option not found");

                MapUpdate(updateDto, entity);
                _unitOfWork.Repositary<FIELD_OPTIONS>().Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field option updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating field option: {ex.Message}");
            }
        }

        // ================================
        // DELETE (HARD)
        // ================================
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FieldOptionsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Field option not found");

                _unitOfWork.Repositary<FIELD_OPTIONS>().Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field option deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting field option: {ex.Message}");
            }
        }

        // ================================
        // SOFT DELETE
        // ================================
        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FieldOptionsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Field option not found");

                entity.IsActive = false;
                _unitOfWork.Repositary<FIELD_OPTIONS>().Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field option soft deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error soft deleting field option: {ex.Message}");
            }
        }

        // ================================
        // GET DEFAULT OPTION
        // ================================
        public async Task<ApiResponse> GetDefaultOptionAsync(int fieldId)
        {
            try
            {
                var defaultOption = await _unitOfWork.FieldOptionsRepository.GetDefaultOptionAsync(fieldId);
                if (defaultOption == null)
                    return new ApiResponse(404, "No default option found for this field");

                var optionDto = ToDto(defaultOption);
                return new ApiResponse(200, "Default option retrieved successfully", optionDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving default option: {ex.Message}");
            }
        }

        // ================================
        // GET OPTIONS COUNT
        // ================================
        public async Task<ApiResponse> GetOptionsCountAsync(int fieldId)
        {
            try
            {
                var count = await _unitOfWork.FieldOptionsRepository.GetOptionsCountAsync(fieldId);
                return new ApiResponse(200, "Options count retrieved successfully", count);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving options count: {ex.Message}");
            }
        }

        // ================================
        // VALIDATION
        // ================================
        public async Task<bool> FieldHasOptionsAsync(int fieldId)
        {
            return await _unitOfWork.FieldOptionsRepository.FieldHasOptionsAsync(fieldId);
        }

        // ================================
        // MAPPING
        // ================================
        private FieldOptionDto ToDto(FIELD_OPTIONS entity)
        {
            if (entity == null) return null;

            return new FieldOptionDto
            {
                Id = entity.Id,
                FieldId = entity.FieldId,
                OptionText = entity.OptionText ?? string.Empty,
                OptionValue = entity.OptionValue ?? string.Empty,
                OptionOrder = entity.OptionOrder,
                IsDefault = entity.IsDefault,
                IsActive = entity.IsActive
            };
        }

        private FIELD_OPTIONS ToEntity(CreateFieldOptionDto dto)
        {
            return new FIELD_OPTIONS
            {
                FieldId = dto.FieldId,
                OptionText = dto.OptionText,
                OptionValue = dto.OptionValue,
                OptionOrder = dto.OptionOrder,
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive
            };
        }

        private void MapUpdate(UpdateFieldOptionDto dto, FIELD_OPTIONS entity)
        {
            entity.OptionText = dto.OptionText;
            entity.OptionValue = dto.OptionValue;
            entity.OptionOrder = dto.OptionOrder;
            entity.IsDefault = dto.IsDefault;
            entity.IsActive = dto.IsActive;
        }
    }
}