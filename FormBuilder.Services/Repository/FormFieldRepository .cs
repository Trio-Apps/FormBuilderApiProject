using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Repository
{
    public class FormFieldRepository : BaseRepository<FORM_FIELDS>, IFormFieldRepository
    {
        private readonly FormBuilderDbContext _context;

        public FormFieldRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> IsFieldCodeUniqueAsync(string fieldCode, int? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(fieldCode))
                return false;

            return !await _context.FORM_FIELDS
                .AnyAsync(f => f.FieldCode == fieldCode.Trim() &&
                              (!ignoreId.HasValue || f.id != ignoreId.Value));
        }

        public async Task<bool> IsFieldNameUniqueAsync(string fieldName, int? ignoreId = null, int? tabId = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return false;

            var query = _context.FORM_FIELDS.AsQueryable();

            if (tabId.HasValue)
                query = query.Where(f => f.TabId == tabId.Value);

            return !await query
                .AnyAsync(f => f.FieldName == fieldName.Trim() &&
                              (!ignoreId.HasValue || f.id != ignoreId.Value));
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByTabIdAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.TabId == tabId && f.IsActive)
                .Include(f => f.FIELD_TYPES)
                .Include(f => f.FIELD_OPTIONS)
                .OrderBy(f => f.FieldOrder)
                .ThenBy(f => f.FieldName)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByFormIdAsync(int formBuilderId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.FORM_TABS.FormBuilderId == formBuilderId && f.IsActive)
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .OrderBy(f => f.FORM_TABS.TabOrder)
                .ThenBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetMandatoryFieldsAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.TabId == tabId && f.IsMandatory && f.IsActive)
                .Include(f => f.FIELD_TYPES)
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetVisibleFieldsAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.TabId == tabId && f.IsVisible && f.IsActive)
                .Include(f => f.FIELD_TYPES)
                .Include(f => f.FIELD_OPTIONS)
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByDataTypeAsync(string dataType)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.DataType == dataType && f.IsActive)
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .OrderBy(f => f.FieldName)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByFieldTypeAsync(int fieldTypeId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.FieldTypeId == fieldTypeId && f.IsActive)
                .Include(f => f.FORM_TABS)
                .OrderBy(f => f.FieldName)
                .ToListAsync();
        }

        public async Task<int> GetFieldsCountByTabAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .CountAsync(f => f.TabId == tabId && f.IsActive);
        }

        public async Task<int> GetFieldsCountByFormAsync(int formBuilderId)
        {
            return await _context.FORM_FIELDS
                .CountAsync(f => f.FORM_TABS.FormBuilderId == formBuilderId && f.IsActive);
        }

        public async Task<bool> UpdateFieldOrderAsync(int fieldId, int newOrder)
        {
            var field = await _context.FORM_FIELDS.FindAsync(fieldId);
            if (field == null)
                return false;

            field.FieldOrder = newOrder;
            field.UpdatedDate = DateTime.UtcNow;

            _context.FORM_FIELDS.Update(field);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateFieldsOrderAsync(Dictionary<int, int> fieldOrders)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var (fieldId, newOrder) in fieldOrders)
                {
                    var field = await _context.FORM_FIELDS.FindAsync(fieldId);
                    if (field != null)
                    {
                        field.FieldOrder = newOrder;
                        field.UpdatedDate = DateTime.UtcNow;
                        _context.FORM_FIELDS.Update(field);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsWithValidationRulesAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.TabId == tabId && f.IsActive &&
                           (!string.IsNullOrEmpty(f.RegexPattern) ||
                            !string.IsNullOrEmpty(f.ValidationMessage) ||
                            f.MinValue.HasValue ||
                            f.MaxValue.HasValue ||
                            f.MaxLength.HasValue))
                .Include(f => f.FIELD_TYPES)
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        // Additional specialized methods
        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsWithOptionsAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.TabId == tabId && f.IsActive)
                .Include(f => f.FIELD_OPTIONS)
                .Include(f => f.FIELD_TYPES)
                .Where(f => f.FIELD_OPTIONS.Any())
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsWithDataSourceAsync(int tabId)
        {
            return await _context.FORM_FIELDS
                .Where(f => f.TabId == tabId && f.IsActive)
                .Include(f => f.FIELD_DATA_SOURCES)
                .Include(f => f.FIELD_TYPES)
                .Where(f => f.FIELD_DATA_SOURCES.Any())
                .OrderBy(f => f.FieldOrder)
                .ToListAsync();
        }
    }
}