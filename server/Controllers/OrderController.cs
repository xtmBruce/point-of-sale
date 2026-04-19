using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Creator)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Creator)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // GET: api/Order/by-customer/5
        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomer(Guid customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        // GET: api/Order/by-shop/5
        [HttpGet("by-shop/{shopId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByShop(Guid shopId)
        {
            return await _context.Orders
                .Where(o => o.ShopId == shopId)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        // GET: api/Order/by-status/completed
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByStatus(string status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequest request)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                ShopId = request.ShopId,
                OrderNumber = await GenerateOrderNumber(),
                OrderType = request.OrderType ?? "regular",
                Status = "pending",
                Currency = request.Currency ?? "RWF",
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "pending",
                Notes = request.Notes,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add order items
            decimal subtotal = 0;
            foreach (var itemRequest in request.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.UnitPrice,
                    TotalPrice = itemRequest.Quantity * itemRequest.UnitPrice,
                    Currency = request.Currency ?? "RWF",
                    DiscountPercent = itemRequest.DiscountPercent,
                    ProductName = itemRequest.ProductName,
                    ProductType = itemRequest.ProductType,
                    CreatedAt = DateTime.UtcNow
                };

                order.OrderItems.Add(orderItem);
                subtotal += orderItem.TotalPrice;
            }

            // Calculate totals
            order.Subtotal = subtotal;
            order.DiscountAmount = request.DiscountAmount;
            order.LoyaltyDiscount = request.LoyaltyDiscount;
            order.TaxAmount = request.TaxAmount;
            order.TotalAmount = subtotal - order.DiscountAmount - order.LoyaltyDiscount + order.TaxAmount;
            order.AmountPaid = request.AmountPaid;
            order.RemainingAmount = order.TotalAmount - order.AmountPaid;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, UpdateOrderRequest request)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Update order properties
            order.Status = request.Status ?? order.Status;
            order.PaymentMethod = request.PaymentMethod ?? order.PaymentMethod;
            order.PaymentStatus = request.PaymentStatus ?? order.PaymentStatus;
            order.Notes = request.Notes ?? order.Notes;
            order.AmountPaid = request.AmountPaid ?? order.AmountPaid;
            order.RemainingAmount = order.TotalAmount - order.AmountPaid;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Order/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, UpdateOrderStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Order/5/payment
        [HttpPut("{id}/payment")]
        public async Task<IActionResult> UpdatePayment(Guid id, UpdatePaymentRequest request)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.AmountPaid = request.AmountPaid;
            order.PaymentMethod = request.PaymentMethod;
            order.PaymentStatus = request.PaymentStatus;
            order.RemainingAmount = order.TotalAmount - order.AmountPaid;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Order/summary
        [HttpGet("summary")]
        public async Task<ActionResult<OrderSummary>> GetOrderSummary()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "pending");
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "completed");
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            return new OrderSummary
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                TotalRevenue = totalRevenue
            };
        }

        private async Task<string> GenerateOrderNumber()
        {
            var today = DateTime.Today;
            var orderCount = await _context.Orders
                .CountAsync(o => o.CreatedAt.Date == today);
            
            return $"ORD-{today:yyyyMMdd}-{(orderCount + 1):D4}";
        }
    }

    // DTOs
    public class CreateOrderRequest
    {
        public Guid? CustomerId { get; set; }
        public Guid? ShopId { get; set; }
        public string? OrderType { get; set; }
        public string? Currency { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal LoyaltyDiscount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal AmountPaid { get; set; } = 0;
        public List<CreateOrderItemRequest> OrderItems { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        public Guid? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; } = 0;
        public string? ProductName { get; set; }
        public string? ProductType { get; set; }
    }

    public class UpdateOrderRequest
    {
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? Notes { get; set; }
        public decimal? AmountPaid { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class UpdatePaymentRequest
    {
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class OrderSummary
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}