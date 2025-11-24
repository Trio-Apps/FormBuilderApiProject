using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.core;
using FormBuilder.Domian.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                               (!excludeId.HasValue || f.id != excludeId));
        }
    }
}
