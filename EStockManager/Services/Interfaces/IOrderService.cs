using EStockManager.Models.DTOs;
using EStockManager.Models.Entities;

namespace EStockManager.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(int userId, OrderCreateDto dto);
    }
}
