using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Application.DTOs.ApprovalWorkflow;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
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
    public class ApprovalWorkflowService : BaseService<APPROVAL_WORKFLOWS, ApprovalWorkflowDto, ApprovalWorkflowCreateDto, ApprovalWorkflowUpdateDto>, IApprovalWorkflowService
    {
        private readonly IunitOfwork _unitOfWork;

        public ApprovalWorkflowService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<APPROVAL_WORKFLOWS> Repository => _unitOfWork.ApprovalWorkflowRepository;

        public override async Task<ServiceResult<IEnumerable<ApprovalWorkflowDto>>> GetAllAsync(Expression<Func<APPROVAL_WORKFLOWS, bool>>? filter = null)
        {
            var items = await _unitOfWork.ApprovalWorkflowRepository.GetAllAsync(filter,
                x => x.DOCUMENT_TYPES,
                x => x.APPROVAL_STAGES);

            var dtos = _mapper.Map<IEnumerable<ApprovalWorkflowDto>>(items);
            return ServiceResult<IEnumerable<ApprovalWorkflowDto>>.Ok(dtos);
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            return ConvertToApiResponse(result);
        }

        public override async Task<ServiceResult<ApprovalWorkflowDto>> GetByIdAsync(int id, bool asNoTracking = false)
        {
            var entities = await _unitOfWork.ApprovalWorkflowRepository.GetAllAsync(x => x.Id == id,
                x => x.DOCUMENT_TYPES,
                x => x.APPROVAL_STAGES);

            var entity = entities.FirstOrDefault();
            if (entity == null)
                return ServiceResult<ApprovalWorkflowDto>.NotFound();

            var dto = _mapper.Map<ApprovalWorkflowDto>(entity);
            return ServiceResult<ApprovalWorkflowDto>.Ok(dto);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var result = await base.GetByIdAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> CreateAsync(ApprovalWorkflowCreateDto dto)
        {
            var result = await base.CreateAsync(dto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(ApprovalWorkflowCreateDto dto)
        {
            var nameExists = await _unitOfWork.ApprovalWorkflowRepository.AnyAsync(x => x.Name == dto.Name);
            if (nameExists)
                return ValidationResult.Failure("Workflow name already exists");

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> UpdateAsync(int id, ApprovalWorkflowUpdateDto dto)
        {
            var result = await base.UpdateAsync(id, dto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, ApprovalWorkflowUpdateDto dto, APPROVAL_WORKFLOWS entity)
        {
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != entity.Name)
            {
                var exists = await _unitOfWork.ApprovalWorkflowRepository.AnyAsync(x => x.Name == dto.Name && x.Id != id);
                if (exists)
                    return ValidationResult.Failure("Workflow name already exists");
            }

            return ValidationResult.Success();
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

        public async Task<ApiResponse> GetByNameAsync(string name)
        {
            var entity = await _unitOfWork.ApprovalWorkflowRepository.GetByNameAsync(name);
            if (entity == null)
                return new ApiResponse(404, "Workflow not found");

            var dto = _mapper.Map<ApprovalWorkflowDto>(entity);
            return new ApiResponse(200, "Workflow retrieved", dto);
        }

        public async Task<ApiResponse> GetActiveAsync()
        {
            var result = await base.GetActiveAsync();
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            var exists = await _unitOfWork.ApprovalWorkflowRepository.AnyAsync(x => x.Id == id);
            return new ApiResponse(200, "Existence checked", exists);
        }

        public async Task<ApiResponse> NameExistsAsync(string name, int? excludeId = null)
        {
            var exists = await _unitOfWork.ApprovalWorkflowRepository.NameExistsAsync(name, excludeId);
            return new ApiResponse(200, "Name existence checked", exists);
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
