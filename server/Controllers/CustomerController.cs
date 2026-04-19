using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // GET: api/Customer/search?query=john
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> SearchCustomers(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return await _context.Customers.Take(10).ToListAsync();
            }

            return await _context.Customers
                .Where(c => c.FirstName.Contains(query) || 
                           c.LastName.Contains(query) || 
                           c.Email.Contains(query) || 
                           c.Phone.Contains(query))
                .ToListAsync();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerRequest request)
        {
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, UpdateCustomerRequest request)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.FirstName = request.FirstName ?? customer.FirstName;
            customer.LastName = request.LastName ?? customer.LastName;
            customer.Email = request.Email ?? customer.Email;
            customer.Phone = request.Phone ?? customer.Phone;
            customer.Address = request.Address ?? customer.Address;
            customer.City = request.City ?? customer.City;
            customer.State = request.State ?? customer.State;
            customer.Country = request.Country ?? customer.Country;
            customer.PostalCode = request.PostalCode ?? customer.PostalCode;
            customer.Birthday = request.Birthday ?? customer.Birthday;
            customer.AnniversaryDate = request.AnniversaryDate ?? customer.AnniversaryDate;
            customer.LoyaltyTier = request.LoyaltyTier ?? customer.LoyaltyTier;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Customer/5/loyalty
        [HttpPut("{id}/loyalty")]
        public async Task<IActionResult> UpdateLoyalty(Guid id, UpdateLoyaltyRequest request)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.LoyaltyPoints = request.LoyaltyPoints;
            customer.LoyaltyTier = request.LoyaltyTier;
            customer.TotalSpent = request.TotalSpent;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Customer/stats
        [HttpGet("stats")]
        public async Task<ActionResult<CustomerStats>> GetCustomerStats()
        {
            var totalCustomers = await _context.Customers.CountAsync();
            var activeCustomers = await _context.Customers.CountAsync(c => c.IsActive);
            var totalSpent = await _context.Customers.SumAsync(c => c.TotalSpent);
            var averageSpent = totalCustomers > 0 ? totalSpent / totalCustomers : 0;

            return new CustomerStats
            {
                TotalCustomers = totalCustomers,
                ActiveCustomers = activeCustomers,
                TotalSpent = totalSpent,
                AverageSpent = averageSpent
            };
        }
    }

    // DTOs
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateOnly? Birthday { get; set; }
        public DateOnly? AnniversaryDate { get; set; }
        public string? LoyaltyTier { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateOnly? Birthday { get; set; }
        public DateOnly? AnniversaryDate { get; set; }
        public string? LoyaltyTier { get; set; }
    }

    public class UpdateLoyaltyRequest
    {
        public int LoyaltyPoints { get; set; }
        public string LoyaltyTier { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
    }

    public class CustomerStats
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageSpent { get; set; }
    }
}