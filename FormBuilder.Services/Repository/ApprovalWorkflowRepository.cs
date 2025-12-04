//using FormBuilder.API.Data;
//using FormBuilder.core;
//using FormBuilder.Core.DTOS.Workflow;
//using FormBuilder.Domain.Interfaces.Repositories;
//using FormBuilder.Domian.Entitys.FromBuilder;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace FormBuilder.Infrastructure.Repositories
//{
//    public class ApprovalWorkflowRepository : BaseRepository<APPROVAL_WORKFLOWS>, IApprovalWorkflowRepository
//    {
//        private readonly FormBuilderDbContext _context;
//        private readonly ILogger<ApprovalWorkflowRepository> _logger;

//        public ApprovalWorkflowRepository(FormBuilderDbContext context, ILogger<ApprovalWorkflowRepository> logger)
//            : base(context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        // ================================
//        // FROM IBASEREPOSITORY (Explicit implementation)
//        // ================================
//        public async Task<APPROVAL_WORKFLOWS> GetByIdAsync(int id)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflow by ID: {Id}", id);
//                return await _context.APPROVAL_WORKFLOWS
//                    .FirstOrDefaultAsync(w => w.Id == id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflow by ID: {Id}", id);
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetAllAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Getting all approval workflows");
//                return await _context.APPROVAL_WORKFLOWS
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all approval workflows");
//                throw;
//            }
//        }

//        // ================================
//        // GET METHODS WITH DETAILS
//        // ================================
//        public async Task<APPROVAL_WORKFLOWS> GetByIdWithDetailsAsync(int id)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflow with details by ID: {Id}", id);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .FirstOrDefaultAsync(w => w.Id == id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflow with details by ID: {Id}", id);
//                throw;
//            }
//        }

//        public async Task<APPROVAL_WORKFLOWS> GetByIdWithStagesAsync(int id)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflow with stages by ID: {Id}", id);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Include(w => w.APPROVAL_STAGES)
//                    .FirstOrDefaultAsync(w => w.Id == id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflow with stages by ID: {Id}", id);
//                throw;
//            }
//        }

//        public async Task<APPROVAL_WORKFLOWS> GetByIdWithFullDetailsAsync(int id)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflow with full details by ID: {Id}", id);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Include(w => w.APPROVAL_STAGES)
//                    .FirstOrDefaultAsync(w => w.Id == id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflow with full details by ID: {Id}", id);
//                throw;
//            }
//        }

//        // ================================
//        // GET BY DOCUMENT TYPE
//        // ================================
//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByDocumentTypeIdAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.DocumentTypeId == documentTypeId)
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetActiveByDocumentTypeIdAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Getting active approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.DocumentTypeId == documentTypeId && w.IsActive)
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting active approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByDocumentTypeIdsAsync(List<int> documentTypeIds)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows by {Count} document type IDs", documentTypeIds.Count);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => documentTypeIds.Contains(w.DocumentTypeId))
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows by document type IDs");
//                throw;
//            }
//        }

//        // ================================
//        // STATUS-BASED QUERIES
//        // ================================
//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetActiveAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Getting active approval workflows");
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.IsActive)
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting active approval workflows");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetInactiveAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Getting inactive approval workflows");
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => !w.IsActive)
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting inactive approval workflows");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByStatusAsync(bool isActive)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows by status: {IsActive}", isActive);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.IsActive == isActive)
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows by status: {IsActive}", isActive);
//                throw;
//            }
//        }

//        // ================================
//        // SEARCH AND FILTER
//        // ================================
//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> SearchAsync(string searchTerm, int? documentTypeId = null)
//        {
//            try
//            {
//                _logger.LogDebug("Searching approval workflows with term: {SearchTerm}", searchTerm);
//                var query = _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .AsQueryable();

//                if (!string.IsNullOrWhiteSpace(searchTerm))
//                {
//                    searchTerm = searchTerm.ToLower();
//                    query = query.Where(w => w.Name.ToLower().Contains(searchTerm) ||
//                                           w.DOCUMENT_TYPES.Name.ToLower().Contains(searchTerm));
//                }

//                if (documentTypeId.HasValue)
//                {
//                    query = query.Where(w => w.DocumentTypeId == documentTypeId.Value);
//                }

//                return await query
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error searching approval workflows with term: {SearchTerm}", searchTerm);
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByNameAsync(string name)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows by name: {Name}", name);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.Name.ToLower() == name.ToLower())
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows by name: {Name}", name);
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByDocumentTypeAndStatusAsync(int documentTypeId, bool isActive)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows by document type ID: {DocumentTypeId} and status: {IsActive}",
//                    documentTypeId, isActive);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.DocumentTypeId == documentTypeId && w.IsActive == isActive)
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows by document type and status");
//                throw;
//            }
//        }

