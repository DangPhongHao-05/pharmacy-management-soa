using System.ComponentModel.DataAnnotations;

namespace Core_Service.DTOs.Imports
{
    public class CreateImportRequestDto
    {
        [Required]
        public long SupplierId { get; set; }

        [Required, MinLength(1, ErrorMessage = "Phiếu nhập phải có ít nhất 1 sản phẩm")]
        public List<ImportDetailItemDto> Details { get; set; } = new();
    }

    public class ImportDetailItemDto
    {
        [Required]
        public long MedicineId { get; set; }

        [Required]
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly ExpiryDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } // Đã quy đổi về base_unit

        [Range(0, double.MaxValue, ErrorMessage = "Giá nhập không được âm")]
        public decimal ImportPrice { get; set; }
    }
}
