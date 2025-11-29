using EStockManager.Models.Entities;

namespace EStockManager.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
