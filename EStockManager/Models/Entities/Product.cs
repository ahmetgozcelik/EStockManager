using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // [Column] için

namespace EStockManager.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")] // PostgreSQL'de numeric(18,2) gibi bir tipe eşleşmesini sağlar
        public decimal Price { get; set; }

        public int StockQuantity { get; set; } // quantity -> miktar
    }
}