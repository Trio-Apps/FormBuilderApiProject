using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class AttachmentTypeService : IAttachmentTypeService
    {
        private readonly IunitOfwork _unitOfWork;

        public AttachmentTypeService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // ================================
        // GET ALL ATTACHMENT TYPES
        // ================================
        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var attachmentTypes = await _unitOfWork.AttachmentTypeRepository.GetAllAsync();
                var attachmentTypeDtos = attachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "All attachment types retrieved successfully", attachmentTypeDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all attachment types: {ex.Message}");
            }
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var attachmentType = await _unitOfWork.AttachmentTypeRepository.GetByIdAsync(id);
                if (attachmentType == null)
                    return new ApiResponse(404, "Attachment type not found");

                var attachmentTypeDto = ToDto(attachmentType);
                return new ApiResponse(200, "Attachment type retrieved successfully", attachmentTypeDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving attachment type: {ex.Message}");
            }
        }

        // ================================
        // GET BY CODE
        // ================================
        public async Task<ApiResponse> GetByCodeAsync(string code)
        {
            try
            {
                var attachmentType = await _unitOfWork.AttachmentTypeRepository.GetByCodeAsync(code);
                if (attachmentType == null)
                    return new ApiResponse(404, "Attachment type not found");

                var attachmentTypeDto = ToDto(attachmentType);
                return new ApiResponse(200, "Attachment type retrieved successfully", attachmentTypeDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving attachment type: {ex.Message}");
            }
        }

        // ================================
        // GET ACTIVE
        // ================================
        public async Task<ApiResponse> GetActiveAsync()
        {
            try
            {
                var attachmentTypes = await _unitOfWork.AttachmentTypeRepository.GetActiveAsync();
                var attachmentTypeDtos = attachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Active attachment types retrieved successfully", attachmentTypeDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active attachment types: {ex.Message}");
            }
        }

        // ================================
        // CREATE
        // ================================
        public async Task<ApiResponse> CreateAsync(CreateAttachmentTypeDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if code already exists
                var codeExists = await _unitOfWork.AttachmentTypeRepository.CodeExistsAsync(createDto.Code);
                if (codeExists)
                    return new ApiResponse(400, "Attachment type code already exists");

                var entity = ToEntity(createDto);
                _unitOfWork.AttachmentTypeRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Attachment type created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating attachment type: {ex.Message}");
            }
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<ApiResponse> UpdateAsync(int id, UpdateAttachmentTypeDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.AttachmentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Attachment type not found");

                // Check if code already exists (excluding current record)
                if (!string.IsNullOrEmpty(updateDto.Code) && updateDto.Code != entity.Code)
                {
                    var codeExists = await _unitOfWork.AttachmentTypeRepository.CodeExistsAsync(updateDto.Code, id);
                    if (codeExists)
                        return new ApiResponse(400, "Attachment type code already exists");
                }

                MapUpdate(updateDto, entity);
                _unitOfWork.AttachmentTypeRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Attachment type updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating attachment type: {ex.Message}");
            }
        }

        // ================================
        // DELETE
        // ================================
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.AttachmentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Attachment type not found");

                _unitOfWork.AttachmentTypeRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Attachment type deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting attachment type: {ex.Message}");
            }
        }

        // ================================
        // TOGGLE ACTIVE
        // ================================
        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.AttachmentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Attachment type not found");

                entity.IsActive = isActive;
                _unitOfWork.AttachmentTypeRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Attachment type {message} successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling attachment type active status: {ex.Message}");
            }
        }

        // ================================
        // EXISTS
        // ================================
        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.AttachmentTypeRepository.AnyAsync(s=>s.Id==id);
                return new ApiResponse(200, "Attachment type existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking attachment type existence: {ex.Message}");
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private AttachmentTypeDto ToDto(ATTACHMENT_TYPES entity)
        {
            if (entity == null) return null;

            return new AttachmentTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Description = entity.Description,
                MaxSizeMB = entity.MaxSizeMB,
                IsActive = entity.IsActive
            };
        }

        private ATTACHMENT_TYPES ToEntity(CreateAttachmentTypeDto dto)
        {
            return new ATTACHMENT_TYPES
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                MaxSizeMB = dto.MaxSizeMB,
                IsActive = dto.IsActive
            };
        }

        private void MapUpdate(UpdateAttachmentTypeDto dto, ATTACHMENT_TYPES entity)
        {
            if (!string.IsNullOrEmpty(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Code))
                entity.Code = dto.Code;

            if (!string.IsNullOrEmpty(dto.Description))
                entity.Description = dto.Description;

            if (dto.MaxSizeMB.HasValue)
                entity.MaxSizeMB = dto.MaxSizeMB.Value;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;
        }
    }
}