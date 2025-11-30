using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Services
{
    public interface IFormSubmissionsService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByIdWithDetailsAsync(int id);
        Task<ApiResponse> GetByDocumentNumberAsync(string documentNumber);
        Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId);
        Task<ApiResponse> GetByDocumentTypeIdAsync(int documentTypeId);
        Task<ApiResponse> GetByUserIdAsync(string userId);
        Task<ApiResponse> GetByStatusAsync(string status);
        Task<ApiResponse> CreateAsync(CreateFormSubmissionDto createDto);
        Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionDto updateDto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> SubmitAsync(SubmitFormDto submitDto);
        Task<ApiResponse> UpdateStatusAsync(int id, string status);
        Task<ApiResponse> ExistsAsync(int id);
    }
}