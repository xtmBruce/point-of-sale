using System.Text.Json.Serialization;

namespace SmartPOS.API.DTOs
{
    public class AssignProductToShopRequest
    {
        [JsonPropertyName("shop_id")]
        public Guid ShopId { get; set; }

        [JsonPropertyName("location_type")]
        public string? LocationType { get; set; }

        [JsonPropertyName("location_id")]
        public Guid? LocationId { get; set; }

        [JsonPropertyName("product_id")]
        public Guid ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("min_stock_level")]
        public int? MinStockLevel { get; set; }

        [JsonPropertyName("max_stock_level")]
        public int? MaxStockLevel { get; set; }

        [JsonPropertyName("reorder_point")]
        public int? ReorderPoint { get; set; }

        [JsonPropertyName("safety_stock")]
        public int? SafetyStock { get; set; }

        public Guid EffectiveShopId => ShopId != Guid.Empty
            ? ShopId
            : (LocationId ?? Guid.Empty);

        public bool IsValid(out string errorMessage)
        {
            if (EffectiveShopId == Guid.Empty)
            {
                errorMessage = "shop_id or location_id is required";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(LocationType) &&
                !string.Equals(LocationType, "shop", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Only location_type=shop is supported";
                return false;
            }

            if (ProductId == Guid.Empty)
            {
                errorMessage = "product_id is required";
                return false;
            }

            if (Quantity <= 0)
            {
                errorMessage = "quantity must be greater than 0";
                return false;
            }

            if (MinStockLevel.HasValue && MinStockLevel.Value < 0)
            {
                errorMessage = "min_stock_level must be greater than or equal to 0";
                return false;
            }

            if (MaxStockLevel.HasValue && MaxStockLevel.Value < 0)
            {
                errorMessage = "max_stock_level must be greater than or equal to 0";
                return false;
            }

            if (ReorderPoint.HasValue && ReorderPoint.Value < 0)
            {
                errorMessage = "reorder_point must be greater than or equal to 0";
                return false;
            }

            if (SafetyStock.HasValue && SafetyStock.Value < 0)
            {
                errorMessage = "safety_stock must be greater than or equal to 0";
                return false;
            }

            if (MaxStockLevel.HasValue && MinStockLevel.HasValue && MaxStockLevel.Value < MinStockLevel.Value)
            {
                errorMessage = "max_stock_level cannot be less than min_stock_level";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}