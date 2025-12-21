using formBuilder.Domian.Entitys;
using FormBuilder.Infrastructure.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Infrastructure.Repositories
{
    public class FormFieldRepository : BaseRepository<FORM_FIELDS>, IFormFieldRepository
    {
        private readonly FormBuilderDbContext _context;

        public FormFieldRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context;
        }

        // Field Code Validation
        public async Task<bool> IsFieldCodeUniqueAsync(string fieldCode, int? ignoreId = null)
        {
            var query = _context.FORM_FIELDS
                .Where(f => f.FieldCode == fieldCode && f.IsActive);

            if (ignoreId.HasValue)
            {
                query = query.Where(f => f.Id != ignoreId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsFieldNameUniqueAsync(string fieldName, int? ignoreId = null, int? tabId = null)
        {
            var query = _context.FORM_FIELDS
                .Where(f => f.FieldName == fieldName && f.IsActive);

            if (ignoreId.HasValue)
            {
                query = query.Where(f => f.Id != ignoreId.Value);
            }

            if (tabId.HasValue)
            {
                query = query.Where(f => f.TabId == tabId.Value);
            }

            return !await query.AnyAsync();
        }

        // Specialized Queries
        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByTabIdAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .Include(f => f.FIELD_OPTIONS)
                .Include(f => f.FIELD_DATA_SOURCES)
                .Where(f => f.TabId == tabId && f.IsActive)
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByFormIdAsync(int formBuilderId)
        {
            return await _context.FORM_FIELDS
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .Include(f => f.FIELD_OPTIONS)
                .Include(f => f.FIELD_DATA_SOURCES)
                .Where(f => f.FORM_TABS.FormBuilderId == formBuilderId && f.IsActive)
                .OrderBy(f => f.FORM_TABS.TabOrder)
                .ThenBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetMandatoryFieldsAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .Where(f => f.TabId == tabId && (f.IsMandatory ?? false) && f.IsActive)
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetVisibleFieldsAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .Where(f => f.TabId == tabId && f.IsVisible && f.IsActive)
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByGridIdAsync(int gridId)
        {
            return await _context.FORM_FIELDS
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .Include(f => f.Grid)
                .Include(f => f.FIELD_OPTIONS)
                .Where(f => f.GridId == gridId && f.IsActive)
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        // Get by ID with included entities
        public async Task<FORM_FIELDS?> GetByIdAsync(int id, params Expression<Func<FIELD_TYPES, object>>[] includes)
        {
            var query = _context.FORM_FIELDS
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_OPTIONS)
                .Include(f => f.FIELD_DATA_SOURCES)
                .Include(f => f.FIELD_TYPES)
                .Where(f => f.Id == id && f.IsActive);

            return await query.FirstOrDefaultAsync();
        }

        // Note: Base GetByIdAsync and ExistsAsync are inherited from BaseRepository
        // This overload provides additional includes for specialized queries

        // Override the base GetAllAsync to include related entities and filtering
        public override async Task<ICollection<FORM_FIELDS>> GetAllAsync(Expression<Func<FORM_FIELDS, bool>>? filter = null, params Expression<Func<FORM_FIELDS, object>>[] includes)
        {
            var query = _context.FORM_FIELDS
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_OPTIONS)
                .Include(f => f.FIELD_DATA_SOURCES)
                .Include(f => f.FIELD_TYPES)
                .Where(f => f.IsActive)
                .OrderBy(f => f.FieldOrder)
                .AsQueryable();

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply additional includes if provided
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }
    }
}