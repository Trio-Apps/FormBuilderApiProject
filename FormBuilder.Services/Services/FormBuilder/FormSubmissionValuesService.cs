using formBuilder.Domian.Interfaces;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Services.Services.Base;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormBuilder.API.Models;

namespace FormBuilder.Services
{
    public class FormSubmissionValuesService : BaseService<FORM_SUBMISSION_VALUES, FormSubmissionValueDto, CreateFormSubmissionValueDto, UpdateFormSubmissionValueDto>, IFormSubmissionValuesService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionValuesService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<FORM_SUBMISSION_VALUES> Repository => _unitOfWork.FormSubmissionValuesRepository;

        public async Task<ApiResponse> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var result = await base.GetByIdAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetBySubmissionIdAsync(int submissionId)
        {
            var values = await _unitOfWork.FormSubmissionValuesRepository.GetBySubmissionIdAsync(submissionId);
            var valueDtos = _mapper.Map<IEnumerable<FormSubmissionValueDto>>(values);
            return new ApiResponse(200, "Form submission values retrieved successfully", valueDtos);
        }

        public async Task<ApiResponse> GetByFieldIdAsync(int fieldId)
        {
            var values = await _unitOfWork.FormSubmissionValuesRepository.GetByFieldIdAsync(fieldId);
            var valueDtos = _mapper.Map<IEnumerable<FormSubmissionValueDto>>(values);
            return new ApiResponse(200, "Form submission values by field retrieved successfully", valueDtos);
        }

        public async Task<ApiResponse> GetBySubmissionAndFieldAsync(int submissionId, int fieldId)
        {
            var value = await _unitOfWork.FormSubmissionValuesRepository.GetBySubmissionAndFieldAsync(submissionId, fieldId);
            if (value == null)
                return new ApiResponse(404, "Form submission value not found");

            var valueDto = _mapper.Map<FormSubmissionValueDto>(value);
            return new ApiResponse(200, "Form submission value retrieved successfully", valueDto);
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionValueDto createDto)
        {
            var result = await base.CreateAsync(createDto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFormSubmissionValueDto dto)
        {
            // Check if value already exists for this field and submission
            var exists = await _unitOfWork.FormSubmissionValuesRepository
                .ExistsBySubmissionAndFieldAsync(dto.SubmissionId, dto.FieldId);

            if (exists)
                return ValidationResult.Failure("Form submission value already exists for this field");

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> CreateBulkAsync(BulkFormSubmissionValuesDto bulkDto)
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
                    var entity = _mapper.Map<FORM_SUBMISSION_VALUES>(valueDto);
                    entity.SubmissionId = bulkDto.SubmissionId; // Override with bulk submission ID
                    entities.Add(entity);
                }
            }

            if (entities.Any())
            {
                _unitOfWork.FormSubmissionValuesRepository.AddRange(entities);
                await _unitOfWork.CompleteAsyn();
            }

            var createdDtos = _mapper.Map<IEnumerable<FormSubmissionValueDto>>(entities);
            return new ApiResponse(200, "Form submission values created successfully", createdDtos);
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionValueDto updateDto)
        {
            var result = await base.UpdateAsync(id, updateDto);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> UpdateByFieldAsync(int submissionId, int fieldId, UpdateFormSubmissionValueDto updateDto)
        {
            if (updateDto == null)
                return new ApiResponse(400, "DTO is required");

            var entity = await _unitOfWork.FormSubmissionValuesRepository
                .GetBySubmissionAndFieldAsync(submissionId, fieldId);

            if (entity == null)
                return new ApiResponse(404, "Form submission value not found");

            _mapper.Map(updateDto, entity);
            entity.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.FormSubmissionValuesRepository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            var valueDto = _mapper.Map<FormSubmissionValueDto>(entity);
            return new ApiResponse(200, "Form submission value updated successfully", valueDto);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> DeleteBySubmissionIdAsync(int submissionId)
        {
            var deleted = await _unitOfWork.FormSubmissionValuesRepository.DeleteBySubmissionIdAsync(submissionId);
            var message = deleted ? "Form submission values deleted successfully" : "No form submission values found";

            return new ApiResponse(200, message);
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            var exists = await _unitOfWork.FormSubmissionValuesRepository.AnyAsync(v => v.Id == id);
            return new ApiResponse(200, "Form submission value existence checked successfully", exists);
        }

        // ================================
        // HELPER METHODS
        // ================================
        private ApiResponse ConvertToApiResponse<T>(ServiceResult<T> result)
        {
            if (result.Success)
                return new ApiResponse(result.StatusCode, "Success", result.Data);
            else
                return new ApiResponse(result.StatusCode, result.ErrorMessage);
        }

        private ApiResponse ConvertToApiResponse(ServiceResult<bool> result)
        {
            if (result.Success)
                return new ApiResponse(result.StatusCode, "Success", result.Data);
            else
                return new ApiResponse(result.StatusCode, result.ErrorMessage);
        }
    }
}
