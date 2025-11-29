using System;
using EStockManager.Models.DTOs;
using EStockManager.Models.Entities;
using EStockManager.Repositories.Interfaces;
using EStockManager.Services.Interfaces;

namespace EStockManager.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;

        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        // register işlemi
        public async Task<User> RegisterAsync(UserRegisterDto registerDto)
        {
            // e posta mevcutta var mı yok mu
            var existingUser = _userRepository.Where(u => u.Email == registerDto.Email).FirstOrDefault();
            if (existingUser != null)
            {
                throw new Exception("Bu e-posta adresi zaten kullanımda.");
            }

            // parola hash
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // entity 
            var newUser = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                // parolanın hashli hali
                PasswordHash = hashedPassword,
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            return newUser;
        }

        // kayıt
        public async Task<User> LoginAsync(UserLoginDto loginDto)
        {
            // e posta ile user bul
            var user = _userRepository.Where(u => u.Email == loginDto.Email).FirstOrDefault();

            if (user == null)
            {
                // k.adı veya şifre hatalı gibi bir şey döndür. ya da k. yok de.
                return null;
            }

            bool isPasswordVerified = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordVerified)
            {
                return null; // şifre hatalı
            }

            return user;
        }
    }
}
