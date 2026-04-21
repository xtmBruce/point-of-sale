using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<CustomerListResponseDto>> GetCustomers(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 100)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.FirstName.Contains(search) ||
                    c.LastName.Contains(search) ||
                    (c.Email != null && c.Email.Contains(search)) ||
                    (c.Phone != null && c.Phone.Contains(search)));
            }

            var total = await query.CountAsync();
            var customers = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(c => MapToDto(c))
                .ToListAsync();

            return Ok(new CustomerListResponseDto
            {
                Customers = customers,
                Total = total,
                Page = page,
                Limit = limit,
                Pagination = new PaginationDto
                {
                    Total = total,
                    Page = page,
                    Limit = limit,
                    TotalPages = (int)Math.Ceiling((double)total / limit)
                }
            });
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound(new { error = "Customer not found" });
            return Ok(new { customer = MapToDto(customer) });
        }

        // GET: api/customers/{id}/orders
        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetCustomerOrders(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            if (!await _context.Customers.AnyAsync(c => c.Id == id))
                return NotFound(new { error = "Customer not found" });

            var orders = await _context.Orders
                .Where(o => o.CustomerId == id)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(o => new
                {
                    o.Id,
                    o.Status,
                    o.PaymentMethod,
                    o.PaymentStatus,
                    o.TotalAmount,
                    o.CreatedAt
                })
                .ToListAsync();

            var total = await _context.Orders.CountAsync(o => o.CustomerId == id);
            return Ok(new { orders, total });
        }

        // GET: api/customers/stats
        [HttpGet("stats")]
        public async Task<ActionResult<CustomerStatsDto>> GetStats()
        {
            var total = await _context.Customers.CountAsync();
            var active = await _context.Customers.CountAsync(c => c.IsActive);
            var totalSpent = await _context.Customers.SumAsync(c => c.TotalSpent);

            return Ok(new CustomerStatsDto
            {
                TotalCustomers = total,
                ActiveCustomers = active,
                TotalSpent = totalSpent,
                AverageSpent = total > 0 ? totalSpent / total : 0
            });
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerResponseDto>> CreateCustomer(
            [FromBody] CreateCustomerRequest request)
        {
            if (!request.IsValid(out var validationError))
                return BadRequest(new { error = validationError });

            if (!string.IsNullOrEmpty(request.Email))
            {
                var exists = await _context.Customers.AnyAsync(c => c.Email == request.Email);
                if (exists)
                    return Conflict(new { error = $"Customer with email '{request.Email}' already exists" });
            }

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode,
                Birthday = request.Birthday,
                AnniversaryDate = request.AnniversaryDate,
                LoyaltyTier = request.LoyaltyTier ?? "bronze",
                LoyaltyPoints = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id },
                new { customer = MapToDto(customer) });
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(
            Guid id,
            [FromBody] UpdateCustomerRequest request)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound(new { error = "Customer not found" });

            if (request.FirstName != null) customer.FirstName = request.FirstName;
            if (request.LastName != null) customer.LastName = request.LastName;
            if (request.Email != null) customer.Email = request.Email;
            if (request.Phone != null) customer.Phone = request.Phone;
            if (request.Address != null) customer.Address = request.Address;
            if (request.City != null) customer.City = request.City;
            if (request.State != null) customer.State = request.State;
            if (request.Country != null) customer.Country = request.Country;
            if (request.PostalCode != null) customer.PostalCode = request.PostalCode;
            if (request.Birthday.HasValue) customer.Birthday = request.Birthday;
            if (request.AnniversaryDate.HasValue) customer.AnniversaryDate = request.AnniversaryDate;
            if (request.LoyaltyTier != null) customer.LoyaltyTier = request.LoyaltyTier;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { customer = MapToDto(customer) });
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound(new { error = "Customer not found" });

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Customer deleted" });
        }

        private static CustomerResponseDto MapToDto(Customer c) => new()
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            City = c.City,
            State = c.State,
            Country = c.Country,
            PostalCode = c.PostalCode,
            LoyaltyPoints = c.LoyaltyPoints,
            LoyaltyTier = c.LoyaltyTier,
            TotalSpent = c.TotalSpent,
            Birthday = c.Birthday,
            AnniversaryDate = c.AnniversaryDate,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}
