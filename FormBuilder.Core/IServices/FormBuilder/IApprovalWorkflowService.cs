using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.Workflow;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Services
{
    public interface IApprovalWorkflowService
    {
        // ================================
        // BASIC CRUD OPERATIONS
        // ================================
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByIdWithDetailsAsync(int id);
        Task<ApiResponse> GetByIdWithStagesAsync(int id);
        Task<ApiResponse> CreateAsync(CreateApprovalWorkflowDto createDto);
        Task<ApiResponse> UpdateAsync(int id, UpdateApprovalWorkflowDto updateDto);
        Task<ApiResponse> DeleteAsync(int id);

        // ================================
        // BULK OPERATIONS
        // ================================
        Task<ApiResponse> CreateBulkAsync(BulkCreateWorkflowDto bulkDto);
        Task<ApiResponse> BatchToggleActiveAsync(BulkWorkflowStatusDto statusDto);
        Task<ApiResponse> BatchDeleteAsync(List<int> workflowIds);

        // ================================
        // DOCUMENT TYPE RELATED OPERATIONS
        // ================================
        Task<ApiResponse> GetByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> GetActiveByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> GetByDocumentTypeIdsAsync(List<int> documentTypeIds);
        Task<ApiResponse> GetDefaultWorkflowForDocumentTypeAsync(int documentTypeId);
        Task<ApiResponse> SetAsDefaultWorkflowAsync(int workflowId);
        Task<ApiResponse> RemoveDefaultStatusAsync(int workflowId);

        // ================================
        // STATUS-BASED OPERATIONS
        // ================================
        Task<ApiResponse> GetActiveAsync();
        Task<ApiResponse> GetInactiveAsync();
        Task<ApiResponse> GetByStatusAsync(bool isActive);
        Task<ApiResponse> ToggleActiveAsync(int id, bool isActive);

        // ================================
        // SEARCH AND FILTER OPERATIONS
        // ================================
        Task<ApiResponse> SearchAsync(WorkflowSearchDto searchDto);
        Task<ApiResponse> GetByNameAsync(string name);
        Task<ApiResponse> GetByDocumentTypeAndStatusAsync(int documentTypeId, bool isActive);

        // ================================
        // VALIDATION OPERATIONS
        // ================================
        Task<ApiResponse> ValidateWorkflowAsync(CreateApprovalWorkflowDto createDto);
        Task<ApiResponse> NameExistsAsync(string name, int? excludeId = null);
        Task<ApiResponse> NameExistsForDocumentTypeAsync(WorkflowNameCheckDto checkDto);
        Task<ApiResponse> IsActiveAsync(int id);
        Task<ApiResponse> HasStagesAsync(int workflowId);
        Task<ApiResponse> IsWorkflowUsedAsync(int workflowId);

        // ================================
        // COUNT AND CHECK OPERATIONS
        // ================================
        Task<ApiResponse> CountByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> CountActiveByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> CountInactiveByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> CountTotalAsync();
        Task<ApiResponse> CountActiveAsync();
        Task<ApiResponse> CountInactiveAsync();
        Task<ApiResponse> HasWorkflowsAsync(int documentTypeId);
        Task<ApiResponse> HasActiveWorkflowsAsync(int documentTypeId);

        // ================================
        // STAGE-RELATED OPERATIONS
        // ================================
        Task<ApiResponse> GetWorkflowsWithStagesAsync(int? documentTypeId = null);
        Task<ApiResponse> GetWorkflowWithStagesAsync(int workflowId);
        Task<ApiResponse> GetWorkflowStagesAsync(int workflowId);
        Task<ApiResponse> GetWorkflowsWithoutStagesAsync(int? documentTypeId = null);

        // ================================
        // STATISTICS AND REPORTS
        // ================================
        Task<ApiResponse> GetStatisticsByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> GetOverallStatisticsAsync();
        Task<ApiResponse> GetWorkflowNamesByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> GetWorkflowCountByDocumentTypesAsync(List<int> documentTypeIds);

        // ================================
        // DATE-BASED OPERATIONS
        // ================================
        Task<ApiResponse> GetRecentlyCreatedAsync(int count = 10);
        Task<ApiResponse> GetRecentlyUpdatedAsync(int count = 10);
        Task<ApiResponse> GetByCreationDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse> GetByUpdateDateRangeAsync(DateTime startDate, DateTime endDate);

        // ================================
        // UTILITY OPERATIONS
        // ================================
        Task<ApiResponse> GetWorkflowsWithDocumentTypeAsync();
        Task<ApiResponse> GetByIdsAsync(List<int> ids);
        Task<ApiResponse> GetByIdsWithDetailsAsync(List<int> ids);
        Task<ApiResponse> ExportWorkflowsAsync(WorkflowSearchDto searchDto);
        Task<ApiResponse> ImportWorkflowsAsync(List<CreateApprovalWorkflowDto> workflows, bool overwriteExisting = false);
    }
}