using FormBuilder.API.Models;
using FormBuilder.API.Models.DTOs;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Services
{
    public interface IFormSubmissionService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId);
        Task<ApiResponse> GetByUserIdAsync(string userId);
        Task<ApiResponse> GetByStatusAsync(string status);
        Task<ApiResponse> GetByDocumentNumberAsync(string documentNumber);
        Task<ApiResponse> CreateAsync(CreateFormSubmissionDto createDto);
        Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionDto updateDto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> UpdateStatusAsync(int id, FormSubmissionStatusDto statusDto);
        Task<ApiResponse> GetSubmissionsCountAsync(int formBuilderId);
        Task<ApiResponse> ExistsAsync(int id);
        Task<ApiResponse> FilterSubmissionsAsync(FormSubmissionFilterDto filterDto);
    }
}