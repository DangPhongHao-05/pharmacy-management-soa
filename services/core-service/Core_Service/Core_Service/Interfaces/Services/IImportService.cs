using Core_Service.DTOs.Imports;

namespace Core_Service.Interfaces.Services
{
    public interface IImportService
    {
        Task<IEnumerable<ImportReceiptResponseDto>> GetAllAsync(long? supplierId, string? status);
        Task<ImportReceiptResponseDto> GetByIdAsync(long id);
        Task<ImportReceiptResponseDto> CreateDraftAsync(CreateImportRequestDto dto, long userId);
        Task<ImportReceiptResponseDto> UpdateDraftAsync(long id, CreateImportRequestDto dto);
        Task DeleteDraftAsync(long id);
        Task ConfirmImportAsync(long id, long userId);
    }
}
