using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormTabs.FormBuilder.Core.DTOS.FormTabs;
using FormBuilder.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FormFieldService : IFormFieldService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly ILogger<FormFieldService> _logger;

        public FormFieldService(IunitOfwork unitOfWork, ILogger<FormFieldService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        // ================================
        // CREATE
        // ================================
        public async Task<ApiResponse> CreateAsync(CreateFormFieldDto dto)
        {
            if (dto == null)
                return new ApiResponse(400, "DTO is required");

            try
            {
                // Validate field code uniqueness
                if (!await _unitOfWork.FormFieldRepository.IsFieldCodeUniqueAsync(dto.FieldCode))
                    return new ApiResponse(400, $"Field code '{dto.FieldCode}' already exists");

                // Validate field name uniqueness within tab
                if (!await _unitOfWork.FormFieldRepository.IsFieldNameUniqueAsync(dto.FieldName, null, dto.TabId))
                    return new ApiResponse(400, $"Field name '{dto.FieldName}' already exists in this tab");

                var entity = ToEntity(dto);

                _unitOfWork.Repositary<FORM_FIELDS>().Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form field created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating form field");
                return new ApiResponse(500, "An error occurred while creating the form field");
            }
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<ApiResponse> UpdateAsync(UpdateFormFieldDto dto, int id)
        {
            if (dto == null)
                return new ApiResponse(400, "DTO is required");

            try
            {
                var entity = await _unitOfWork.FormFieldRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form field not found");

                // Validate field code uniqueness (ignore current field)
                if (!await _unitOfWork.FormFieldRepository.IsFieldCodeUniqueAsync(dto.FieldCode, id))
                    return new ApiResponse(400, $"Field code '{dto.FieldCode}' already exists");

                // Validate field name uniqueness within tab (ignore current field)
                if (!await _unitOfWork.FormFieldRepository.IsFieldNameUniqueAsync(dto.FieldName, id, dto.TabId))
                    return new ApiResponse(400, $"Field name '{dto.FieldName}' already exists in this tab");

                MapUpdate(dto, entity);

                _unitOfWork.Repositary<FORM_FIELDS>().Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form field updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating form field with ID {FormFieldId}", id);
                return new ApiResponse(500, "An error occurred while updating the form field");
            }
        }

        // ================================
        // DELETE (HARD)
        // ================================
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormFieldRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form field not found");

                var usageCount = await GetUsageCountAsync(id);
                if (usageCount > 0)
                    return new ApiResponse(400, $"Form field is used {usageCount} times — cannot delete");

                _unitOfWork.Repositary<FORM_FIELDS>().Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form field deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting form field with ID {FormFieldId}", id);
                return new ApiResponse(500, "An error occurred while deleting the form field");
            }
        }

        // ================================
        // SOFT DELETE
        // ================================
        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormFieldRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form field not found");

                entity.IsActive = false;
                _unitOfWork.Repositary<FORM_FIELDS>().Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form field soft deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while soft deleting form field with ID {FormFieldId}", id);
                return new ApiResponse(500, "An error occurred while soft deleting the form field");
            }
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<FormFieldDto> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.FormFieldRepository.GetByIdAsync(id);
            return ToDto(entity);
        }

        // ================================
        // GET ALL
        // ================================
        public async Task<IEnumerable<FormFieldDto>> GetAllAsync()
        {
            var list = await _unitOfWork.FormFieldRepository.GetAllAsync();
            return list.Select(ToDto);
        }

        // ================================
        // SPECIAL QUERIES
        // ================================
        public async Task<IEnumerable<FormFieldDto>> GetActiveAsync()
        {
            var list = await _unitOfWork.FormFieldRepository.GetAllAsync();
            return list.Where(x => x.IsActive).Select(ToDto);
        }

        public async Task<IEnumerable<FormFieldDto>> GetByTabIdAsync(int tabId)
        {
            var list = await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
            return list.Select(ToDto);
        }

        public async Task<IEnumerable<FormFieldDto>> GetByFormIdAsync(int formBuilderId)
        {
            var list = await _unitOfWork.FormFieldRepository.GetFieldsByFormIdAsync(formBuilderId);
            return list.Select(ToDto);
        }

        public async Task<IEnumerable<FormFieldDto>> GetMandatoryFieldsAsync(int tabId)
        {
            var list = await _unitOfWork.FormFieldRepository.GetMandatoryFieldsAsync(tabId);
            return list.Select(ToDto);
        }

        public async Task<IEnumerable<FormFieldDto>> GetVisibleFieldsAsync(int tabId)
        {
            var list = await _unitOfWork.FormFieldRepository.GetVisibleFieldsAsync(tabId);
            return list.Select(ToDto);
        }

        public async Task<FormFieldDto> GetByFieldCodeAsync(string fieldCode)
        {
            var list = await _unitOfWork.Repositary<FORM_FIELDS>().GetAllAsync(x => x.FieldCode == fieldCode && x.IsActive);
            var entity = list.FirstOrDefault();
            return ToDto(entity);
        }

        public async Task<IEnumerable<FormFieldDropdownDto>> GetForDropdownAsync(int tabId)
        {
            var list = await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
            return list.Select(x => new FormFieldDropdownDto
            {
                Id = x.id,
                FieldName = x.FieldName,
                FieldCode = x.FieldCode
            });
        }

        public async Task<IEnumerable<FormFieldDto>> GetEditableFieldsAsync(int tabId)
        {
            var list = await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
            return list.Where(x => x.IsEditable).Select(ToDto);
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
            return await _unitOfWork.Repositary<FORM_FIELDS>().AnyAsync(x => x.id == id);
        }

        public async Task<int> GetUsageCountAsync(int fieldId)
        {
            // Check if field is used in form submissions, formulas, etc.
            var entity = await _unitOfWork.FormFieldRepository.GetByIdAsync(fieldId);
            if (entity == null) return 0;

            // Add logic to check usage in related entities
            // For example: FORM_SUBMISSION_VALUES, FORMULA_VARIABLES, etc.
            int count = 0;

            // Example: count form submission values
            // count += entity.FORM_SUBMISSION_VALUES?.Count(x => x.IsActive) ?? 0;

            return count;
        }

        public async Task<int> GetFieldsCountByTabAsync(int tabId)
        {
            var fields = await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
            return fields.Count();
        }

        public async Task<int> GetFieldsCountByFormAsync(int formBuilderId)
        {
            var fields = await _unitOfWork.FormFieldRepository.GetFieldsByFormIdAsync(formBuilderId);
            return fields.Count();
        }

        // ================================
        // MAPPING
        // ================================
        private FormFieldDto ToDto(FORM_FIELDS e)
        {
            if (e == null) return null;

            return new FormFieldDto
            {
                Id = e.id,
                TabId = e.TabId,
                FieldTypeId = e.FieldTypeId,
                FieldTypeName = e.FIELD_TYPES?.TypeName,
                FieldName = e.FieldName,
                FieldCode = e.FieldCode,
                FieldOrder = e.FieldOrder,
                Placeholder = e.Placeholder,
                HintText = e.HintText,
                IsMandatory = e.IsMandatory,
                IsEditable = e.IsEditable,
                IsVisible = e.IsVisible,
                DefaultValueJson = e.DefaultValueJson,
                DataType = e.DataType,
                MaxLength = e.MaxLength,
                MinValue = e.MinValue,
                MaxValue = e.MaxValue,
                RegexPattern = e.RegexPattern,
                ValidationMessage = e.ValidationMessage,
                VisibilityRuleJson = e.VisibilityRuleJson,
                ReadOnlyRuleJson = e.ReadOnlyRuleJson,
                CreatedDate = e.CreatedDate,
                CreatedByUserId = e.CreatedByUserId,
                CreatedByUserName = e.CreatedByUser?.UserName,
                IsActive = e.IsActive,
                Tab = e.FORM_TABS != null ? new FormTabDto
                {
                    Id = e.FORM_TABS.id,
                    TabName = e.FORM_TABS.TabName,
                    TabCode = e.FORM_TABS.TabCode,
                    TabOrder = e.FORM_TABS.TabOrder,
                    IsActive = e.FORM_TABS.IsActive
                } : null,
                FieldType = e.FIELD_TYPES != null ? new FieldTypeDto
                {
                    Id = e.FIELD_TYPES.id,
                    TypeName = e.FIELD_TYPES.TypeName,
                    AllowMultiple = e.FIELD_TYPES.AllowMultiple,
                    DataType = e.FIELD_TYPES.DataType,
                    IsActive = e.FIELD_TYPES.IsActive
                } : null,
                FieldOptions = e.FIELD_OPTIONS?.Where(fo => fo.IsActive).Select(fo => new FieldOptionDto
                {
                    Id = fo.id,
                    FieldId = fo.FieldId,
                    OptionText = fo.OptionText,
                    OptionValue = fo.OptionValue,
                    OptionOrder = fo.OptionOrder,
                    IsActive = fo.IsActive
                }).ToList()
            };
        }

        private FORM_FIELDS ToEntity(CreateFormFieldDto dto)
        {
            return new FORM_FIELDS
            {
                TabId = dto.TabId,
                FieldTypeId = dto.FieldTypeId,
                FieldName = dto.FieldName,
                FieldCode = dto.FieldCode,
                FieldOrder = dto.FieldOrder,
                Placeholder = dto.Placeholder,
                HintText = dto.HintText,
                IsMandatory = dto.IsMandatory,
                IsEditable = dto.IsEditable,
                IsVisible = dto.IsVisible,
                DefaultValueJson = dto.DefaultValueJson,
                DataType = dto.DataType,
                MaxLength = dto.MaxLength,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
                RegexPattern = dto.RegexPattern,
                ValidationMessage = dto.ValidationMessage,
                VisibilityRuleJson = dto.VisibilityRuleJson,
                ReadOnlyRuleJson = dto.ReadOnlyRuleJson,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };
        }

        private void MapUpdate(UpdateFormFieldDto dto, FORM_FIELDS e)
        {
            e.TabId = dto.TabId;
            e.FieldTypeId = dto.FieldTypeId;
            e.FieldName = dto.FieldName;
            e.FieldCode = dto.FieldCode;
            e.FieldOrder = dto.FieldOrder;
            e.Placeholder = dto.Placeholder;
            e.HintText = dto.HintText;
            e.IsMandatory = dto.IsMandatory;
            e.IsEditable = dto.IsEditable;
            e.IsVisible = dto.IsVisible;
            e.DefaultValueJson = dto.DefaultValueJson;
            e.DataType = dto.DataType;
            e.MaxLength = dto.MaxLength;
            e.MinValue = dto.MinValue;
            e.MaxValue = dto.MaxValue;
            e.RegexPattern = dto.RegexPattern;
            e.ValidationMessage = dto.ValidationMessage;
            e.VisibilityRuleJson = dto.VisibilityRuleJson;
            e.ReadOnlyRuleJson = dto.ReadOnlyRuleJson;
        }
    }

    // Additional DTO for dropdown
    public class FormFieldDropdownDto
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldCode { get; set; }
    }
}