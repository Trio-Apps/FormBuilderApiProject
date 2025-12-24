using AutoMapper;
using formBuilder.Domian.Interfaces;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Core.DTOS.FormFields;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.API.Models;
using FormBuilder.Services.Services.Base;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CreateFormFieldDto = FormBuilder.Core.DTOS.FormFields.CreateFormFieldDto;
using UpdateFormFieldDto = FormBuilder.API.Models.UpdateFormFieldDto;
using FormBuilder.Domian.Entitys.froms;

namespace FormBuilder.Services.Services
{
    public class FormFieldService : BaseService<FORM_FIELDS, FormFieldDto, CreateFormFieldDto, UpdateFormFieldDto>, IFormFieldService
    {
        private readonly IStringLocalizer<FormFieldService>? _localizer;

        public FormFieldService(IunitOfwork unitOfWork, IMapper mapper, IStringLocalizer<FormFieldService>? localizer = null)
            : base(unitOfWork, mapper, null)
        {
            _localizer = localizer;
        }

        protected override IBaseRepository<FORM_FIELDS> Repository => _unitOfWork.FormFieldRepository;

        // ================================
        // CUSTOM OPERATIONS
        // ================================

        public async Task<ServiceResult<IEnumerable<FormFieldDto>>> GetActiveAsync()
        {
            var entities = await Repository.GetAllAsync(e => e.IsActive);
            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(entities);
            return ServiceResult<IEnumerable<FormFieldDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<IEnumerable<FormFieldDto>>> GetByTabIdAsync(int tabId)
        {
            var entities = await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(entities);
            return ServiceResult<IEnumerable<FormFieldDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<IEnumerable<FormFieldDto>>> GetByFormIdAsync(int formBuilderId)
        {
            var entities = await _unitOfWork.FormFieldRepository.GetFieldsByFormIdAsync(formBuilderId);
            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(entities);
            return ServiceResult<IEnumerable<FormFieldDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<IEnumerable<FormFieldDto>>> GetMandatoryFieldsAsync(int tabId)
        {
            var entities = await _unitOfWork.FormFieldRepository.GetMandatoryFieldsAsync(tabId);
            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(entities);
            return ServiceResult<IEnumerable<FormFieldDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<IEnumerable<FormFieldDto>>> GetVisibleFieldsAsync(int tabId)
        {
            var entities = await _unitOfWork.FormFieldRepository.GetVisibleFieldsAsync(tabId);
            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(entities);
            return ServiceResult<IEnumerable<FormFieldDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<FormFieldDto>> GetByFieldCodeAsync(string fieldCode)
        {
            if (string.IsNullOrWhiteSpace(fieldCode))
            {
                var message = _localizer?["FormField_FieldCodeRequired"] ?? "Field code is required";
                return ServiceResult<FormFieldDto>.BadRequest(message);
            }

            var entities = await Repository.GetAllAsync(e => e.FieldCode == fieldCode && e.IsActive);
            var entity = entities.FirstOrDefault();
            
            if (entity == null)
                return ServiceResult<FormFieldDto>.NotFound();

            var dto = _mapper.Map<FormFieldDto>(entity);
            return ServiceResult<FormFieldDto>.Ok(dto);
        }

        public async Task<ServiceResult<IEnumerable<FormFieldDto>>> GetEditableFieldsAsync(int tabId)
        {
            var entities = await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
            var editableEntities = entities.Where(e => e.IsEditable ?? false);
            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(editableEntities);
            return ServiceResult<IEnumerable<FormFieldDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<IEnumerable<FormFieldDto>>> GetFieldsByGridIdAsync(int gridId)
        {
            var entities = await _unitOfWork.FormFieldRepository.GetFieldsByGridIdAsync(gridId);
            var dtos = _mapper.Map<IEnumerable<FormFieldDto>>(entities);
            return ServiceResult<IEnumerable<FormFieldDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<bool>> SoftDeleteAsync(int id)
        {
            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return ServiceResult<bool>.NotFound();

            entity.IsActive = false;
            entity.UpdatedDate = DateTime.UtcNow;
            Repository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }

        // ================================
        // VALIDATION
        // ================================

        public async Task<bool> IsFieldCodeUniqueAsync(string fieldCode, int? ignoreId = null)
        {
            return await _unitOfWork.FormFieldRepository.IsFieldCodeUniqueAsync(fieldCode, ignoreId);
        }

        public async Task<bool> IsFieldNameUniqueAsync(string fieldName, int? ignoreId = null, int? tabId = null)
        {
            return await _unitOfWork.FormFieldRepository.IsFieldNameUniqueAsync(fieldName, ignoreId, tabId);
        }

        // ================================
        // UTILITY
        // ================================

        public async Task<bool> ExistsAsync(int id)
        {
            return await Repository.AnyAsync(e => e.Id == id);
        }

        public async Task<ServiceResult<int>> GetUsageCountAsync(int fieldId)
        {
            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == fieldId);
            if (entity == null)
                return ServiceResult<int>.NotFound();

            // Add logic to check usage in related entities
            // For example: FORM_SUBMISSION_VALUES, FORMULA_VARIABLES, etc.
            int count = 0;

            // Example: count form submission values
            // count += entity.FORM_SUBMISSION_VALUES?.Count(x => x.IsActive) ?? 0;

            return ServiceResult<int>.Ok(count);
        }

        public async Task<ServiceResult<int>> GetFieldsCountByTabAsync(int tabId)
        {
            var fields = await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
            return ServiceResult<int>.Ok(fields.Count());
        }

        public async Task<ServiceResult<int>> GetFieldsCountByFormAsync(int formBuilderId)
        {
            var fields = await _unitOfWork.FormFieldRepository.GetFieldsByFormIdAsync(formBuilderId);
            return ServiceResult<int>.Ok(fields.Count());
        }

        // ================================
        // CREATE OVERRIDE - Support optional FieldOptions
        // ================================

        public override async Task<ServiceResult<FormFieldDto>> CreateAsync(CreateFormFieldDto dto)
        {
            if (dto == null)
            {
                var message = _localizer?["Common_PayloadRequired"] ?? "Payload is required";
                return ServiceResult<FormFieldDto>.BadRequest(message);
            }

            var validation = await ValidateCreateAsync(dto);
            if (!validation.IsValid)
            {
                var message = validation.ErrorMessage ?? (_localizer?["Common_ValidationFailed"] ?? "Validation failed");
                return ServiceResult<FormFieldDto>.BadRequest(message);
            }

            var entity = _mapper.Map<FORM_FIELDS>(dto);
            entity.CreatedDate = entity.CreatedDate == default ? DateTime.UtcNow : entity.CreatedDate;
            entity.IsActive = true;

            Repository.Add(entity);
            await _unitOfWork.CompleteAsyn();

            // Create field options if provided (optional)
            if (dto.FieldOptions != null && dto.FieldOptions.Any())
            {
                var fieldOptions = dto.FieldOptions.Select((option, index) => new FIELD_OPTIONS
                {
                    FieldId = entity.Id,
                    OptionText = option.OptionText,
                    OptionValue = option.OptionValue ?? option.OptionText,
                    OptionOrder = option.SortOrder != 0 ? option.SortOrder : index + 1,
                    IsDefault = option.IsDefault,
                    IsActive = option.IsActive,
                    CreatedDate = DateTime.UtcNow
                }).ToList();

                foreach (var option in fieldOptions)
                {
                    _unitOfWork.FieldOptionsRepository.Add(option);
                }

                await _unitOfWork.CompleteAsyn();
            }

            return ServiceResult<FormFieldDto>.Ok(_mapper.Map<FormFieldDto>(entity));
        }

        // ================================
        // VALIDATION OVERRIDES
        // ================================

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFormFieldDto dto)
        {
            // Validate field code uniqueness
            if (!await _unitOfWork.FormFieldRepository.IsFieldCodeUniqueAsync(dto.FieldCode))
            {
                var message = _localizer?["FormField_FieldCodeExists", dto.FieldCode] ?? $"Field code '{dto.FieldCode}' already exists";
                return ValidationResult.Failure(message);
            }

            // Validate field name uniqueness within tab
            if (!await _unitOfWork.FormFieldRepository.IsFieldNameUniqueAsync(dto.FieldName, null, dto.TabId))
            {
                var message = _localizer?["FormField_FieldNameExists", dto.FieldName] ?? $"Field name '{dto.FieldName}' already exists in this tab";
                return ValidationResult.Failure(message);
            }

            // Validate GridId if provided (for Grid field type)
            if (dto.GridId.HasValue)
            {
                var fieldType = await _unitOfWork.FieldTypesRepository.GetByIdAsync(dto.FieldTypeId);
                if (fieldType?.TypeName?.ToLower() == "grid")
                {
                    // التحقق من وجود Grid
                    var grid = await _unitOfWork.FormGridRepository.GetByIdAsync(dto.GridId.Value);
                    if (grid == null)
                    {
                        var message = _localizer?["FormField_GridNotFound"] ?? "Grid not found";
                        return ValidationResult.Failure(message);
                    }
                    
                    // التحقق من أن Grid ينتمي لنفس Tab
                    if (grid.TabId.HasValue && grid.TabId != dto.TabId)
                    {
                        var message = _localizer?["FormField_GridNotInTab"] ?? "Grid does not belong to this tab";
                        return ValidationResult.Failure(message);
                    }
                }
                else
                {
                    var message = _localizer?["FormField_GridIdNotAllowed"] ?? "GridId can only be set for Grid field type";
                    return ValidationResult.Failure(message);
                }
            }

            // FieldOptions are optional - no validation needed
            // If provided, they will be created after the field is created

            return ValidationResult.Success();
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateFormFieldDto dto, FORM_FIELDS entity)
        {
            // Validate field code uniqueness (ignore current field)
            if (!await _unitOfWork.FormFieldRepository.IsFieldCodeUniqueAsync(dto.FieldCode, id))
            {
                var message = _localizer?["FormField_FieldCodeExists", dto.FieldCode] ?? $"Field code '{dto.FieldCode}' already exists";
                return ValidationResult.Failure(message);
            }

            // Validate field name uniqueness within tab (ignore current field)
            if (!await _unitOfWork.FormFieldRepository.IsFieldNameUniqueAsync(dto.FieldName, id, dto.TabId))
            {
                var message = _localizer?["FormField_FieldNameExists", dto.FieldName] ?? $"Field name '{dto.FieldName}' already exists in this tab";
                return ValidationResult.Failure(message);
            }

            // Validate GridId if provided (for Grid field type)
            if (dto.GridId.HasValue)
            {
                var fieldType = await _unitOfWork.FieldTypesRepository.GetByIdAsync(dto.FieldTypeId);
                if (fieldType?.TypeName?.ToLower() == "grid")
                {
                    // التحقق من وجود Grid
                    var grid = await _unitOfWork.FormGridRepository.GetByIdAsync(dto.GridId.Value);
                    if (grid == null)
                    {
                        var message = _localizer?["FormField_GridNotFound"] ?? "Grid not found";
                        return ValidationResult.Failure(message);
                    }
                    
                    // التحقق من أن Grid ينتمي لنفس Tab
                    if (grid.TabId.HasValue && grid.TabId != dto.TabId)
                    {
                        var message = _localizer?["FormField_GridNotInTab"] ?? "Grid does not belong to this tab";
                        return ValidationResult.Failure(message);
                    }
                }
                else
                {
                    var message = _localizer?["FormField_GridIdNotAllowed"] ?? "GridId can only be set for Grid field type";
                    return ValidationResult.Failure(message);
                }
            }

            return ValidationResult.Success();
        }

        public override async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return ServiceResult<bool>.NotFound();

            var usageCountResult = await GetUsageCountAsync(id);
            if (usageCountResult.Success && usageCountResult.Data > 0)
            {
                var message = _localizer?["FormField_CannotDeleteUsed", usageCountResult.Data] ?? $"Form field is used {usageCountResult.Data} times — cannot delete";
                return ServiceResult<bool>.BadRequest(message);
            }

            Repository.Delete(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }
    }
}
