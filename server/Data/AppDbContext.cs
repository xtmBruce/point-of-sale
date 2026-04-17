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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Handle the composite unique key for CustomerDiscountUsage
            modelBuilder.Entity<CustomerDiscountUsage>()
                .HasIndex(c => new { c.CustomerId, c.DiscountId }).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}