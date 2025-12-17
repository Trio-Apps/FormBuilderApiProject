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
using FormBuilder.Core.DTOS.FormTabs;
using FormBuilder.API.Models;
using System.Linq;

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

            // Load the form with its tabs and fields for public/anonymous usage
            var entity = await _unitOfWork.FormBuilderRepository.GetFormWithTabsAndFieldsByCodeAsync(formCode.Trim());
            if (entity == null) return ServiceResult<FormBuilderDto>.NotFound();

            // Map the basic form data
            var dto = _mapper.Map<FormBuilderDto>(entity);

            // Manually map tabs and fields for the public form view
            dto.Tabs = entity.FORM_TABS
                .Where(t => t.IsActive)
                .OrderBy(t => t.TabOrder)
                .Select(t => new FormTabDto
                {
                    Id = t.Id,
                    FormBuilderId = t.FormBuilderId,
                    TabName = t.TabName,
                    TabCode = t.TabCode,
                    TabOrder = t.TabOrder,
                    IsActive = t.IsActive,
                    CreatedByUserId = t.CreatedByUserId,
                    CreatedDate = t.CreatedDate,
                    Fields = t.FORM_FIELDS
                        .Where(f => f.IsActive)
                        .OrderBy(f => f.FieldOrder)
                        .Select(f => new FormFieldDto
                        {
                            Id = f.Id,
                            TabId = f.TabId,
                            FieldTypeId = f.FieldTypeId,
                            FieldTypeName = f.FIELD_TYPES?.TypeName,
                            FieldName = f.FieldName,
                            FieldCode = f.FieldCode,
                            FieldOrder = f.FieldOrder,
                            Placeholder = f.Placeholder,
                            HintText = f.HintText,
                            IsMandatory = f.IsMandatory ?? false,
                            IsEditable = f.IsEditable ?? false,
                            IsVisible = f.IsVisible,
                            DefaultValueJson = f.DefaultValueJson,
                            MinValue = f.MinValue,
                            MaxValue = f.MaxValue,
                            RegexPattern = f.RegexPattern,
                            ValidationMessage = f.ValidationMessage,
                          
                            CreatedDate = f.CreatedDate,
                            CreatedByUserId = f.CreatedByUserId,
                            IsActive = f.IsActive,
                            // Map FieldType
                            FieldType = f.FIELD_TYPES != null ? new FieldTypeDto
                            {
                                Id = f.FIELD_TYPES.Id,
                                TypeName = f.FIELD_TYPES.TypeName,
                                DataType = f.FIELD_TYPES.DataType,
                                MaxLength = f.FIELD_TYPES.MaxLength,
                                HasOptions = f.FIELD_TYPES.HasOptions,
                                AllowMultiple = f.FIELD_TYPES.AllowMultiple,
                                IsActive = f.FIELD_TYPES.IsActive
                            } : null,
                            // For public view we only need basic option data
                            FieldOptions = f.FIELD_OPTIONS?
                                .Where(fo => fo.IsActive)
                                .Select(fo => new FieldOptionDto
                                {
                                    Id = fo.Id,
                                    FieldId = fo.FieldId,
                                    OptionText = fo.OptionText,
                                    OptionValue = fo.OptionValue,
                                    OptionOrder = fo.OptionOrder,
                                    IsActive = fo.IsActive
                                }).ToList() ?? new System.Collections.Generic.List<FieldOptionDto>()
                        }).ToList()
                })
                .ToList();

            return ServiceResult<FormBuilderDto>.Ok(dto);
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