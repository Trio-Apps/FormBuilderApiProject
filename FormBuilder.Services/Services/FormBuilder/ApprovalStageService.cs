using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Application.DTOs.ApprovalWorkflow;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Services.Services.Base;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class ApprovalStageService : BaseService<APPROVAL_STAGES, ApprovalStageDto, ApprovalStageCreateDto, ApprovalStageUpdateDto>, IApprovalStageService
    {
        private readonly IunitOfwork _unitOfWork;

        public ApprovalStageService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<APPROVAL_STAGES> Repository => _unitOfWork.ApprovalStageRepository;

        public async Task<ApiResponse> GetAllAsync(int workflowId)
        {
            Expression<Func<APPROVAL_STAGES, bool>> filter = workflowId > 0 
                ? (s => s.WorkflowId == workflowId) 
                : null;
            
            var result = await base.GetAllAsync(filter);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var result = await base.GetByIdAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> CreateAsync(ApprovalStageCreateDto dto)
        {
            var result = await base.CreateAsync(dto);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> UpdateAsync(int id, ApprovalStageUpdateDto dto)
        {
            var result = await base.UpdateAsync(id, dto);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            var result = await base.ToggleActiveAsync(id, isActive);
            return ConvertToApiResponse(result);
        }

        // ===============================
        //          HELPER METHODS
        // ===============================
        private ApiResponse ConvertToApiResponse<T>(ServiceResult<T> result)
        {
            if (result.Success)
                return new ApiResponse(result.StatusCode, "Success", result.Data);
            else
                return new ApiResponse(result.StatusCode, result.ErrorMessage);
        }
    }
}
