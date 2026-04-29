using AgencyRealEstate.API.Data;
using AgencyRealEstate.API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgencyRealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,Manager,Realtor")]
public class ImagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ImagesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("upload/{propertyId}")]
    public async Task<IActionResult> Upload(int propertyId, List<IFormFile> files)
    {
        var property = await _context.Properties.FindAsync(propertyId);
        if (property == null)
            return NotFound("Объект не найден");

        if (files == null || files.Count == 0)
            return BadRequest("Не выбраны файлы");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            // Уникальное имя, чтобы избежать конфликтов
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Запись в БД
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var photo = new PropertyPhoto
            {
                PropertyId = propertyId,
                PhotoUrl = fileName,
                IsMain = !_context.PropertyPhotos.Any(p => p.PropertyId == propertyId),
                UploadedByUserId = userId,   // настоящий пользователь
                UploadDate = DateTime.UtcNow
            };
            _context.PropertyPhotos.Add(photo);
        }

        await _context.SaveChangesAsync();
        return Ok(new { uploaded = files.Count });
    }
}