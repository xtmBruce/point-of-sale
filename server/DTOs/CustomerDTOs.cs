using System.Text.Json.Serialization;

namespace SmartPOS.API.DTOs
{
    // ─── Customer Request DTOs ───────────────────────────────────────────────────

    public class CreateCustomerRequest
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("birthday")]
        public DateOnly? Birthday { get; set; }

        [JsonPropertyName("anniversaryDate")]
        public DateOnly? AnniversaryDate { get; set; }

        [JsonPropertyName("loyaltyTier")]
        public string? LoyaltyTier { get; set; }

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                errorMessage = "First name is required";
                return false;
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                errorMessage = "Last name is required";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }

    public class UpdateCustomerRequest
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("birthday")]
        public DateOnly? Birthday { get; set; }

        [JsonPropertyName("anniversaryDate")]
        public DateOnly? AnniversaryDate { get; set; }

        [JsonPropertyName("loyaltyTier")]
        public string? LoyaltyTier { get; set; }
    }

    // ─── Customer Response DTOs ──────────────────────────────────────────────────

    public class CustomerResponseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("loyalty_points")]
        public int LoyaltyPoints { get; set; }

        [JsonPropertyName("loyalty_tier")]
        public string LoyaltyTier { get; set; } = "bronze";

        [JsonPropertyName("total_spent")]
        public decimal TotalSpent { get; set; }

        [JsonPropertyName("birthday")]
        public DateOnly? Birthday { get; set; }

        [JsonPropertyName("anniversary_date")]
        public DateOnly? AnniversaryDate { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class CustomerListResponseDto
    {
        [JsonPropertyName("customers")]
        public List<CustomerResponseDto> Customers { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }

    public class CustomerStatsDto
    {
        [JsonPropertyName("total_customers")]
        public int TotalCustomers { get; set; }

        [JsonPropertyName("active_customers")]
        public int ActiveCustomers { get; set; }

        [JsonPropertyName("total_spent")]
        public decimal TotalSpent { get; set; }

        [JsonPropertyName("average_spent")]
        public decimal AverageSpent { get; set; }
    }

    // ─── Loyalty Request DTOs ────────────────────────────────────────────────────

    public class AddPointsRequest
    {
        [JsonPropertyName("customerId")]
        public Guid CustomerId { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonPropertyName("orderAmount")]
        public decimal OrderAmount { get; set; }

        [JsonPropertyName("orderId")]
        public Guid? OrderId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        public bool IsValid(out string errorMessage)
        {
            if (CustomerId == Guid.Empty)
            {
                errorMessage = "customerId is required";
                return false;
            }
            if (Points <= 0 && OrderAmount <= 0)
            {
                errorMessage = "Either points or orderAmount must be greater than 0";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }

    public class RedeemPointsRequest
    {
        [JsonPropertyName("customerId")]
        public Guid CustomerId { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonPropertyName("orderId")]
        public Guid? OrderId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        public bool IsValid(out string errorMessage)
        {
            if (CustomerId == Guid.Empty)
            {
                errorMessage = "customerId is required";
                return false;
            }
            if (Points <= 0)
            {
                errorMessage = "points must be greater than 0";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }

    // ─── Loyalty Response DTOs ───────────────────────────────────────────────────

    public class LoyaltyTransactionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; } = string.Empty;

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("order_id")]
        public Guid? OrderId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerLoyaltyDto
    {
        [JsonPropertyName("customer_id")]
        public Guid CustomerId { get; set; }

        [JsonPropertyName("loyalty_points")]
        public int LoyaltyPoints { get; set; }

        [JsonPropertyName("loyalty_tier")]
        public string LoyaltyTier { get; set; } = string.Empty;

        [JsonPropertyName("total_spent")]
        public decimal TotalSpent { get; set; }

        [JsonPropertyName("next_tier")]
        public string? NextTier { get; set; }

        [JsonPropertyName("points_to_next_tier")]
        public int PointsToNextTier { get; set; }

        [JsonPropertyName("transactions")]
        public List<LoyaltyTransactionDto> Transactions { get; set; } = new();
    }

    public class TierDistributionDto
    {
        [JsonPropertyName("tier")]
        public string Tier { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class LoyaltyStatsDto
    {
        [JsonPropertyName("total_customers")]
        public int TotalCustomers { get; set; }

        [JsonPropertyName("total_points_issued")]
        public int TotalPointsIssued { get; set; }

        [JsonPropertyName("tier_distribution")]
        public List<TierDistributionDto> TierDistribution { get; set; } = new();

        [JsonPropertyName("recent_transactions")]
        public List<LoyaltyTransactionDto> RecentTransactions { get; set; } = new();
    }

    public class AddPointsResponseDto
    {
        [JsonPropertyName("customer_id")]
        public Guid CustomerId { get; set; }

        [JsonPropertyName("points_added")]
        public int PointsAdded { get; set; }

        [JsonPropertyName("total_points")]
        public int TotalPoints { get; set; }

        [JsonPropertyName("loyalty_tier")]
        public string LoyaltyTier { get; set; } = string.Empty;

        [JsonPropertyName("transaction_id")]
        public Guid TransactionId { get; set; }
    }

    public class RedeemPointsResponseDto
    {
        [JsonPropertyName("customer_id")]
        public Guid CustomerId { get; set; }

        [JsonPropertyName("points_redeemed")]
        public int PointsRedeemed { get; set; }

        [JsonPropertyName("total_points")]
        public int TotalPoints { get; set; }

        [JsonPropertyName("loyalty_tier")]
        public string LoyaltyTier { get; set; } = string.Empty;

        [JsonPropertyName("transaction_id")]
        public Guid TransactionId { get; set; }
    }

    public class LoyaltyTierDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("min_points")]
        public int MinPoints { get; set; }

        [JsonPropertyName("benefits")]
        public string Benefits { get; set; } = string.Empty;
    }
}
