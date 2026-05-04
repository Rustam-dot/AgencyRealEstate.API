using System.Security.Claims;
using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using AgencyRealEstate.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgencyRealEstate.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize(Roles = "Client")] // Только клиенты могут редактировать свой профиль
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/profile – получить данные текущего пользователя
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        int userId = GetCurrentUserId();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("Пользователь не найден");

        // Вместо Include(u => u.Client) делаем отдельный запрос к Clients
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);

        return Ok(new
        {
            Login = user.Login,
            Email = user.Email,
            FullName = client?.FullName ?? "",
            Phone = client?.Phone ?? "",
            PassportData = client?.PassportData ?? "",
            Preferences = client?.Preferences ?? ""
        });
    }

    // PUT api/profile – обновить профиль
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        int userId = GetCurrentUserId();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("Пользователь не найден");

        // Обновляем логин, если передан и не занят
        if (!string.IsNullOrWhiteSpace(request.Login) && request.Login != user.Login)
        {
            if (await _context.Users.AnyAsync(u => u.Login == request.Login && u.UserId != userId))
                return Conflict("Этот логин уже используется");
            user.Login = request.Login;
        }

        // Обновляем email
        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        // Обновляем пароль
        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            var (hash, salt) = PasswordService.CreatePasswordHash(request.NewPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
        }

        // Обновляем данные клиента
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (client == null)
        {
            // Если записи клиента ещё нет – создаём
            client = new Client
            {
                UserId = userId,
                CreatedByUserId = userId
            };
            _context.Clients.Add(client);
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
            client.FullName = request.FullName;
        if (!string.IsNullOrWhiteSpace(request.Phone))
            client.Phone = request.Phone;
        
        client.PassportData = request.PassportData;
        client.Preferences = request.Preferences;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Профиль обновлён" });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) throw new UnauthorizedAccessException();
        return int.Parse(userIdClaim);
    }

    public class UpdateProfileRequest
    {
        public string? Login { get; set; }
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? PassportData { get; set; }
        public string? Preferences { get; set; }
    }
}