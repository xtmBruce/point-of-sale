using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;

namespace SmartPOS.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class IncomeReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IncomeReportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("income-report")]
        public async Task<ActionResult<IncomeReportResponseDto>> GetIncomeReport([FromQuery] IncomeReportQueryDto query)
        {
            var report = await BuildIncomeReport(query);
            return Ok(report);
        }

        [HttpGet("income-report/pdf")]
        public async Task<IActionResult> GetIncomeReportPdf([FromQuery] IncomeReportQueryDto query)
        {
            var report = await BuildIncomeReport(query);
            var csv = BuildCsv(report);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"income-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
        }

        [HttpGet("income-report/excel")]
        public async Task<IActionResult> GetIncomeReportExcel([FromQuery] IncomeReportQueryDto query)
        {
            var report = await BuildIncomeReport(query);
            var csv = BuildCsv(report);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"income-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
        }

        private async Task<IncomeReportResponseDto> BuildIncomeReport(IncomeReportQueryDto query)
        {
            var (startDate, endDate) = ResolveDateRange(query.start_date, query.end_date);

            var ordersQuery = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate.AddDays(1));

            if (query.shop_id.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.ShopId == query.shop_id);
            }

            var orders = await ordersQuery
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var expenseQuery = _context.Expenses
                .AsNoTracking()
                .Include(e => e.Shop)
                .Where(e => e.ExpenseDate >= DateOnly.FromDateTime(startDate) && e.ExpenseDate <= DateOnly.FromDateTime(endDate));

            if (query.shop_id.HasValue)
            {
                expenseQuery = expenseQuery.Where(e => e.ShopId == query.shop_id);
            }

            var expenses = await expenseQuery
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();

            var completedOrders = orders
                .Where(o => string.Equals(o.Status, "completed", StringComparison.OrdinalIgnoreCase) || string.Equals(o.PaymentStatus, "complete", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
            var totalCogs = completedOrders.Sum(o => o.OrderItems.Sum(item => item.Quantity * (item.Product?.CostPrice ?? 0m)));
            var totalExpenses = expenses.Sum(e => e.Amount);
            var grossProfit = totalRevenue - totalCogs;
            var netProfit = grossProfit - totalExpenses;
            var profitMargin = totalRevenue > 0 ? (netProfit / totalRevenue) * 100m : 0m;

            return new IncomeReportResponseDto
            {
                Summary = new IncomeReportSummaryDto
                {
                    TotalRevenue = totalRevenue,
                    TotalCogs = totalCogs,
                    GrossProfit = grossProfit,
                    TotalExpenses = totalExpenses,
                    NetProfit = netProfit,
                    ProfitMargin = profitMargin,
                    TotalOrders = orders.Count
                },
                Orders = orders.Select(order => new IncomeReportOrderDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    CreatedAt = order.CreatedAt,
                    CustomerName = order.Customer == null ? null : $"{order.Customer.FirstName} {order.Customer.LastName}".Trim(),
                    ShopName = order.Shop?.Name,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    ProductNames = string.Join(", ", order.OrderItems
                        .Where(item => !string.IsNullOrWhiteSpace(item.ProductName))
                        .Select(item => item.ProductName)
                        .Distinct()),
                    Categories = string.Join(", ", order.OrderItems
                        .Where(item => item.Product?.Category?.Name != null)
                        .Select(item => item.Product!.Category!.Name)
                        .Distinct())
                }).ToList(),
                Expenses = expenses.Select(expense => new IncomeReportExpenseDto
                {
                    Id = expense.Id,
                    ExpenseDate = expense.ExpenseDate,
                    Category = expense.Category,
                    Description = expense.Description,
                    Amount = expense.Amount,
                    ShopName = expense.Shop?.Name
                }).ToList()
            };
        }

        private static (DateTime Start, DateTime End) ResolveDateRange(string? startDate, string? endDate)
        {
            var start = DateTime.Today.AddDays(-30);
            var end = DateTime.Today;

            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var parsedStart))
            {
                start = parsedStart.Date;
            }

            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var parsedEnd))
            {
                end = parsedEnd.Date;
            }

            if (end < start)
            {
                (start, end) = (end, start);
            }

            return (start, end);
        }

        private static string BuildCsv(IncomeReportResponseDto report)
        {
            var lines = new List<string>
            {
                "Income Report",
                $"Total Revenue,{report.Summary.TotalRevenue}",
                $"Cost of Goods Sold,{report.Summary.TotalCogs}",
                $"Gross Profit,{report.Summary.GrossProfit}",
                $"Total Expenses,{report.Summary.TotalExpenses}",
                $"Net Profit,{report.Summary.NetProfit}",
                $"Profit Margin,{report.Summary.ProfitMargin}",
                $"Total Orders,{report.Summary.TotalOrders}",
                string.Empty,
                "Orders",
                "Order Number,Date,Customer,Shop,Amount,Status,Product Names,Categories"
            };

            foreach (var order in report.Orders)
            {
                lines.Add(string.Join(",", new[]
                {
                    EscapeCsv(order.OrderNumber),
                    EscapeCsv(order.CreatedAt.ToString("yyyy-MM-dd")),
                    EscapeCsv(order.CustomerName ?? string.Empty),
                    EscapeCsv(order.ShopName ?? string.Empty),
                    EscapeCsv(order.TotalAmount.ToString()),
                    EscapeCsv(order.Status),
                    EscapeCsv(order.ProductNames ?? string.Empty),
                    EscapeCsv(order.Categories ?? string.Empty)
                }));
            }

            lines.Add(string.Empty);
            lines.Add("Expenses");
            lines.Add("Date,Category,Description,Amount,Shop");

            foreach (var expense in report.Expenses)
            {
                lines.Add(string.Join(",", new[]
                {
                    EscapeCsv(expense.ExpenseDate.ToString("yyyy-MM-dd")),
                    EscapeCsv(expense.Category),
                    EscapeCsv(expense.Description),
                    EscapeCsv(expense.Amount.ToString()),
                    EscapeCsv(expense.ShopName ?? string.Empty)
                }));
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}