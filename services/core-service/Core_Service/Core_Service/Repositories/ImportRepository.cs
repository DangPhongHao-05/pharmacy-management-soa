// Repositories/ImportRepository.cs
using Core_Service.Data;
using Core_Service.Interfaces.Repositories;
using Core_Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Core_Service.Repositories
{
    public class ImportRepository : IImportRepository
    {
        private readonly AppDbContext _context; // Thay bằng tên DbContext của bạn

        public ImportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImportReceipt>> GetAllAsync(long? supplierId, string? status)
        {
            var query = _context.ImportReceipts
                .Include(r => r.Supplier)
                .Include(r => r.ImportDetails)
                .AsQueryable();

            if (supplierId.HasValue)
                query = query.Where(r => r.SupplierId == supplierId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status.ToUpper());

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<ImportReceipt?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.ImportReceipts
                .Include(r => r.Supplier)
                .Include(r => r.ImportDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ImportReceipt> CreateAsync(ImportReceipt receipt)
        {
            _context.ImportReceipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task UpdateAsync(ImportReceipt receipt)
        {
            _context.ImportReceipts.Update(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ImportReceipt receipt)
        {
            _context.ImportReceipts.Remove(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmImportTransactionAsync(ImportReceipt receipt, long userId)
        {
            // Bắt đầu Database Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cập nhật trạng thái phiếu nhập
                receipt.Status = "CONFIRMED";
                _context.ImportReceipts.Update(receipt);

                // 2. Duyệt từng chi tiết để Upsert lô thuốc & Ghi thẻ kho
                foreach (var detail in receipt.ImportDetails)
                {
                    // Kiểm tra lô thuốc đã tồn tại theo (medicine_id, batch_number) chưa
                    var batch = await _context.MedicineBatches
                        .FirstOrDefaultAsync(b => b.MedicineId == detail.MedicineId
                                               && b.BatchNumber == detail.BatchNumber);

                    if (batch == null)
                    {
                        // Chưa có -> Tạo lô mới
                        batch = new MedicineBatch
                        {
                            MedicineId = detail.MedicineId,
                            BatchNumber = detail.BatchNumber,
                            ExpiryDate = detail.ExpiryDate,
                            Quantity = detail.Quantity
                        };
                        _context.MedicineBatches.Add(batch);
                        await _context.SaveChangesAsync(); // Lưu để lấy batch.Id
                    }
                    else
                    {
                        // Đã có -> Kiểm tra lệch hạn sử dụng và cộng dồn số lượng
                        if (batch.ExpiryDate != detail.ExpiryDate)
                            throw new InvalidOperationException($"Lô {detail.BatchNumber} đã tồn tại với hạn sử dụng khác ({batch.ExpiryDate}).");

                        batch.Quantity += detail.Quantity;
                        _context.MedicineBatches.Update(batch);
                    }

                    // 3. Ghi lịch sử biến động kho (stock_movements)
                    var movement = new StockMovement
                    {
                        BatchId = batch.Id,
                        MovementType = "IMPORT",
                        QuantityChange = detail.Quantity, // Số dương
                        ReferenceId = receipt.Id,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.StockMovements.Add(movement);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}