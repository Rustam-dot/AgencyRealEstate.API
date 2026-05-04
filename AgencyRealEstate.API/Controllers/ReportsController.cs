using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgencyRealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,Manager")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReport()
    {
        var statusCounts = await _context.Properties
            .Where(p => !p.IsDeleted)
            .GroupBy(p => p.PropertyStatus.StatusName)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        var typeCounts = await _context.Properties
            .Where(p => !p.IsDeleted)
            .GroupBy(p => p.PropertyType.Name)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        return Ok(new
        {
            StatusCounts = statusCounts.ToDictionary(x => x.Key, x => x.Count),
            TypeCounts = typeCounts.ToDictionary(x => x.Key, x => x.Count)
        });
    }
}