using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
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
        public async Task<IActionResult> Create(Shop shop)
        {
            shop.Id = Guid.NewGuid();
            shop.CreatedAt = shop.UpdatedAt = DateTime.UtcNow;
            _db.Shops.Add(shop);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = shop.Id }, shop);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Shop updated)
        {
            var shop = await _db.Shops.FindAsync(id);
            if (shop == null) return NotFound();
            shop.Name = updated.Name;
            shop.Address = updated.Address;
            shop.City = updated.City;
            shop.Phone = updated.Phone;
            shop.Email = updated.Email;
            shop.IsActive = updated.IsActive;
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
