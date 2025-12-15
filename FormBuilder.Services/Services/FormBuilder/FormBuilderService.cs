using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using formBuilder.Domian.Interfaces;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Services.Services.Base;

namespace FormBuilder.Services.Services
{
    public class FormBuilderService
        : BaseService<FORM_BUILDER, FormBuilderDto, CreateFormBuilderDto, UpdateFormBuilderDto>,
          IFormBuilderService
    {
        public FormBuilderService(IunitOfwork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        protected override IBaseRepository<FORM_BUILDER> Repository => _unitOfWork.FormBuilderRepository;

        public async Task<ServiceResult<FormBuilderDto>> GetByCodeAsync(string formCode, bool asNoTracking = false)
        {
            if (string.IsNullOrWhiteSpace(formCode))
                return ServiceResult<FormBuilderDto>.BadRequest("Form code is required");

            var entity = await Repository.SingleOrDefaultAsync(f => f.FormCode == formCode.Trim(), asNoTracking);
            if (entity == null) return ServiceResult<FormBuilderDto>.NotFound();

            return ServiceResult<FormBuilderDto>.Ok(_mapper.Map<FormBuilderDto>(entity));
        }

        public async Task<ServiceResult<IEnumerable<FormBuilderDto>>> GetAllAsync(Expression<Func<FORM_BUILDER, bool>>? filter = null)
        {
            return await base.GetAllAsync(filter);
        }

        public async Task<ServiceResult<PagedResult<FormBuilderDto>>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            return await base.GetPagedAsync(page, pageSize);
        }

        public async Task<ServiceResult<bool>> IsFormCodeExistsAsync(string formCode, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(formCode))
                return ServiceResult<bool>.BadRequest("Form code is required");

            var exists = await _unitOfWork.FormBuilderRepository.IsFormCodeExistsAsync(formCode.Trim(), excludeId);
            return ServiceResult<bool>.Ok(exists);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFormBuilderDto dto)
        {
            var exists = await _unitOfWork.FormBuilderRepository.IsFormCodeExistsAsync(dto.FormCode);
            if (exists)
            {
                return ValidationResult.Failure($"Form code '{dto.FormCode}' already exists.");
            }

            return ValidationResult.Success();
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateFormBuilderDto dto, FORM_BUILDER entity)
        {
            var exists = await _unitOfWork.FormBuilderRepository.IsFormCodeExistsAsync(dto.FormCode, id);
            if (exists)
            {
                return ValidationResult.Failure($"Form code '{dto.FormCode}' already exists.");
            }

            return ValidationResult.Success();
        }
    }
}