//        // ================================
//        // VALIDATION METHODS
//        // ================================
//        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if approval workflow name exists: {Name}", name);
//                var query = _context.APPROVAL_WORKFLOWS
//                    .Where(w => w.Name.ToLower() == name.ToLower());

//                if (excludeId.HasValue)
//                {
//                    query = query.Where(w => w.Id != excludeId.Value);
//                }

//                return await query.AnyAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if approval workflow name exists: {Name}", name);
//                throw;
//            }
//        }

//        public async Task<bool> NameExistsForDocumentTypeAsync(string name, int documentTypeId, int? excludeId = null)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if approval workflow name exists for document type: {Name}, {DocumentTypeId}",
//                    name, documentTypeId);
//                var query = _context.APPROVAL_WORKFLOWS
//                    .Where(w => w.Name.ToLower() == name.ToLower() && w.DocumentTypeId == documentTypeId);

//                if (excludeId.HasValue)
//                {
//                    query = query.Where(w => w.Id != excludeId.Value);
//                }

//                return await query.AnyAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if approval workflow name exists for document type");
//                throw;
//            }
//        }

//        public async Task<bool> IsActiveAsync(int id)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if approval workflow is active: {Id}", id);
//                return await _context.APPROVAL_WORKFLOWS
//                    .AnyAsync(w => w.Id == id && w.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if approval workflow is active: {Id}", id);
//                throw;
//            }
//        }

//        public async Task<bool> HasStagesAsync(int workflowId)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if approval workflow has stages: {WorkflowId}", workflowId);
//                return await _context.APPROVAL_STAGES
//                    .AnyAsync(s => s.WorkflowId == workflowId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if approval workflow has stages: {WorkflowId}", workflowId);
//                throw;
//            }
//        }

//        public async Task<bool> HasActiveStagesAsync(int workflowId)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if approval workflow has active stages: {WorkflowId}", workflowId);
//                return await _context.APPROVAL_STAGES
//                    .AnyAsync(s => s.WorkflowId == workflowId && s.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if approval workflow has active stages: {WorkflowId}", workflowId);
//                throw;
//            }
//        }

//        public async Task<bool> IsWorkflowUsedAsync(int workflowId)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if approval workflow is used: {WorkflowId}", workflowId);
//                // Check if workflow is used in any documents
//                // Adjust based on your actual database schema
//                return await _context.DOCUMENT_TYPES
//                    .AnyAsync(d => d.WorkflowId == workflowId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if approval workflow is used: {WorkflowId}", workflowId);
//                throw;
//            }
//        }

//        // ================================
//        // COUNT METHODS
//        // ================================
//        public async Task<int> CountByDocumentTypeIdAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Counting approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .CountAsync(w => w.DocumentTypeId == documentTypeId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error counting approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<int> CountActiveByDocumentTypeIdAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Counting active approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .CountAsync(w => w.DocumentTypeId == documentTypeId && w.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error counting active approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<int> CountInactiveByDocumentTypeIdAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Counting inactive approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .CountAsync(w => w.DocumentTypeId == documentTypeId && !w.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error counting inactive approval workflows by document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<int> CountTotalAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Counting total approval workflows");
//                return await _context.APPROVAL_WORKFLOWS.CountAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error counting total approval workflows");
//                throw;
//            }
//        }

//        public async Task<int> CountActiveAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Counting active approval workflows");
//                return await _context.APPROVAL_WORKFLOWS.CountAsync(w => w.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error counting active approval workflows");
//                throw;
//            }
//        }

//        public async Task<int> CountInactiveAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Counting inactive approval workflows");
//                return await _context.APPROVAL_WORKFLOWS.CountAsync(w => !w.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error counting inactive approval workflows");
//                throw;
//            }
//        }

//        // ================================
//        // CHECK METHODS
//        // ================================
//        public async Task<bool> HasWorkflowsAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if document type has workflows: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .AnyAsync(w => w.DocumentTypeId == documentTypeId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if document type has workflows: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<bool> HasActiveWorkflowsAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if document type has active workflows: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .AnyAsync(w => w.DocumentTypeId == documentTypeId && w.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if document type has active workflows: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<bool> HasInactiveWorkflowsAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if document type has inactive workflows: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .AnyAsync(w => w.DocumentTypeId == documentTypeId && !w.IsActive);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if document type has inactive workflows: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        // ================================
//        // BULK OPERATIONS
//        // ================================
//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByIdsAsync(List<int> ids)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows by {Count} IDs", ids.Count);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Where(w => ids.Contains(w.Id))
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows by IDs");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetByIdsWithDetailsAsync(List<int> ids)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows with details by {Count} IDs", ids.Count);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => ids.Contains(w.Id))
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows with details by IDs");
//                throw;
//            }
//        }

