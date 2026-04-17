using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Models;

namespace SmartPOS.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DiscountApplication> DiscountApplications { get; set; }
        public DbSet<CustomerDiscountUsage> CustomerDiscountUsages { get; set; }
        public DbSet<GiftCard> GiftCards { get; set; }
        public DbSet<GiftCardTransaction> GiftCardTransactions { get; set; }
        public DbSet<GiftCardTemplate> GiftCardTemplates { get; set; }

        public DbSet<GLAccountCategory> GLAccountCategories { get; set; }
        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<GLJournalEntry> GLJournalEntries { get; set; }
        public DbSet<GLJournalEntryLine> GLJournalEntryLines { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public DbSet<ShopInventory> ShopInventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<WarehouseInventory> WarehouseInventories { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<PriceChangeLog> PriceChangeLogs { get; set; }

        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierMaterial> SupplierMaterials { get; set; }
        public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; }
        public DbSet<PurchaseRequisitionItem> PurchaseRequisitionItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }
        public DbSet<SupplierPerformance> SupplierPerformances { get; set; }

        public DbSet<BottlingRecipe> BottlingRecipes { get; set; }
        public DbSet<RecipeMaterial> RecipeMaterials { get; set; }
        public DbSet<BottlingBatch> BottlingBatches { get; set; }
        public DbSet<CostComponent> CostComponents { get; set; }
        public DbSet<StockLedger> StockLedgers { get; set; }
        public DbSet<BottleSize> BottleSizes { get; set; }
        public DbSet<PerfumeBulk> PerfumeBulkItems { get; set; }
        public DbSet<PerfumeBottling> PerfumeBottlings { get; set; }

        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        public DbSet<BottleReturn> BottleReturns { get; set; }
        public DbSet<OrderStatusLog> OrderStatusLogs { get; set; }

        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationCampaign> NotificationCampaigns { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTrigger> NotificationTriggers { get; set; }
        public DbSet<CustomerNotificationPreference> CustomerNotificationPreferences { get; set; }
        public DbSet<NotificationAnalytic> NotificationAnalytics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Handle the composite unique key for CustomerDiscountUsage
            modelBuilder.Entity<CustomerDiscountUsage>()
                .HasIndex(c => new { c.CustomerId, c.DiscountId }).IsUnique();

            modelBuilder.Entity<ShopInventory>()
                .HasIndex(x => new { x.ShopId, x.ProductId }).IsUnique();

            modelBuilder.Entity<WarehouseInventory>()
                .HasIndex(x => new { x.WarehouseId, x.ProductId }).IsUnique();

            modelBuilder.Entity<CurrencyRate>()
                .HasIndex(x => new { x.FromCurrency, x.ToCurrency, x.RateDate }).IsUnique();

            modelBuilder.Entity<ExchangeRate>()
                .HasIndex(x => new { x.BaseCurrency, x.TargetCurrency }).IsUnique();

            modelBuilder.Entity<CustomerNotificationPreference>()
                .HasIndex(x => x.CustomerId).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}