using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EStockManager.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // İlişki: Bir kullanıcı birden fazla siparişe dahil olabilir 1 kullanıcı -> n sipariş
        public ICollection<Order> Orders { get; set; }
    }
}