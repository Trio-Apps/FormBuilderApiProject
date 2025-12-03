using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionAttachmentsService : IFormSubmissionAttachmentsService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionAttachmentsService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse<List<FormSubmissionAttachmentDto>>> GetAllAsync()
        {
            try
            {
                var attachments = await _unitOfWork.FormSubmissionAttachmentsRepository.GetAllAsync();
                var attachmentDtos = attachments.Select(ToDto).ToList();
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(200, "All form submission attachments retrieved successfully", attachmentDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(500, $"Error retrieving form submission attachments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<FormSubmissionAttachmentDto>> GetByIdAsync(int id)
        {
            try
            {
                var attachment = await _unitOfWork.FormSubmissionAttachmentsRepository.GetByIdAsync(id);
                if (attachment == null)
                    return new ApiResponse<FormSubmissionAttachmentDto>(404, "Form submission attachment not found");

                var attachmentDto = ToDto(attachment);
                return new ApiResponse<FormSubmissionAttachmentDto>(200, "Form submission attachment retrieved successfully", attachmentDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FormSubmissionAttachmentDto>(500, $"Error retrieving form submission attachment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<FormSubmissionAttachmentDto>>> GetBySubmissionIdAsync(int submissionId)
        {
            try
            {
                var attachments = await _unitOfWork.FormSubmissionAttachmentsRepository.GetBySubmissionIdAsync(submissionId);
                var attachmentDtos = attachments.Select(ToDto).ToList();
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(200, "Form submission attachments retrieved successfully", attachmentDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(500, $"Error retrieving form submission attachments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<FormSubmissionAttachmentDto>>> GetByFieldIdAsync(int fieldId)
        {
            try
            {
                var attachments = await _unitOfWork.FormSubmissionAttachmentsRepository.GetByFieldIdAsync(fieldId);
                var attachmentDtos = attachments.Select(ToDto).ToList();
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(200, "Form submission attachments by field retrieved successfully", attachmentDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(500, $"Error retrieving form submission attachments by field: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<FormSubmissionAttachmentDto>>> GetBySubmissionAndFieldAsync(int submissionId, int fieldId)
        {
            try
            {
                var attachments = await _unitOfWork.FormSubmissionAttachmentsRepository.GetBySubmissionAndFieldAsync(submissionId, fieldId);
                var attachmentDtos = attachments.Select(ToDto).ToList();
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(200, "Form submission attachments retrieved successfully", attachmentDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(500, $"Error retrieving form submission attachments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<FormSubmissionAttachmentDto>> CreateAsync(CreateFormSubmissionAttachmentDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse<FormSubmissionAttachmentDto>(400, "DTO is required");

                // Check if file name already exists for this submission
                var fileNameExists = await _unitOfWork.FormSubmissionAttachmentsRepository
                    .FileNameExistsAsync(createDto.SubmissionId, createDto.FileName);

                if (fileNameExists)
                    return new ApiResponse<FormSubmissionAttachmentDto>(400, "File name already exists for this submission");

                var entity = ToEntity(createDto);
                _unitOfWork.FormSubmissionAttachmentsRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                var attachmentDto = ToDto(entity);
                return new ApiResponse<FormSubmissionAttachmentDto>(200, "Form submission attachment created successfully", attachmentDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FormSubmissionAttachmentDto>(500, $"Error creating form submission attachment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<FormSubmissionAttachmentDto>>> CreateBulkAsync(BulkAttachmentsDto bulkDto)
        {
            try
            {
                if (bulkDto == null || !bulkDto.Attachments.Any())
                    return new ApiResponse<List<FormSubmissionAttachmentDto>>(400, "No attachments provided");

                var entities = new List<FORM_SUBMISSION_ATTACHMENTS>();

                foreach (var attachmentDto in bulkDto.Attachments)
                {
                    var entity = new FORM_SUBMISSION_ATTACHMENTS
                    {
                        SubmissionId = bulkDto.SubmissionId,
                        FieldId = attachmentDto.FieldId,
                        FileName = attachmentDto.FileName,
                        FilePath = attachmentDto.FilePath,
                        FileSize = attachmentDto.FileSize,
                        ContentType = attachmentDto.ContentType,
                        UploadedDate = DateTime.UtcNow
                    };
                    entities.Add(entity);
                }

                _unitOfWork.FormSubmissionAttachmentsRepository.AddRange(entities);
                await _unitOfWork.CompleteAsyn();

                var createdDtos = entities.Select(ToDto).ToList();
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(200, "Form submission attachments created successfully", createdDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FormSubmissionAttachmentDto>>(500, $"Error creating form submission attachments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<FormSubmissionAttachmentDto>> UpdateAsync(int id, UpdateFormSubmissionAttachmentDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse<FormSubmissionAttachmentDto>(400, "DTO is required");

                var entity = await _unitOfWork.FormSubmissionAttachmentsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse<FormSubmissionAttachmentDto>(404, "Form submission attachment not found");

                MapUpdate(updateDto, entity);
                _unitOfWork.FormSubmissionAttachmentsRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var attachmentDto = ToDto(entity);
                return new ApiResponse<FormSubmissionAttachmentDto>(200, "Form submission attachment updated successfully", attachmentDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FormSubmissionAttachmentDto>(500, $"Error updating form submission attachment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionAttachmentsRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse<bool>(404, "Form submission attachment not found", false);

                // TODO: Delete physical file from storage
                // await DeletePhysicalFile(entity.FilePath);

                _unitOfWork.FormSubmissionAttachmentsRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse<bool>(200, "Form submission attachment deleted successfully", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(500, $"Error deleting form submission attachment: {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<bool>> DeleteBySubmissionIdAsync(int submissionId)
        {
            try
            {
                var deleted = await _unitOfWork.FormSubmissionAttachmentsRepository.DeleteBySubmissionIdAsync(submissionId);
                var message = deleted ? "Form submission attachments deleted successfully" : "No form submission attachments found";

                return new ApiResponse<bool>(200, message, deleted);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(500, $"Error deleting form submission attachments: {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<bool>> DeleteBySubmissionAndFieldAsync(int submissionId, int fieldId)
        {
            try
            {
                var deleted = await _unitOfWork.FormSubmissionAttachmentsRepository.DeleteBySubmissionAndFieldAsync(submissionId, fieldId);
                var message = deleted ? "Form submission attachments deleted successfully" : "No form submission attachments found";

                return new ApiResponse<bool>(200, message, deleted);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(500, $"Error deleting form submission attachments: {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<FormSubmissionAttachmentDto>> UploadAttachmentAsync(IFormFile file, UploadAttachmentDto uploadDto)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new ApiResponse<FormSubmissionAttachmentDto>(400, "No file provided");

                // Validate file size (e.g., 10MB max)
                if (file.Length > 10 * 1024 * 1024)
                    return new ApiResponse<FormSubmissionAttachmentDto>(400, "File size exceeds maximum allowed size (10MB)");

                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine("uploads", "attachments", fileName);

                // TODO: Save file to storage (local, Azure Blob, etc.)
                // using (var stream = new FileStream(filePath, FileMode.Create))
                // {
                //     await file.CopyToAsync(stream);
                // }

                var createDto = new CreateFormSubmissionAttachmentDto
                {
                    SubmissionId = uploadDto.SubmissionId,
                    FieldId = uploadDto.FieldId,
                    FieldCode = uploadDto.FieldCode,
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                };

                return await CreateAsync(createDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FormSubmissionAttachmentDto>(500, $"Error uploading attachment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<AttachmentUploadResultDto>>> UploadMultipleAttachmentsAsync(List<IFormFile> files, UploadAttachmentDto uploadDto)
        {
            try
            {
                if (files == null || !files.Any())
                    return new ApiResponse<List<AttachmentUploadResultDto>>(400, "No files provided");

                var results = new List<AttachmentUploadResultDto>();

                foreach (var file in files)
                {
                    var uploadResult = await UploadAttachmentAsync(file, uploadDto);
                    if (uploadResult.StatusCode == 200)
                    {
                        var attachment = uploadResult.Data;
                        results.Add(new AttachmentUploadResultDto
                        {
                            AttachmentId = attachment.Id,
                            FileName = attachment.FileName,
                            FilePath = attachment.FilePath,
                            FileSize = attachment.FileSize,
                            ContentType = attachment.ContentType,
                            UploadedDate = attachment.UploadedDate,
                            Success = true,
                            Message = "Upload successful"
                        });
                    }
                    else
                    {
                        results.Add(new AttachmentUploadResultDto
                        {
                            FileName = file.FileName,
                            Success = false,
                            Message = uploadResult.Message
                        });
                    }
                }

                return new ApiResponse<List<AttachmentUploadResultDto>>(200, "Multiple attachments upload completed", results);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AttachmentUploadResultDto>>(500, $"Error uploading multiple attachments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AttachmentStatsDto>> GetAttachmentStatsAsync(int submissionId)
        {
            try
            {
                var totalSize = await _unitOfWork.FormSubmissionAttachmentsRepository.GetTotalSizeBySubmissionAsync(submissionId);
                var count = await _unitOfWork.FormSubmissionAttachmentsRepository.GetCountBySubmissionAsync(submissionId);
                var attachments = await _unitOfWork.FormSubmissionAttachmentsRepository.GetBySubmissionIdAsync(submissionId);

                var stats = new AttachmentStatsDto
                {
                    SubmissionId = submissionId,
                    TotalAttachments = count,
                    TotalSize = totalSize,
                    TotalSizeFormatted = FormatFileSize(totalSize),
                    AttachmentsByType = attachments
                        .GroupBy(a => a.ContentType)
                        .ToDictionary(g => g.Key, g => g.Count())
                };

                return new ApiResponse<AttachmentStatsDto>(200, "Attachment statistics retrieved successfully", stats);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AttachmentStatsDto>(500, $"Error retrieving attachment statistics: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormSubmissionAttachmentsRepository.AnyAsync(a => a.Id == id);
                return new ApiResponse<bool>(200, "Form submission attachment existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(500, $"Error checking form submission attachment existence: {ex.Message}", false);
            }
        }

        // ================================
        // MAPPING METHODS (تبقى كما هي)
        // ================================
        private FormSubmissionAttachmentDto ToDto(FORM_SUBMISSION_ATTACHMENTS entity)
        {
            if (entity == null) return null;

            return new FormSubmissionAttachmentDto
            {
                Id = entity.Id,
                SubmissionId = entity.SubmissionId,
                SubmissionDocumentNumber = entity.FORM_SUBMISSIONS?.DocumentNumber,
                FieldId = entity.FieldId,
                FieldCode = entity.FORM_FIELDS?.FieldCode,
                FieldName = entity.FORM_FIELDS?.FieldName,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                FileSize = entity.FileSize,
                ContentType = entity.ContentType,
                UploadedDate = entity.UploadedDate,
                FileSizeFormatted = FormatFileSize(entity.FileSize),
                DownloadUrl = $"/api/attachments/download/{entity.Id}"
            };
        }

        private FORM_SUBMISSION_ATTACHMENTS ToEntity(CreateFormSubmissionAttachmentDto dto)
        {
            return new FORM_SUBMISSION_ATTACHMENTS
            {
                SubmissionId = dto.SubmissionId,
                FieldId = dto.FieldId,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                FileSize = dto.FileSize,
                ContentType = dto.ContentType,
                UploadedDate = DateTime.UtcNow
            };
        }

        private void MapUpdate(UpdateFormSubmissionAttachmentDto dto, FORM_SUBMISSION_ATTACHMENTS entity)
        {
            if (!string.IsNullOrEmpty(dto.FileName))
                entity.FileName = dto.FileName;

            if (!string.IsNullOrEmpty(dto.FilePath))
                entity.FilePath = dto.FilePath;

            if (dto.FileSize.HasValue)
                entity.FileSize = dto.FileSize.Value;

            if (!string.IsNullOrEmpty(dto.ContentType))
                entity.ContentType = dto.ContentType;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}