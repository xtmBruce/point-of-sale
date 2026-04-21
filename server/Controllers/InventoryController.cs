using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public InventoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("levels")]
        public async Task<IActionResult> GetLevels(
            [FromQuery(Name = "location_type")] string? locationType = "shop",
            [FromQuery(Name = "location_id")] Guid? locationId = null,
            [FromQuery] string? category = null,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 20 : Math.Min(limit, 200);

            if (!string.IsNullOrWhiteSpace(locationType) && !string.Equals(locationType, "shop", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only location_type=shop is currently supported" });

            var query = _db.ShopInventories
                .AsNoTracking()
                .Include(si => si.Product)
                .Include(si => si.Shop)
                .AsQueryable();

            if (locationId.HasValue)
                query = query.Where(si => si.ShopId == locationId.Value);

            if (!string.IsNullOrWhiteSpace(category) && Guid.TryParse(category, out var categoryId))
                query = query.Where(si => si.Product != null && si.Product.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(si =>
                    (si.Product != null && si.Product.Name.ToLower().Contains(term)) ||
                    (si.Product != null && si.Product.Sku != null && si.Product.Sku.ToLower().Contains(term)));
            }

            if (startDate.HasValue)
                query = query.Where(si => si.LastUpdated >= startDate.Value);

            if (endDate.HasValue)
            {
                var endExclusive = endDate.Value.Date.AddDays(1);
                query = query.Where(si => si.LastUpdated < endExclusive);
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(si => si.LastUpdated)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(si => new
                {
                    id = si.Id,
                    product_id = si.ProductId,
                    product_name = si.Product != null ? si.Product.Name : "Unknown Product",
                    sku = si.Product != null ? si.Product.Sku : null,
                    size = si.Product != null ? si.Product.Size : null,
                    product_type = si.Product != null ? si.Product.ProductType : "general",
                    category_name = si.Product != null && si.Product.Category != null ? si.Product.Category.Name : null,
                    location_type = "shop",
                    location_id = si.ShopId,
                    location_name = si.Shop != null ? si.Shop.Name : "Unknown Shop",
                    quantity = si.Quantity,
                    min_stock_level = si.MinStockLevel,
                    max_stock_level = si.MaxStockLevel,
                    reorder_point = si.ReorderPoint,
                    safety_stock = si.SafetyStock,
                    last_updated = si.LastUpdated
                })
                .ToListAsync();

            return Ok(new
            {
                data,
                pagination = new
                {
                    total,
                    page,
                    limit,
                    totalPages = (int)Math.Ceiling(total / (double)limit)
                }
            });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var activeAssignments = _db.ShopInventories
                .AsNoTracking()
                .Include(si => si.Product)
                .Where(si => si.Product != null && si.Product.IsActive);

            var totalProducts = await activeAssignments
                .Select(si => si.ProductId)
                .Distinct()
                .CountAsync();

            var outOfStockCount = await activeAssignments.CountAsync(si => si.Quantity <= 0);
            var lowStockCount = await activeAssignments.CountAsync(si => si.Quantity > 0 && si.Quantity <= si.MinStockLevel);

            var totalStockValue = await activeAssignments
                .SumAsync(si => (decimal)si.Quantity * (si.Product != null ? si.Product.Price : 0m));

            return Ok(new
            {
                total_products = totalProducts,
                total_stock_value = totalStockValue,
                low_stock_count = lowStockCount,
                out_of_stock_count = outOfStockCount
            });
        }

        [HttpGet("stock-info/{productId:guid}/{locationId:guid}")]
        public async Task<IActionResult> GetStockInfo(Guid productId, Guid locationId)
        {
            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
            if (product == null)
                return NotFound(new { error = "Product not found" });

            var shopExists = await _db.Shops.AsNoTracking().AnyAsync(s => s.Id == locationId && s.IsActive);
            if (!shopExists)
                return NotFound(new { error = "Shop not found" });

            var assignment = await _db.ShopInventories
                .AsNoTracking()
                .FirstOrDefaultAsync(si => si.ProductId == productId && si.ShopId == locationId);

            var totalAssigned = await _db.ShopInventories
                .AsNoTracking()
                .Where(si => si.ProductId == productId)
                .SumAsync(si => (int?)si.Quantity) ?? 0;

            var globalStock = product.CurrentStock > 0 ? product.CurrentStock : product.StockQuantity;
            var availableQuantity = Math.Max(globalStock - totalAssigned, 0);

            var shopSold = await _db.OrderItems
                .AsNoTracking()
                .Where(oi => oi.ProductId == productId && oi.Order != null && oi.Order.ShopId == locationId)
                .SumAsync(oi => (int?)oi.Quantity) ?? 0;

            var shopAssigned = assignment?.Quantity ?? 0;
            var shopRemaining = Math.Max(shopAssigned - shopSold, 0);

            return Ok(new
            {
                product_id = productId,
                location_id = locationId,
                available_quantity = availableQuantity,
                global_stock = globalStock,
                total_assigned = totalAssigned,
                shop_assigned = shopAssigned,
                shop_sold = shopSold,
                shop_remaining = shopRemaining
            });
        }

        [HttpGet("locations/{locationType}/{locationId:guid}/products/{productId:guid}/quantity")]
        public async Task<IActionResult> GetLocationProductQuantity(string locationType, Guid locationId, Guid productId)
        {
            if (!string.Equals(locationType, "shop", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only shop locations are currently supported" });

            var assignment = await _db.ShopInventories
                .AsNoTracking()
                .FirstOrDefaultAsync(si => si.ShopId == locationId && si.ProductId == productId);

            return Ok(new
            {
                location_type = "shop",
                location_id = locationId,
                product_id = productId,
                quantity = assignment?.Quantity ?? 0
            });
        }

        [HttpPost("assign-product-to-shop")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignProductToShop([FromBody] AssignProductToShopRequest req)
        {
            if (!req.IsValid(out var validationError))
                return BadRequest(new { error = validationError });

            var effectiveShopId = req.EffectiveShopId;

            var shopExists = await _db.Shops.AnyAsync(s => s.Id == effectiveShopId && s.IsActive);
            if (!shopExists)
                return BadRequest(new { error = "Shop not found or inactive" });

            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == req.ProductId && p.IsActive);
            if (product == null)
                return BadRequest(new { error = "Product not found or inactive" });

            var existingAssignment = await _db.ShopInventories
                .FirstOrDefaultAsync(x => x.ShopId == effectiveShopId && x.ProductId == req.ProductId);

            if (existingAssignment == null)
            {
                var newAssignment = new ShopInventory
                {
                    Id = Guid.NewGuid(),
                    ShopId = effectiveShopId,
                    ProductId = req.ProductId,
                    Quantity = req.Quantity,
                    MinStockLevel = req.MinStockLevel ?? product.MinStockLevel,
                    MaxStockLevel = req.MaxStockLevel ?? product.MaxStockLevel,
                    ReorderPoint = req.ReorderPoint ?? product.ReorderPoint,
                    SafetyStock = req.SafetyStock ?? 0,
                    LastUpdated = DateTime.UtcNow
                };

                _db.ShopInventories.Add(newAssignment);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Product assigned to shop successfully",
                    action = "inserted",
                    assignment = new
                    {
                        id = newAssignment.Id,
                        shop_id = newAssignment.ShopId,
                        product_id = newAssignment.ProductId,
                        quantity = newAssignment.Quantity,
                        min_stock_level = newAssignment.MinStockLevel,
                        max_stock_level = newAssignment.MaxStockLevel,
                        reorder_point = newAssignment.ReorderPoint,
                        safety_stock = newAssignment.SafetyStock,
                        last_updated = newAssignment.LastUpdated
                    }
                });
            }

            existingAssignment.Quantity = req.Quantity;
            existingAssignment.MinStockLevel = req.MinStockLevel ?? existingAssignment.MinStockLevel;
            existingAssignment.MaxStockLevel = req.MaxStockLevel ?? existingAssignment.MaxStockLevel;
            existingAssignment.ReorderPoint = req.ReorderPoint ?? existingAssignment.ReorderPoint;
            existingAssignment.SafetyStock = req.SafetyStock ?? existingAssignment.SafetyStock;
            existingAssignment.LastUpdated = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Shop product assignment updated successfully",
                action = "updated",
                assignment = new
                {
                    id = existingAssignment.Id,
                    shop_id = existingAssignment.ShopId,
                    product_id = existingAssignment.ProductId,
                    quantity = existingAssignment.Quantity,
                    min_stock_level = existingAssignment.MinStockLevel,
                    max_stock_level = existingAssignment.MaxStockLevel,
                    reorder_point = existingAssignment.ReorderPoint,
                    safety_stock = existingAssignment.SafetyStock,
                    last_updated = existingAssignment.LastUpdated
                }
            });
        }
    }
}