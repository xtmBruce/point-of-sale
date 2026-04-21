namespace SmartPOS.API.DTOs
{
    public class DashboardOverviewResponse
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int PeriodDays { get; set; }
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class TopProductDto
    {
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class SalesAnalyticsResponse
    {
        public List<DailySalesDto> DailySales { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
    }

    public class CategoryBreakdownDto
    {
        public string? Category { get; set; }
        public int Count { get; set; }
    }

    public class InventoryAnalyticsResponse
    {
        public int TotalProducts { get; set; }
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<CategoryBreakdownDto> CategoryBreakdown { get; set; } = new();
    }

    public class TopCustomerDto
    {
        public Guid? CustomerId { get; set; }
        public decimal TotalSpent { get; set; }
        public int Orders { get; set; }
    }

    public class CustomerAnalyticsResponse
    {
        public int TotalCustomers { get; set; }
        public int NewCustomers { get; set; }
        public List<TopCustomerDto> TopCustomers { get; set; } = new();
    }

    public class RevenueTrendDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class RevenueTrendsResponse
    {
        public List<RevenueTrendDto> Trends { get; set; } = new();
    }
}
