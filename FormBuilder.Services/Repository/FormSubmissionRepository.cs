using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domian.Entitys.froms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Infrastructure.Repositories
{
    public class FormSubmissionRepository : BaseRepository<FORM_SUBMISSIONS>, IFormSubmissionRepository
    {
        public FormBuilderDbContext _context { get; }

        public FormSubmissionRepository(FormBuilderDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FORM_SUBMISSIONS>> GetByFormBuilderIdAsync(int formBuilderId)
        {
            return await _context.FORM_SUBMISSIONS
                .Where(fs => fs.FormBuilderId == formBuilderId)
                .OrderByDescending(fs => fs.CreatedDate)
                .ToListAsync();
        }

        // إزالة الدالة المكررة وإبقاء واحدة فقط
        public async Task<IEnumerable<FORM_SUBMISSIONS>> GetByUserIdAsync(string userId)
        {
            return await _context.FORM_SUBMISSIONS
                .Where(fs => fs.SubmittedByUserId == userId)
                .OrderByDescending(fs => fs.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FORM_SUBMISSIONS>> GetByStatusAsync(string status)
        {
            return await _context.FORM_SUBMISSIONS
                .Where(fs => fs.Status == status)
                .OrderByDescending(fs => fs.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.FORM_SUBMISSIONS
                .AnyAsync(fs => fs.id == id);
        }

        public async Task<int> GetSubmissionsCountAsync(int formBuilderId)
        {
            return await _context.FORM_SUBMISSIONS
                .CountAsync(fs => fs.FormBuilderId == formBuilderId);
        }

        public async Task<FORM_SUBMISSIONS> GetByDocumentNumberAsync(string documentNumber)
        {
            return await _context.FORM_SUBMISSIONS
                .FirstOrDefaultAsync(fs => fs.DocumentNumber == documentNumber);
        }

        public async Task<FORM_SUBMISSIONS> GetByIdAsync(int id)
        {
            return await _context.FORM_SUBMISSIONS
                .FirstOrDefaultAsync(fs => fs.id == id);
        }
    }
}