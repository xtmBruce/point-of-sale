using Microsoft.AspNetCore.Mvc;
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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _db.Users
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,
                    u.FirstName,
                    u.LastName,
                    u.Phone,
                    u.ShopId,
                    u.IsActive,
                    u.CreatedAt,
                    u.UpdatedAt
                }).ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(new {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.ShopId,
                user.IsActive,
                user.CreatedAt,
                user.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Username and password are required" });

            if (await _db.Users.AnyAsync(u => u.Username == req.Username || u.Email == req.Email))
                return BadRequest(new { error = "Username or email already exists" });

            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                Password = HashPassword(req.Password),
                Role = req.Role ?? "cashier",
                ShopId = req.ShopId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.ShopId,
                user.IsActive,
                user.CreatedAt,
                user.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest req)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(req.Username)) user.Username = req.Username;
            if (!string.IsNullOrWhiteSpace(req.Email)) user.Email = req.Email;
            if (!string.IsNullOrWhiteSpace(req.Role)) user.Role = req.Role;
            if (req.IsActive.HasValue) user.IsActive = req.IsActive.Value;
            if (req.ShopId.HasValue) user.ShopId = req.ShopId;

            user.UpdatedAt = DateTime.UtcNow;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Ok(new {
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.ShopId,
                user.IsActive,
                user.CreatedAt,
                user.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
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
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = req.Role;
            user.UpdatedAt = DateTime.UtcNow;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Ok(new { success = true });
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
