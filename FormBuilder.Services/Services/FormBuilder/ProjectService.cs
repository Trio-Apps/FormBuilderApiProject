using AutoMapper;
using FormBuilder.API.Models.DTOs;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using formBuilder.Domian.Interfaces;
using FormBuilder.Services.Services.Base;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class ProjectService
        : BaseService<PROJECTS, ProjectDto, CreateProjectDto, UpdateProjectDto>,
          IProjectService
    {
        private readonly IStringLocalizer<ProjectService>? _localizer;

        public ProjectService(IunitOfwork unitOfWork, IMapper mapper, IStringLocalizer<ProjectService>? localizer = null)
            : base(unitOfWork, mapper, null)
        {
            _localizer = localizer;
        }

        protected override IBaseRepository<PROJECTS> Repository => _unitOfWork.ProjectRepository;

        public async Task<ServiceResult<IEnumerable<ProjectDto>>> GetAllAsync(Expression<Func<PROJECTS, bool>>? filter = null)
            => await base.GetAllAsync(filter);

        public async Task<ServiceResult<PagedResult<ProjectDto>>> GetPagedAsync(int page = 1, int pageSize = 20, Expression<Func<PROJECTS, bool>>? filter = null)
            => await base.GetPagedAsync(page, pageSize, filter);

        public async Task<ServiceResult<ProjectDto>> GetByIdAsync(int id, bool asNoTracking = false)
            => await base.GetByIdAsync(id, asNoTracking);

        public async Task<ServiceResult<ProjectDto>> GetByCodeAsync(string code, bool asNoTracking = false)
        {
            if (string.IsNullOrWhiteSpace(code))
                return ServiceResult<ProjectDto>.BadRequest("Project code is required");

            var entity = await Repository.SingleOrDefaultAsync(p => p.Code == code.Trim(), asNoTracking);
            if (entity == null) return ServiceResult<ProjectDto>.NotFound();

            return ServiceResult<ProjectDto>.Ok(_mapper.Map<ProjectDto>(entity));
        }

        public async Task<ServiceResult<IEnumerable<ProjectDto>>> GetActiveAsync()
        {
            return await base.GetAllAsync(p => p.IsActive);
        }

        public override async Task<ServiceResult<ProjectDto>> CreateAsync(CreateProjectDto createDto)
        {
            return await base.CreateAsync(createDto);
        }

        public override async Task<ServiceResult<ProjectDto>> UpdateAsync(int id, UpdateProjectDto updateDto)
        {
            return await base.UpdateAsync(id, updateDto);
        }

        public override async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }

        public async Task<ServiceResult<bool>> ToggleActiveAsync(int id, bool isActive)
        {
            var entity = await Repository.SingleOrDefaultAsync(p => p.Id == id);
            if (entity == null) return ServiceResult<bool>.NotFound();

            entity.IsActive = isActive;
            entity.UpdatedDate = DateTime.UtcNow;
            Repository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ExistsAsync(int id)
        {
            var exists = await Repository.AnyAsync(p => p.Id == id);
            return ServiceResult<bool>.Ok(exists);
        }

        public async Task<ServiceResult<bool>> CodeExistsAsync(string code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                var message = _localizer?["Project_CodeRequired"] ?? "Project code is required";
                return ServiceResult<bool>.BadRequest(message);
            }

            var exists = await _unitOfWork.ProjectRepository.CodeExistsAsync(code.Trim(), excludeId);
            return ServiceResult<bool>.Ok(exists);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateProjectDto dto)
        {
            if (dto == null)
            {
                var message = _localizer?["Common_PayloadRequired"] ?? "Payload is required";
                return ValidationResult.Failure(message);
            }

            var exists = await _unitOfWork.ProjectRepository.CodeExistsAsync(dto.Code);
            if (exists)
            {
                var message = _localizer?["Project_CodeExists", dto.Code] ?? $"Project code '{dto.Code}' already exists.";
                return ValidationResult.Failure(message);
            }

            return ValidationResult.Success();
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateProjectDto dto, PROJECTS entity)
        {
            if (dto == null)
            {
                var message = _localizer?["Common_PayloadRequired"] ?? "Payload is required";
                return ValidationResult.Failure(message);
            }

            if (!string.IsNullOrWhiteSpace(dto.Code) && !string.Equals(dto.Code, entity.Code, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _unitOfWork.ProjectRepository.CodeExistsAsync(dto.Code, id);
                if (exists)
                {
                    var message = _localizer?["Project_CodeExists", dto.Code] ?? $"Project code '{dto.Code}' already exists.";
                    return ValidationResult.Failure(message);
                }
            }

            return ValidationResult.Success();
        }
    }
}