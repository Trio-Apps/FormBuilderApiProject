using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionValuesService : IFormSubmissionValuesService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionValuesService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var values = await _unitOfWork.FormSubmissionValuesRepository.GetAllAsync();
                var valueDtos = values.Select(ToDto).ToList();
                return new ApiResponse(200, "All form submission values retrieved successfully", valueDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission values: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var value = await _unitOfWork.FormSubmissionValuesRepository.GetByIdAsync(id);
                if (value == null)
                    return new ApiResponse(404, "Form submission value not found");

                var valueDto = ToDto(value);
                return new ApiResponse(200, "Form submission value retrieved successfully", valueDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission value: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetBySubmissionIdAsync(int submissionId)
        {
            try
            {
                var values = await _unitOfWork.FormSubmissionValuesRepository.GetBySubmissionIdAsync(submissionId);
                var valueDtos = values.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submission values retrieved successfully", valueDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission values: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByFieldIdAsync(int fieldId)
        {
            try
            {
                var values = await _unitOfWork.FormSubmissionValuesRepository.GetByFieldIdAsync(fieldId);
                var valueDtos = values.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submission values by field retrieved successfully", valueDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission values by field: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetBySubmissionAndFieldAsync(int submissionId, int fieldId)
        {
            try
            {
                var value = await _unitOfWork.FormSubmissionValuesRepository.GetBySubmissionAndFieldAsync(submissionId, fieldId);
                if (value == null)
                    return new ApiResponse(404, "Form submission value not found");

                var valueDto = ToDto(value);
                return new ApiResponse(200, "Form submission value retrieved successfully", valueDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission value: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionValueDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if value already exists for this field and submission
                var exists = await _unitOfWork.FormSubmissionValuesRepository
                    .ExistsBySubmissionAndFieldAsync(createDto.SubmissionId, createDto.FieldId);

                if (exists)
                    return new ApiResponse(400, "Form submission value already exists for this field");

                var entity = ToEntity(createDto);
                _unitOfWork.FormSubmissionValuesRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission value created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating form submission value: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateBulkAsync(BulkFormSubmissionValuesDto bulkDto)
        {
            try
            {
                if (bulkDto == null || !bulkDto.Values.Any())
                    return new ApiResponse(400, "No values provided");

                var entities = new List<FORM_SUBMISSION_VALUES>();

                foreach (var valueDto in bulkDto.Values)
                {
                    // Skip if value already exists
                    var exists = await _unitOfWork.FormSubmissionValuesRepository
                        .ExistsBySubmissionAndFieldAsync(bulkDto.SubmissionId, valueDto.FieldId);

                    if (!exists)
                    {
                        var entity = new FORM_SUBMISSION_VALUES
                        {
                            SubmissionId = bulkDto.SubmissionId,
                            FieldId = valueDto.FieldId,
                            FieldCode = valueDto.FieldCode,
                            ValueString = valueDto.ValueString,
                            ValueNumber = valueDto.ValueNumber,
                            ValueDate = valueDto.ValueDate,
                            ValueBool = valueDto.ValueBool,
                            ValueJson = valueDto.ValueJson
                        };
                        entities.Add(entity);
                    }
                }

                if (entities.Any())
                {
                    _unitOfWork.FormSubmissionValuesRepository.AddRange(entities);
                    await _unitOfWork.CompleteAsyn();
                }

                var createdDtos = entities.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submission values created successfully", createdDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating form submission values: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionValueDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormSubmissionValuesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission value not found");

                MapUpdate(updateDto, entity);
                _unitOfWork.FormSubmissionValuesRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission value updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form submission value: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateByFieldAsync(int submissionId, int fieldId, UpdateFormSubmissionValueDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormSubmissionValuesRepository
                    .GetBySubmissionAndFieldAsync(submissionId, fieldId);

                if (entity == null)
                    return new ApiResponse(404, "Form submission value not found");

                MapUpdate(updateDto, entity);
                _unitOfWork.FormSubmissionValuesRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission value updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form submission value: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionValuesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission value not found");

                _unitOfWork.FormSubmissionValuesRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission value deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting form submission value: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteBySubmissionIdAsync(int submissionId)
        {
            try
            {
                var deleted = await _unitOfWork.FormSubmissionValuesRepository.DeleteBySubmissionIdAsync(submissionId);
                var message = deleted ? "Form submission values deleted successfully" : "No form submission values found";

                return new ApiResponse(200, message);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting form submission values: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormSubmissionValuesRepository.AnyAsync(v => v.Id == id);
                return new ApiResponse(200, "Form submission value existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking form submission value existence: {ex.Message}");
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private FormSubmissionValueDto ToDto(FORM_SUBMISSION_VALUES entity)
        {
            if (entity == null) return null;

            return new FormSubmissionValueDto
            {
                Id = entity.Id,
                SubmissionId = entity.SubmissionId,
                FieldId = entity.FieldId,
                FieldCode = entity.FieldCode,
                FieldName = entity.FORM_FIELDS?.FieldName,
               
                ValueString = entity.ValueString,
                ValueNumber = entity.ValueNumber,
                ValueDate = entity.ValueDate,
                ValueBool = entity.ValueBool,
                ValueJson = entity.ValueJson
            };
        }

        private FORM_SUBMISSION_VALUES ToEntity(CreateFormSubmissionValueDto dto)
        {
            return new FORM_SUBMISSION_VALUES
            {
                SubmissionId = dto.SubmissionId,
                FieldId = dto.FieldId,
                FieldCode = dto.FieldCode,
                ValueString = dto.ValueString,
                ValueNumber = dto.ValueNumber,
                ValueDate = dto.ValueDate,
                ValueBool = dto.ValueBool,
                ValueJson = dto.ValueJson
            };
        }

        private void MapUpdate(UpdateFormSubmissionValueDto dto, FORM_SUBMISSION_VALUES entity)
        {
            entity.ValueString = dto.ValueString;
            entity.ValueNumber = dto.ValueNumber;
            entity.ValueDate = dto.ValueDate;
            entity.ValueBool = dto.ValueBool;
            entity.ValueJson = dto.ValueJson;
        }
    }
}