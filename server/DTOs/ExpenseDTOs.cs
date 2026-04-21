namespace SmartPOS.API.DTOs
{
    public class CreateExpenseRequest
    {
        public string? ShopId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "RWF";
        public string ExpenseDate { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string? VendorName { get; set; }
        public string? ReceiptUrl { get; set; }
        public bool IsRecurring { get; set; } = false;
        public string? RecurringFrequency { get; set; }
    }

    public class UpdateExpenseRequest
    {
        public string? ShopId { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? ExpenseDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? VendorName { get; set; }
        public string? ReceiptUrl { get; set; }
    }
}
