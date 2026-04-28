using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using AgencyRealEstate.API.DTOs;
using AgencyRealEstate.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgencyRealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShowingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShowingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShowingRequest request)
    {
       
        int currentUserId = GetCurrentUserId();

       
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Phone == request.ClientPhone);
        if (client == null)
        {
            client = new Client
            {
                FullName = request.ClientName,
                Phone = request.ClientPhone,
                CreatedByUserId = currentUserId
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

      
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == currentUserId);
        int realtorId;
        if (employee != null)
        {
            realtorId = employee.EmployeeId;
        }
        else
        {
          
            realtorId = await _context.Employees.MinAsync(e => e.EmployeeId);
        }

       
        var showing = new Showing
        {
            PropertyId = request.PropertyId,
            ClientId = client.ClientId,
            RealtorId = realtorId,
            ShowingDateTime = request.Date.Add(request.Time ?? TimeSpan.Zero),
            Comments = request.Comments,
            CreatedByUserId = currentUserId
        };

        _context.Showings.Add(showing);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Заявка создана", showingId = showing.ShowingId });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("Пользователь не найден в токене");
        return int.Parse(userIdClaim);
    }
}