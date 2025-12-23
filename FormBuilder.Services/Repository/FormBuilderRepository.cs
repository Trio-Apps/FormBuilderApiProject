using formBuilder.Domian.Interfaces;
using FormBuilder.Infrastructure.Data;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.core;
using FormBuilder.Domian.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Repository
{
    public class FormBuilderRepository
        : BaseRepository<FORM_BUILDER>, IFormBuilderRepository
    {
        private readonly FormBuilderDbContext _context;

        public FormBuilderRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsFormCodeExistsAsync(string formCode, int? excludeId = null)
        {
            return await _context.FORM_BUILDER
                .AnyAsync(f => f.FormCode == formCode &&
                               (!excludeId.HasValue || f.Id != excludeId));
        }

        public async Task<FORM_BUILDER?> GetFormWithTabsAndFieldsByCodeAsync(string formCode)
        {
            if (string.IsNullOrWhiteSpace(formCode))
            {
                return null;
            }

            var normalizedCode = formCode.Trim();

            return await _context.FORM_BUILDER
                .AsNoTracking()
                .Include(f => f.FORM_TABS.Where(t => t.IsActive))
                    .ThenInclude(t => t.FORM_FIELDS.Where(ff => ff.IsActive))
                        .ThenInclude(ff => ff.FIELD_TYPES)
                .Include(f => f.FORM_TABS.Where(t => t.IsActive))
                    .ThenInclude(t => t.FORM_FIELDS.Where(ff => ff.IsActive))
                        .ThenInclude(ff => ff.FIELD_OPTIONS)
                .Include(f => f.FORM_TABS.Where(t => t.IsActive))
                    .ThenInclude(t => t.FORM_FIELDS.Where(ff => ff.IsActive))
                        .ThenInclude(ff => ff.FIELD_DATA_SOURCES.Where(fds => fds.IsActive))
                .FirstOrDefaultAsync(f => f.FormCode == normalizedCode && f.IsActive && f.IsPublished);
        }
    }
}
