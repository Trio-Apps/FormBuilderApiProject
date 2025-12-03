using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Models.DTOs;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormAttachmentTypeService : IFormAttachmentTypeService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormAttachmentTypeService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                // Get all with includes for FormBuilder and AttachmentType
                var formAttachmentTypes = await _unitOfWork.FormAttachmentTypeRepository.GetAllAsync();
                var dtos = formAttachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "All form attachment types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all form attachment types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                // Get by ID with includes for FormBuilder and AttachmentType
                var formAttachmentType = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                if (formAttachmentType == null)
                    return new ApiResponse(404, "Form attachment type not found");

                var dto = ToDto(formAttachmentType);
                return new ApiResponse(200, "Form attachment type retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form attachment type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var formAttachmentTypes = await _unitOfWork.FormAttachmentTypeRepository.GetByFormBuilderIdAsync(formBuilderId);
                var dtos = formAttachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Form attachment types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form attachment types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByAttachmentTypeIdAsync(int attachmentTypeId)
        {
            try
            {
                var formAttachmentTypes = await _unitOfWork.FormAttachmentTypeRepository.GetByAttachmentTypeIdAsync(attachmentTypeId);
                var dtos = formAttachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Form attachment types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving form attachment types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveAsync()
        {
            try
            {
                var formAttachmentTypes = await _unitOfWork.FormAttachmentTypeRepository.GetActiveAsync();
                var dtos = formAttachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Active form attachment types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active form attachment types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var formAttachmentTypes = await _unitOfWork.FormAttachmentTypeRepository.GetActiveByFormBuilderIdAsync(formBuilderId);
                var dtos = formAttachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Active form attachment types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active form attachment types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetMandatoryByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var formAttachmentTypes = await _unitOfWork.FormAttachmentTypeRepository.GetMandatoryByFormBuilderIdAsync(formBuilderId);
                var dtos = formAttachmentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Mandatory form attachment types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving mandatory form attachment types: {ex.Message}");
            }
        }

        // باقي الدوال تبقى كما هي بدون تغيير...
        public async Task<ApiResponse> CreateAsync(CreateFormAttachmentTypeDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(e=>e.Id== createDto.FormBuilderId);
                if (!formBuilderExists)
                    return new ApiResponse(400, "Invalid form builder ID");

                var attachmentTypeExists = await _unitOfWork.AttachmentTypeRepository.AnyAsync(e => e.Id == createDto.AttachmentTypeId);
                if (!attachmentTypeExists)
                    return new ApiResponse(400, "Invalid attachment type ID");

                var exists = await _unitOfWork.FormAttachmentTypeRepository.ExistsAsync(createDto.FormBuilderId, createDto.AttachmentTypeId);
                if (exists)
                    return new ApiResponse(400, "Form attachment type association already exists");

                var entity = ToEntity(createDto);
                _unitOfWork.FormAttachmentTypeRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                // Get the created entity with includes
                var createdEntity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(entity.Id);
                return new ApiResponse(200, "Form attachment type created successfully", ToDto(createdEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating form attachment type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateBulkAsync(List<CreateFormAttachmentTypeDto> createDtos)
        {
            try
            {
                if (createDtos == null || !createDtos.Any())
                    return new ApiResponse(400, "No form attachment types provided");

                var entities = new List<FORM_ATTACHMENT_TYPES>();

                foreach (var createDto in createDtos)
                {
                    var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(e => e.Id == createDto.FormBuilderId);
                    if (!formBuilderExists)
                        return new ApiResponse(400, $"Invalid form builder ID: {createDto.FormBuilderId}");

                    var attachmentTypeExists = await _unitOfWork.AttachmentTypeRepository.AnyAsync(e => e.Id == createDto.AttachmentTypeId);
                    if (!attachmentTypeExists)
                        return new ApiResponse(400, $"Invalid attachment type ID: {createDto.AttachmentTypeId}");

                    var exists = await _unitOfWork.FormAttachmentTypeRepository.ExistsAsync(createDto.FormBuilderId, createDto.AttachmentTypeId);
                    if (!exists)
                    {
                        var entity = ToEntity(createDto);
                        entities.Add(entity);
                    }
                }

                if (entities.Any())
                {
                    _unitOfWork.FormAttachmentTypeRepository.AddRange(entities);
                    await _unitOfWork.CompleteAsyn();
                }

                var resultDtos = new List<FormAttachmentTypeDto>();
                foreach (var entity in entities)
                {
                    var createdEntity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(entity.Id);
                    resultDtos.Add(ToDto(createdEntity));
                }

                return new ApiResponse(200, "Form attachment types created successfully", resultDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating form attachment types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormAttachmentTypeDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form attachment type not found");

                MapUpdate(updateDto, entity);
                _unitOfWork.FormAttachmentTypeRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                return new ApiResponse(200, "Form attachment type updated successfully", ToDto(updatedEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating form attachment type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form attachment type not found");

                _unitOfWork.FormAttachmentTypeRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Form attachment type deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting form attachment type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var count = await _unitOfWork.FormAttachmentTypeRepository.DeleteByFormBuilderIdAsync(formBuilderId);
                return new ApiResponse(200, $"{count} form attachment types deleted successfully", count);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting form attachment types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form attachment type not found");

                entity.IsActive = isActive;
                _unitOfWork.FormAttachmentTypeRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Form attachment type {message} successfully", ToDto(updatedEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling form attachment type active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleMandatoryAsync(int id, bool isMandatory)
        {
            try
            {
                var entity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Form attachment type not found");

                entity.IsMandatory = isMandatory;
                _unitOfWork.FormAttachmentTypeRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.FormAttachmentTypeRepository.GetByIdAsync(id);
                var message = isMandatory ? "set as mandatory" : "set as optional";
                return new ApiResponse(200, $"Form attachment type {message} successfully", ToDto(updatedEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling form attachment type mandatory status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormAttachmentTypeRepository.AnyAsync(e => e.Id == id);
                return new ApiResponse(200, "Form attachment type existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking form attachment type existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> IsActiveAsync(int id)
        {
            try
            {
                var isActive = await _unitOfWork.FormAttachmentTypeRepository.IsActiveAsync(id);
                return new ApiResponse(200, "Form attachment type active status checked successfully", isActive);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking form attachment type active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> HasMandatoryAttachmentsAsync(int formBuilderId)
        {
            try
            {
                var hasMandatory = await _unitOfWork.FormAttachmentTypeRepository.HasMandatoryAttachmentsAsync(formBuilderId);
                return new ApiResponse(200, "Mandatory attachments check completed successfully", hasMandatory);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking mandatory attachments: {ex.Message}");
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private FormAttachmentTypeDto ToDto(FORM_ATTACHMENT_TYPES entity)
        {
            if (entity == null) return null;

            return new FormAttachmentTypeDto
            {
                Id = entity.Id,
                FormBuilderId = entity.FormBuilderId,
                AttachmentTypeId = entity.AttachmentTypeId,
                IsMandatory = entity.IsMandatory,
                IsActive = entity.IsActive,
                FormBuilderName = entity.FORM_BUILDER?.FormName,
                AttachmentTypeName = entity.ATTACHMENT_TYPES?.Name,
                AttachmentTypeCode = entity.ATTACHMENT_TYPES?.Code,
                AttachmentTypeMaxSizeMB = entity.ATTACHMENT_TYPES?.MaxSizeMB ?? 0
            };
        }

        private FORM_ATTACHMENT_TYPES ToEntity(CreateFormAttachmentTypeDto dto)
        {
            return new FORM_ATTACHMENT_TYPES
            {
                FormBuilderId = dto.FormBuilderId,
                AttachmentTypeId = dto.AttachmentTypeId,
                IsMandatory = dto.IsMandatory,
                IsActive = dto.IsActive
            };
        }

        private void MapUpdate(UpdateFormAttachmentTypeDto dto, FORM_ATTACHMENT_TYPES entity)
        {
            if (dto.IsMandatory.HasValue)
                entity.IsMandatory = dto.IsMandatory.Value;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;
        }
    }
}