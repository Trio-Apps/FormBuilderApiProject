using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Infrastructure.Repositories
{
    public class ProjectRepository : BaseRepository<PROJECTS>, IProjectRepository
    {
        public FormBuilderDbContext _context { get; }

        public ProjectRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PROJECTS> GetByIdAsync(int id)
        {
            return await _context.PROJECTS
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PROJECTS> GetByCodeAsync(string code)
        {
            return await _context.PROJECTS
                .FirstOrDefaultAsync(p => p.Code == code && p.IsActive);
        }

        public async Task<IEnumerable<PROJECTS>> GetActiveAsync()
        {
            return await _context.PROJECTS
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.PROJECTS.Where(p => p.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            return await _context.PROJECTS
                .AnyAsync(p => p.Id == id && p.IsActive);
        }
    }
}