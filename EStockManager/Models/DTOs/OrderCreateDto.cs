using System.ComponentModel.DataAnnotations;

namespace EStockManager.Models.DTOs
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "Ürün ID'si zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Ürün ID'si geçerli olmalıdır.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Quantity { get; set; }
    }
}
