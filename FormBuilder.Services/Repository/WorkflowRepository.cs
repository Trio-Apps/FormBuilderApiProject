//using FormBuilder.API.Data;
//using FormBuilder.core;
//using FormBuilder.Domain.Interfaces.Repositories;
//using FormBuilder.Domian.Entitys.FromBuilder;
//using FormBuilder.Domian.Entitys.froms;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace FormBuilder.Infrastructure.Repositories
//{
//    public class WorkflowRepository : BaseRepository<APPROVAL_WORKFLOWS>, IWorkflowRepository
//    {
//        private readonly FormBuilderDbContext _context;

//        public WorkflowRepository(FormBuilderDbContext context) : base(context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }

//        public async Task<APPROVAL_WORKFLOWS> GetByIdWithDetailsAsync(int id)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Include(w => w.DOCUMENT_TYPES)
//                .Include(w => w.APPROVAL_STAGES)
//                .FirstOrDefaultAsync(w => w.Id == id);
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByDocumentTypeIdAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Where(w => w.DocumentTypeId == documentTypeId)
//                .Include(w => w.DOCUMENT_TYPES)
//                .OrderBy(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetActiveAsync()
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Where(w => w.IsActive)
//                .Include(w => w.DOCUMENT_TYPES)
//                .OrderBy(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetActiveByDocumentTypeIdAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Where(w => w.DocumentTypeId == documentTypeId && w.IsActive)
//                .Include(w => w.DOCUMENT_TYPES)
//                .OrderBy(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
//        {
//            var query = _context.APPROVAL_WORKFLOWS
//                .Where(w => w.Name.ToLower() == name.ToLower());

//            if (excludeId.HasValue)
//            {
//                query = query.Where(w => w.Id != excludeId.Value);
//            }

//            return await query.AnyAsync();
//        }

//        public async Task<bool> IsNameUniqueForDocumentTypeAsync(string name, int documentTypeId, int? excludeId = null)
//        {
//            var query = _context.APPROVAL_WORKFLOWS
//                .Where(w => w.Name.ToLower() == name.ToLower() && w.DocumentTypeId == documentTypeId);

//            if (excludeId.HasValue)
//            {
//                query = query.Where(w => w.Id != excludeId.Value);
//            }

//            return await query.AnyAsync();
//        }

//        public async Task<bool> HasWorkflowsAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .AnyAsync(w => w.DocumentTypeId == documentTypeId);
//        }

//        public async Task<bool> HasActiveWorkflowsAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .AnyAsync(w => w.DocumentTypeId == documentTypeId && w.IsActive);
//        }

//        public async Task<int> CountByDocumentTypeIdAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .CountAsync(w => w.DocumentTypeId == documentTypeId);
//        }

//        public async Task<int> CountActiveByDocumentTypeIdAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .CountAsync(w => w.DocumentTypeId == documentTypeId && w.IsActive);
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> SearchAsync(string searchTerm, int? documentTypeId = null)
//        {
//            var query = _context.APPROVAL_WORKFLOWS
//                .Include(w => w.DOCUMENT_TYPES)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(searchTerm))
//            {
//                query = query.Where(w => w.Name.Contains(searchTerm) ||
//                                        w.DOCUMENT_TYPES.Name.Contains(searchTerm));
//            }

//            if (documentTypeId.HasValue)
//            {
//                query = query.Where(w => w.DocumentTypeId == documentTypeId.Value);
//            }

//            return await query
//                .OrderBy(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByDocumentTypeIdsAsync(List<int> documentTypeIds)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Where(w => documentTypeIds.Contains(w.DocumentTypeId))
//                .Include(w => w.DOCUMENT_TYPES)
//                .OrderBy(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<APPROVAL_WORKFLOWS> GetDefaultWorkflowForDocumentTypeAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Include(w => w.DOCUMENT_TYPES)
//                .Include(w => w.APPROVAL_STAGES)
//                .FirstOrDefaultAsync(w => w.DocumentTypeId == documentTypeId && w.DOCUMENT_TYPES.IsActive);
//        }

