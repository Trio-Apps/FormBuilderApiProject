using FormBuilder.Infrastructure.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Repository
{
    public class FieldTypesRepository : BaseRepository<FIELD_TYPES>, IFieldTypesRepository
    {
        private readonly FormBuilderDbContext _context;

        public FieldTypesRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<FIELD_TYPES>> GetActiveFieldTypesAsync()
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.IsActive)
                .OrderBy(ft => ft.TypeName)
                .ToListAsync();
        }

        public async Task<FIELD_TYPES> GetByTypeNameAsync(string typeName)
        {
            return await _context.FIELD_TYPES
                .FirstOrDefaultAsync(ft => ft.TypeName == typeName && ft.IsActive);
        }

        public async Task<IEnumerable<FIELD_TYPES>> GetFieldTypesWithOptionsAsync()
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.HasOptions && ft.IsActive)
                .OrderBy(ft => ft.TypeName)
                .ToListAsync();
        }

        public async Task<IEnumerable<FIELD_TYPES>> GetByDataTypeAsync(string dataType)
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.DataType == dataType && ft.IsActive)
                .OrderBy(ft => ft.TypeName)
                .ToListAsync();
        }

        public async Task<bool> IsTypeNameUniqueAsync(string typeName, int? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return false;

            return !await _context.FIELD_TYPES
                .AnyAsync(ft => ft.TypeName == typeName.Trim() &&
                               (!ignoreId.HasValue || ft.Id != ignoreId.Value));
        }

        public async Task<IEnumerable<FIELD_TYPES>> GetFieldTypesWithMultipleValuesAsync()
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.AllowMultiple && ft.IsActive)
                .OrderBy(ft => ft.TypeName)
                .ToListAsync();
        }

        public async Task<IEnumerable<FIELD_TYPES>> GetFieldTypesForDropdownAsync()
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.IsActive)
                .OrderBy(ft => ft.TypeName)
                .Select(ft => new FIELD_TYPES
                {
                    Id = ft.Id,
                    TypeName = ft.TypeName,
                    DataType = ft.DataType,
                    HasOptions = ft.HasOptions,
                    AllowMultiple = ft.AllowMultiple
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetFieldTypesWithUsageCountAsync()
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.IsActive)
                .OrderBy(ft => ft.TypeName)
                .Select(ft => new
                {
                    FieldType = ft,
                    FormFieldsCount = ft.FORM_FIELDS.Count(f => f.IsActive),
                    GridColumnsCount = ft.FORM_GRID_COLUMNS.Count(g => g.IsActive)
                })
                .ToListAsync();
        }

        // Additional utility methods
        public async Task<IEnumerable<FIELD_TYPES>> GetBasicFieldTypesAsync()
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.IsActive && !ft.HasOptions && !ft.AllowMultiple)
                .OrderBy(ft => ft.TypeName)
                .ToListAsync();
        }

        public async Task<IEnumerable<FIELD_TYPES>> GetAdvancedFieldTypesAsync()
        {
            return await _context.FIELD_TYPES
                .Where(ft => ft.IsActive && (ft.HasOptions || ft.AllowMultiple))
                .OrderBy(ft => ft.TypeName)
                .ToListAsync();
        }

        public async Task<FIELD_TYPES> GetByIdAsync(int id, params Expression<Func<FIELD_TYPES, object>>[] includes)
        {
            IQueryable<FIELD_TYPES> query = _context.FIELD_TYPES;

            // إضافة الـ includes إذا وجدت
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // البحث حسب الـ id
            return await query.FirstOrDefaultAsync(ft => ft.Id == id && ft.IsActive);
        }

    }
}