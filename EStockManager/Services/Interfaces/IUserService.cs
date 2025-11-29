using EStockManager.Models.DTOs;
using EStockManager.Models.Entities;

namespace EStockManager.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(UserRegisterDto registerDto);
        Task<User> LoginAsync(UserLoginDto loginDto);
    }
}