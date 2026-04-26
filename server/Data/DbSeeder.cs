using SmartPOS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartPOS.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Seed Users
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@smartpos.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var managerUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "manager",
                Email = "manager@smartpos.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                FirstName = "Manager",
                LastName = "User",
                Role = "Manager",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cashierUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "cashier",
                Email = "cashier@smartpos.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Cashier123!"),
                FirstName = "Cashier",
                LastName = "User",
                Role = "Cashier",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(adminUser, managerUser, cashierUser);

            // Seed Shops
            var mainShop = new Shop
            {
                Id = Guid.NewGuid(),
                Name = "Main Store",
                Address = "123 Main Street",
                City = "Kigali",
                Country = "Rwanda",
                Phone = "+250788123456",
                Email = "main@smartpos.com",
                ManagerId = managerUser.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var branchShop = new Shop
            {
                Id = Guid.NewGuid(),
                Name = "Branch Store",
                Address = "456 Branch Avenue",
                City = "Kigali",
                Country = "Rwanda",
                Phone = "+250788654321",
                Email = "branch@smartpos.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Shops.AddRange(mainShop, branchShop);

            // Seed Warehouses
            var mainWarehouse = new Warehouse
            {
                Id = Guid.NewGuid(),
                Name = "Main Warehouse",
                Code = "WH001",
                Address = "Industrial Zone, Kigali",
                City = "Kigali",
                Country = "Rwanda",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Warehouses.Add(mainWarehouse);

            // Seed Categories
            var perfumeCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Perfumes",
                Description = "Fragrances and perfumes",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cosmeticsCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Cosmetics",
                Description = "Beauty and cosmetic products",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Categories.AddRange(perfumeCategory, cosmeticsCategory);

            // Seed Brands
            var chanelBrand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Chanel",
                Description = "Luxury French fashion house",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var diorBrand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = "Dior",
                Description = "French luxury goods company",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Brands.AddRange(chanelBrand, diorBrand);

            // Seed Products
            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Chanel No. 5",
                Description = "Classic floral aldehyde fragrance",
                Sku = "CHANEL-NO5-50ML",
                Barcode = "3145891355505",
                CategoryId = perfumeCategory.Id,
                BrandId = chanelBrand.Id,
                Price = 12000,
                CostPrice = 8000,
                Currency = "RWF",
                StockQuantity = 50,
                CurrentStock = 50,
                ReservedStock = 0,
                AvailableStock = 50,
                MinStockLevel = 10,
                ReorderPoint = 15,
                Unit = "bottle",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Dior Sauvage",
                Description = "Fresh and spicy fragrance",
                Sku = "DIOR-SAV-60ML",
                Barcode = "3348901419628",
                CategoryId = perfumeCategory.Id,
                BrandId = diorBrand.Id,
                Price = 15000,
                CostPrice = 10000,
                Currency = "RWF",
                StockQuantity = 30,
                CurrentStock = 30,
                ReservedStock = 0,
                AvailableStock = 30,
                MinStockLevel = 5,
                ReorderPoint = 10,
                Unit = "bottle",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Products.AddRange(product1, product2);

            // Seed Customers
            var customer1 = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+250788111222",
                LoyaltyPoints = 100,
                TotalSpent = 50000,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var customer2 = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "+250788333444",
                LoyaltyPoints = 50,
                TotalSpent = 25000,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Customers.AddRange(customer1, customer2);

            // Seed Shop Inventories
            var shopInventory1 = new ShopInventory
            {
                Id = Guid.NewGuid(),
                ShopId = mainShop.Id,
                ProductId = product1.Id,
                Quantity = 30,
                MinStockLevel = 10,
                ReorderPoint = 15,
                SafetyStock = 5,
                LastUpdated = DateTime.UtcNow
            };

            var shopInventory2 = new ShopInventory
            {
                Id = Guid.NewGuid(),
                ShopId = mainShop.Id,
                ProductId = product2.Id,
                Quantity = 20,
                MinStockLevel = 5,
                ReorderPoint = 10,
                SafetyStock = 3,
                LastUpdated = DateTime.UtcNow
            };

            context.ShopInventories.AddRange(shopInventory1, shopInventory2);

            // Seed Warehouse Inventories
            var warehouseInventory1 = new WarehouseInventory
            {
                Id = Guid.NewGuid(),
                WarehouseId = mainWarehouse.Id,
                ProductId = product1.Id,
                Quantity = 20,
                MinStockLevel = 10,
                ReorderPoint = 15,
                SafetyStock = 5,
                LastUpdated = DateTime.UtcNow
            };

            var warehouseInventory2 = new WarehouseInventory
            {
                Id = Guid.NewGuid(),
                WarehouseId = mainWarehouse.Id,
                ProductId = product2.Id,
                Quantity = 10,
                MinStockLevel = 5,
                ReorderPoint = 10,
                SafetyStock = 3,
                LastUpdated = DateTime.UtcNow
            };

            context.WarehouseInventories.AddRange(warehouseInventory1, warehouseInventory2);

            // Seed Expense Categories
            var rentCategory = new ExpenseCategory
            {
                Id = Guid.NewGuid(),
                Name = "Rent",
                Description = "Shop and warehouse rent",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var utilitiesCategory = new ExpenseCategory
            {
                Id = Guid.NewGuid(),
                Name = "Utilities",
                Description = "Electricity, water, internet",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.ExpenseCategories.AddRange(rentCategory, utilitiesCategory);

            // Seed Exchange Rates
            var usdRate = new ExchangeRate
            {
                Id = Guid.NewGuid(),
                BaseCurrency = "RWF",
                TargetCurrency = "USD",
                Rate = 0.00078m,
                LastUpdated = DateTime.UtcNow
            };

            var eurRate = new ExchangeRate
            {
                Id = Guid.NewGuid(),
                BaseCurrency = "RWF",
                TargetCurrency = "EUR",
                Rate = 0.00072m,
                LastUpdated = DateTime.UtcNow
            };

            context.ExchangeRates.AddRange(usdRate, eurRate);

            // Seed Bottle Sizes
            var size50ml = new BottleSize
            {
                Id = Guid.NewGuid(),
                SizeMl = 50,
                BottleCost = 500,
                LabelCost = 100,
                PackagingCost = 200,
                LaborCost = 150,
                Quantity = 100,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var size100ml = new BottleSize
            {
                Id = Guid.NewGuid(),
                SizeMl = 100,
                BottleCost = 800,
                LabelCost = 150,
                PackagingCost = 300,
                LaborCost = 200,
                Quantity = 80,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.BottleSizes.AddRange(size50ml, size100ml);

            // Seed Gift Card Templates
            var template1 = new GiftCardTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Birthday Special",
                Description = "Birthday gift card design",
                DesignUrl = "/templates/birthday.png",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.GiftCardTemplates.Add(template1);

            // Seed Settings
            var taxSetting = new Setting
            {
                Id = Guid.NewGuid(),
                Key = "TaxRate",
                Value = "18",
                Description = "VAT tax rate percentage",
                Category = "tax",
                UpdatedAt = DateTime.UtcNow
            };

            var currencySetting = new Setting
            {
                Id = Guid.NewGuid(),
                Key = "DefaultCurrency",
                Value = "RWF",
                Description = "Default currency for transactions",
                Category = "general",
                UpdatedAt = DateTime.UtcNow
            };

            context.Settings.AddRange(taxSetting, currencySetting);

            await context.SaveChangesAsync();
        }
    }
}
