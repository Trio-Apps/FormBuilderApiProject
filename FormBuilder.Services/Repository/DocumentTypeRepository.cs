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
    public class DocumentTypeRepository : BaseRepository<DOCUMENT_TYPES>, IDocumentTypeRepository
    {
        public FormBuilderDbContext _context { get; }

        public DocumentTypeRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DOCUMENT_TYPES> GetByIdAsync(int id)
        {
            return await _context.DOCUMENT_TYPES
                .Include(dt => dt.FORM_BUILDER)
                .Include(dt => dt.ParentMenu)
                .FirstOrDefaultAsync(dt => dt.id == id);
        }

        public async Task<DOCUMENT_TYPES> GetByCodeAsync(string code)
        {
            return await _context.DOCUMENT_TYPES
                .Include(dt => dt.FORM_BUILDER)
                .FirstOrDefaultAsync(dt => dt.Code == code && dt.IsActive);
        }

        public async Task<IEnumerable<DOCUMENT_TYPES>> GetByFormBuilderIdAsync(int formBuilderId)
        {
            return await _context.DOCUMENT_TYPES
                .Include(dt => dt.FORM_BUILDER)
                .Where(dt => dt.FormBuilderId == formBuilderId)
                .OrderBy(dt => dt.MenuOrder)
                .ThenBy(dt => dt.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DOCUMENT_TYPES>> GetActiveAsync()
        {
            return await _context.DOCUMENT_TYPES
                .Include(dt => dt.FORM_BUILDER)
                .Include(dt => dt.ParentMenu)
                .Where(dt => dt.IsActive)
                .OrderBy(dt => dt.MenuOrder)
                .ThenBy(dt => dt.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DOCUMENT_TYPES>> GetByParentMenuIdAsync(int? parentMenuId)
        {
            return await _context.DOCUMENT_TYPES
                .Include(dt => dt.FORM_BUILDER)
                .Where(dt => dt.ParentMenuId == parentMenuId && dt.IsActive)
                .OrderBy(dt => dt.MenuOrder)
                .ThenBy(dt => dt.Name)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.DOCUMENT_TYPES.Where(dt => dt.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(dt => dt.id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            return await _context.DOCUMENT_TYPES
                .AnyAsync(dt => dt.id == id && dt.IsActive);
        }

        public async Task<IEnumerable<DOCUMENT_TYPES>> GetMenuItemsAsync()
        {
            return await _context.DOCUMENT_TYPES
                .Include(dt => dt.FORM_BUILDER)
                .Include(dt => dt.Children)
                .Where(dt => dt.IsActive)
                .OrderBy(dt => dt.MenuOrder)
                .ThenBy(dt => dt.Name)
                .ToListAsync();
        }
    }
}