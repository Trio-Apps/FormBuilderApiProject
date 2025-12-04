using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domian.Entitys.FormBuilder;
using Microsoft.EntityFrameworkCore;


namespace FormBuilder.Infrastructure.Repositories
{
    public class FormGridColumnRepository : BaseRepository<FORM_GRID_COLUMNS>, IFormGridColumnRepository
    {
        private readonly FormBuilderDbContext _context;

        public FormGridColumnRepository(FormBuilderDbContext context)
            : base(context)
        {
            _context = context;
        }

        // Explicit implementation of GetByIdAsync with navigation properties
        public async Task<FORM_GRID_COLUMNS> GetByIdAsync(int id)
        {
           
                return await _context.FORM_GRID_COLUMNS
                    .Include(c => c.FORM_GRIDS)
                        .ThenInclude(g => g.FORM_BUILDER)
                    .Include(c => c.FIELD_TYPES)
                    .FirstOrDefaultAsync(c => c.Id == id);
            
            
        }

        // Explicit implementation of GetAllAsync with navigation properties
        public async Task<IEnumerable<FORM_GRID_COLUMNS>> GetAllAsync()
        {
            
                return await _context.FORM_GRID_COLUMNS
                    .Include(c => c.FORM_GRIDS)
                        .ThenInclude(g => g.FORM_BUILDER)
                    .Include(c => c.FIELD_TYPES)
                    .OrderBy(c => c.GridId)
                    .ThenBy(c => c.ColumnOrder)
                    .ThenBy(c => c.ColumnName)
                    .ToListAsync();
            
            
        }

        public async Task<IEnumerable<FORM_GRID_COLUMNS>> GetByGridIdAsync(int gridId)
        {
            try
            {
                return await _context.FORM_GRID_COLUMNS
                    .Include(c => c.FORM_GRIDS)
                    .Include(c => c.FIELD_TYPES)
                    .Where(c => c.GridId == gridId)
                    .OrderBy(c => c.ColumnOrder)
                    .ThenBy(c => c.ColumnName)
                    .ToListAsync();
            }
            catch (Exception )
            {
                throw;
            }
        }

        public async Task<IEnumerable<FORM_GRID_COLUMNS>> GetActiveByGridIdAsync(int gridId)
        {
            try
            {
                return await _context.FORM_GRID_COLUMNS
                    .Include(c => c.FORM_GRIDS)
                    .Include(c => c.FIELD_TYPES)
                    .Where(c => c.GridId == gridId && c.IsActive)
                    .OrderBy(c => c.ColumnOrder)
                    .ThenBy(c => c.ColumnName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FORM_GRID_COLUMNS> GetByColumnCodeAsync(string columnCode, int gridId)
        {
             return await _context.FORM_GRID_COLUMNS
                    .Include(c => c.FORM_GRIDS)
                    .Include(c => c.FIELD_TYPES)
                    .FirstOrDefaultAsync(c =>
                        c.ColumnCode == columnCode &&
                        c.GridId == gridId);
           
        }

        public async Task<bool> ColumnCodeExistsAsync(string columnCode, int gridId, int? excludeId = null)
        {
            try
            {
                var query = _context.FORM_GRID_COLUMNS
                    .Where(c => c.ColumnCode == columnCode && c.GridId == gridId);

                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public async Task<int> GetNextColumnOrderAsync(int gridId)
        {
            try
            {
                var maxOrder = await _context.FORM_GRID_COLUMNS
                    .Where(c => c.GridId == gridId)
                    .MaxAsync(c => (int?)c.ColumnOrder) ?? 0;

                return maxOrder + 1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            try
            {
                var column = await _context.FORM_GRID_COLUMNS
                    .Where(c => c.Id == id)
                    .Select(c => new { c.IsActive })
                    .FirstOrDefaultAsync();

                return column?.IsActive ?? false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<FORM_GRID_COLUMNS>> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                return await _context.FORM_GRID_COLUMNS
                    .Include(c => c.FORM_GRIDS)
                    .Include(c => c.FIELD_TYPES)
                    .Where(c => c.FORM_GRIDS != null && c.FORM_GRIDS.FormBuilderId == formBuilderId)
                    .OrderBy(c => c.GridId)
                    .ThenBy(c => c.ColumnOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<FORM_GRID_COLUMNS>> GetByFieldTypeIdAsync(int fieldTypeId)
        {
            try
            {
                return await _context.FORM_GRID_COLUMNS
                    .Include(c => c.FORM_GRIDS)
                    .Include(c => c.FIELD_TYPES)
                    .Where(c => c.FieldTypeId == fieldTypeId)
                    .OrderBy(c => c.GridId)
                    .ThenBy(c => c.ColumnOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}