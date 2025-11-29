using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EStockManager.Models.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // 1.foreign key
        public int UserId { get; set; }

        // 2.navigation property
        // ef core bu özellik üzerinden ilişkiyi kurar n sipariş -> 1 kullanıcı
        [ForeignKey("UserId")]
        public User User { get; set; }

        // Bir siparişin birden fazla ögesi (item) olabilir
        //public ICollection<OrderItem> Items { get; set; }
    }
}
