namespace SmartPOS.API.DTOs
{
    public class AssignProductToShopRequest
    {
        public Guid ProductId { get; set; }
        public Guid? ShopId { get; set; }
        public int Quantity { get; set; } = 0;
        public int? MinStockLevel { get; set; }
        public int? MaxStockLevel { get; set; }
        public int? ReorderPoint { get; set; }
        public int? SafetyStock { get; set; }

        public Guid EffectiveShopId => ShopId ?? Guid.Empty;

        public bool IsValid(out string error)
        {
            if (ProductId == Guid.Empty) { error = "ProductId is required"; return false; }
            if (!ShopId.HasValue || ShopId == Guid.Empty) { error = "ShopId is required"; return false; }
            if (Quantity < 0) { error = "Quantity cannot be negative"; return false; }
            error = string.Empty;
            return true;
        }
    }

    public class AssignProductDto
    {
        public Guid ProductId { get; set; }
        public Guid ShopId { get; set; }
        public int Quantity { get; set; } = 0;
        public int MinStockLevel { get; set; } = 0;
        public int? MaxStockLevel { get; set; }
        public int ReorderPoint { get; set; } = 0;
    }

    public class UpdateAssignmentDto
    {
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; } = 0;
        public int? MaxStockLevel { get; set; }
        public int ReorderPoint { get; set; } = 0;
    }
}
