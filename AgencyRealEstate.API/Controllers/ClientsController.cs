using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgencyRealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,Manager,Realtor")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        // Получаем всех пользователей с ролью Client (RoleId = 4)
        var users = await _context.Users
            .Where(u => u.RoleId == 4 && u.IsActive)  // только активные клиенты
            .Select(u => new
            {
                u.UserId,
                u.Login,
                u.Email,
                u.IsActive,
                FullName = _context.Clients
                    .Where(c => c.UserId == u.UserId)
                    .Select(c => c.FullName)
                    .FirstOrDefault() ?? u.Login,   // если нет имени клиента, показываем логин
                Phone = _context.Clients
                    .Where(c => c.UserId == u.UserId)
                    .Select(c => c.Phone)
                    .FirstOrDefault() ?? "—",
                CreatedAt = u.CreatedAt
            })
            .OrderBy(u => u.Login)
            .ToListAsync();

        return Ok(users);
    }
}