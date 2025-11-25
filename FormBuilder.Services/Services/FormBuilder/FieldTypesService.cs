using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Models.FormBuilder.API.Models;
using FormBuilder.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FieldTypesService : IFieldTypesService
    {
        private readonly IunitOfwork _unitOfWork;

        public FieldTypesService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // ================================
        // CREATE
        // ================================
        public async Task<ApiResponse> CreateAsync(FieldTypeCreateDto dto)
        {
            if (dto == null)
                return new ApiResponse(400, "DTO is required");

            if (!await _unitOfWork.FieldTypesRepository.IsTypeNameUniqueAsync(dto.TypeName))
                return new ApiResponse(400, $"TypeName '{dto.TypeName}' already exists");

            var entity = ToEntity(dto);

            _unitOfWork.Repositary<FIELD_TYPES>().Add(entity);
            await _unitOfWork.CompleteAsyn();

            return new ApiResponse(200, "FieldType created successfully");
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<ApiResponse> UpdateAsync(FieldTypeUpdateDto dto, int id)
        {
            if (dto == null)
                return new ApiResponse(400, "DTO is required");

            var entity = await _unitOfWork.FieldTypesRepository.GetByIdAsync(id);
            if (entity == null)
                return new ApiResponse(404, "FieldType not found");

            if (!await _unitOfWork.FieldTypesRepository.IsTypeNameUniqueAsync(dto.TypeName, id))
                return new ApiResponse(400, $"TypeName '{dto.TypeName}' already exists");

            MapUpdate(dto, entity);

            _unitOfWork.Repositary<FIELD_TYPES>().Update(entity);
            await _unitOfWork.CompleteAsyn();

            return new ApiResponse(200, "FieldType updated successfully");
        }

        // ================================
        // DELETE (HARD)
        // ================================
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.FieldTypesRepository.GetByIdAsync(id);
            if (entity == null)
                return new ApiResponse(404, "FieldType not found");

            var usageCount = await GetUsageCountAsync(id);
            if (usageCount > 0)
                return new ApiResponse(400, $"FieldType is used {usageCount} times — cannot delete");

            _unitOfWork.Repositary<FIELD_TYPES>().Delete(entity);
            await _unitOfWork.CompleteAsyn();

            return new ApiResponse(200, "FieldType deleted successfully");
        }

        // ================================
        // SOFT DELETE
        // ================================
        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var entity = await _unitOfWork.FieldTypesRepository.GetByIdAsync(id);
            if (entity == null)
                return new ApiResponse(404, "FieldType not found");

            entity.IsActive = false;
            _unitOfWork.Repositary<FIELD_TYPES>().Update(entity);
            await _unitOfWork.CompleteAsyn();

            return new ApiResponse(200, "FieldType soft deleted successfully");
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<FieldTypeDto> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.FieldTypesRepository.GetByIdAsync(id);
            return ToDto(entity);
        }

        // ================================
        // GET ALL
        // ================================
        public async Task<IEnumerable<FieldTypeDto>> GetAllAsync()
        {
            var list = await _unitOfWork.Repositary<FIELD_TYPES>().GetAllAsync();
            return list.Select(ToDto);
        }

        // ================================
        // SPECIAL QUERIES
        // ================================
        public async Task<IEnumerable<FieldTypeDto>> GetActiveAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetActiveFieldTypesAsync();
            return list.Select(ToDto);
        }

        public async Task<FieldTypeDto> GetByTypeNameAsync(string typeName)
        {
            var entity = await _unitOfWork.FieldTypesRepository.GetByTypeNameAsync(typeName);
            return ToDto(entity);
        }

        public async Task<IEnumerable<FieldTypeDto>> GetWithOptionsAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetFieldTypesWithOptionsAsync();
            return list.Select(ToDto);
        }

        public async Task<IEnumerable<FieldTypeDto>> GetByDataTypeAsync(string dataType)
        {
            var list = await _unitOfWork.FieldTypesRepository.GetByDataTypeAsync(dataType);
            return list.Select(ToDto);
        }

        public async Task<IEnumerable<FieldTypeDto>> GetWithMultipleValuesAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetFieldTypesWithMultipleValuesAsync();
            return list.Select(ToDto);
        }

        public async Task<IEnumerable<FieldTypeDropdownDto>> GetForDropdownAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetFieldTypesForDropdownAsync();
            return list.Select(x => new FieldTypeDropdownDto
            {
                Id = x.id,
                TypeName = x.TypeName
            });
        }

        public async Task<IEnumerable<FieldTypeDto>> GetBasicAsync()
        {
            var list = await _unitOfWork.Repositary<FIELD_TYPES>().GetAllAsync(x => !x.HasOptions && !x.AllowMultiple);
            return list.Select(ToDto);
        }

        public async Task<IEnumerable<FieldTypeDto>> GetAdvancedAsync()
        {
            var list = await _unitOfWork.Repositary<FIELD_TYPES>().GetAllAsync(x => x.HasOptions || x.AllowMultiple);
            return list.Select(ToDto);
        }

        // ================================
        // VALIDATION
        // ================================
        public async Task<bool> IsTypeNameUniqueAsync(string typeName, int? ignoreId = null)
        {
            return await _unitOfWork.FieldTypesRepository.IsTypeNameUniqueAsync(typeName, ignoreId);
        }

        // ================================
        // UTILITY
        // ================================
        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.Repositary<FIELD_TYPES>().AnyAsync(x => x.id == id);
        }

        public async Task<int> GetUsageCountAsync(int fieldTypeId)
        {
            var entity = await _unitOfWork.FieldTypesRepository.GetByIdAsync(fieldTypeId, x => x.FORM_FIELDS, x => x.FORM_GRID_COLUMNS);
            if (entity == null) return 0;

            int countFields = entity.FORM_FIELDS?.Count(x => x.IsActive) ?? 0;
            int countGrid = entity.FORM_GRID_COLUMNS?.Count(x => x.IsActive) ?? 0;

            return countFields + countGrid;
        }

        // ================================
        // MAPPING
        // ================================
        private FieldTypeDto ToDto(FIELD_TYPES e)
        {
            if (e == null) return null;

            return new FieldTypeDto
            {
                Id = e.id,
                TypeName = e.TypeName ?? string.Empty,
                DataType = e.DataType ?? string.Empty,
                MaxLength = e.MaxLength,
                HasOptions = e.HasOptions,
                AllowMultiple = e.AllowMultiple,
                IsActive = e.IsActive
            };
        }

        private FIELD_TYPES ToEntity(FieldTypeCreateDto dto)
        {
            return new FIELD_TYPES
            {
                TypeName = dto.TypeName,
                DataType = dto.DataType,
                MaxLength = dto.MaxLength,
                HasOptions = dto.HasOptions,
                AllowMultiple = dto.AllowMultiple,
                IsActive = true,
                CreatedByUserId = null, // يمكن ربطه بالمستخدم الحالي
            };
        }

        private void MapUpdate(FieldTypeUpdateDto dto, FIELD_TYPES e)
        {
            e.TypeName = dto.TypeName;
            e.DataType = dto.DataType;
            e.MaxLength = dto.MaxLength;
            e.HasOptions = dto.HasOptions;
            e.AllowMultiple = dto.AllowMultiple;
            e.IsActive = dto.IsActive;
        }
    }
}
