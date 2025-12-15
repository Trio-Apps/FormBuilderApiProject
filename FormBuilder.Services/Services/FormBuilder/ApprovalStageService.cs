using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Application.DTOs.ApprovalWorkflow;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class ApprovalStageService : IApprovalStageService
    {
        private readonly IunitOfwork _unitOfWork;

        public ApprovalStageService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync(int workflowId)
        {
            try
            {
                var stages = await _unitOfWork.ApprovalStageRepository.GetAllAsync();
                var dtos = stages.Select(ToDto).ToList();
                return new ApiResponse(200, "Stages retrieved", dtos);
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
                var stage = await _unitOfWork.ApprovalStageRepository.GetByIdAsync(id);
                if (stage == null)
                    return new ApiResponse(404, "Stage not found");

                return new ApiResponse(200, "Stage retrieved", ToDto(stage));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(ApprovalStageCreateDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse(400, "DTO required");

                var entity = new APPROVAL_STAGES
                {
                    WorkflowId = dto.WorkflowId,
                    StageName = dto.StageName,
                    StageOrder = dto.StageOrder,
                    MinAmount = dto.MinAmount,
                    MaxAmount = dto.MaxAmount,
                    IsFinalStage = dto.IsFinalStage,
                    IsActive = dto.IsActive
                };

                _unitOfWork.ApprovalStageRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Stage created", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, ApprovalStageUpdateDto dto)
        {
            try
            {
                var stage = await _unitOfWork.ApprovalStageRepository.GetByIdAsync(id);
                if (stage == null)
                    return new ApiResponse(404, "Stage not found");

                if (!string.IsNullOrEmpty(dto.StageName))
                    stage.StageName = dto.StageName;

                if (dto.WorkflowId.HasValue)
                    stage.WorkflowId = dto.WorkflowId.Value;

                if (dto.StageOrder.HasValue)
                    stage.StageOrder = dto.StageOrder.Value;

                if (dto.MinAmount.HasValue)
                    stage.MinAmount = dto.MinAmount;

                if (dto.MaxAmount.HasValue)
                    stage.MaxAmount = dto.MaxAmount;

                if (dto.IsFinalStage.HasValue)
                    stage.IsFinalStage = dto.IsFinalStage.Value;

                if (dto.IsActive.HasValue)
                    stage.IsActive = dto.IsActive.Value;

                _unitOfWork.ApprovalStageRepository.Update(stage);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Stage updated", ToDto(stage));
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
                var stage = await _unitOfWork.ApprovalStageRepository.GetByIdAsync(id);
                if (stage == null)
                    return new ApiResponse(404, "Stage not found");

                _unitOfWork.ApprovalStageRepository.Delete(stage);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Stage deleted");
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
                var stage = await _unitOfWork.ApprovalStageRepository.GetByIdAsync(id);
                if (stage == null)
                    return new ApiResponse(404, "Stage not found");

                stage.IsActive = isActive;
                _unitOfWork.ApprovalStageRepository.Update(stage);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Status updated", ToDto(stage));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        // ===============================
        //          MAPPING
        // ===============================
        private ApprovalStageDto ToDto(APPROVAL_STAGES s)
        {
            return new ApprovalStageDto
            {
                Id = s.Id,
                WorkflowId = s.WorkflowId,
                StageName = s.StageName,
                StageOrder = s.StageOrder,
                MinAmount = s.MinAmount,
                MaxAmount = s.MaxAmount,
                IsFinalStage = s.IsFinalStage,
                IsActive = s.IsActive,
                WorkflowName = s.APPROVAL_WORKFLOWS?.Name
            };
        }
    }
}
