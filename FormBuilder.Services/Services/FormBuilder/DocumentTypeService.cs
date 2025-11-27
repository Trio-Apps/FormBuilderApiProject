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
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IunitOfwork _unitOfWork;

        public DocumentTypeService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var documentTypes = await _unitOfWork.DocumentTypeRepository.GetAllAsync();
                var dtos = documentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "All document types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all document types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var documentType = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(id);
                if (documentType == null)
                    return new ApiResponse(404, "Document type not found");

                var dto = ToDto(documentType);
                return new ApiResponse(200, "Document type retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByCodeAsync(string code)
        {
            try
            {
                var documentType = await _unitOfWork.DocumentTypeRepository.GetByCodeAsync(code);
                if (documentType == null)
                    return new ApiResponse(404, "Document type not found");

                var dto = ToDto(documentType);
                return new ApiResponse(200, "Document type retrieved successfully", dto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var documentTypes = await _unitOfWork.DocumentTypeRepository.GetByFormBuilderIdAsync(formBuilderId);
                var dtos = documentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Document types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveAsync()
        {
            try
            {
                var documentTypes = await _unitOfWork.DocumentTypeRepository.GetActiveAsync();
                var dtos = documentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Active document types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active document types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByParentMenuIdAsync(int? parentMenuId)
        {
            try
            {
                var documentTypes = await _unitOfWork.DocumentTypeRepository.GetByParentMenuIdAsync(parentMenuId);
                var dtos = documentTypes.Select(ToDto).ToList();
                return new ApiResponse(200, "Document types retrieved successfully", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document types: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateDocumentTypeDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if code already exists
                var codeExists = await _unitOfWork.DocumentTypeRepository.CodeExistsAsync(createDto.Code);
                if (codeExists)
                    return new ApiResponse(400, "Document type code already exists");

                // Validate if form builder exists
                if (createDto.FormBuilderId.HasValue)
                {
                    var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(e => e.id == createDto.FormBuilderId.Value);
                    if (!formBuilderExists)
                        return new ApiResponse(400, "Invalid form builder ID");
                }

                // Validate parent menu if provided
                if (createDto.ParentMenuId.HasValue)
                {
                    var parentExists = await _unitOfWork.DocumentTypeRepository.AnyAsync(e => e.id == createDto.ParentMenuId.Value);
                    if (!parentExists)
                        return new ApiResponse(400, "Invalid parent menu ID");
                }

                var entity = ToEntity(createDto);
                _unitOfWork.DocumentTypeRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                var createdEntity = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(entity.id);
                return new ApiResponse(200, "Document type created successfully", ToDto(createdEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating document type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateDocumentTypeDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Document type not found");

                // Check if code already exists (excluding current record)
                if (!string.IsNullOrEmpty(updateDto.Code) && updateDto.Code != entity.Code)
                {
                    var codeExists = await _unitOfWork.DocumentTypeRepository.CodeExistsAsync(updateDto.Code, id);
                    if (codeExists)
                        return new ApiResponse(400, "Document type code already exists");
                }

                // Validate if form builder exists
                if (updateDto.FormBuilderId.HasValue)
                {
                    var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(e => e.id == updateDto.FormBuilderId.Value);
                    if (!formBuilderExists)
                        return new ApiResponse(400, "Invalid form builder ID");
                }

                // Validate parent menu if provided
                if (updateDto.ParentMenuId.HasValue)
                {
                    var parentExists = await _unitOfWork.DocumentTypeRepository.AnyAsync(e => e.id == updateDto.ParentMenuId.Value);
                    if (!parentExists)
                        return new ApiResponse(400, "Invalid parent menu ID");
                }

                MapUpdate(updateDto, entity);
                _unitOfWork.DocumentTypeRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(id);
                return new ApiResponse(200, "Document type updated successfully", ToDto(updatedEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating document type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Document type not found");

                _unitOfWork.DocumentTypeRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Document type deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting document type: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Document type not found");

                entity.IsActive = isActive;
                _unitOfWork.DocumentTypeRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(id);
                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Document type {message} successfully", ToDto(updatedEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling document type active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.DocumentTypeRepository.AnyAsync(e => e.id == id);
                return new ApiResponse(200, "Document type existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking document type existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CodeExistsAsync(string code, int? excludeId = null)
        {
            try
            {
                var exists = await _unitOfWork.DocumentTypeRepository.CodeExistsAsync(code, excludeId);
                return new ApiResponse(200, "Document type code existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking document type code existence: {ex.Message}");
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private DocumentTypeDto ToDto(DOCUMENT_TYPES entity)
        {
            if (entity == null) return null;

            return new DocumentTypeDto
            {
                Id = entity.id,
                Name = entity.Name,
                Code = entity.Code,
                FormBuilderId = entity.FormBuilderId,
                MenuCaption = entity.MenuCaption,
                MenuOrder = entity.MenuOrder,
                ParentMenuId = entity.ParentMenuId,
                IsActive = entity.IsActive,
                FormBuilderName = entity.FORM_BUILDER?.FormName,
                ParentMenuName = entity.ParentMenu?.Name
            };
        }

        private DOCUMENT_TYPES ToEntity(CreateDocumentTypeDto dto)
        {
            return new DOCUMENT_TYPES
            {
                Name = dto.Name,
                Code = dto.Code,
                FormBuilderId = dto.FormBuilderId,
                MenuCaption = dto.MenuCaption,
                MenuOrder = dto.MenuOrder,
                ParentMenuId = dto.ParentMenuId,
                IsActive = dto.IsActive
            };
        }

        private void MapUpdate(UpdateDocumentTypeDto dto, DOCUMENT_TYPES entity)
        {
            if (!string.IsNullOrEmpty(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Code))
                entity.Code = dto.Code;

            if (dto.FormBuilderId.HasValue)
                entity.FormBuilderId = dto.FormBuilderId.Value;

            if (!string.IsNullOrEmpty(dto.MenuCaption))
                entity.MenuCaption = dto.MenuCaption;

            if (dto.MenuOrder.HasValue)
                entity.MenuOrder = dto.MenuOrder.Value;

            if (dto.ParentMenuId.HasValue)
                entity.ParentMenuId = dto.ParentMenuId.Value;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;
        }
    }
}