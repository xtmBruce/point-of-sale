using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ShopsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shops = await _db.Shops.Where(s => s.IsActive).ToListAsync();
            return Ok(new { shops });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var shop = await _db.Shops.FindAsync(id);
            return shop == null ? NotFound() : Ok(shop);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShopRequest req)
        {
            // Validate request
            if (!req.IsValid(out var errorMessage))
                return BadRequest(new { error = errorMessage });

            var shop = new Shop
            {
                Id = Guid.NewGuid(),
                Name = req.Name.Trim(),
                Address = req.Address?.Trim(),
                City = req.City?.Trim(),
                State = req.State?.Trim(),
                Country = req.Country?.Trim(),
                PostalCode = req.PostalCode?.Trim(),
                Phone = req.Phone?.Trim(),
                Email = req.Email?.Trim(),
                ManagerId = req.ManagerId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Shops.Add(shop);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = shop.Id }, shop);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShopRequest req)
        {
            var shop = await _db.Shops.FindAsync(id);
            if (shop == null) return NotFound();

            if (string.IsNullOrWhiteSpace(req.Name) && req.Name != null)
                return BadRequest(new { error = "Shop name cannot be empty" });

            if (req.Name != null)
                shop.Name = req.Name.Trim();
            if (req.Address != null)
                shop.Address = req.Address.Trim();
            if (req.City != null)
                shop.City = req.City.Trim();
            if (req.State != null)
                shop.State = req.State.Trim();
            if (req.Country != null)
                shop.Country = req.Country.Trim();
            if (req.PostalCode != null)
                shop.PostalCode = req.PostalCode.Trim();
            if (req.Phone != null)
                shop.Phone = req.Phone.Trim();
            if (req.Email != null)
                shop.Email = req.Email.Trim();
            if (req.ManagerId.HasValue)
                shop.ManagerId = req.ManagerId;
            if (req.IsActive.HasValue)
                shop.IsActive = req.IsActive.Value;

            shop.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(shop);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var shop = await _db.Shops.FindAsync(id);
            if (shop == null) return NotFound();
            shop.IsActive = false;
            shop.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
