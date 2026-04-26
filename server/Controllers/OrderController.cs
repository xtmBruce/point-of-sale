using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [Route("api/orders")]
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
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? search = null,
            [FromQuery] string? customer = null,
            [FromQuery] string? status = null,
            [FromQuery] string? payment_method = null,
            [FromQuery] Guid? shop_id = null,
            [FromQuery] string? start_date = null,
            [FromQuery] string? end_date = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Search by order number or product name
            if (!string.IsNullOrEmpty(search))
                query = query.Where(o =>
                    o.OrderNumber.Contains(search) ||
                    o.OrderItems.Any(i => i.ProductName != null && i.ProductName.Contains(search)));

            // Search by customer name or email
            if (!string.IsNullOrEmpty(customer))
                query = query.Where(o =>
                    o.Customer != null && (
                        o.Customer.FirstName.Contains(customer) ||
                        o.Customer.LastName.Contains(customer) ||
                        (o.Customer.FirstName + " " + o.Customer.LastName).Contains(customer) ||
                        (o.Customer.Email != null && o.Customer.Email.Contains(customer)) ||
                        (o.Customer.Phone != null && o.Customer.Phone.Contains(customer))));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrEmpty(payment_method))
                query = query.Where(o => o.PaymentMethod == payment_method);

            if (shop_id.HasValue)
                query = query.Where(o => o.ShopId == shop_id);

            if (!string.IsNullOrEmpty(start_date) && DateTime.TryParse(start_date, out var startDt))
                query = query.Where(o => o.CreatedAt >= startDt);

            if (!string.IsNullOrEmpty(end_date) && DateTime.TryParse(end_date, out var endDt))
                query = query.Where(o => o.CreatedAt <= endDt.AddDays(1));

            var total = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                orders,
                pagination = new
                {
                    total,
                    page,
                    limit,
                    total_pages = (int)Math.Ceiling((double)total / limit)
                }
            });
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

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = new Order
                {
                    CustomerId = request.CustomerId,
                    ShopId = request.ShopId,
                    OrderNumber = await GenerateOrderNumber(),
                    OrderType = request.OrderType ?? "regular",
                    Status = request.Status ?? (request.PaymentStatus == "pending" ? "pending" : "completed"),
                    Currency = request.Currency ?? "RWF",
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = request.PaymentStatus ?? "complete",
                    Notes = request.Notes,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                decimal subtotal = 0;
                foreach (var item in request.Items)
                {
                    var unitPrice = item.Price > 0 ? item.Price : item.UnitPrice;
                    var totalPrice = item.Total > 0 ? item.Total : item.Quantity * unitPrice;

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = totalPrice,
                        Currency = request.Currency ?? "RWF",
                        ProductName = item.ProductName,
                        ProductType = item.ProductType,
                        CreatedAt = DateTime.UtcNow
                    };

                    order.OrderItems.Add(orderItem);
                    subtotal += totalPrice;
                }

                order.Subtotal = request.Subtotal > 0 ? request.Subtotal : subtotal;
                order.TaxAmount = request.TaxAmount;
                order.TotalAmount = request.TotalAmount > 0 ? request.TotalAmount : subtotal;
                order.AmountPaid = request.AmountPaid;
                order.RemainingAmount = request.RemainingAmount > 0 ? request.RemainingAmount : order.TotalAmount - order.AmountPaid;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    order_id = order.Id,
                    order_number = order.OrderNumber,
                    status = order.Status,
                    total_amount = order.TotalAmount,
                    order = order
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to create order", details = ex.Message });
            }
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
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
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
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
        public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] UpdatePaymentRequest request)
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
        public string? PaymentStatus { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        public decimal Subtotal { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public decimal AmountPaid { get; set; } = 0;
        public decimal RemainingAmount { get; set; } = 0;
        public List<CreateOrderItemRequest> Items { get; set; } = new();
        public object? PaymentDetails { get; set; }
    }

    public class CreateOrderItemRequest
    {
        public Guid? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
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