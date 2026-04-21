using System.Text.Json.Serialization;

namespace SmartPOS.API.DTOs
{
    public class CreateProductRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("sku")]
        public string? Sku { get; set; }

        [JsonPropertyName("barcode")]
        public string? Barcode { get; set; }

        [JsonPropertyName("brand_id")]
        public Guid? BrandId { get; set; }

        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }

        [JsonPropertyName("product_type")]
        public string ProductType { get; set; } = "general";

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("variant")]
        public string? Variant { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("cost_price")]
        public decimal? CostPrice { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "RWF";

        [JsonPropertyName("stock_quantity")]
        public int StockQuantity { get; set; } = 0;

        [JsonPropertyName("min_stock_level")]
        public int MinStockLevel { get; set; } = 0;

        [JsonPropertyName("max_stock_level")]
        public int? MaxStockLevel { get; set; }

        [JsonPropertyName("reorder_point")]
        public int ReorderPoint { get; set; } = 20;

        [JsonPropertyName("unit")]
        public string Unit { get; set; } = "piece";

        [JsonPropertyName("weight")]
        public decimal? Weight { get; set; }

        [JsonPropertyName("dimensions")]
        public string? Dimensions { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Product name is required";
                return false;
            }
            if (Price < 0)
            {
                errorMessage = "Price cannot be negative";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }

    public class UpdateProductRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("sku")]
        public string? Sku { get; set; }

        [JsonPropertyName("barcode")]
        public string? Barcode { get; set; }

        [JsonPropertyName("brand_id")]
        public Guid? BrandId { get; set; }

        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }

        [JsonPropertyName("product_type")]
        public string? ProductType { get; set; }

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("variant")]
        public string? Variant { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("cost_price")]
        public decimal? CostPrice { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("stock_quantity")]
        public int? StockQuantity { get; set; }

        [JsonPropertyName("min_stock_level")]
        public int? MinStockLevel { get; set; }

        [JsonPropertyName("max_stock_level")]
        public int? MaxStockLevel { get; set; }

        [JsonPropertyName("reorder_point")]
        public int? ReorderPoint { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        [JsonPropertyName("weight")]
        public decimal? Weight { get; set; }

        [JsonPropertyName("dimensions")]
        public string? Dimensions { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }

    public class ProductResponseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("sku")]
        public string? Sku { get; set; }

        [JsonPropertyName("barcode")]
        public string? Barcode { get; set; }

        [JsonPropertyName("brand_id")]
        public Guid? BrandId { get; set; }

        [JsonPropertyName("brand_name")]
        public string? BrandName { get; set; }

        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }

        [JsonPropertyName("category_name")]
        public string? CategoryName { get; set; }

        [JsonPropertyName("product_type")]
        public string ProductType { get; set; } = "general";

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("variant")]
        public string? Variant { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("cost_price")]
        public decimal? CostPrice { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "RWF";

        [JsonPropertyName("stock_quantity")]
        public int StockQuantity { get; set; }

        [JsonPropertyName("current_stock")]
        public int CurrentStock { get; set; }

        [JsonPropertyName("reserved_stock")]
        public int ReservedStock { get; set; }

        [JsonPropertyName("available_stock")]
        public int AvailableStock { get; set; }

        [JsonPropertyName("min_stock_level")]
        public int MinStockLevel { get; set; }

        [JsonPropertyName("max_stock_level")]
        public int? MaxStockLevel { get; set; }

        [JsonPropertyName("reorder_point")]
        public int ReorderPoint { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; } = "piece";

        [JsonPropertyName("weight")]
        public decimal? Weight { get; set; }

        [JsonPropertyName("dimensions")]
        public string? Dimensions { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductListResponseDto
    {
        [JsonPropertyName("products")]
        public List<ProductResponseDto> Products { get; set; } = new();

        [JsonPropertyName("pagination")]
        public PaginationDto Pagination { get; set; } = new();
    }

    public class PaginationDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
    }
}
