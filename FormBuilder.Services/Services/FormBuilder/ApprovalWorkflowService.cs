using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Application.DTOs.ApprovalWorkflow;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FormBuilder.Services
{
    public class ApprovalWorkflowService : IApprovalWorkflowService
    {
        private readonly IunitOfwork _unitOfWork;

        public ApprovalWorkflowService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var items = await _unitOfWork.ApprovalWorkflowRepository.GetAllAsync(null,
                    x => x.DOCUMENT_TYPES,
                    x => x.APPROVAL_STAGES);

                var dtos = items.Select(ToDto).ToList();
                return new ApiResponse(200, "All workflows retrieved", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var entities = await _unitOfWork.ApprovalWorkflowRepository.GetAllAsync(x => x.Id == id,
                    x => x.DOCUMENT_TYPES,
                    x => x.APPROVAL_STAGES);

                var entity = entities.FirstOrDefault();
                if (entity == null)
                    return new ApiResponse(404, "Workflow not found");

                return new ApiResponse(200, "Workflow retrieved", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(ApprovalWorkflowCreateDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse(400, "DTO required");

                var nameExists = await _unitOfWork.ApprovalWorkflowRepository.AnyAsync(x => x.Name == dto.Name);
                if (nameExists)
                    return new ApiResponse(400, "Workflow name already exists");

                var entity = new APPROVAL_WORKFLOWS
                {
                    Name = dto.Name,
                    DocumentTypeId = dto.DocumentTypeId,
                    IsActive = dto.IsActive
                };

                _unitOfWork.ApprovalWorkflowRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Workflow created", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, ApprovalWorkflowUpdateDto dto)
        {
            try
            {
                var entities = await _unitOfWork.ApprovalWorkflowRepository.GetAllAsync(x => x.Id == id);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                    return new ApiResponse(404, "Workflow not found");

                if (!string.IsNullOrEmpty(dto.Name) && dto.Name != entity.Name)
                {
                    var exists = await _unitOfWork.ApprovalWorkflowRepository.AnyAsync(x => x.Name == dto.Name && x.Id != id);
                    if (exists)
                        return new ApiResponse(400, "Workflow name already exists");
                }

                if (!string.IsNullOrEmpty(dto.Name))
                    entity.Name = dto.Name;

                if (dto.DocumentTypeId.HasValue)
                    entity.DocumentTypeId = dto.DocumentTypeId.Value;

                if (dto.IsActive.HasValue)
                    entity.IsActive = dto.IsActive.Value;

                _unitOfWork.ApprovalWorkflowRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Workflow updated", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entities = await _unitOfWork.ApprovalWorkflowRepository.GetAllAsync(x => x.Id == id);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                    return new ApiResponse(404, "Workflow not found");

                _unitOfWork.ApprovalWorkflowRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Workflow deleted");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entities = await _unitOfWork.ApprovalWorkflowRepository.GetAllAsync(x => x.Id == id);
                var entity = entities.FirstOrDefault();
                if (entity == null)
                    return new ApiResponse(404, "Workflow not found");

                entity.IsActive = isActive;
                _unitOfWork.ApprovalWorkflowRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Status updated", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByNameAsync(string name)
        {
            try
            {
                var entity = await _unitOfWork.ApprovalWorkflowRepository.GetByNameAsync(name);
                if (entity == null)
                    return new ApiResponse(404, "Workflow not found");

                return new ApiResponse(200, "Workflow retrieved", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveAsync()
        {
            try
            {
                var items = await _unitOfWork.ApprovalWorkflowRepository.GetActiveAsync();
                var dtos = items.Select(ToDto).ToList();
                return new ApiResponse(200, "Active workflows retrieved", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.ApprovalWorkflowRepository.AnyAsync(x => x.Id == id);
                return new ApiResponse(200, "Existence checked", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> NameExistsAsync(string name, int? excludeId = null)
        {
            try
            {
                var exists = await _unitOfWork.ApprovalWorkflowRepository.NameExistsAsync(name, excludeId);
                return new ApiResponse(200, "Name existence checked", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        // ===============================
        //          MAPPING
        // ===============================
        private ApprovalWorkflowDto ToDto(APPROVAL_WORKFLOWS e)
        {
            return new ApprovalWorkflowDto
            {
                Id = e.Id,
                Name = e.Name,
                DocumentTypeId = e.DocumentTypeId,
                DocumentTypeName = e.DOCUMENT_TYPES?.Name,
                IsActive = e.IsActive,
                Stages = e.APPROVAL_STAGES?.Select(s => new ApprovalStageDto
                {
                    Id = s.Id,
                    StageName = s.StageName,
                    StageOrder = s.StageOrder,
                    MinAmount = s.MinAmount,
                    MaxAmount = s.MaxAmount,
                    IsFinalStage = s.IsFinalStage
                }).ToList()
            };
        }
    }
}
