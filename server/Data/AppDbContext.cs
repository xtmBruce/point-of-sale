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

        // Warehouse and Inventory
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ShopInventory> ShopInventories { get; set; }
        public DbSet<WarehouseInventory> WarehouseInventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }

        // GL Accounting
        public DbSet<GLAccountCategory> GLAccountCategories { get; set; }
        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<GLJournalEntry> GLJournalEntries { get; set; }
        public DbSet<GLJournalEntryLine> GLJournalEntryLines { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<PriceChangeLog> PriceChangeLogs { get; set; }
        public DbSet<Setting> Settings { get; set; }

        // Procurement
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

        // Bottling and Production
        public DbSet<BottleSizes> BottleSizes { get; set; }
        public DbSet<PerfumeBulk> PerfumeBulks { get; set; }
        public DbSet<BottlingRecipe> BottlingRecipes { get; set; }
        public DbSet<RecipeMaterial> RecipeMaterials { get; set; }
        public DbSet<BottlingBatch> BottlingBatches { get; set; }
        public DbSet<CostComponent> CostComponents { get; set; }
        public DbSet<StockLedger> StockLedgers { get; set; }
        public DbSet<PerfumeBottling> PerfumeBottlings { get; set; }

        // Notifications
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationCampaign> NotificationCampaigns { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTrigger> NotificationTriggers { get; set; }
        public DbSet<CustomerNotificationPreference> CustomerNotificationPreferences { get; set; }
        public DbSet<NotificationAnalytic> NotificationAnalytics { get; set; }

        // Loyalty
        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        public DbSet<BottleReturn> BottleReturns { get; set; }

        // Orders
        public DbSet<OrderStatusLog> OrderStatusLogs { get; set; }

        // Expenses
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite Unique Constraints
            modelBuilder.Entity<CustomerDiscountUsage>()
                .HasIndex(c => new { c.CustomerId, c.DiscountId }).IsUnique();

            modelBuilder.Entity<ShopInventory>()
                .HasIndex(si => new { si.ShopId, si.ProductId }).IsUnique();

            modelBuilder.Entity<WarehouseInventory>()
                .HasIndex(wi => new { wi.WarehouseId, wi.ProductId }).IsUnique();

            modelBuilder.Entity<ExchangeRate>()
                .HasIndex(er => new { er.BaseCurrency, er.TargetCurrency }).IsUnique();

            modelBuilder.Entity<CurrencyRate>()
                .HasIndex(cr => new { cr.FromCurrency, cr.ToCurrency, cr.RateDate }).IsUnique();

            modelBuilder.Entity<CustomerNotificationPreference>()
                .HasIndex(cnp => cnp.CustomerId).IsUnique();

            // Unique Constraints for single columns
            modelBuilder.Entity<GLAccountCategory>()
                .HasIndex(glac => glac.Code).IsUnique();

            modelBuilder.Entity<GLAccount>()
                .HasIndex(gla => gla.AccountCode).IsUnique();

            modelBuilder.Entity<GLJournalEntry>()
                .HasIndex(glje => glje.EntryNumber).IsUnique();

            modelBuilder.Entity<Warehouse>()
                .HasIndex(w => w.Code).IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Name).IsUnique();

            modelBuilder.Entity<PurchaseRequisition>()
                .HasIndex(pr => pr.RequisitionNumber).IsUnique();

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(po => po.PONumber).IsUnique();

            modelBuilder.Entity<GoodsReceipt>()
                .HasIndex(gr => gr.GRNNumber).IsUnique();

            modelBuilder.Entity<BottlingBatch>()
                .HasIndex(bb => bb.BatchNumber).IsUnique();

            modelBuilder.Entity<Setting>()
                .HasIndex(s => s.Key).IsUnique();

            // Foreign Key Cascading Deletes
            modelBuilder.Entity<GLJournalEntryLine>()
                .HasOne(gjel => gjel.JournalEntry)
                .WithMany(gje => gje.Lines)
                .HasForeignKey(gjel => gjel.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseRequisitionItem>()
                .HasOne(pri => pri.Requisition)
                .WithMany(pr => pr.Items)
                .HasForeignKey(pri => pri.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GoodsReceiptItem>()
                .HasOne(gri => gri.GoodsReceipt)
                .WithMany(gr => gr.Items)
                .HasForeignKey(gri => gri.GoodsReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeMaterial>()
                .HasOne(rm => rm.Recipe)
                .WithMany(br => br.RecipeMaterials)
                .HasForeignKey(rm => rm.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CostComponent>()
                .HasOne(cc => cc.Batch)
                .WithMany(bb => bb.CostComponents)
                .HasForeignKey(cc => cc.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}