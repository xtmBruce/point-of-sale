using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) => _db = db;

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] int period = 30,
            [FromQuery] string? shop = null)
        {
            var start = string.IsNullOrEmpty(startDate) ? DateTime.UtcNow.AddDays(-period) : DateTime.Parse(startDate);
            var end = string.IsNullOrEmpty(endDate) ? DateTime.UtcNow : DateTime.Parse(endDate).AddDays(1);

            var ordersQuery = _db.Orders.Where(o => o.CreatedAt >= start && o.CreatedAt <= end);
            if (!string.IsNullOrEmpty(shop) && shop != "all" && Guid.TryParse(shop, out var shopId))
                ordersQuery = ordersQuery.Where(o => o.ShopId == shopId);

            var totalRevenue = await ordersQuery.Where(o => o.Status != "cancelled").SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
            var totalOrders = await ordersQuery.CountAsync();
            var totalCustomers = await _db.Customers.CountAsync(c => c.IsActive);
            var totalProducts = await _db.Products.CountAsync(p => p.IsActive);
            var lowStockProducts = await _db.Products.CountAsync(p => p.IsActive && p.StockQuantity <= p.MinStockLevel);
            var totalExpenses = await _db.Expenses.Where(e => e.ExpenseDate >= DateOnly.FromDateTime(start) && e.ExpenseDate <= DateOnly.FromDateTime(end)).SumAsync(e => (decimal?)e.Amount) ?? 0;

            var paymentMethodStats = await ordersQuery
                .Where(o => o.Status != "cancelled" && o.PaymentMethod != null)
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new
                {
                    payment_method = g.Key,
                    total_amount = g.Sum(o => o.TotalAmount),
                    count = g.Count()
                })
                .ToListAsync();

            return Ok(new
            {
                total_revenue = totalRevenue,
                total_orders = totalOrders,
                total_customers = totalCustomers,
                total_products = totalProducts,
                low_stock_products = lowStockProducts,
                total_expenses = totalExpenses,
                net_profit = totalRevenue - totalExpenses,
                period_days = period,
                payment_method_stats = paymentMethodStats
            });
        }

        [HttpGet("sales-analytics")]
        public async Task<IActionResult> GetSalesAnalytics(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] int period = 30)
        {
            var start = string.IsNullOrEmpty(startDate) ? DateTime.UtcNow.AddDays(-period) : DateTime.Parse(startDate);
            var end = string.IsNullOrEmpty(endDate) ? DateTime.UtcNow : DateTime.Parse(endDate).AddDays(1);

            var dailySales = await _db.Orders
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end && o.Status != "cancelled")
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailySalesDto { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount), Orders = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var topProducts = await _db.OrderItems
                .Where(oi => oi.Order.CreatedAt >= start && oi.Order.CreatedAt <= end)
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(5)
                .ToListAsync();

            return Ok(new SalesAnalyticsResponse { DailySales = dailySales, TopProducts = topProducts });
        }

        [HttpGet("inventory-analytics")]
        public async Task<IActionResult> GetInventoryAnalytics()
        {
            var totalProducts = await _db.Products.CountAsync(p => p.IsActive);
            var lowStock = await _db.Products.CountAsync(p => p.IsActive && p.StockQuantity <= p.MinStockLevel && p.StockQuantity > 0);
            var outOfStock = await _db.Products.CountAsync(p => p.IsActive && p.StockQuantity == 0);
            var totalValue = await _db.Products.Where(p => p.IsActive).SumAsync(p => (decimal?)p.StockQuantity * p.CostPrice) ?? 0;

            var categoryBreakdown = await _db.Products
                .Where(p => p.IsActive && p.CategoryId != null)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new CategoryBreakdownDto { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(new InventoryAnalyticsResponse
            {
                TotalProducts = totalProducts,
                LowStock = lowStock,
                OutOfStock = outOfStock,
                TotalInventoryValue = totalValue,
                CategoryBreakdown = categoryBreakdown
            });
        }

        [HttpGet("customer-analytics")]
        public async Task<IActionResult> GetCustomerAnalytics(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] int period = 30)
        {
            var start = string.IsNullOrEmpty(startDate) ? DateTime.UtcNow.AddDays(-period) : DateTime.Parse(startDate);
            var end = string.IsNullOrEmpty(endDate) ? DateTime.UtcNow : DateTime.Parse(endDate).AddDays(1);

            var totalCustomers = await _db.Customers.CountAsync(c => c.IsActive);
            var newCustomers = await _db.Customers.CountAsync(c => c.CreatedAt >= start && c.CreatedAt <= end);
            var topCustomers = await _db.Orders
                .Where(o => o.CustomerId != null && o.CreatedAt >= start && o.CreatedAt <= end)
                .GroupBy(o => o.CustomerId)
                .Select(g => new TopCustomerDto { CustomerId = g.Key, TotalSpent = g.Sum(o => o.TotalAmount), Orders = g.Count() })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToListAsync();

            return Ok(new CustomerAnalyticsResponse
            {
                TotalCustomers = totalCustomers,
                NewCustomers = newCustomers,
                TopCustomers = topCustomers
            });
        }

        [HttpGet("revenue-trends")]
        public async Task<IActionResult> GetRevenueTrends([FromQuery] int period = 30)
        {
            var start = DateTime.UtcNow.AddDays(-period);

            var trends = await _db.Orders
                .Where(o => o.CreatedAt >= start && o.Status != "cancelled")
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new RevenueTrendDto { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount), Orders = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Ok(new RevenueTrendsResponse { Trends = trends });
        }
    }
}
