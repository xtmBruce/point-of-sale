namespace SmartPOS.API.DTOs
{
    public class CreatePurchaseOrderRequest
    {
        public string? SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string OrderDate { get; set; } = string.Empty;
        public string? ExpectedDeliveryDate { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ShippingAddress { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal CurrencyRate { get; set; } = 1;
        public decimal TransportSupplierCost { get; set; } = 0;
        public decimal BankCharges { get; set; } = 0;
        public decimal TransportKigaliCost { get; set; } = 0;
        public decimal LaisseSuivreCost { get; set; } = 0;
        public decimal ImportTaxes { get; set; } = 0;
        public decimal StorageCost { get; set; } = 0;
        public decimal DeclarantFees { get; set; } = 0;
        public decimal TransportWarehouseCost { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public decimal? TotalAmountRfw { get; set; }
        public string? Notes { get; set; }
        public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderItemRequest
    {
        public string? MaterialId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal QuantityOrdered { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class UpdatePurchaseOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class CreateSupplierRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? PaymentTerms { get; set; }
        public string? Notes { get; set; }
    }
}
