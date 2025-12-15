using FormBuilder.Infrastructure.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domian.Entitys.FromBuilder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Infrastructure.Repositories
{
    public class ApprovalWorkflowRepository :BaseRepository<APPROVAL_WORKFLOWS>, IApprovalWorkflowRepository
    {
        private readonly FormBuilderDbContext _context;

        public ApprovalWorkflowRepository(FormBuilderDbContext context): base(context)
        {
            _context = context;
        }

        public void Add(APPROVAL_WORKFLOWS entity)
        {
            _context.APPROVAL_WORKFLOWS.Add(entity);
        }

        public void Update(APPROVAL_WORKFLOWS entity)
        {
            _context.APPROVAL_WORKFLOWS.Update(entity);
        }

        public void Delete(APPROVAL_WORKFLOWS entity)
        {
            _context.APPROVAL_WORKFLOWS.Remove(entity);
        }

        public async Task<APPROVAL_WORKFLOWS> GetByIdAsync(int id)
        {
            return await _context.APPROVAL_WORKFLOWS
                .Include(w => w.DOCUMENT_TYPES)
                .Include(w => w.APPROVAL_STAGES)
                    .ThenInclude(s => s.APPROVAL_STAGE_ASSIGNEES)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<APPROVAL_WORKFLOWS> GetByNameAsync(string name)
        {
            return await _context.APPROVAL_WORKFLOWS
                .Include(w => w.DOCUMENT_TYPES)
                .Include(w => w.APPROVAL_STAGES)
                .FirstOrDefaultAsync(w => w.Name == name);
        }

        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetAllAsync()
        {
            return await _context.APPROVAL_WORKFLOWS
                .Include(w => w.DOCUMENT_TYPES)
                .Include(w => w.APPROVAL_STAGES)
                    .ThenInclude(s => s.APPROVAL_STAGE_ASSIGNEES)
                .ToListAsync();
        }

        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetActiveAsync()
        {
            return await _context.APPROVAL_WORKFLOWS
                .Where(w => w.IsActive)
                .Include(w => w.DOCUMENT_TYPES)
                .Include(w => w.APPROVAL_STAGES)
                    .ThenInclude(s => s.APPROVAL_STAGE_ASSIGNEES)
                .ToListAsync();
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.APPROVAL_WORKFLOWS.AsQueryable();
            query = query.Where(w => w.Name == name);
            if (excludeId.HasValue)
                query = query.Where(w => w.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<APPROVAL_WORKFLOWS, bool>> predicate)
        {
            return await _context.APPROVAL_WORKFLOWS.AnyAsync(predicate);
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            return await _context.APPROVAL_WORKFLOWS
                .AnyAsync(w => w.Id == id && w.IsActive);
        }
    }
}