//        // ================================
//        // UTILITY METHODS
//        // ================================
//        public async Task<IEnumerable<string>> GetWorkflowNamesByDocumentTypeIdAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Getting workflow names by document type ID: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Where(w => w.DocumentTypeId == documentTypeId)
//                    .OrderBy(w => w.Name)
//                    .Select(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting workflow names by document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<Dictionary<int, int>> GetWorkflowCountByDocumentTypesAsync(List<int> documentTypeIds)
//        {
//            try
//            {
//                _logger.LogDebug("Getting workflow count by {Count} document types", documentTypeIds.Count);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Where(w => documentTypeIds.Contains(w.DocumentTypeId))
//                    .GroupBy(w => w.DocumentTypeId)
//                    .Select(g => new { DocumentTypeId = g.Key, Count = g.Count() })
//                    .ToDictionaryAsync(x => x.DocumentTypeId, x => x.Count);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting workflow count by document types");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetWorkflowsWithStagesAsync(int? documentTypeId = null)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflows with stages");
//                var query = _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Include(w => w.APPROVAL_STAGES)
//                    .Where(w => w.APPROVAL_STAGES.Any());

//                if (documentTypeId.HasValue)
//                {
//                    query = query.Where(w => w.DocumentTypeId == documentTypeId.Value);
//                }

//                return await query
//                    .OrderBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflows with stages");
//                throw;
//            }
//        }

//        public async Task<APPROVAL_WORKFLOWS> GetWorkflowWithStagesAsync(int workflowId)
//        {
//            try
//            {
//                _logger.LogDebug("Getting approval workflow with stages by ID: {WorkflowId}", workflowId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Include(w => w.APPROVAL_STAGES)
//                    .FirstOrDefaultAsync(w => w.Id == workflowId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting approval workflow with stages by ID: {WorkflowId}", workflowId);
//                throw;
//            }
//        }

//        public async Task<APPROVAL_WORKFLOWS> GetDefaultWorkflowForDocumentTypeAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Getting default workflow for document type ID: {DocumentTypeId}", documentTypeId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Include(w => w.APPROVAL_STAGES)
//                    .FirstOrDefaultAsync(w => w.DocumentTypeId == documentTypeId && w.IsDefault);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting default workflow for document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<bool> IsDefaultWorkflowAsync(int workflowId)
//        {
//            try
//            {
//                _logger.LogDebug("Checking if workflow is default: {WorkflowId}", workflowId);
//                return await _context.APPROVAL_WORKFLOWS
//                    .AnyAsync(w => w.Id == workflowId && w.IsDefault);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if workflow is default: {WorkflowId}", workflowId);
//                throw;
//            }
//        }

//        public async Task SetAsDefaultWorkflowAsync(int workflowId)
//        {
//            try
//            {
//                _logger.LogDebug("Setting workflow as default: {WorkflowId}", workflowId);
//                var workflow = await _context.APPROVAL_WORKFLOWS.FindAsync(workflowId);
//                if (workflow != null)
//                {
//                    workflow.UpdatedDate = DateTime.UtcNow;
//                    _context.APPROVAL_WORKFLOWS.Update(workflow);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error setting workflow as default: {WorkflowId}", workflowId);
//                throw;
//            }
//        }

//        public async Task RemoveDefaultStatusFromOtherWorkflowsAsync(int documentTypeId, int excludeWorkflowId)
//        {
//            try
//            {
//                _logger.LogDebug("Removing default status from other workflows for document type: {DocumentTypeId}", documentTypeId);
//                var workflows = await _context.APPROVAL_WORKFLOWS
//                    .Where(w => w.DocumentTypeId == documentTypeId &&
//                               w.Id != excludeWorkflowId)
//                    .ToListAsync();

//                foreach (var workflow in workflows)
//                {
//                    workflow.UpdatedDate = DateTime.UtcNow;
//                }

//                if (workflows.Any())
//                {
//                    _context.APPROVAL_WORKFLOWS.UpdateRange(workflows);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error removing default status from other workflows");
//                throw;
//            }
//        }

//        // ================================
//        // STATISTICS METHODS
//        // ================================
//        public async Task<WorkflowStatisticsDto> GetStatisticsByDocumentTypeIdAsync(int documentTypeId)
//        {
//            try
//            {
//                _logger.LogDebug("Getting statistics for document type ID: {DocumentTypeId}", documentTypeId);
//                var documentType = await _context.DOCUMENT_TYPES.FindAsync(documentTypeId);
//                if (documentType == null)
//                    return null;

