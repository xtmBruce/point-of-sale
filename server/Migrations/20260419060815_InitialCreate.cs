using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPOS.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BottleSizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SizeMl = table.Column<int>(type: "int", nullable: false),
                    BottleCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LabelCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PackagingCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LaborCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottleSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromCurrency = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToCurrency = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(15,6)", nullable: false),
                    RateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoyaltyPoints = table.Column<int>(type: "int", nullable: false),
                    LoyaltyTier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalSpent = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    AnniversaryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseCurrency = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TargetCurrency = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(15,6)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiftCardTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftCardTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLAccountCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLAccountCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLAccountCategories_GLAccountCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "GLAccountCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RawMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStock = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MinStockLevel = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxStockLevel = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ReorderPoint = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SafetyStock = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostPerUnit = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    ShelfLifeDays = table.Column<int>(type: "int", nullable: true),
                    StorageRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualityStandards = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    SupplierCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CapacitySqm = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BottlingRecipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BottleSizeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedProductionTime = table.Column<int>(type: "int", nullable: true),
                    TargetCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MarkupPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QualityStandards = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShelfLifeDays = table.Column<int>(type: "int", nullable: true),
                    BatchSizeMin = table.Column<int>(type: "int", nullable: true),
                    BatchSizeMax = table.Column<int>(type: "int", nullable: true),
                    ProductionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YieldPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    WastePercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecipeImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstructionManualUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SafetyInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestingRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualityCheckpoints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EfficiencyRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottlingRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BottlingRecipes_BottleSizes_BottleSizeId",
                        column: x => x.BottleSizeId,
                        principalTable: "BottleSizes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PerfumeBulkItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BulkQuantityMl = table.Column<int>(type: "int", nullable: false),
                    CostPerMl = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfumeBulkItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfumeBulkItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Variant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CurrentStock = table.Column<int>(type: "int", nullable: false),
                    ReservedStock = table.Column<int>(type: "int", nullable: false),
                    AvailableStock = table.Column<int>(type: "int", nullable: false),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false),
                    MaxStockLevel = table.Column<int>(type: "int", nullable: true),
                    ReorderPoint = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(8,3)", nullable: true),
                    Dimensions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerNotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EmailEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PushEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MarketingSms = table.Column<bool>(type: "bit", nullable: false),
                    MarketingEmail = table.Column<bool>(type: "bit", nullable: false),
                    MarketingPush = table.Column<bool>(type: "bit", nullable: false),
                    LoyaltyNotifications = table.Column<bool>(type: "bit", nullable: false),
                    PaymentReminders = table.Column<bool>(type: "bit", nullable: false),
                    OrderUpdates = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerNotificationPreferences_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalBalance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsContra = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLAccounts_GLAccountCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "GLAccountCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTriggers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Conditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTriggers_NotificationTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierPartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardCost = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    MinimumOrderQuantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierMaterials_RawMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "RawMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierMaterials_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BottleReturns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BottleSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountApplied = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottleReturns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BottleReturns_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BottleReturns_Users_ProcessedBy",
                        column: x => x.ProcessedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MinPurchaseAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsagePerCustomer = table.Column<int>(type: "int", nullable: true),
                    ApplicableTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerTiers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BottleReturnCount = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AutoApply = table.Column<bool>(type: "bit", nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowPartialPayment = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discounts_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalDebits = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TotalCredits = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLJournalEntries_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampaignType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetAudience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Filters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalRecipients = table.Column<int>(type: "int", nullable: false),
                    SentCount = table.Column<int>(type: "int", nullable: false),
                    OpenedCount = table.Column<int>(type: "int", nullable: false),
                    ClickedCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationCampaigns_NotificationTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationCampaigns_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyRate = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    TransportSupplierCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    BankCharges = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TransportKigaliCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    LaisseSuivreCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ImportTaxes = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    StorageCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DeclarantFees = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TransportWarehouseCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalAmountRFW = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequisitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalEstimatedCost = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitions_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitions_Users_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shops_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockLedgers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLedgers_RawMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "RawMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockLedgers_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierPerformances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeliveryRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    QualityRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    PriceRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    ServiceRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    OverallRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPerformances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPerformances_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierPerformances_Users_EvaluatedBy",
                        column: x => x.EvaluatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecipeMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityPerUnit = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeMaterials_BottlingRecipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "BottlingRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeMaterials_RawMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "RawMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BottlingBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BulkPerfumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuantityPlanned = table.Column<int>(type: "int", nullable: false),
                    QuantityProduced = table.Column<int>(type: "int", nullable: false),
                    QuantityDefective = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ProfitMargin = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    OperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualityScore = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    EfficiencyRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottlingBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BottlingBatches_BottlingRecipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "BottlingRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BottlingBatches_PerfumeBulkItems_BulkPerfumeId",
                        column: x => x.BulkPerfumeId,
                        principalTable: "PerfumeBulkItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BottlingBatches_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BottlingBatches_Users_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BottlingBatches_Users_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PerfumeBottlings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BulkPerfumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BottleSizeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityBottled = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    SellingPricePerMl = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfumeBottlings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfumeBottlings_BottleSizes_BottleSizeId",
                        column: x => x.BottleSizeId,
                        principalTable: "BottleSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfumeBottlings_PerfumeBulkItems_BulkPerfumeId",
                        column: x => x.BulkPerfumeId,
                        principalTable: "PerfumeBulkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfumeBottlings_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceChangeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    NewPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricingStrategy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceChangeLogs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceChangeLogs_Users_CalculatedBy",
                        column: x => x.CalculatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransfers_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTransfers_Users_CompletedBy",
                        column: x => x.CompletedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTransfers_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false),
                    MaxStockLevel = table.Column<int>(type: "int", nullable: true),
                    ReorderPoint = table.Column<int>(type: "int", nullable: false),
                    SafetyStock = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseInventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseInventories_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GLAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialTransactions_GLAccounts_GLAccountId",
                        column: x => x.GLAccountId,
                        principalTable: "GLAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FinancialTransactions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerDiscountUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDiscountUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerDiscountUsages_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerDiscountUsages_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntryLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GLAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntryLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLJournalEntryLines_GLAccounts_GLAccountId",
                        column: x => x.GLAccountId,
                        principalTable: "GLAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLJournalEntryLines_GLJournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "GLJournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationAnalytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalSent = table.Column<int>(type: "int", nullable: false),
                    TotalDelivered = table.Column<int>(type: "int", nullable: false),
                    TotalOpened = table.Column<int>(type: "int", nullable: false),
                    TotalClicked = table.Column<int>(type: "int", nullable: false),
                    TotalFailed = table.Column<int>(type: "int", nullable: false),
                    DeliveryRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OpenRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ClickRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationAnalytics_NotificationCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "NotificationCampaigns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "NotificationCampaigns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GRNNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReceivedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_Users_ReceivedBy",
                        column: x => x.ReceivedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantityOrdered = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_RawMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "RawMaterials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequisitionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityRequired = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequisitionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitionItems_PurchaseRequisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "PurchaseRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequisitionItems_RawMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "RawMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpenseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PreviousStock = table.Column<int>(type: "int", nullable: true),
                    NewStock = table.Column<int>(type: "int", nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LoyaltyDiscount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShopInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false),
                    MaxStockLevel = table.Column<int>(type: "int", nullable: true),
                    ReorderPoint = table.Column<int>(type: "int", nullable: false),
                    SafetyStock = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopInventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopInventories_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComponentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostComponents_BottlingBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "BottlingBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoodsReceiptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    QualityStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_GoodsReceipts_GoodsReceiptId",
                        column: x => x.GoodsReceiptId,
                        principalTable: "GoodsReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_RawMaterials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "RawMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AmountApplied = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PercentageApplied = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OriginalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountApplications_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscountApplications_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GiftCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchasedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDigital = table.Column<bool>(type: "bit", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftCards_Orders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GiftCards_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoyaltyTransactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoyaltyTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderStatusLogs_Users_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GiftCardTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BalanceBefore = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftCardTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftCardTransactions_GiftCards_GiftCardId",
                        column: x => x.GiftCardId,
                        principalTable: "GiftCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiftCardTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GiftCardTransactions_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GiftCardTransactions_Users_ProcessedBy",
                        column: x => x.ProcessedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BottleReturns_CustomerId",
                table: "BottleReturns",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BottleReturns_ProcessedBy",
                table: "BottleReturns",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BottlingBatches_BulkPerfumeId",
                table: "BottlingBatches",
                column: "BulkPerfumeId");

            migrationBuilder.CreateIndex(
                name: "IX_BottlingBatches_CreatedBy",
                table: "BottlingBatches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BottlingBatches_OperatorId",
                table: "BottlingBatches",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BottlingBatches_RecipeId",
                table: "BottlingBatches",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_BottlingBatches_SupervisorId",
                table: "BottlingBatches",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_BottlingRecipes_BottleSizeId",
                table: "BottlingRecipes",
                column: "BottleSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CostComponents_BatchId",
                table: "CostComponents",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_FromCurrency_ToCurrency_RateDate",
                table: "CurrencyRates",
                columns: new[] { "FromCurrency", "ToCurrency", "RateDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDiscountUsages_CustomerId_DiscountId",
                table: "CustomerDiscountUsages",
                columns: new[] { "CustomerId", "DiscountId" },
                unique: true,
                filter: "[CustomerId] IS NOT NULL AND [DiscountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDiscountUsages_DiscountId",
                table: "CustomerDiscountUsages",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNotificationPreferences_CustomerId",
                table: "CustomerNotificationPreferences",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountApplications_DiscountId",
                table: "DiscountApplications",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountApplications_OrderId",
                table: "DiscountApplications",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CreatedBy",
                table: "Discounts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_BaseCurrency_TargetCurrency",
                table: "ExchangeRates",
                columns: new[] { "BaseCurrency", "TargetCurrency" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CreatedBy",
                table: "Expenses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ShopId",
                table: "Expenses",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_CreatedBy",
                table: "FinancialTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTransactions_GLAccountId",
                table: "FinancialTransactions",
                column: "GLAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCards_CreatedBy",
                table: "GiftCards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCards_PurchaseOrderId",
                table: "GiftCards",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCardTransactions_GiftCardId",
                table: "GiftCardTransactions",
                column: "GiftCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCardTransactions_OrderId",
                table: "GiftCardTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCardTransactions_ProcessedBy",
                table: "GiftCardTransactions",
                column: "ProcessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCardTransactions_ShopId",
                table: "GiftCardTransactions",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccountCategories_ParentId",
                table: "GLAccountCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GLAccounts_CategoryId",
                table: "GLAccounts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntries_CreatedBy",
                table: "GLJournalEntries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryLines_GLAccountId",
                table: "GLJournalEntryLines",
                column: "GLAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryLines_JournalEntryId",
                table: "GLJournalEntryLines",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_GoodsReceiptId",
                table: "GoodsReceiptItems",
                column: "GoodsReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_MaterialId",
                table: "GoodsReceiptItems",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_PurchaseOrderId",
                table: "GoodsReceipts",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_ReceivedBy",
                table: "GoodsReceipts",
                column: "ReceivedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_SupplierId",
                table: "GoodsReceipts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_CreatedBy",
                table: "InventoryTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductId",
                table: "InventoryTransactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ShopId",
                table: "InventoryTransactions",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_WarehouseId",
                table: "InventoryTransactions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_CustomerId",
                table: "LoyaltyTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_OrderId",
                table: "LoyaltyTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAnalytics_CampaignId",
                table: "NotificationAnalytics",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationCampaigns_CreatedBy",
                table: "NotificationCampaigns",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationCampaigns_TemplateId",
                table: "NotificationCampaigns",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CampaignId",
                table: "Notifications",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomerId",
                table: "Notifications",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTriggers_TemplateId",
                table: "NotificationTriggers",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedBy",
                table: "Orders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShopId",
                table: "Orders",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusLogs_ChangedBy",
                table: "OrderStatusLogs",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusLogs_OrderId",
                table: "OrderStatusLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfumeBottlings_BottleSizeId",
                table: "PerfumeBottlings",
                column: "BottleSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfumeBottlings_BulkPerfumeId",
                table: "PerfumeBottlings",
                column: "BulkPerfumeId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfumeBottlings_CreatedBy",
                table: "PerfumeBottlings",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PerfumeBulkItems_CategoryId",
                table: "PerfumeBulkItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChangeLogs_CalculatedBy",
                table: "PriceChangeLogs",
                column: "CalculatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChangeLogs_ProductId",
                table: "PriceChangeLogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_MaterialId",
                table: "PurchaseOrderItems",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CreatedBy",
                table: "PurchaseOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitionItems_MaterialId",
                table: "PurchaseRequisitionItems",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitionItems_RequisitionId",
                table: "PurchaseRequisitionItems",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitions_ApprovedBy",
                table: "PurchaseRequisitions",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequisitions_RequestedBy",
                table: "PurchaseRequisitions",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeMaterials_MaterialId",
                table: "RecipeMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeMaterials_RecipeId",
                table: "RecipeMaterials",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopInventories_ProductId",
                table: "ShopInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopInventories_ShopId_ProductId",
                table: "ShopInventories",
                columns: new[] { "ShopId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shops_ManagerId",
                table: "Shops",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_CreatedBy",
                table: "StockLedgers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_MaterialId",
                table: "StockLedgers",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_CompletedBy",
                table: "StockTransfers",
                column: "CompletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_CreatedBy",
                table: "StockTransfers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ProductId",
                table: "StockTransfers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterials_MaterialId",
                table: "SupplierMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierMaterials_SupplierId",
                table: "SupplierMaterials",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPerformances_EvaluatedBy",
                table: "SupplierPerformances",
                column: "EvaluatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPerformances_SupplierId",
                table: "SupplierPerformances",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventories_ProductId",
                table: "WarehouseInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseInventories_WarehouseId_ProductId",
                table: "WarehouseInventories",
                columns: new[] { "WarehouseId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BottleReturns");

            migrationBuilder.DropTable(
                name: "CostComponents");

            migrationBuilder.DropTable(
                name: "CurrencyRates");

            migrationBuilder.DropTable(
                name: "CustomerDiscountUsages");

            migrationBuilder.DropTable(
                name: "CustomerNotificationPreferences");

            migrationBuilder.DropTable(
                name: "DiscountApplications");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "FinancialTransactions");

            migrationBuilder.DropTable(
                name: "GiftCardTemplates");

            migrationBuilder.DropTable(
                name: "GiftCardTransactions");

            migrationBuilder.DropTable(
                name: "GLJournalEntryLines");

            migrationBuilder.DropTable(
                name: "GoodsReceiptItems");

            migrationBuilder.DropTable(
                name: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "LoyaltyTransactions");

            migrationBuilder.DropTable(
                name: "NotificationAnalytics");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTriggers");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderStatusLogs");

            migrationBuilder.DropTable(
                name: "PerfumeBottlings");

            migrationBuilder.DropTable(
                name: "PriceChangeLogs");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "PurchaseRequisitionItems");

            migrationBuilder.DropTable(
                name: "RecipeMaterials");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "ShopInventories");

            migrationBuilder.DropTable(
                name: "StockLedgers");

            migrationBuilder.DropTable(
                name: "StockTransfers");

            migrationBuilder.DropTable(
                name: "SupplierMaterials");

            migrationBuilder.DropTable(
                name: "SupplierPerformances");

            migrationBuilder.DropTable(
                name: "WarehouseInventories");

            migrationBuilder.DropTable(
                name: "BottlingBatches");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "GiftCards");

            migrationBuilder.DropTable(
                name: "GLAccounts");

            migrationBuilder.DropTable(
                name: "GLJournalEntries");

            migrationBuilder.DropTable(
                name: "GoodsReceipts");

            migrationBuilder.DropTable(
                name: "NotificationCampaigns");

            migrationBuilder.DropTable(
                name: "PurchaseRequisitions");

            migrationBuilder.DropTable(
                name: "RawMaterials");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "BottlingRecipes");

            migrationBuilder.DropTable(
                name: "PerfumeBulkItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "GLAccountCategories");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "BottleSizes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
