using EStockManager.Models.DTOs;
using EStockManager.Services.Concrete;
using EStockManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EStockManager.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // post api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            try
            {
                var user = await _userService.RegisterAsync(registerDto);
                // password hash döndürülmez
                return StatusCode(201, new { Message = "Kullanıcı başarıyla kaydedildi.", Email = user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // post api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var user = await _userService.LoginAsync(loginDto);
            if(user == null)
            {
                return Unauthorized(new { Message = "Kullanıcı adı veya şifre hatalı." });
            }

            string token = _tokenService.CreateToken(user);

            return Ok(new { Message = "Giriş başarılı.", Email = user.Email, Token = token });
        }
    }
}
