using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EStockManager.Models.Entities;
using EStockManager.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace EStockManager.Services.Concrete
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // secterkey i byte dizisine dönüşüm
            string secretKey = _config["JwtSettings:SecretKey"]!;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string CreateToken(User user)
        {
            // token içinde saklanacaklar (claims)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // kimlik bilgilerini (credentials) oluştur
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // token tanımı (descriptor) oluşturma
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(double.Parse(_config["JwtSettings:ExpirationMinutes"]!)),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
