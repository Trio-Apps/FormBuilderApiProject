using FormBuilder.API.Data;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Infrastructure.Repositories
{
    public class ApprovalStageRepository : IApprovalStageRepository
    {
        private readonly FormBuilderDbContext _context;

        public ApprovalStageRepository(FormBuilderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<APPROVAL_STAGES>> GetAllAsync()
        {
            return await _context.APPROVAL_STAGES
                .Include(s => s.APPROVAL_WORKFLOWS)
                .ToListAsync();
        }

        public async Task<APPROVAL_STAGES> GetByIdAsync(int id)
        {
            return await _context.APPROVAL_STAGES
                .Include(s => s.APPROVAL_WORKFLOWS)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public void Add(APPROVAL_STAGES entity)
        {
            _context.APPROVAL_STAGES.Add(entity);
        }

        public void Update(APPROVAL_STAGES entity)
        {
            _context.APPROVAL_STAGES.Update(entity);
        }

        public void Delete(APPROVAL_STAGES entity)
        {
            _context.APPROVAL_STAGES.Remove(entity);
        }

        public async Task<bool> AnyAsync(int id)
        {
            return await _context.APPROVAL_STAGES.AnyAsync(s => s.Id == id);
        }
    }
}
