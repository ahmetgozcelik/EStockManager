using EStockManager.Infrastructure;
using EStockManager.Models.DTOs;
using EStockManager.Models.Entities;
using EStockManager.Repositories.Interfaces;
using EStockManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EStockManager.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Product> _productRepository; // stok kontrol
        private readonly ApplicationDbContext _dbContext;

        public OrderService(IGenericRepository<Order> orderRepository, IGenericRepository<Product> productRepository, ApplicationDbContext dbContext)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _dbContext = dbContext;
        }

        public async Task<Order> PlaceOrderAsync(int userId, OrderCreateDto dto)
        {
            using (IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var product = await _productRepository.GetByIdAsync(dto.ProductId);

                    if (product == null)
                    {
                        throw new Exception($"Hata: ID'si {dto.ProductId} olan ürün bulunamadı.");
                    }

                    if (product.StockQuantity < dto.Quantity)
                    {
                        throw new Exception($"Hata: {product.Name} ürünü için yeterli stok yok.");
                    }

                    // stoktan düşme
                    product.StockQuantity -= dto.Quantity;

                    // Product Repository update metodu çağrılıyor. 
                    // GenericRepository'deki Update metodu takip edilen (tracked) Entity'yi günceller.
                    _productRepository.Update(product);

                    var newOrder = new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.UtcNow
                    };

                    // siparişi kaydetme
                    await _orderRepository.AddAsync(newOrder);

                    // Transaction (İşlem) Kaydetme: Hem ürün stok güncellemesini hem de sipariş kaydını tek bir işlemde (Transaction) yapar.
                    // Eğer SaveChangesAsync başarısız olursa (örneğin veritabanı hatası), her iki değişiklik de geri alınır (rollback).
                    await _orderRepository.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return newOrder;
                }
                catch (Exception ex)
                {
                    // hata olursa tüm değişiklikler geri alınır
                    await transaction.RollbackAsync();
                    throw; // hatanın controller'a fırlatılmasını sağlar -> middleware yakalar
                }
            }
            
        }
    }
}