//                var workflows = await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.APPROVAL_STAGES)
//                    .Where(w => w.DocumentTypeId == documentTypeId)
//                    .ToListAsync();

//                var statistics = new WorkflowStatisticsDto
//                {
//                    DocumentTypeId = documentTypeId,
//                    DocumentTypeName = documentType.Name,
//                    TotalWorkflows = workflows.Count,
//                    ActiveWorkflows = workflows.Count(w => w.IsActive),
//                    InactiveWorkflows = workflows.Count(w => !w.IsActive),
//                    WorkflowsWithStages = workflows.Count(w => w.APPROVAL_STAGES.Any()),
//                    WorkflowsWithoutStages = workflows.Count(w => !w.APPROVAL_STAGES.Any()),
//                    OldestWorkflowDate = workflows.Min(w => w.CreatedDate),
//                    NewestWorkflowDate = workflows.Max(w => w.CreatedDate)
//                };

//                return statistics;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting statistics for document type ID: {DocumentTypeId}", documentTypeId);
//                throw;
//            }
//        }

//        public async Task<OverallWorkflowStatisticsDto> GetOverallStatisticsAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Getting overall workflow statistics");
//                var workflows = await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Include(w => w.APPROVAL_STAGES)
//                    .ToListAsync();

//                var documentTypes = await _context.DOCUMENT_TYPES.ToListAsync();

//                var workflowCountByDocumentType = workflows
//                    .GroupBy(w => w.DOCUMENT_TYPES.Name)
//                    .ToDictionary(g => g.Key, g => g.Count());

//                var activeWorkflowCountByDocumentType = workflows
//                    .Where(w => w.IsActive)
//                    .GroupBy(w => w.DOCUMENT_TYPES.Name)
//                    .ToDictionary(g => g.Key, g => g.Count());

//                var statistics = new OverallWorkflowStatisticsDto
//                {
//                    TotalWorkflows = workflows.Count,
//                    ActiveWorkflows = workflows.Count(w => w.IsActive),
//                    InactiveWorkflows = workflows.Count(w => !w.IsActive),
//                    TotalDocumentTypes = documentTypes.Count,
//                    DocumentTypesWithWorkflows = documentTypes.Count(dt => workflows.Any(w => w.DocumentTypeId == dt.Id)),
//                    DocumentTypesWithoutWorkflows = documentTypes.Count(dt => !workflows.Any(w => w.DocumentTypeId == dt.Id)),
//                    WorkflowsWithStages = workflows.Count(w => w.APPROVAL_STAGES.Any()),
//                    WorkflowsWithoutStages = workflows.Count(w => !w.APPROVAL_STAGES.Any()),
//                    DefaultWorkflows = workflows.Count(w => w.IsDefault),
//                    WorkflowCountByDocumentType = workflowCountByDocumentType,
//                    ActiveWorkflowCountByDocumentType = activeWorkflowCountByDocumentType
//                };

//                return statistics;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting overall workflow statistics");
//                throw;
//            }
//        }

//        // ================================
//        // COMPLEX QUERIES
//        // ================================
//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetWorkflowsWithDocumentTypeAsync()
//        {
//            try
//            {
//                _logger.LogDebug("Getting workflows with document type");
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .OrderBy(w => w.DOCUMENT_TYPES.Name)
//                    .ThenBy(w => w.Name)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting workflows with document type");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetWorkflowsByCreationDateRangeAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                _logger.LogDebug("Getting workflows by creation date range: {StartDate} to {EndDate}", startDate, endDate);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.CreatedDate >= startDate && w.CreatedDate <= endDate)
//                    .OrderByDescending(w => w.CreatedDate)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting workflows by creation date range");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetWorkflowsByUpdateDateRangeAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                _logger.LogDebug("Getting workflows by update date range: {StartDate} to {EndDate}", startDate, endDate);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .Where(w => w.UpdatedDate >= startDate && w.UpdatedDate <= endDate)
//                    .OrderByDescending(w => w.UpdatedDate)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting workflows by update date range");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetRecentlyCreatedAsync(int count = 10)
//        {
//            try
//            {
//                _logger.LogDebug("Getting {Count} recently created workflows", count);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .OrderByDescending(w => w.CreatedDate)
//                    .Take(count)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting recently created workflows");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<APPROVAL_WORKFLOWS>> GetRecentlyUpdatedAsync(int count = 10)
//        {
//            try
//            {
//                _logger.LogDebug("Getting {Count} recently updated workflows", count);
//                return await _context.APPROVAL_WORKFLOWS
//                    .Include(w => w.DOCUMENT_TYPES)
//                    .OrderByDescending(w => w.UpdatedDate)
//                    .Take(count)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting recently updated workflows");
//                throw;
//            }
//        }
//    }
//}