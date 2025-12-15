using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Services.Services.Base
{
    /// <summary>
    /// Generic base service to reduce duplicated CRUD logic.
    /// </summary>
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto>
        where TEntity : BaseEntity
    {
        protected readonly IunitOfwork _unitOfWork;
        protected readonly IMapper _mapper;

        protected BaseService(IunitOfwork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected abstract IBaseRepository<TEntity> Repository { get; }

        public virtual async Task<ServiceResult<IEnumerable<TDto>>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            var data = await Repository.GetAllAsync(filter);
            var mapped = _mapper.Map<IEnumerable<TDto>>(data);
            return ServiceResult<IEnumerable<TDto>>.Ok(mapped);
        }

        public virtual async Task<ServiceResult<PagedResult<TDto>>> GetPagedAsync(int page = 1, int pageSize = 20, Expression<Func<TEntity, bool>>? filter = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var query = Repository.GetAll();
            if (filter != null) query = query.Where(filter);

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<IEnumerable<TDto>>(entities);
            var paged = new PagedResult<TDto>(mapped, totalCount, page, pageSize);

            return ServiceResult<PagedResult<TDto>>.Ok(paged);
        }

        public virtual async Task<ServiceResult<TDto>> GetByIdAsync(int id, bool asNoTracking = false)
        {
            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == id, asNoTracking);
            if (entity == null) return ServiceResult<TDto>.NotFound();

            return ServiceResult<TDto>.Ok(_mapper.Map<TDto>(entity));
        }

        public virtual async Task<ServiceResult<TDto>> CreateAsync(TCreateDto dto)
        {
            if (dto == null) return ServiceResult<TDto>.BadRequest("Payload is required");

            var validation = await ValidateCreateAsync(dto);
            if (!validation.IsValid) return ServiceResult<TDto>.BadRequest(validation.ErrorMessage ?? "Validation failed");

            var entity = _mapper.Map<TEntity>(dto);
            entity.CreatedDate = entity.CreatedDate == default ? DateTime.UtcNow : entity.CreatedDate;
            entity.IsActive = true;

            Repository.Add(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<TDto>.Ok(_mapper.Map<TDto>(entity));
        }

        public virtual async Task<ServiceResult<TDto>> UpdateAsync(int id, TUpdateDto dto)
        {
            if (dto == null) return ServiceResult<TDto>.BadRequest("Payload is required");

            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null) return ServiceResult<TDto>.NotFound();

            var validation = await ValidateUpdateAsync(id, dto, entity);
            if (!validation.IsValid) return ServiceResult<TDto>.BadRequest(validation.ErrorMessage ?? "Validation failed");

            _mapper.Map(dto, entity);
            entity.UpdatedDate = DateTime.UtcNow;

            Repository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<TDto>.Ok(_mapper.Map<TDto>(entity));
        }

        public virtual async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null) return ServiceResult<bool>.NotFound();

            Repository.Delete(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }

        protected virtual Task<ValidationResult> ValidateCreateAsync(TCreateDto dto) => Task.FromResult(ValidationResult.Success());

        protected virtual Task<ValidationResult> ValidateUpdateAsync(int id, TUpdateDto dto, TEntity entity) =>
            Task.FromResult(ValidationResult.Success());
    }
}

