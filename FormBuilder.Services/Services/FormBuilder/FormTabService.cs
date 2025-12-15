using AutoMapper;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Core.DTOS.FormTabs;
using FormBuilder.Domian.Entitys.FormBuilder;
using formBuilder.Domian.Interfaces;
using FormBuilder.Services.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FormTabService
        : BaseService<FORM_TABS, FormTabDto, CreateFormTabDto, UpdateFormTabDto>,
          IFormTabService
    {
        public FormTabService(IunitOfwork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        protected override IBaseRepository<FORM_TABS> Repository => _unitOfWork.FormTabRepository;

        public async Task<ServiceResult<IEnumerable<FormTabDto>>> GetAllAsync(Expression<Func<FORM_TABS, bool>>? filter = null)
            => await base.GetAllAsync(filter);

        public async Task<ServiceResult<PagedResult<FormTabDto>>> GetPagedAsync(int page = 1, int pageSize = 20, Expression<Func<FORM_TABS, bool>>? filter = null)
            => await base.GetPagedAsync(page, pageSize, filter);

        public async Task<ServiceResult<FormTabDto>> GetByIdAsync(int id, bool asNoTracking = false)
            => await base.GetByIdAsync(id, asNoTracking);

        public async Task<ServiceResult<FormTabDto>> GetByCodeAsync(string tabCode, bool asNoTracking = false)
        {
            if (string.IsNullOrWhiteSpace(tabCode))
                return ServiceResult<FormTabDto>.BadRequest("Tab code is required");

            var entity = await Repository.SingleOrDefaultAsync(t => t.TabCode == tabCode.Trim(), asNoTracking);
            if (entity == null) return ServiceResult<FormTabDto>.NotFound();

            return ServiceResult<FormTabDto>.Ok(_mapper.Map<FormTabDto>(entity));
        }

        public async Task<ServiceResult<IEnumerable<FormTabDto>>> GetByFormIdAsync(int formBuilderId)
        {
            var tabs = await _unitOfWork.FormTabRepository.GetTabsByFormIdAsync(formBuilderId);
            var dtos = _mapper.Map<IEnumerable<FormTabDto>>(tabs);
            return ServiceResult<IEnumerable<FormTabDto>>.Ok(dtos);
        }

        public override async Task<ServiceResult<FormTabDto>> CreateAsync(CreateFormTabDto dto)
        {
            return await base.CreateAsync(dto);
        }

        public override async Task<ServiceResult<FormTabDto>> UpdateAsync(int id, UpdateFormTabDto dto)
        {
            return await base.UpdateAsync(id, dto);
        }

        public override async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }

        public async Task<ServiceResult<bool>> ToggleActiveAsync(int id, bool isActive)
        {
            var entity = await Repository.SingleOrDefaultAsync(t => t.Id == id);
            if (entity == null) return ServiceResult<bool>.NotFound();

            entity.IsActive = isActive;
            entity.UpdatedDate = DateTime.UtcNow;
            Repository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ExistsAsync(int id)
        {
            var exists = await Repository.AnyAsync(t => t.Id == id);
            return ServiceResult<bool>.Ok(exists);
        }

        public async Task<ServiceResult<bool>> CodeExistsAsync(string tabCode, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(tabCode))
                return ServiceResult<bool>.BadRequest("Tab code is required");

            var isUnique = await _unitOfWork.FormTabRepository.IsTabCodeUniqueAsync(tabCode.Trim(), excludeId);
            // IsTabCodeUniqueAsync returns true if unique (doesn't exist), so exists = !isUnique
            return ServiceResult<bool>.Ok(!isUnique);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFormTabDto dto)
        {
            if (dto == null) return ValidationResult.Failure("Payload is required");

            // Validate FormBuilder exists
            var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(f => f.Id == dto.FormBuilderId);
            if (!formBuilderExists)
                return ValidationResult.Failure($"FormBuilder with ID '{dto.FormBuilderId}' does not exist.");

            // Validate TabCode uniqueness
            var isUnique = await _unitOfWork.FormTabRepository.IsTabCodeUniqueAsync(dto.TabCode);
            if (!isUnique)
                return ValidationResult.Failure($"Tab code '{dto.TabCode}' already exists.");

            return ValidationResult.Success();
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateFormTabDto dto, FORM_TABS entity)
        {
            if (dto == null) return ValidationResult.Failure("Payload is required");

            // Validate TabCode uniqueness (excluding current tab)
            if (!string.IsNullOrWhiteSpace(dto.TabCode) && !string.Equals(dto.TabCode, entity.TabCode, StringComparison.OrdinalIgnoreCase))
            {
                var isUnique = await _unitOfWork.FormTabRepository.IsTabCodeUniqueAsync(dto.TabCode, id);
                if (!isUnique)
                    return ValidationResult.Failure($"Tab code '{dto.TabCode}' already exists.");
            }

            return ValidationResult.Success();
        }
    }
}
