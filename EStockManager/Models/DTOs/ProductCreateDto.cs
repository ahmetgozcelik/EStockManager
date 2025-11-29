using System.ComponentModel.DataAnnotations;

namespace EStockManager.Models.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 999999.99, ErrorMessage = "Geçerli bir fiyat giriniz.")]
        public decimal Price { get; set; }

        [Range(0, 99999, ErrorMessage = "Stok adedi negatif olamaz.")]
        public int StockQuantity { get; set; }
    }
}
