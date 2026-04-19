using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BrandsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _db.Brands.Where(b => b.IsActive).ToListAsync();
            return Ok(new { brands });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var brand = await _db.Brands.FindAsync(id);
            return brand == null ? NotFound() : Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            brand.Id = Guid.NewGuid();
            brand.CreatedAt = brand.UpdatedAt = DateTime.UtcNow;
            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Brand updated)
        {
            var brand = await _db.Brands.FindAsync(id);
            if (brand == null) return NotFound();
            brand.Name = updated.Name;
            brand.Description = updated.Description;
            brand.LogoUrl = updated.LogoUrl;
            brand.Website = updated.Website;
            brand.Country = updated.Country;
            brand.IsActive = updated.IsActive;
            brand.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(brand);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var brand = await _db.Brands.FindAsync(id);
            if (brand == null) return NotFound();
            brand.IsActive = false;
            brand.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
