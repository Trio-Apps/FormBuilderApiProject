using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionsService : IFormSubmissionsService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionsService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionsRepository.GetSubmissionsWithDetailsAsync();
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "All form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdWithDetailsAsync(id);
                if (submission == null)
                    return new ApiResponse(404, "Form submission not found");

                var submissionDto = ToDetailDto(submission);
                return new ApiResponse(200, "Form submission retrieved successfully", submissionDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdWithDetailsAsync(id);
                if (submission == null)
                    return new ApiResponse(404, "Form submission not found");

                var submissionDto = ToDetailDto(submission);
                return new ApiResponse(200, "Form submission with details retrieved successfully", submissionDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission with details: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                var submission = await _unitOfWork.FormSubmissionsRepository.GetByDocumentNumberAsync(documentNumber);
                if (submission == null)
                    return new ApiResponse(404, "Form submission not found");

                var submissionDto = ToDto(submission);
                return new ApiResponse(200, "Form submission retrieved successfully", submissionDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submission: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionsRepository.GetByFormBuilderIdAsync(formBuilderId);
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByDocumentTypeIdAsync(int documentTypeId)
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionsRepository.GetByDocumentTypeIdAsync(documentTypeId);
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByUserIdAsync(string userId)
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionsRepository.GetByUserIdAsync(userId);
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "User form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving user form submissions: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByStatusAsync(string status)
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionsRepository.GetByStatusAsync(status);
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submissions by status retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions by status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Generate document number
                var series = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(createDto.SeriesId);
                if (series == null)
                    return new ApiResponse(404, "Document series not found");

                var nextNumber = await _unitOfWork.DocumentSeriesRepository.GetNextNumberAsync(createDto.SeriesId);
                var documentNumber = $"{series.SeriesCode}-{nextNumber:D6}";

                // Get next version
                var version = await _unitOfWork.FormSubmissionsRepository.GetNextVersionAsync(createDto.FormBuilderId);

                var entity = new FORM_SUBMISSIONS
                {
                    FormBuilderId = createDto.FormBuilderId,
                    DocumentTypeId = createDto.DocumentTypeId,
                    SeriesId = createDto.SeriesId,
                    DocumentNumber = documentNumber,
                    SubmittedByUserId = createDto.SubmittedByUserId,
                    Version = version,
                    Status = createDto.Status,
                    // Fixed: remove invalid cast from string to DateTime
                    // Use current UTC time as initial submitted date (avoids casting and nullability issues)
                    SubmittedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _unitOfWork.FormSubmissionsRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating form submission: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission not found");

                if (!string.IsNullOrEmpty(updateDto.DocumentNumber) && updateDto.DocumentNumber != entity.DocumentNumber)
                {
                    var documentNumberExists = await _unitOfWork.FormSubmissionsRepository.DocumentNumberExistsAsync(updateDto.DocumentNumber);
                    if (documentNumberExists)
                        return new ApiResponse(400, "Document number already exists");
                }

                MapUpdate(updateDto, entity);
                entity.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormSubmissionsRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form submission: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission not found");

                _unitOfWork.FormSubmissionsRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting form submission: {ex.Message}");
            }
        }

        public async Task<ApiResponse> SubmitAsync(SubmitFormDto submitDto)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(submitDto.SubmissionId);
                if (entity == null)
                    return new ApiResponse(404, "Form submission not found");

                if (entity.Status == "Submitted")
                    return new ApiResponse(400, "Form submission is already submitted");

                entity.Status = "Submitted";
                entity.SubmittedDate = DateTime.UtcNow;
                entity.SubmittedByUserId = submitDto.SubmittedByUserId;
                entity.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormSubmissionsRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission submitted successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error submitting form submission: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateStatusAsync(int id, string status)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission not found");

                entity.Status = status;
                entity.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormSubmissionsRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, $"Form submission status updated to {status} successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form submission status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormSubmissionsRepository.AnyAsync(s => s.Id == id);
                return new ApiResponse(200, "Form submission existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking form submission existence: {ex.Message}");
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private FormSubmissionDto ToDto(FORM_SUBMISSIONS entity)
        {
            if (entity == null) return null;

            return new FormSubmissionDto
            {
                Id = entity.Id,
                FormBuilderId = entity.FormBuilderId,
                FormName = entity.FORM_BUILDER?.FormName,
                Version = entity.Version,
                DocumentTypeId = entity.DocumentTypeId,
                DocumentTypeName = entity.DOCUMENT_TYPES?.Name,
                SeriesId = entity.SeriesId,
                SeriesCode = entity.DOCUMENT_SERIES?.SeriesCode,
                DocumentNumber = entity.DocumentNumber,
                SubmittedByUserId = entity.SubmittedByUserId,
                SubmittedDate = entity.SubmittedDate,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
            };
        }

        private FormSubmissionDetailDto ToDetailDto(FORM_SUBMISSIONS entity)
        {
            if (entity == null) return null;

            var dto = new FormSubmissionDetailDto
            {
                Id = entity.Id,
                FormBuilderId = entity.FormBuilderId,
                FormName = entity.FORM_BUILDER?.FormName,
                Version = entity.Version,
                DocumentTypeId = entity.DocumentTypeId,
                DocumentTypeName = entity.DOCUMENT_TYPES?.Name,
                SeriesId = entity.SeriesId,
                SeriesCode = entity.DOCUMENT_SERIES?.SeriesCode,
                DocumentNumber = entity.DocumentNumber,
                SubmittedByUserId = entity.SubmittedByUserId,
                SubmittedDate = entity.SubmittedDate,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
            };

            // Map field values, attachments, and grid data if needed
            // This would require additional mapping methods

            return dto;
        }

        private void MapUpdate(UpdateFormSubmissionDto dto, FORM_SUBMISSIONS entity)
        {
            if (!string.IsNullOrEmpty(dto.DocumentNumber))
                entity.DocumentNumber = dto.DocumentNumber;

            if (!string.IsNullOrEmpty(dto.Status))
                entity.Status = dto.Status;

            if (dto.SubmittedDate.HasValue)
                entity.SubmittedDate = dto.SubmittedDate.Value;
        }
    }
}