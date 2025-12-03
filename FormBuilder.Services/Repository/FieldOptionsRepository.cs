using DocumentFormat.OpenXml.InkML;
using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Infrastructure.Repositories
{
    public class FieldOptionsRepository : BaseRepository<FIELD_OPTIONS>, IFieldOptionsRepository
    {
        public FormBuilderDbContext _context { get; }

        // REMOVE this line: private readonly FormBuilderDbContext _context;

        public FieldOptionsRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context;
            // The base constructor already sets up _context
        }

        public async Task<IEnumerable<FIELD_OPTIONS>> GetByFieldIdAsync(int fieldId)
        {
            return await _context.FIELD_OPTIONS
                .Where(fo => fo.FieldId == fieldId)
                .OrderBy(fo => fo.OptionOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FIELD_OPTIONS>> GetActiveByFieldIdAsync(int fieldId)
        {
            return await _context.FIELD_OPTIONS
                .Where(fo => fo.FieldId == fieldId && fo.IsActive)
                .OrderBy(fo => fo.OptionOrder)
                .ToListAsync();
        }

        public async Task<FIELD_OPTIONS> GetDefaultOptionAsync(int fieldId)
        {
            return await _context.FIELD_OPTIONS
                .FirstOrDefaultAsync(fo => fo.FieldId == fieldId && fo.IsDefault && fo.IsActive);
        }

        public async Task<bool> FieldHasOptionsAsync(int fieldId)
        {
            return await _context.FIELD_OPTIONS
                .AnyAsync(fo => fo.FieldId == fieldId && fo.IsActive);
        }

        public async Task<int> GetOptionsCountAsync(int fieldId)
        {
            return await _context.FIELD_OPTIONS
                .CountAsync(fo => fo.FieldId == fieldId && fo.IsActive);
        }

        // FIXED: Use the base class method instead of custom implementation
        public  async Task<FIELD_OPTIONS> GetByIdAsync(int id)
        {
            return await _context.FIELD_OPTIONS.FindAsync(id);
        }

        // FIXED: Check FIELD_OPTIONS table, not FORM_FIELDS
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.FIELD_OPTIONS.AnyAsync(x => x.Id == id);
        }
    }
}