//        public async Task<bool> IsDefaultWorkflowAsync(int workflowId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .AnyAsync(w => w.Id == workflowId && w.IsActive);
//        }

//        public async Task SetAsDefaultWorkflowAsync(int workflowId)
//        {
//            var workflow = await _context.APPROVAL_WORKFLOWS.FindAsync(workflowId);
//            if (workflow != null)
//            {
//                workflow.IsActive = true;
//                workflow.UpdatedDate = DateTime.UtcNow;
//                _context.APPROVAL_WORKFLOWS.Update(workflow);
//            }
//        }

//        public async Task RemoveDefaultStatusFromOtherWorkflowsAsync(int documentTypeId, int excludeWorkflowId)
//        {
//            var workflows = await _context.APPROVAL_WORKFLOWS
//                .Where(w => w.DocumentTypeId == documentTypeId &&
//                           w.Id != excludeWorkflowId &&
//                           w.IsActive)
//                .ToListAsync();

//            foreach (var workflow in workflows)
//            {
//                workflow.IsActive = false;
//                workflow.UpdatedDate = DateTime.UtcNow;
//            }

//            if (workflows.Any())
//            {
//                _context.APPROVAL_WORKFLOWS.UpdateRange(workflows);
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetWorkflowsWithStepsAsync(int? documentTypeId = null)
//        {
//            var query = _context.APPROVAL_WORKFLOWS
//                .Include(w => w.DOCUMENT_TYPES)
//                .Include(w => w.APPROVAL_STAGES)
//                .ThenInclude(ws => ws.r)
//                .AsQueryable();

//            if (documentTypeId.HasValue)
//            {
//                query = query.Where(w => w.DocumentTypeId == documentTypeId.Value);
//            }

//            return await query
//                .OrderBy(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<WORKFLOWS> GetWorkflowWithStepsAsync(int workflowId)
//        {
//            return await _context.WORKFLOWS
//                .Include(w => w.DOCUMENT_TYPES)
//                .Include(w => w.WORKFLOW_STEPS)
//                .ThenInclude(ws => ws.ROLES)
//                .FirstOrDefaultAsync(w => w.Id == workflowId);
//        }

//        public async Task<bool> IsWorkflowUsedAsync(int workflowId)
//        {
//            // Check if workflow is used in any documents or other related entities
//            // Adjust based on your actual database schema
//            return await _context.DOCUMENTS
//                .AnyAsync(d => d.WorkflowId == workflowId) ||
//                   await _context.WORKFLOW_HISTORY
//                .AnyAsync(wh => wh.WorkflowId == workflowId);
//        }

//        public async Task<IEnumerable<WORKFLOWS>> GetByStatusAsync(bool isActive, int? documentTypeId = null)
//        {
//            var query = _context.APPROVAL_WORKFLOWS
//                .Include(w => w.DOCUMENT_TYPES)
//                .Where(w => w.IsActive == isActive);

//            if (documentTypeId.HasValue)
//            {
//                query = query.Where(w => w.DocumentTypeId == documentTypeId.Value);
//            }

//            return await query
//                .OrderBy(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<string>> GetWorkflowNamesByDocumentTypeIdAsync(int documentTypeId)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Where(w => w.DocumentTypeId == documentTypeId)
//                .OrderBy(w => w.Name)
//                .Select(w => w.Name)
//                .ToListAsync();
//        }

//        public async Task<Dictionary<int, int>> GetWorkflowCountByDocumentTypesAsync(List<int> documentTypeIds)
//        {
//            return await _context.APPROVAL_WORKFLOWS
//                .Where(w => documentTypeIds.Contains(w.DocumentTypeId))
//                .GroupBy(w => w.DocumentTypeId)
//                .Select(g => new { DocumentTypeId = g.Key, Count = g.Count() })
//                .ToDictionaryAsync(x => x.DocumentTypeId, x => x.Count);
//        }
//    }
//}