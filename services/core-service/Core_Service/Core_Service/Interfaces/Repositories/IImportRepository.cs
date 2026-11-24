using Core_Service.Models;

namespace Core_Service.Interfaces.Repositories
{
    public interface IImportRepository
    {
        Task<IEnumerable<ImportReceipt>> GetAllAsync(long? supplierId, string? status);
        Task<ImportReceipt?> GetByIdWithDetailsAsync(long id);
        Task<ImportReceipt> CreateAsync(ImportReceipt receipt);
        Task UpdateAsync(ImportReceipt receipt);
        Task DeleteAsync(ImportReceipt receipt);

        // Hàm xử lý Transaction quan trọng nhất khi duyệt phiếu
        Task ConfirmImportTransactionAsync(ImportReceipt receipt, long userId);
    }
}
