using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    [Route("api/loyalty")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        private readonly AppDbContext _context;

        private const decimal PointsPerRwf = 0.01m;

        private static readonly Dictionary<string, int> TierThresholds = new()
        {
            { "bronze", 0 },
            { "silver", 500 },
            { "gold", 2000 },
            { "platinum", 5000 }
        };

        public LoyaltyController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/loyalty/stats
        [HttpGet("stats")]
        public async Task<ActionResult<LoyaltyStatsDto>> GetStats()
        {
            var totalCustomers = await _context.Customers.CountAsync();
            var totalPoints = await _context.Customers.SumAsync(c => c.LoyaltyPoints);

            var tierDistribution = await _context.Customers
                .GroupBy(c => c.LoyaltyTier)
                .Select(g => new TierDistributionDto
                {
                    Tier = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var recentTransactions = await _context.LoyaltyTransactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new LoyaltyTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.TransactionType,
                    Points = t.Points,
                    Description = t.Description,
                    OrderId = t.OrderId,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(new LoyaltyStatsDto
            {
                TotalCustomers = totalCustomers,
                TotalPointsIssued = totalPoints,
                TierDistribution = tierDistribution,
                RecentTransactions = recentTransactions
            });
        }

        // GET: api/loyalty/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<CustomerLoyaltyDto>> GetCustomerLoyalty(Guid customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return NotFound(new { error = "Customer not found" });

            var transactions = await _context.LoyaltyTransactions
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(20)
                .Select(t => new LoyaltyTransactionDto
                {
                    Id = t.Id,
                    TransactionType = t.TransactionType,
                    Points = t.Points,
                    Description = t.Description,
                    OrderId = t.OrderId,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            var nextTier = GetNextTier(customer.LoyaltyTier);
            var pointsToNextTier = nextTier != null
                ? Math.Max(0, TierThresholds[nextTier] - customer.LoyaltyPoints)
                : 0;

            return Ok(new CustomerLoyaltyDto
            {
                CustomerId = customer.Id,
                LoyaltyPoints = customer.LoyaltyPoints,
                LoyaltyTier = customer.LoyaltyTier,
                TotalSpent = customer.TotalSpent,
                NextTier = nextTier,
                PointsToNextTier = pointsToNextTier,
                Transactions = transactions
            });
        }

        // POST: api/loyalty/points/add
        [HttpPost("points/add")]
        public async Task<ActionResult<AddPointsResponseDto>> AddPoints(
            [FromBody] AddPointsRequest request)
        {
            if (!request.IsValid(out var validationError))
                return BadRequest(new { error = validationError });

            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null) return NotFound(new { error = "Customer not found" });

            var points = request.Points > 0
                ? request.Points
                : (int)(request.OrderAmount * PointsPerRwf);

            customer.LoyaltyPoints += points;
            if (request.OrderAmount > 0) customer.TotalSpent += request.OrderAmount;
            customer.LoyaltyTier = CalculateTier(customer.LoyaltyPoints);
            customer.UpdatedAt = DateTime.UtcNow;

            var transaction = new LoyaltyTransaction
            {
                CustomerId = customer.Id,
                TransactionType = "earn",
                Points = points,
                Description = request.Description ?? $"Earned {points} points",
                OrderId = request.OrderId,
                CreatedAt = DateTime.UtcNow
            };

            _context.LoyaltyTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new AddPointsResponseDto
            {
                CustomerId = customer.Id,
                PointsAdded = points,
                TotalPoints = customer.LoyaltyPoints,
                LoyaltyTier = customer.LoyaltyTier,
                TransactionId = transaction.Id
            });
        }

        // POST: api/loyalty/points/redeem
        [HttpPost("points/redeem")]
        public async Task<ActionResult<RedeemPointsResponseDto>> RedeemPoints(
            [FromBody] RedeemPointsRequest request)
        {
            if (!request.IsValid(out var validationError))
                return BadRequest(new { error = validationError });

            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null) return NotFound(new { error = "Customer not found" });

            if (customer.LoyaltyPoints < request.Points)
                return BadRequest(new { error = $"Insufficient points. Available: {customer.LoyaltyPoints}" });

            customer.LoyaltyPoints -= request.Points;
            customer.LoyaltyTier = CalculateTier(customer.LoyaltyPoints);
            customer.UpdatedAt = DateTime.UtcNow;

            var transaction = new LoyaltyTransaction
            {
                CustomerId = customer.Id,
                TransactionType = "redeem",
                Points = -request.Points,
                Description = request.Description ?? $"Redeemed {request.Points} points",
                OrderId = request.OrderId,
                CreatedAt = DateTime.UtcNow
            };

            _context.LoyaltyTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new RedeemPointsResponseDto
            {
                CustomerId = customer.Id,
                PointsRedeemed = request.Points,
                TotalPoints = customer.LoyaltyPoints,
                LoyaltyTier = customer.LoyaltyTier,
                TransactionId = transaction.Id
            });
        }

        // GET: api/loyalty/tiers
        [HttpGet("tiers")]
        public IActionResult GetTiers()
        {
            var tiers = TierThresholds.Select(t => new LoyaltyTierDto
            {
                Name = t.Key,
                MinPoints = t.Value,
                Benefits = GetTierBenefits(t.Key)
            }).ToList();

            return Ok(new { tiers });
        }

        private static string CalculateTier(int points) =>
            points >= 5000 ? "platinum" :
            points >= 2000 ? "gold" :
            points >= 500 ? "silver" : "bronze";

        private static string? GetNextTier(string currentTier) =>
            currentTier switch
            {
                "bronze" => "silver",
                "silver" => "gold",
                "gold" => "platinum",
                _ => null
            };

        private static string GetTierBenefits(string tier) =>
            tier switch
            {
                "bronze" => "1% points on purchases",
                "silver" => "1.5% points + birthday bonus",
                "gold" => "2% points + priority support",
                "platinum" => "3% points + exclusive offers",
                _ => ""
            };
    }
}
