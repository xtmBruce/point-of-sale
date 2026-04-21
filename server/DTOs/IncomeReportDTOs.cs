namespace SmartPOS.API.DTOs
{
    public class IncomeReportQueryDto
    {
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public Guid? shop_id { get; set; }
    }

    public class IncomeReportResponseDto
    {
        public IncomeReportSummaryDto Summary { get; set; } = new();
        public List<IncomeReportOrderDto> Orders { get; set; } = new();
        public List<IncomeReportExpenseDto> Expenses { get; set; } = new();
    }

    public class IncomeReportSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCogs { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public int TotalOrders { get; set; }
    }

    public class IncomeReportOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CustomerName { get; set; }
        public string? ShopName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ProductNames { get; set; }
        public string? Categories { get; set; }
    }

    public class IncomeReportExpenseDto
    {
        public Guid Id { get; set; }
        public DateOnly ExpenseDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? ShopName { get; set; }
    }
}