using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [Route("api/expenses")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ExpenseController(AppDbContext context) => _context = context;

        // GET: api/expenses
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] Guid? shop_id = null,
            [FromQuery] string? start_date = null,
            [FromQuery] string? end_date = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 50)
        {
            var query = _context.Expenses
                .Include(e => e.Shop)
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.Description.Contains(search) || e.Category.Contains(search) || (e.VendorName != null && e.VendorName.Contains(search)));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(e => e.Category == category);

            if (shop_id.HasValue)
                query = query.Where(e => e.ShopId == shop_id);

            if (!string.IsNullOrEmpty(start_date) && DateOnly.TryParse(start_date, out var startDt))
                query = query.Where(e => e.ExpenseDate >= startDt);

            if (!string.IsNullOrEmpty(end_date) && DateOnly.TryParse(end_date, out var endDt))
                query = query.Where(e => e.ExpenseDate <= endDt);

            var total = await query.CountAsync();
            var expenses = await query
                .OrderByDescending(e => e.ExpenseDate)
                .ThenByDescending(e => e.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(e => new
                {
                    e.Id,
                    e.Category,
                    e.Description,
                    e.Amount,
                    e.Currency,
                    expense_date = e.ExpenseDate.ToString("yyyy-MM-dd"),
                    e.PaymentMethod,
                    e.VendorName,
                    e.ReceiptUrl,
                    e.ShopId,
                    shop_name = e.Shop != null ? e.Shop.Name : null,
                    is_recurring = e.IsRecurring,
                    recurring_frequency = e.RecurringFrequency,
                    e.CreatedAt
                })
                .ToListAsync();

            return Ok(new { expenses, total, page, limit });
        }

        // GET: api/expenses/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var expense = await _context.Expenses.Include(e => e.Shop).FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
            if (expense == null) return NotFound(new { error = "Expense not found" });
            return Ok(new { expense });
        }

        // GET: api/expenses/categories/list
        [HttpGet("categories/list")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Expenses
                .Where(e => e.IsActive)
                .Select(e => new { category = e.Category })
                .Distinct()
                .OrderBy(c => c.category)
                .ToListAsync();
            return Ok(new { categories });
        }

        // GET: api/expenses/stats/overview
        [HttpGet("stats/overview")]
        public async Task<IActionResult> GetStats([FromQuery] Guid? shop_id = null)
        {
            var query = _context.Expenses.Where(e => e.IsActive);
            if (shop_id.HasValue) query = query.Where(e => e.ShopId == shop_id);

            var now = DateTime.UtcNow;
            var thisMonth = new DateOnly(now.Year, now.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            var total = await query.SumAsync(e => e.Amount);
            var thisMonthTotal = await query.Where(e => e.ExpenseDate >= thisMonth).SumAsync(e => e.Amount);
            var lastMonthTotal = await query.Where(e => e.ExpenseDate >= lastMonth && e.ExpenseDate < thisMonth).SumAsync(e => e.Amount);
            var count = await query.CountAsync();

            var byCategory = await query
                .GroupBy(e => e.Category)
                .Select(g => new { category = g.Key, total = g.Sum(e => e.Amount), count = g.Count() })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            return Ok(new { total, this_month = thisMonthTotal, last_month = lastMonthTotal, count, by_category = byCategory });
        }

        // POST: api/expenses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Category))
                return BadRequest(new { error = "Category is required" });
            if (string.IsNullOrWhiteSpace(request.Description))
                return BadRequest(new { error = "Description is required" });
            if (request.Amount <= 0)
                return BadRequest(new { error = "Amount must be greater than 0" });

            Guid? shopId = Guid.TryParse(request.ShopId, out var sid) ? sid : null;

            var expense = new Expense
            {
                ShopId = shopId,
                Category = request.Category,
                Description = request.Description,
                Amount = request.Amount,
                Currency = request.Currency,
                ExpenseDate = DateOnly.TryParse(request.ExpenseDate, out var d) ? d : DateOnly.FromDateTime(DateTime.UtcNow),
                PaymentMethod = request.PaymentMethod,
                VendorName = request.VendorName,
                ReceiptUrl = request.ReceiptUrl,
                IsRecurring = request.IsRecurring,
                RecurringFrequency = request.RecurringFrequency,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return Ok(new { expense });
        }

        // PUT: api/expenses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseRequest request)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null || !expense.IsActive) return NotFound(new { error = "Expense not found" });

            if (request.Category != null) expense.Category = request.Category;
            if (request.Description != null) expense.Description = request.Description;
            if (request.Amount.HasValue) expense.Amount = request.Amount.Value;
            if (request.Currency != null) expense.Currency = request.Currency;
            if (request.ExpenseDate != null && DateOnly.TryParse(request.ExpenseDate, out var d)) expense.ExpenseDate = d;
            if (request.PaymentMethod != null) expense.PaymentMethod = request.PaymentMethod;
            if (request.VendorName != null) expense.VendorName = request.VendorName;
            if (request.ReceiptUrl != null) expense.ReceiptUrl = request.ReceiptUrl;
            if (request.ShopId != null) expense.ShopId = Guid.TryParse(request.ShopId, out var sid) ? sid : expense.ShopId;
            expense.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { expense });
        }

        // DELETE: api/expenses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null || !expense.IsActive) return NotFound(new { error = "Expense not found" });

            expense.IsActive = false;
            expense.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Expense deleted" });
        }
    }
}
