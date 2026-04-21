namespace SmartPOS.API.DTOs
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? ShopId { get; set; }
        public string? ShopName { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}