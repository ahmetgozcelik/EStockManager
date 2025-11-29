using System.Security.Claims;
using EStockManager.Models.DTOs;
using EStockManager.Models.Entities;
using EStockManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EStockManager.Controllers
{
    [Authorize] // kontrollerin tamamı yetkilendirme gerektirir
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if(userIdClaim == null)
                {
                    return Unauthorized(new { Message = "Yetkili kullanıcı ID'si bulunamadı." });
                }

                int userId = int.Parse(userIdClaim.Value);

                var newOrder = await _orderService.PlaceOrderAsync(userId, orderDto);

                return Created("", new { Message = "Sipariş başarıyla oluşturuldu.", OrderId = newOrder.Id });
            }
            catch (Exception ex)
            {
                // middleware devreye girecek ve 500 döndürecek.
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
