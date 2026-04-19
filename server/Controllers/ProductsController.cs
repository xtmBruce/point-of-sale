using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 12,
            [FromQuery] string? search = null, [FromQuery] string? sortBy = "name", [FromQuery] string? sortOrder = "asc")
        {
            var query = _db.Products.Where(p => p.IsActive)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Sku != null && p.Sku.Contains(search)));

            query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
            {
                ("price", "desc") => query.OrderByDescending(p => p.Price),
                ("price", _) => query.OrderBy(p => p.Price),
                (_, "desc") => query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };

            var total = await query.CountAsync();
            var products = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

            return Ok(new
            {
                products,
                pagination = new { total, page, limit, total_pages = (int)Math.Ceiling((double)total / limit) }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.Id = Guid.NewGuid();
            product.CreatedAt = product.UpdatedAt = DateTime.UtcNow;
            product.AvailableStock = product.CurrentStock - product.ReservedStock;
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Product updated)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            product.Name = updated.Name;
            product.Description = updated.Description;
            product.Sku = updated.Sku;
            product.Barcode = updated.Barcode;
            product.BrandId = updated.BrandId;
            product.CategoryId = updated.CategoryId;
            product.ProductType = updated.ProductType;
            product.Size = updated.Size;
            product.Color = updated.Color;
            product.Variant = updated.Variant;
            product.Price = updated.Price;
            product.CostPrice = updated.CostPrice;
            product.Currency = updated.Currency;
            product.MinStockLevel = updated.MinStockLevel;
            product.MaxStockLevel = updated.MaxStockLevel;
            product.ReorderPoint = updated.ReorderPoint;
            product.Unit = updated.Unit;
            product.Weight = updated.Weight;
            product.Dimensions = updated.Dimensions;
            product.ImageUrl = updated.ImageUrl;
            product.IsActive = updated.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
