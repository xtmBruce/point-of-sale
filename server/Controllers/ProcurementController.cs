using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [Route("api/procurement")]
    [ApiController]
    public class ProcurementController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProcurementController(AppDbContext context) => _context = context;

        // ─── SUPPLIERS ────────────────────────────────────────────────────────────

        // GET: api/procurement/suppliers
        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliers([FromQuery] string? search = null)
        {
            var query = _context.Suppliers.Where(s => s.IsActive).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.Name.Contains(search) || (s.ContactPerson != null && s.ContactPerson.Contains(search)));

            var suppliers = await query.OrderBy(s => s.Name).ToListAsync();
            return Ok(new { suppliers });
        }

        // POST: api/procurement/suppliers
        [HttpPost("suppliers")]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { error = "Supplier name is required" });

            var supplier = new Supplier
            {
                Name = request.Name,
                ContactPerson = request.ContactPerson,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Country = request.Country,
                PaymentTerms = request.PaymentTerms,
                Notes = request.Notes,
                IsActive = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return Ok(new { supplier });
        }

        // PUT: api/procurement/suppliers/{id}
        [HttpPut("suppliers/{id}")]
        public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] CreateSupplierRequest request)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null || !supplier.IsActive) return NotFound(new { error = "Supplier not found" });

            supplier.Name = request.Name ?? supplier.Name;
            supplier.ContactPerson = request.ContactPerson ?? supplier.ContactPerson;
            supplier.Email = request.Email ?? supplier.Email;
            supplier.Phone = request.Phone ?? supplier.Phone;
            supplier.Address = request.Address ?? supplier.Address;
            supplier.Country = request.Country ?? supplier.Country;
            supplier.PaymentTerms = request.PaymentTerms ?? supplier.PaymentTerms;
            supplier.Notes = request.Notes ?? supplier.Notes;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { supplier });
        }

        // DELETE: api/procurement/suppliers/{id}
        [HttpDelete("suppliers/{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound(new { error = "Supplier not found" });
            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Supplier deleted" });
        }

        // ─── RAW MATERIALS ────────────────────────────────────────────────────────

        // GET: api/procurement/materials
        [HttpGet("materials")]
        public async Task<IActionResult> GetMaterials()
        {
            var materials = await _context.RawMaterials
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
            return Ok(new { materials });
        }

        // ─── PURCHASE ORDERS ──────────────────────────────────────────────────────

        // GET: api/procurement/purchase-orders
        [HttpGet("purchase-orders")]
        public async Task<IActionResult> GetPurchaseOrders(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 50)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.Supplier)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(po => po.PONumber.Contains(search) ||
                    (po.Supplier != null && po.Supplier.Name.Contains(search)) ||
                    (po.Notes != null && po.Notes.Contains(search)));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(po => po.Status == status);

            var total = await query.CountAsync();
            var orders = await query
                .OrderByDescending(po => po.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(po => new
                {
                    po.Id,
                    po_number = po.PONumber,
                    supplier_id = po.SupplierId,
                    supplier_name = po.Supplier != null ? po.Supplier.Name : null,
                    order_date = po.OrderDate.ToString("yyyy-MM-dd"),
                    expected_delivery_date = po.ExpectedDeliveryDate != null ? po.ExpectedDeliveryDate.Value.ToString("yyyy-MM-dd") : null,
                    po.Status,
                    po.TotalAmount,
                    total_amount_rfw = po.TotalAmountRFW,
                    po.Currency,
                    po.CurrencyRate,
                    po.Notes,
                    po.CreatedAt,
                    items_count = _context.PurchaseOrderItems.Count(i => i.PurchaseOrderId == po.Id),
                    total_quantity = _context.PurchaseOrderItems.Where(i => i.PurchaseOrderId == po.Id).Sum(i => (decimal?)i.QuantityOrdered) ?? 0
                })
                .ToListAsync();

            return Ok(new { purchase_orders = orders, total, page, limit });
        }

        // GET: api/procurement/purchase-orders/{id}
        [HttpGet("purchase-orders/{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(Guid id)
        {
            var po = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null) return NotFound(new { error = "Purchase order not found" });

            var items = await _context.PurchaseOrderItems
                .Where(i => i.PurchaseOrderId == id)
                .Select(i => new
                {
                    i.Id,
                    material_id = i.MaterialId,
                    item_name = i.ItemName,
                    i.QuantityOrdered,
                    i.QuantityReceived,
                    i.UnitCost,
                    i.TotalCost,
                    i.Notes
                })
                .ToListAsync();

            var result = new
            {
                po.Id,
                po_number = po.PONumber,
                supplier_id = po.SupplierId,
                supplier_name = po.Supplier?.Name,
                order_date = po.OrderDate.ToString("yyyy-MM-dd"),
                expected_delivery_date = po.ExpectedDeliveryDate?.ToString("yyyy-MM-dd"),
                po.Status,
                po.TotalAmount,
                total_amount_rfw = po.TotalAmountRFW,
                po.Currency,
                currency_rate = po.CurrencyRate,
                transport_supplier_cost = po.TransportSupplierCost,
                bank_charges = po.BankCharges,
                transport_kigali_cost = po.TransportKigaliCost,
                laisse_suivre_cost = po.LaisseSuivreCost,
                import_taxes = po.ImportTaxes,
                storage_cost = po.StorageCost,
                declarant_fees = po.DeclarantFees,
                transport_warehouse_cost = po.TransportWarehouseCost,
                po.PaymentTerms,
                po.ShippingAddress,
                po.Notes,
                po.CreatedAt,
                items
            };

            return Ok(new { purchase_order = result });
        }

        // POST: api/procurement/purchase-orders
        [HttpPost("purchase-orders")]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SupplierName))
                return BadRequest(new { error = "Supplier name is required" });
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(new { error = "At least one item is required" });

            // Find or create supplier by name
            Guid supplierId;
            if (Guid.TryParse(request.SupplierId, out var parsedSupplierId) && parsedSupplierId != Guid.Empty)
            {
                supplierId = parsedSupplierId;
            }
            else
            {
                var existing = await _context.Suppliers.FirstOrDefaultAsync(s => s.Name == request.SupplierName && s.IsActive);
                if (existing != null)
                {
                    supplierId = existing.Id;
                }
                else
                {
                    var newSupplier = new Supplier
                    {
                        Name = request.SupplierName,
                        IsActive = true,
                        IsApproved = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Suppliers.Add(newSupplier);
                    await _context.SaveChangesAsync();
                    supplierId = newSupplier.Id;
                }
            }

            var poNumber = await GeneratePONumber();

            var po = new PurchaseOrder
            {
                PONumber = poNumber,
                SupplierId = supplierId,
                OrderDate = DateOnly.TryParse(request.OrderDate, out var od) ? od : DateOnly.FromDateTime(DateTime.UtcNow),
                ExpectedDeliveryDate = DateOnly.TryParse(request.ExpectedDeliveryDate, out var edd) ? edd : null,
                Status = "pending",
                Currency = request.Currency,
                CurrencyRate = (decimal?)request.CurrencyRate,
                TotalAmount = request.TotalAmount,
                TotalAmountRFW = request.TotalAmountRfw,
                TransportSupplierCost = request.TransportSupplierCost,
                BankCharges = request.BankCharges,
                TransportKigaliCost = request.TransportKigaliCost,
                LaisseSuivreCost = request.LaisseSuivreCost,
                ImportTaxes = request.ImportTaxes,
                StorageCost = request.StorageCost,
                DeclarantFees = request.DeclarantFees,
                TransportWarehouseCost = request.TransportWarehouseCost,
                PaymentTerms = request.PaymentTerms,
                ShippingAddress = request.ShippingAddress,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();

            foreach (var item in request.Items)
            {
                var poItem = new PurchaseOrderItem
                {
                    PurchaseOrderId = po.Id,
                    MaterialId = Guid.TryParse(item.MaterialId, out var mid) ? mid : null,
                    ItemName = item.ItemName,
                    QuantityOrdered = item.QuantityOrdered,
                    UnitCost = item.UnitCost,
                    TotalCost = item.QuantityOrdered * item.UnitCost,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PurchaseOrderItems.Add(poItem);
            }

            await _context.SaveChangesAsync();
            return Ok(new { purchase_order = new { po.Id, po_number = po.PONumber, po.Status } });
        }

        // PUT: api/procurement/purchase-orders/{id}
        [HttpPut("purchase-orders/{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(Guid id, [FromBody] CreatePurchaseOrderRequest request)
        {
            var po = await _context.PurchaseOrders.FindAsync(id);
            if (po == null) return NotFound(new { error = "Purchase order not found" });
            if (po.Status != "pending") return BadRequest(new { error = "Only pending orders can be edited" });

            po.Currency = request.Currency;
            po.CurrencyRate = request.CurrencyRate;
            po.TotalAmount = request.TotalAmount;
            po.TotalAmountRFW = request.TotalAmountRfw;
            po.TransportSupplierCost = request.TransportSupplierCost;
            po.BankCharges = request.BankCharges;
            po.TransportKigaliCost = request.TransportKigaliCost;
            po.LaisseSuivreCost = request.LaisseSuivreCost;
            po.ImportTaxes = request.ImportTaxes;
            po.StorageCost = request.StorageCost;
            po.DeclarantFees = request.DeclarantFees;
            po.TransportWarehouseCost = request.TransportWarehouseCost;
            po.PaymentTerms = request.PaymentTerms;
            po.ShippingAddress = request.ShippingAddress;
            po.Notes = request.Notes;
            if (DateOnly.TryParse(request.OrderDate, out var od)) po.OrderDate = od;
            if (DateOnly.TryParse(request.ExpectedDeliveryDate, out var edd)) po.ExpectedDeliveryDate = edd;
            po.UpdatedAt = DateTime.UtcNow;

            // Replace items
            var existingItems = _context.PurchaseOrderItems.Where(i => i.PurchaseOrderId == id);
            _context.PurchaseOrderItems.RemoveRange(existingItems);

            foreach (var item in request.Items)
            {
                _context.PurchaseOrderItems.Add(new PurchaseOrderItem
                {
                    PurchaseOrderId = po.Id,
                    MaterialId = Guid.TryParse(item.MaterialId, out var mid) ? mid : null,
                    ItemName = item.ItemName,
                    QuantityOrdered = item.QuantityOrdered,
                    UnitCost = item.UnitCost,
                    TotalCost = item.QuantityOrdered * item.UnitCost,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Purchase order updated" });
        }

        // PATCH: api/procurement/purchase-orders/{id}/status
        [HttpPatch("purchase-orders/{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePurchaseOrderStatusRequest request)
        {
            var po = await _context.PurchaseOrders.FindAsync(id);
            if (po == null) return NotFound(new { error = "Purchase order not found" });

            var validStatuses = new[] { "pending", "approved", "in_transit", "delivered", "cancelled" };
            if (!validStatuses.Contains(request.Status))
                return BadRequest(new { error = $"Invalid status. Valid values: {string.Join(", ", validStatuses)}" });

            po.Status = request.Status;
            po.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Status updated", status = po.Status });
        }

        // DELETE: api/procurement/purchase-orders/{id}
        [HttpDelete("purchase-orders/{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(Guid id)
        {
            var po = await _context.PurchaseOrders.FindAsync(id);
            if (po == null) return NotFound(new { error = "Purchase order not found" });
            if (po.Status != "pending") return BadRequest(new { error = "Only pending orders can be deleted" });

            var items = _context.PurchaseOrderItems.Where(i => i.PurchaseOrderId == id);
            _context.PurchaseOrderItems.RemoveRange(items);
            _context.PurchaseOrders.Remove(po);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Purchase order deleted" });
        }

        private async Task<string> GeneratePONumber()
        {
            var today = DateTime.Today;
            var count = await _context.PurchaseOrders.CountAsync(p => p.CreatedAt.Date == today);
            return $"PO-{today:yyyyMMdd}-{(count + 1):D4}";
        }
    }
}
