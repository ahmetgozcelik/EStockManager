using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EStockManager.Infrastructure;
using EStockManager.Models.DTOs;
using EStockManager.Models.Entities;
using EStockManager.Repositories.Interfaces;
using EStockManager.Services.Concrete;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace EStockManager.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IGenericRepository<Order>> _mockOrderRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepo = new Mock<IGenericRepository<Order>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockDbContext = new Mock<ApplicationDbContext>();

            // Eğer Transaction test edilecekse, DbContext.Database Mock'lanmalıdır.
            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockDbContext.Setup(db => db.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(mockTransaction.Object);

            _orderService = new OrderService(
                _mockOrderRepo.Object,
                _mockProductRepo.Object,
                _mockDbContext.Object // Eğer ApplicationDbContext Mock'lanamazsa, burayı null bırakıp Service'i basitleştirmeniz gerekir.
            );
        }

        // başarılı sipariş ve stok düşürme testi
        [Fact]
        public async Task PlaceOrderAsync_ShouldDecreaseStockAndPlaceOrder_WhenStockIsSufficient()
        {
            // GIVEN (Verilenler/Hazırlık)
            var productId = 1;
            var orderQuantity = 5;
            var initialStock = 10;
            var userId = 1;

            var mockProduct = new Product { Id = productId, Name = "Laptop", StockQuantity = initialStock, Price = 1000m };
            var orderDto = new OrderCreateDto { ProductId = productId, Quantity = orderQuantity };

            // Mock Product Repo: Belirli bir ID istendiğinde sahte ürünü döndür
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
                            .ReturnsAsync(mockProduct);

            // Mock Product Repo: Güncelleme metodunu çağırırken bir şey yapma
            _mockProductRepo.Setup(repo => repo.Update(It.IsAny<Product>()));

            // Mock Order Repo: Ekleme metodunu çağırırken bir şey yapma
            _mockOrderRepo.Setup(repo => repo.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

            // Mock SaveChanges: Kaydetme başarılı olduğunu simüle et
            _mockProductRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);


            // WHEN (Ne Zaman / İşlemi Gerçekleştir)
            var result = await _orderService.PlaceOrderAsync(userId, orderDto);


            // THEN (Beklentiler/Doğrulama)
            // 1. Stok doğru düşürüldü mü?
            Assert.Equal(initialStock - orderQuantity, mockProduct.StockQuantity);

            // 2. Sipariş ekleme metodu çağrıldı mı?
            _mockOrderRepo.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Once);

            // 3. Veritabanı kaydetme metodu çağrıldı mı? (Commit edildi mi?)
            _mockProductRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            // 4. Doğru kullanıcı ID'si atandı mı?
            Assert.Equal(userId, result.UserId);
        }


        // Test 2: Yetersiz Stok Durumu Testi
        [Fact]
        public async Task PlaceOrderAsync_ShouldThrowException_WhenStockIsNotSufficient()
        {
            // GIVEN (Verilenler/Hazırlık)
            var productId = 2;
            var orderQuantity = 50;
            var initialStock = 10; // Yetersiz stok
            var userId = 1;

            var mockProduct = new Product { Id = productId, Name = "Telefon", StockQuantity = initialStock, Price = 5000m };
            var orderDto = new OrderCreateDto { ProductId = productId, Quantity = orderQuantity };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(mockProduct);

            // WHEN / THEN (Beklenti, İşlemin İstisna Fırlatmasıdır)
            // İşlemin bir istisna fırlatmasını bekleriz (Stok Kontrolü hatası)
            await Assert.ThrowsAsync<Exception>(() =>
                _orderService.PlaceOrderAsync(userId, orderDto)
            );

            // Doğrulama: Stok düşürme veya kaydetme metotları çağrılmadı mı?
            _mockOrderRepo.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Never);
            _mockProductRepo.Verify(repo => repo.SaveChangesAsync(), Times.Never);

            // ÖNEMLİ: Transaction geri alındı mı?
            // Transaction geri alma testi (Rollback), DbContext Mock'lamanın karmaşıklığı nedeniyle bu seviyede zordur.
            // Bunun yerine, sadece AddAsync ve SaveChangesAsync'in çağrılmadığını doğrularız.
        }
    }
}
