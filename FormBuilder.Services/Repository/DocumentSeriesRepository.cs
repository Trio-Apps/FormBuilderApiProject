using FormBuilder.Infrastructure.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domian.Entitys.FromBuilder;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Infrastructure.Repositories
{
    public class DocumentSeriesRepository : BaseRepository<DOCUMENT_SERIES>, IDocumentSeriesRepository
    {
        public FormBuilderDbContext _context { get; }

        public DocumentSeriesRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DOCUMENT_SERIES?> GetByIdAsync(int id)
        {
            return await _context.DOCUMENT_SERIES
                .Include(ds => ds.DOCUMENT_TYPES)
                .Include(ds => ds.PROJECTS)
                .FirstOrDefaultAsync(ds => ds.Id == id);
        }

        public async Task<DOCUMENT_SERIES?> GetBySeriesCodeAsync(string seriesCode)
        {
            return await _context.DOCUMENT_SERIES
                .Include(ds => ds.DOCUMENT_TYPES)
                .Include(ds => ds.PROJECTS)
                .FirstOrDefaultAsync(ds => ds.SeriesCode == seriesCode && ds.IsActive);
        }

        public async Task<IEnumerable<DOCUMENT_SERIES>> GetByDocumentTypeIdAsync(int documentTypeId)
        {
            return await _context.DOCUMENT_SERIES
                .Include(ds => ds.DOCUMENT_TYPES)
                .Include(ds => ds.PROJECTS)
                .Where(ds => ds.DocumentTypeId == documentTypeId && ds.IsActive)
                .OrderBy(ds => ds.SeriesCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<DOCUMENT_SERIES>> GetByProjectIdAsync(int projectId)
        {
            return await _context.DOCUMENT_SERIES
                .Include(ds => ds.DOCUMENT_TYPES)
                .Include(ds => ds.PROJECTS)
                .Where(ds => ds.ProjectId == projectId && ds.IsActive)
                .OrderBy(ds => ds.SeriesCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<DOCUMENT_SERIES>> GetActiveAsync()
        {
            return await _context.DOCUMENT_SERIES
                .Include(ds => ds.DOCUMENT_TYPES)
                .Include(ds => ds.PROJECTS)
                .Where(ds => ds.IsActive)
                .OrderBy(ds => ds.SeriesCode)
                .ToListAsync();
        }

        public async Task<DOCUMENT_SERIES?> GetDefaultSeriesAsync(int documentTypeId, int projectId)
        {
            return await _context.DOCUMENT_SERIES
                .Include(ds => ds.DOCUMENT_TYPES)
                .Include(ds => ds.PROJECTS)
                .FirstOrDefaultAsync(ds => ds.DocumentTypeId == documentTypeId &&
                                         ds.ProjectId == projectId &&
                                         ds.IsDefault &&
                                         ds.IsActive);
        }

        public async Task<bool> SeriesCodeExistsAsync(string seriesCode, int? excludeId = null)
        {
            var query = _context.DOCUMENT_SERIES.Where(ds => ds.SeriesCode == seriesCode);

            if (excludeId.HasValue)
            {
                query = query.Where(ds => ds.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsActiveAsync(int id)
        {
            return await _context.DOCUMENT_SERIES
                .AnyAsync(ds => ds.Id == id && ds.IsActive);
        }

        public async Task<int> GetNextNumberAsync(int seriesId)
        {
            var series = await _context.DOCUMENT_SERIES
                .FirstOrDefaultAsync(ds => ds.Id == seriesId);

            if (series == null)
                return -1;

            var nextNumber = series.NextNumber;

            // Increment for next use
            series.NextNumber++;
            _context.DOCUMENT_SERIES.Update(series);
            await _context.SaveChangesAsync();

            return nextNumber;
        }

        public async Task<bool> IsDefaultSeriesAsync(int documentTypeId, int projectId, int seriesId)
        {
            return await _context.DOCUMENT_SERIES
                .AnyAsync(ds => ds.Id == seriesId &&
                              ds.DocumentTypeId == documentTypeId &&
                              ds.ProjectId == projectId &&
                              ds.IsDefault);
        }
    }
}