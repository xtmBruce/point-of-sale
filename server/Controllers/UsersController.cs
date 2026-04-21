using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin",
            "manager",
            "cashier"
        };

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] Guid? shopId = null)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 20 : Math.Min(limit, 100);

            var query = _db.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
                    (u.Phone != null && u.Phone.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Role.ToLower() == role.Trim().ToLower());

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (shopId.HasValue)
                query = query.Where(u => u.ShopId == shopId.Value);

            query = query.OrderBy(u => u.Username);

            var total = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    first_name = u.FirstName,
                    last_name = u.LastName,
                    phone = u.Phone,
                    shop_id = u.ShopId,
                    is_active = u.IsActive,
                    created_at = u.CreatedAt,
                    updated_at = u.UpdatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                users,
                pagination = new
                {
                    total,
                    page,
                    limit,
                    total_pages = (int)Math.Ceiling(total / (double)limit)
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            return Ok(MapUser(user));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            // Validate request
            if (!req.IsValid(out var errorMessage))
                return BadRequest(new { error = errorMessage });

            if (!AllowedRoles.Contains(req.Role))
                return BadRequest(new { error = "Invalid role" });

            if (string.Equals(req.Role, "cashier", StringComparison.OrdinalIgnoreCase) && !req.ShopId.HasValue)
                return BadRequest(new { error = "Shop is required for cashier users" });

            var normalizedUsername = req.Username.Trim();
            var normalizedEmail = req.Email.Trim();

            if (await _db.Users.AnyAsync(u => u.Username == normalizedUsername || u.Email == normalizedEmail))
                return BadRequest(new { error = "Username or email already exists" });

            var user = new User
            {
                Username = normalizedUsername,
                Email = normalizedEmail,
                Password = HashPassword(req.Password),
                FirstName = req.FirstName?.Trim(),
                LastName = req.LastName?.Trim(),
                Phone = req.Phone?.Trim(),
                Role = req.Role.ToLower(),
                ShopId = req.ShopId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, MapUser(user));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(req.Username))
            {
                var normalizedUsername = req.Username.Trim();
                var usernameExists = await _db.Users.AnyAsync(u => u.Id != id && u.Username == normalizedUsername);
                if (usernameExists)
                    return BadRequest(new { error = "Username already exists" });

                user.Username = normalizedUsername;
            }

            if (!string.IsNullOrWhiteSpace(req.Email))
            {
                var normalizedEmail = req.Email.Trim();
                var emailExists = await _db.Users.AnyAsync(u => u.Id != id && u.Email == normalizedEmail);
                if (emailExists)
                    return BadRequest(new { error = "Email already exists" });

                user.Email = normalizedEmail;
            }

            if (req.FirstName != null) user.FirstName = req.FirstName;
            if (req.LastName != null) user.LastName = req.LastName;
            if (req.Phone != null) user.Phone = req.Phone;

            if (!string.IsNullOrWhiteSpace(req.Role))
            {
                if (!AllowedRoles.Contains(req.Role))
                    return BadRequest(new { error = "Invalid role" });

                user.Role = req.Role;
            }

            var effectiveRole = !string.IsNullOrWhiteSpace(req.Role) ? req.Role : user.Role;
            if (string.Equals(effectiveRole, "cashier", StringComparison.OrdinalIgnoreCase) && !req.ShopId.HasValue && !user.ShopId.HasValue)
                return BadRequest(new { error = "Shop is required for cashier users" });

            if (req.Password != null)
            {
                if (req.Password.Length < 8)
                    return BadRequest(new { error = "Password must be at least 8 characters" });

                user.Password = HashPassword(req.Password);
            }

            if (req.IsActive.HasValue) user.IsActive = req.IsActive.Value;
            if (req.ShopId.HasValue) user.ShopId = req.ShopId;

            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            if (req.IsActive.HasValue && !req.IsActive.Value)
            {
                var refreshTokens = await _db.RefreshTokens.Where(rt => rt.UserId == id && rt.Revoked == null).ToListAsync();
                foreach (var refreshToken in refreshTokens)
                {
                    refreshToken.Revoked = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();

            return Ok(MapUser(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            var refreshTokens = await _db.RefreshTokens.Where(rt => rt.UserId == id && rt.Revoked == null).ToListAsync();
            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.Revoked = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = new[] { "admin", "manager", "cashier" };
            return Ok(roles);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleUpdateRequest req)
        {
            if (!AllowedRoles.Contains(req.Role))
                return BadRequest(new { error = "Invalid role" });

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            if (string.Equals(req.Role, "cashier", StringComparison.OrdinalIgnoreCase) && !user.ShopId.HasValue)
                return BadRequest(new { error = "Shop is required for cashier users" });

            user.Role = req.Role;
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }

        private static object MapUser(User user)
        {
            return new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                first_name = user.FirstName,
                last_name = user.LastName,
                phone = user.Phone,
                shop_id = user.ShopId,
                is_active = user.IsActive,
                created_at = user.CreatedAt,
                updated_at = user.UpdatedAt
            };
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
