using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;
using SmartPOS.API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            var (isCashier, cashierShopId) = await GetCurrentUserContextAsync();

            var query = _db.Products.Where(p => p.IsActive)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsQueryable();

            Dictionary<Guid, ShopInventory>? assignedByProduct = null;
            if (isCashier)
            {
                if (!cashierShopId.HasValue || cashierShopId == Guid.Empty)
                {
                    return Ok(new
                    {
                        products = Array.Empty<Product>(),
                        pagination = new { total = 0, page, limit, total_pages = 0 }
                    });
                }

                assignedByProduct = await _db.ShopInventories
                    .AsNoTracking()
                    .Where(si => si.ShopId == cashierShopId.Value)
                    .GroupBy(si => si.ProductId)
                    .Select(g => g.OrderByDescending(x => x.LastUpdated).First())
                    .ToDictionaryAsync(si => si.ProductId, si => si);

                if (assignedByProduct.Count == 0)
                {
                    return Ok(new
                    {
                        products = Array.Empty<Product>(),
                        pagination = new { total = 0, page, limit, total_pages = 0 }
                    });
                }

                var assignedProductIds = assignedByProduct.Keys.ToList();
                query = query.Where(p => assignedProductIds.Contains(p.Id));
            }

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

            if (isCashier && assignedByProduct != null)
            {
                foreach (var product in products)
                {
                    if (!assignedByProduct.TryGetValue(product.Id, out var assignment))
                        continue;

                    product.StockQuantity = assignment.Quantity;
                    product.CurrentStock = assignment.Quantity;
                    product.AvailableStock = assignment.Quantity;
                    product.MinStockLevel = assignment.MinStockLevel;
                    product.MaxStockLevel = assignment.MaxStockLevel;
                    product.ReorderPoint = assignment.ReorderPoint;
                }
            }

            return Ok(new
            {
                products,
                pagination = new { total, page, limit, total_pages = (int)Math.Ceiling((double)total / limit) }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var (isCashier, cashierShopId) = await GetCurrentUserContextAsync();

            var product = await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            if (isCashier)
            {
                if (!cashierShopId.HasValue || cashierShopId == Guid.Empty)
                    return NotFound();

                var assignment = await _db.ShopInventories
                    .AsNoTracking()
                    .Where(si => si.ShopId == cashierShopId.Value && si.ProductId == id)
                    .OrderByDescending(si => si.LastUpdated)
                    .FirstOrDefaultAsync();

                if (assignment == null)
                    return NotFound();

                product.StockQuantity = assignment.Quantity;
                product.CurrentStock = assignment.Quantity;
                product.AvailableStock = assignment.Quantity;
                product.MinStockLevel = assignment.MinStockLevel;
                product.MaxStockLevel = assignment.MaxStockLevel;
                product.ReorderPoint = assignment.ReorderPoint;
            }

            return Ok(product);
        }

        private async Task<(bool IsCashier, Guid? ShopId)> GetCurrentUserContextAsync()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (!string.Equals(role, "cashier", StringComparison.OrdinalIgnoreCase))
                return (false, null);

            var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(subject, out var userId))
                return (true, null);

            var user = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId && u.IsActive)
                .Select(u => new { u.ShopId })
                .FirstOrDefaultAsync();

            return (true, user?.ShopId);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest updated)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            
            if (updated.Name != null) product.Name = updated.Name;
            if (updated.Description != null) product.Description = updated.Description;
            if (updated.Sku != null) product.Sku = updated.Sku;
            if (updated.Barcode != null) product.Barcode = updated.Barcode;
            if (updated.BrandId.HasValue) product.BrandId = updated.BrandId;
            if (updated.CategoryId.HasValue) product.CategoryId = updated.CategoryId;
            if (updated.ProductType != null) product.ProductType = updated.ProductType;
            if (updated.Size != null) product.Size = updated.Size;
            if (updated.Color != null) product.Color = updated.Color;
            if (updated.Variant != null) product.Variant = updated.Variant;
            if (updated.Price.HasValue) product.Price = updated.Price.Value;
            if (updated.CostPrice.HasValue) product.CostPrice = updated.CostPrice;
            if (updated.Currency != null) product.Currency = updated.Currency;
            if (updated.MinStockLevel.HasValue) product.MinStockLevel = updated.MinStockLevel.Value;
            if (updated.MaxStockLevel.HasValue) product.MaxStockLevel = updated.MaxStockLevel;
            if (updated.ReorderPoint.HasValue) product.ReorderPoint = updated.ReorderPoint.Value;
            if (updated.Unit != null) product.Unit = updated.Unit;
            if (updated.Weight.HasValue) product.Weight = updated.Weight;
            if (updated.Dimensions != null) product.Dimensions = updated.Dimensions;
            if (updated.ImageUrl != null) product.ImageUrl = updated.ImageUrl;
            if (updated.IsActive.HasValue) product.IsActive = updated.IsActive.Value;
            
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
