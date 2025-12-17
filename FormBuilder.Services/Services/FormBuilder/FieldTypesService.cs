using AutoMapper;
using FormBuilder.API.Models;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces;
using formBuilder.Domian.Interfaces;
using FormBuilder.Services.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FieldTypesService
        : BaseService<FIELD_TYPES, FieldTypeDto, FieldTypeCreateDto, FieldTypeUpdateDto>,
          IFieldTypesService
    {
        public FieldTypesService(IunitOfwork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        protected override IBaseRepository<FIELD_TYPES> Repository => _unitOfWork.FieldTypesRepository;

        public async Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetAllAsync(Expression<Func<FIELD_TYPES, bool>>? filter = null)
            => await base.GetAllAsync(filter);

        public async Task<ServiceResult<PagedResult<FieldTypeDto>>> GetPagedAsync(int page = 1, int pageSize = 20, Expression<Func<FIELD_TYPES, bool>>? filter = null)
            => await base.GetPagedAsync(page, pageSize, filter);

        public async Task<ServiceResult<FieldTypeDto>> GetByIdAsync(int id, bool asNoTracking = false)
            => await base.GetByIdAsync(id, asNoTracking);

        public override async Task<ServiceResult<FieldTypeDto>> CreateAsync(FieldTypeCreateDto dto)
        {
            return await base.CreateAsync(dto);
        }

        public override async Task<ServiceResult<FieldTypeDto>> UpdateAsync(int id, FieldTypeUpdateDto dto)
        {
            return await base.UpdateAsync(id, dto);
        }

        public override async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var usageCount = await GetUsageCountAsync(id);
            if (!usageCount.Success) return ServiceResult<bool>.BadRequest(usageCount.ErrorMessage ?? "Usage check failed");
            if (usageCount.Data > 0) return ServiceResult<bool>.BadRequest($"FieldType is used {usageCount.Data} times — cannot delete");

            return await base.DeleteAsync(id);
        }

        public async Task<ServiceResult<bool>> SoftDeleteAsync(int id)
        {
            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null) return ServiceResult<bool>.NotFound();

            entity.IsActive = false;
            entity.UpdatedDate = DateTime.UtcNow;
            Repository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetActiveAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetActiveFieldTypesAsync();
            var mapped = _mapper.Map<IEnumerable<FieldTypeDto>>(list);
            return ServiceResult<IEnumerable<FieldTypeDto>>.Ok(mapped);
        }

        public async Task<ServiceResult<FieldTypeDto>> GetByTypeNameAsync(string typeName, bool asNoTracking = false)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return ServiceResult<FieldTypeDto>.BadRequest("TypeName is required");

            var entity = await _unitOfWork.FieldTypesRepository.GetByTypeNameAsync(typeName.Trim());
            if (entity == null) return ServiceResult<FieldTypeDto>.NotFound();

            return ServiceResult<FieldTypeDto>.Ok(_mapper.Map<FieldTypeDto>(entity));
        }

        public async Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetWithOptionsAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetFieldTypesWithOptionsAsync();
            return ServiceResult<IEnumerable<FieldTypeDto>>.Ok(_mapper.Map<IEnumerable<FieldTypeDto>>(list));
        }

        public async Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetByDataTypeAsync(string dataType)
        {
            if (string.IsNullOrWhiteSpace(dataType))
                return ServiceResult<IEnumerable<FieldTypeDto>>.BadRequest("DataType is required");

            var list = await _unitOfWork.FieldTypesRepository.GetByDataTypeAsync(dataType.Trim());
            return ServiceResult<IEnumerable<FieldTypeDto>>.Ok(_mapper.Map<IEnumerable<FieldTypeDto>>(list));
        }

        public async Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetWithMultipleValuesAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetFieldTypesWithMultipleValuesAsync();
            return ServiceResult<IEnumerable<FieldTypeDto>>.Ok(_mapper.Map<IEnumerable<FieldTypeDto>>(list));
        }

        public async Task<ServiceResult<IEnumerable<FieldTypeDropdownDto>>> GetForDropdownAsync()
        {
            var list = await _unitOfWork.FieldTypesRepository.GetFieldTypesForDropdownAsync();
            return ServiceResult<IEnumerable<FieldTypeDropdownDto>>.Ok(_mapper.Map<IEnumerable<FieldTypeDropdownDto>>(list));
        }

        public async Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetBasicAsync()
        {
            var list = await Repository.GetAllAsync(x => (!x.HasOptions.HasValue || !x.HasOptions.Value) && (!x.AllowMultiple.HasValue || !x.AllowMultiple.Value));
            return ServiceResult<IEnumerable<FieldTypeDto>>.Ok(_mapper.Map<IEnumerable<FieldTypeDto>>(list));
        }

        public async Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetAdvancedAsync()
        {
            var list = await Repository.GetAllAsync(x => (x.HasOptions.HasValue && x.HasOptions.Value) || (x.AllowMultiple.HasValue && x.AllowMultiple.Value));
            return ServiceResult<IEnumerable<FieldTypeDto>>.Ok(_mapper.Map<IEnumerable<FieldTypeDto>>(list));
        }

        public async Task<ServiceResult<bool>> IsTypeNameUniqueAsync(string typeName, int? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return ServiceResult<bool>.BadRequest("TypeName is required");

            var unique = await _unitOfWork.FieldTypesRepository.IsTypeNameUniqueAsync(typeName.Trim(), ignoreId);
            return ServiceResult<bool>.Ok(unique);
        }

        public async Task<ServiceResult<bool>> ExistsAsync(int id)
        {
            var exists = await Repository.AnyAsync(x => x.Id == id);
            return ServiceResult<bool>.Ok(exists);
        }

        public async Task<ServiceResult<int>> GetUsageCountAsync(int fieldTypeId)
        {
            var entity = await _unitOfWork.FieldTypesRepository.GetByIdAsync(fieldTypeId, x => x.FORM_FIELDS, x => x.FORM_GRID_COLUMNS);
            if (entity == null) return ServiceResult<int>.NotFound();

            int countFields = entity.FORM_FIELDS?.Count(x => x.IsActive) ?? 0;
            int countGrid = entity.FORM_GRID_COLUMNS?.Count(x => x.IsActive) ?? 0;

            return ServiceResult<int>.Ok(countFields + countGrid);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(FieldTypeCreateDto dto)
        {
            if (dto == null) return ValidationResult.Failure("Payload is required");

            var unique = await _unitOfWork.FieldTypesRepository.IsTypeNameUniqueAsync(dto.TypeName);
            if (!unique) return ValidationResult.Failure($"TypeName '{dto.TypeName}' already exists");

            return ValidationResult.Success();
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, FieldTypeUpdateDto dto, FIELD_TYPES entity)
        {
            if (dto == null) return ValidationResult.Failure("Payload is required");

            if (!string.Equals(dto.TypeName, entity.TypeName, StringComparison.OrdinalIgnoreCase))
            {
                var unique = await _unitOfWork.FieldTypesRepository.IsTypeNameUniqueAsync(dto.TypeName, id);
                if (!unique) return ValidationResult.Failure($"TypeName '{dto.TypeName}' already exists");
            }

            return ValidationResult.Success();
        }
    }
}
