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
using Microsoft.Extensions.Localization;
using System.Linq;

namespace FormBuilder.Services.Services
{
    public class FormBuilderService
        : BaseService<FORM_BUILDER, FormBuilderDto, CreateFormBuilderDto, UpdateFormBuilderDto>,
          IFormBuilderService
    {
        private readonly IStringLocalizer<FormBuilderService>? _localizer;

        public FormBuilderService(IunitOfwork unitOfWork, IMapper mapper, IStringLocalizer<FormBuilderService>? localizer = null)
            : base(unitOfWork, mapper, null)
        {
            _localizer = localizer;
        }

        protected override IBaseRepository<FORM_BUILDER> Repository => _unitOfWork.FormBuilderRepository;

        public async Task<ServiceResult<FormBuilderDto>> GetByCodeAsync(string formCode, bool asNoTracking = false)
        {
            if (string.IsNullOrWhiteSpace(formCode))
            {
                var message = _localizer?["FormBuilder_FormCodeRequired"] ?? "Form code is required";
                return ServiceResult<FormBuilderDto>.BadRequest(message);
            }

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
                    ForeignTabName = t.ForeignTabName,
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
                            FieldTypeName = null,
                            FieldName = f.FieldName,
                            ForeignFieldName = f.ForeignFieldName,
                            FieldCode = f.FieldCode,
                            FieldOrder = f.FieldOrder,
                            Placeholder = f.Placeholder,
                            ForeignPlaceholder = f.ForeignPlaceholder,
                            HintText = f.HintText,
                            ForeignHintText = f.ForeignHintText,
                            IsMandatory = f.IsMandatory ?? false,
                            IsEditable = f.IsEditable ?? false,
                            IsVisible = f.IsVisible,
                            DefaultValueJson = f.DefaultValueJson,
                            MinValue = f.MinValue,
                            MaxValue = f.MaxValue,
                            RegexPattern = f.RegexPattern,
                            ValidationMessage = f.ValidationMessage,
                            ForeignValidationMessage = f.ForeignValidationMessage,
                            CreatedDate = f.CreatedDate,
                            CreatedByUserId = f.CreatedByUserId,
                            IsActive = f.IsActive,
                            // FieldType removed
                            // For public view we only need basic option data
                            FieldOptions = f.FIELD_OPTIONS?
                                .Where(fo => fo.IsActive)
                                .Select(fo => new FieldOptionDto
                                {
                                    Id = fo.Id,
                                    FieldId = fo.FieldId,
                                    OptionText = fo.OptionText,
                                    ForeignOptionText = fo.ForeignOptionText,
                                    OptionValue = fo.OptionValue,
                                    OptionOrder = fo.OptionOrder,
                                    IsActive = fo.IsActive
                                }).ToList() ?? new System.Collections.Generic.List<FieldOptionDto>(),
                            // Map Field Data Source - tells frontend where to load options from
                            FieldDataSource = f.FIELD_DATA_SOURCES?
                                .Where(fds => fds.IsActive)
                                .Select(fds => new FieldDataSourceDto
                                {
                                    Id = fds.Id,
                                    FieldId = fds.FieldId,
                                    SourceType = fds.SourceType,
                                    ApiUrl = fds.ApiUrl,
                                    ApiPath = fds.ApiPath,
                                    HttpMethod = fds.HttpMethod,
                                    RequestBodyJson = fds.RequestBodyJson,
                                    ValuePath = fds.ValuePath,
                                    TextPath = fds.TextPath,
                                    ConfigurationJson = fds.ConfigurationJson,
                                    IsActive = fds.IsActive
                                }).FirstOrDefault()
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
            {
                var message = _localizer?["FormBuilder_FormCodeRequired"] ?? "Form code is required";
                return ServiceResult<bool>.BadRequest(message);
            }

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