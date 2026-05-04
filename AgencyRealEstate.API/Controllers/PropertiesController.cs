using System.Security.Claims;
using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using AgencyRealEstate.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgencyRealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PropertiesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Создание нового объекта (доступно риелтору, менеджеру, администратору)</summary>
    [HttpPost]
    [Authorize(Roles = "Administrator,Manager,Realtor")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var property = new Property
        {
            Address = request.Address,
            PropertyTypeId = request.PropertyTypeId,
            TotalArea = request.TotalArea,
            LivingArea = request.LivingArea,
            Floor = request.Floor,
            TotalFloors = request.TotalFloors,
            Rooms = request.Rooms,
            WallMaterialId = request.WallMaterialId,
            Price = request.Price,
            Description = request.Description,
            PropertyStatusId = 1,   // "Available"
            CreatedByUserId = GetCurrentUserId()
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        return Ok(new { property.PropertyId });
    }

    /// <summary>Получение списка доступных объектов (для всех авторизованных пользователей)</summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<PropertyDto>>> GetAll()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var properties = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.WallMaterial)
            .Include(p => p.PropertyStatus)
            .Include(p => p.PropertyPhotos)
            .Where(p => !p.IsDeleted
                        && p.PropertyStatus.StatusName != "Sold"
                        && p.PropertyStatus.StatusName != "Rented")
            .Select(p => new PropertyDto
            {
                PropertyID = p.PropertyId,
                Address = p.Address,
                PropertyTypeName = p.PropertyType.Name,
                TotalArea = p.TotalArea,
                LivingArea = p.LivingArea,
                Floor = p.Floor,
                TotalFloors = p.TotalFloors,
                Rooms = p.Rooms,
                WallMaterialName = p.WallMaterial != null ? p.WallMaterial.Name : null,
                Price = p.Price,
                Description = p.Description,
                StatusName = p.PropertyStatus.StatusName,
                Latitude = (double?)p.Latitude,
                Longitude = (double?)p.Longitude,
                PhotoUrls = p.PropertyPhotos.Select(ph => $"{baseUrl}/uploads/{ph.PhotoUrl}").ToList()
            })
            .ToListAsync();

        return Ok(properties);
    }

    /// <summary>Детальная информация об объекте (для всех авторизованных)</summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PropertyDto>> GetById(int id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var property = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.WallMaterial)
            .Include(p => p.PropertyStatus)
            .Include(p => p.PropertyPhotos)
            .Where(p => !p.IsDeleted
                        && p.PropertyStatus.StatusName != "Sold"
                        && p.PropertyStatus.StatusName != "Rented")
            .Select(p => new PropertyDto
            {
                PropertyID = p.PropertyId,
                Address = p.Address,
                PropertyTypeName = p.PropertyType.Name,
                TotalArea = p.TotalArea,
                LivingArea = p.LivingArea,
                Floor = p.Floor,
                TotalFloors = p.TotalFloors,
                Rooms = p.Rooms,
                WallMaterialName = p.WallMaterial != null ? p.WallMaterial.Name : null,
                Price = p.Price,
                Description = p.Description,
                StatusName = p.PropertyStatus.StatusName,
                Latitude = (double?)p.Latitude,
                Longitude = (double?)p.Longitude,
                PhotoUrls = p.PropertyPhotos.Select(ph => $"{baseUrl}/uploads/{ph.PhotoUrl}").ToList()
            })
            .FirstOrDefaultAsync(p => p.PropertyID == id);

        if (property == null)
            return NotFound();

        return Ok(property);
    }

    // Вспомогательный метод для получения ID текущего пользователя из JWT
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found in token");
        return int.Parse(userIdClaim);
    }
}

// DTO для создания объекта (можно вынести в отдельный файл)
public class CreatePropertyRequest
{
    public string Address { get; set; }
    public int PropertyTypeId { get; set; }
    public decimal TotalArea { get; set; }
    public decimal? LivingArea { get; set; }
    public int? Floor { get; set; }
    public int? TotalFloors { get; set; }
    public int? Rooms { get; set; }
    public int? WallMaterialId { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
}