using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using AgencyRealEstate.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgencyRealEstate.API.Controllers;

[Authorize(Roles = "Administrator,Manager,Realtor,Client")]
[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PropertiesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<List<PropertyDto>>> GetAll()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var properties = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.WallMaterial)
            .Include(p => p.PropertyStatus)
            .Include(p => p.PropertyPhotos)
            .Where(p => !p.IsDeleted)
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

    [HttpGet("{id}")]
    public async Task<ActionResult<PropertyDto>> GetById(int id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var property = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.WallMaterial)
            .Include(p => p.PropertyStatus)
            .Include(p => p.PropertyPhotos)
            .Where(p => !p.IsDeleted)
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

        if (property == null) return NotFound();
        return Ok(property);
    }


}