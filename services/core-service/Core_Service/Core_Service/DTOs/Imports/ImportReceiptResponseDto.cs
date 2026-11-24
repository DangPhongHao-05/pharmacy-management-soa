namespace Core_Service.DTOs.Imports
{
    public class ImportReceiptResponseDto
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public long CreatorUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<ImportDetailResponseDto> Details { get; set; } = new();
    }

    public class ImportDetailResponseDto
    {
        public long Id { get; set; }
        public long MedicineId { get; set; }
        public string? MedicineName { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateOnly ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}
