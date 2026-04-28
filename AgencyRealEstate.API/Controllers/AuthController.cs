using System.ComponentModel.DataAnnotations;
using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using AgencyRealEstate.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgencyRealEstate.API.Constants;

namespace AgencyRealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public class RegisterRequest
    {
        [Required] public string Login { get; set; }
        [Required] public string Password { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Phone { get; set; }
    }



    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // Модель запроса с валидацией
    public class LoginRequest
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, MinimumLength = 1)]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(1)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Проверка модели
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Ищем пользователя
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == request.Login && u.IsActive);

        // 1 – пользователь не найден
        if (user == null)
        {
            return Unauthorized(new { error = "Пользователь с таким логином не найден или неактивен" });
        }

        // 2 – проверка пароля
        if (!PasswordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Unauthorized(new { error = "Неверный пароль" });
        }

        // Успешный вход – генерация токена
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
            signingCredentials: creds);

        return Ok(new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            FullName = user.Login,
            Role = user.Role.RoleName
        });



    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Login == request.Login))
            return Conflict("Логин уже занят");

        // Хеширование пароля
        var (hash, salt) = PasswordService.CreatePasswordHash(request.Password);

        // Создаём пользователя с ролью Client (4)
        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            RoleId = (byte)UserRoles.Client, 
            IsActive = true,
            CreatedByUserId = 1 // временно администратор
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Создаём запись в Clients
        var client = new Client
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            UserId = user.UserId,
            CreatedByUserId = 1
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Регистрация прошла успешно" });
    }
}