using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Models.DTOs;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.froms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionService : IFormSubmissionService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // ================================
        // GET ALL FORM SUBMISSIONS
        // ================================
        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionRepository.GetAllAsync();
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "All form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all form submissions: {ex.Message}");
            }
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var submission = await _unitOfWork.FormSubmissionRepository.GetByIdAsync(id);
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

        // ================================
        // GET BY FORM BUILDER ID
        // ================================
        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionRepository.GetByFormBuilderIdAsync(formBuilderId);
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions: {ex.Message}");
            }
        }

        // ================================
        // GET BY USER ID
        // ================================
        public async Task<ApiResponse> GetByUserIdAsync(string userId)
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionRepository.GetByUserIdAsync(userId);
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions: {ex.Message}");
            }
        }

        // ================================
        // GET BY STATUS
        // ================================
        public async Task<ApiResponse> GetByStatusAsync(string status)
        {
            try
            {
                var submissions = await _unitOfWork.FormSubmissionRepository.GetByStatusAsync(status);
                var submissionDtos = submissions.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submissions retrieved successfully", submissionDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions: {ex.Message}");
            }
        }

        // ================================
        // GET BY DOCUMENT NUMBER
        // ================================
        public async Task<ApiResponse> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                var submission = await _unitOfWork.FormSubmissionRepository.GetByDocumentNumberAsync(documentNumber);
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

        // ================================
        // CREATE
        // ================================
        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Validate if form builder exists
                var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(x => x.id == createDto.FormBuilderId);
                if (!formBuilderExists)
                    return new ApiResponse(400, "Invalid form builder ID");

                var entity = ToEntity(createDto);
                _unitOfWork.FormSubmissionRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating form submission: {ex.Message}");
            }
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormSubmissionRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission not found");

                MapUpdate(updateDto, entity);
                _unitOfWork.FormSubmissionRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form submission: {ex.Message}");
            }
        }

        // ================================
        // DELETE
        // ================================
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission not found");

                _unitOfWork.FormSubmissionRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting form submission: {ex.Message}");
            }
        }

        // ================================
        // UPDATE STATUS
        // ================================
        public async Task<ApiResponse> UpdateStatusAsync(int id, FormSubmissionStatusDto statusDto)
        {
            try
            {
                if (statusDto == null)
                    return new ApiResponse(400, "Status DTO is required");

                var entity = await _unitOfWork.FormSubmissionRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form submission not found");

                entity.Status = statusDto.Status;
                entity.LastUpdatedDate = DateTime.UtcNow;

                // Set submitted date when status changes to "Submitted"
                if (statusDto.Status == "Submitted" && entity.SubmittedDate == null)
                {
                    entity.SubmittedDate = DateTime.UtcNow;
                }

                _unitOfWork.FormSubmissionRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form submission status updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form submission status: {ex.Message}");
            }
        }

        // ================================
        // GET SUBMISSIONS COUNT
        // ================================
        public async Task<ApiResponse> GetSubmissionsCountAsync(int formBuilderId)
        {
            try
            {
                var count = await _unitOfWork.FormSubmissionRepository.GetSubmissionsCountAsync(formBuilderId);
                return new ApiResponse(200, "Form submissions count retrieved successfully", count);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form submissions count: {ex.Message}");
            }
        }

        // ================================
        // EXISTS
        // ================================
        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormSubmissionRepository.ExistsAsync(id);
                return new ApiResponse(200, "Form submission existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking form submission existence: {ex.Message}");
            }
        }

        // ================================
        // FILTER SUBMISSIONS
        // ================================
        public async Task<ApiResponse> FilterSubmissionsAsync(FormSubmissionFilterDto filterDto)
        {
            try
            {
                var allSubmissions = await _unitOfWork.FormSubmissionRepository.GetAllAsync();
                var filteredSubmissions = allSubmissions.AsQueryable();

                if (filterDto.FormBuilderId.HasValue)
                {
                    filteredSubmissions = filteredSubmissions.Where(s => s.FormBuilderId == filterDto.FormBuilderId.Value);
                }

                if (!string.IsNullOrEmpty(filterDto.UserId))
                {
                    filteredSubmissions = filteredSubmissions.Where(s => s.SubmittedByUserId == filterDto.UserId);
                }

                if (!string.IsNullOrEmpty(filterDto.Status))
                {
                    filteredSubmissions = filteredSubmissions.Where(s => s.Status == filterDto.Status);
                }

                if (!string.IsNullOrEmpty(filterDto.DocumentNumber))
                {
                    filteredSubmissions = filteredSubmissions.Where(s => s.DocumentNumber.Contains(filterDto.DocumentNumber));
                }

                if (filterDto.FromDate.HasValue)
                {
                    filteredSubmissions = filteredSubmissions.Where(s => s.CreatedDate >= filterDto.FromDate.Value);
                }

                if (filterDto.ToDate.HasValue)
                {
                    filteredSubmissions = filteredSubmissions.Where(s => s.CreatedDate <= filterDto.ToDate.Value);
                }

                var result = filteredSubmissions.Select(ToDto).ToList();
                return new ApiResponse(200, "Form submissions filtered successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error filtering form submissions: {ex.Message}");
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
                Id = entity.id,
                FormBuilderId = entity.FormBuilderId,
                Version = entity.Version,
                DocumentTypeId = entity.DocumentTypeId,
                SeriesId = entity.SeriesId,
                DocumentNumber = entity.DocumentNumber,
                SubmittedByUserId = entity.SubmittedByUserId,
                SubmittedDate = entity.SubmittedDate,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
                LastUpdatedDate = entity.LastUpdatedDate
            };
        }

        private FORM_SUBMISSIONS ToEntity(CreateFormSubmissionDto dto)
        {
            return new FORM_SUBMISSIONS
            {
                FormBuilderId = dto.FormBuilderId,
                Version = dto.Version,
                DocumentTypeId = dto.DocumentTypeId,
                SeriesId = dto.SeriesId,
                DocumentNumber = dto.DocumentNumber,
                SubmittedByUserId = dto.SubmittedByUserId,
                SubmittedDate = dto.SubmittedDate.GetValueOrDefault(),
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };
        }

        private void MapUpdate(UpdateFormSubmissionDto dto, FORM_SUBMISSIONS entity)
        {
            if (!string.IsNullOrEmpty(dto.Status))
                entity.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.DocumentNumber))
                entity.DocumentNumber = dto.DocumentNumber;

            if (dto.SubmittedDate.HasValue)
                entity.SubmittedDate = dto.SubmittedDate.Value;

            entity.LastUpdatedDate = DateTime.UtcNow;
        }
    }
}