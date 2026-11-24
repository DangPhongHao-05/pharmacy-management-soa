// Services/ImportService.cs
using Core_Service.DTOs.Imports;
using Core_Service.Interfaces.Repositories;
using Core_Service.Interfaces.Services;
using Core_Service.Models;

namespace Core_Service.Services
{
    public class ImportService : IImportService
    {
        private readonly IImportRepository _importRepo;

        public ImportService(IImportRepository importRepo)
        {
            _importRepo = importRepo;
        }

        public async Task<IEnumerable<ImportReceiptResponseDto>> GetAllAsync(long? supplierId, string? status)
        {
            var receipts = await _importRepo.GetAllAsync(supplierId, status);
            return receipts.Select(MapToDto);
        }

        public async Task<ImportReceiptResponseDto> GetByIdAsync(long id)
        {
            var receipt = await _importRepo.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException($"Không tìm thấy phiếu nhập ID = {id}");
            return MapToDto(receipt);
        }

        public async Task<ImportReceiptResponseDto> CreateDraftAsync(CreateImportRequestDto dto, long userId)
        {
            var receipt = new ImportReceipt
            {
                SupplierId = dto.SupplierId,
                CreatorUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = "PENDING",
                ImportDetails = dto.Details.Select(d => new ImportDetail
                {
                    MedicineId = d.MedicineId,
                    BatchNumber = d.BatchNumber.Trim(),
                    ExpiryDate = d.ExpiryDate,
                    Quantity = d.Quantity,
                    ImportPrice = d.ImportPrice
                }).ToList()
            };

            var created = await _importRepo.CreateAsync(receipt);
            return await GetByIdAsync(created.Id);
        }

        public async Task<ImportReceiptResponseDto> UpdateDraftAsync(long id, CreateImportRequestDto dto)
        {
            var receipt = await _importRepo.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException($"Không tìm thấy phiếu nhập ID = {id}");

            if (receipt.Status != "PENDING")
                throw new InvalidOperationException("Chỉ được phép chỉnh sửa phiếu nhập ở trạng thái PENDING.");

            receipt.SupplierId = dto.SupplierId;

            // Thay thế danh sách chi tiết cũ bằng danh sách mới
            receipt.ImportDetails.Clear();
            foreach (var d in dto.Details)
            {
                receipt.ImportDetails.Add(new ImportDetail
                {
                    MedicineId = d.MedicineId,
                    BatchNumber = d.BatchNumber.Trim(),
                    ExpiryDate = d.ExpiryDate,
                    Quantity = d.Quantity,
                    ImportPrice = d.ImportPrice
                });
            }

            await _importRepo.UpdateAsync(receipt);
            return await GetByIdAsync(id);
        }

        public async Task DeleteDraftAsync(long id)
        {
            var receipt = await _importRepo.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException($"Không tìm thấy phiếu nhập ID = {id}");

            if (receipt.Status != "PENDING")
                throw new InvalidOperationException("Không thể xóa phiếu nhập đã được xác nhận (CONFIRMED).");

            await _importRepo.DeleteAsync(receipt);
        }

        public async Task ConfirmImportAsync(long id, long userId)
        {
            var receipt = await _importRepo.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException($"Không tìm thấy phiếu nhập ID = {id}");

            if (receipt.Status == "CONFIRMED")
                throw new InvalidOperationException("Phiếu nhập này đã được xác nhận trước đó.");

            if (!receipt.ImportDetails.Any())
                throw new InvalidOperationException("Phiếu nhập rỗng, không thể xác nhận.");

            await _importRepo.ConfirmImportTransactionAsync(receipt, userId);
        }

        private static ImportReceiptResponseDto MapToDto(ImportReceipt r) => new()
        {
            Id = r.Id,
            SupplierId = r.SupplierId,
            SupplierName = r.Supplier?.Name,
            CreatorUserId = r.CreatorUserId,
            CreatedAt = r.CreatedAt,
            Status = r.Status,
            TotalAmount = r.ImportDetails.Sum(d => d.Quantity * d.ImportPrice),
            Details = r.ImportDetails.Select(d => new ImportDetailResponseDto
            {
                Id = d.Id,
                MedicineId = d.MedicineId,
                MedicineName = d.Medicine?.Name,
                BatchNumber = d.BatchNumber,
                ExpiryDate = d.ExpiryDate,
                Quantity = d.Quantity,
                ImportPrice = d.ImportPrice
            }).ToList()
        };
    }
}