using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
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
                FirstName = req.FirstName,
                LastName = req.LastName,
                Phone = req.Phone,
                ShopId = req.ShopId,
                Role = "cashier",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // create tokens
            var accessToken = GenerateAccessToken(user);
            var refreshTokenEntity = CreateRefreshToken(Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            refreshTokenEntity.UserId = user.Id;
            _db.RefreshTokens.Add(refreshTokenEntity);
            await _db.SaveChangesAsync();

            SetRefreshTokenCookie(refreshTokenEntity.Token, refreshTokenEntity.Expires);

            return Ok(new AuthResponse { Token = accessToken, User = SanitizeUser(user) });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Username and password are required" });

            var user = await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Username == req.Username || u.Email == req.Username);
            if (user == null)
                return BadRequest(new { error = "Invalid credentials" });

            if (!user.IsActive)
                return BadRequest(new { error = "User account is inactive" });

            if (user.Password != HashPassword(req.Password))
                return BadRequest(new { error = "Invalid credentials" });

            // create tokens
            var accessToken = GenerateAccessToken(user);
            var refreshTokenEntity = CreateRefreshToken(Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            refreshTokenEntity.UserId = user.Id;
            _db.RefreshTokens.Add(refreshTokenEntity);
            await _db.SaveChangesAsync();

            SetRefreshTokenCookie(refreshTokenEntity.Token, refreshTokenEntity.Expires);

            return Ok(new AuthResponse { Token = accessToken, User = SanitizeUser(user) });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var token = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(token)) return Unauthorized(new { error = "No refresh token" });

            var rt = await _db.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == token);
            if (rt == null || !rt.IsActive) return Unauthorized(new { error = "Invalid or expired refresh token" });
            if (rt.User == null || !rt.User.IsActive) return Unauthorized(new { error = "User account is inactive" });

            // rotate
            var newRt = CreateRefreshToken(Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            newRt.UserId = rt.UserId;
            rt.Revoked = DateTime.UtcNow;
            rt.RevokedByIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            rt.ReplacedByToken = newRt.Token;

            _db.RefreshTokens.Add(newRt);
            _db.RefreshTokens.Update(rt);
            await _db.SaveChangesAsync();

            SetRefreshTokenCookie(newRt.Token, newRt.Expires);

            var accessToken = GenerateAccessToken(rt.User!);
            return Ok(new AuthResponse { Token = accessToken, User = SanitizeUser(rt.User!) });
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(token))
            {
                // clear cookie anyway
                Response.Cookies.Append("refreshToken", "", new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.UtcNow.AddDays(-1), HttpOnly = true, Secure = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None });
                return Ok(new { success = true });
            }

            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            if (rt != null && rt.IsActive)
            {
                rt.Revoked = DateTime.UtcNow;
                rt.RevokedByIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                _db.RefreshTokens.Update(rt);
                await _db.SaveChangesAsync();
            }

            Response.Cookies.Append("refreshToken", "", new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.UtcNow.AddDays(-1), HttpOnly = true, Secure = true, SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None });

            return Ok(new { success = true });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await GetUserFromClaimsAsync();
            if (user == null) return Unauthorized();
            return Ok(SanitizeUser(user));
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
        {
            var user = await GetUserFromClaimsAsync();
            if (user == null) return Unauthorized();

            if (!string.IsNullOrWhiteSpace(req.Email)) user.Email = req.Email;
            if (req.FirstName != null) user.FirstName = req.FirstName;
            if (req.LastName != null) user.LastName = req.LastName;
            if (req.Phone != null) user.Phone = req.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Ok(SanitizeUser(user));
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            var user = await GetUserFromClaimsAsync();
            if (user == null) return Unauthorized();

            if (user.Password != HashPassword(req.OldPassword))
                return BadRequest(new { error = "Old password is incorrect" });

            user.Password = HashPassword(req.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // --- Helpers ---
        private async Task<User?> GetUserFromClaimsAsync()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(sub)) return null;
            if (!Guid.TryParse(sub, out var userId)) return null;
            var user = await _db.Users.FindAsync(userId);
            return user != null && user.IsActive ? user : null;
        }

        private object SanitizeUser(User u)
        {
            return new
            {
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
            };
        }

        private string GenerateAccessToken(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");
            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            var minutes = jwtSection.GetValue<int>("AccessTokenExpirationMinutes");

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken CreateRefreshToken(string? ipAddress)
        {
            var jwtSection = _config.GetSection("Jwt");
            var days = jwtSection.GetValue<int>("RefreshTokenExpirationDays");

            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            return new RefreshToken
            {
                Token = token,
                Expires = DateTime.UtcNow.AddDays(days),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            // In development over HTTP we cannot set Secure + SameSite=None (browsers will reject).
            // Use Secure=True and SameSite=None when request is HTTPS (production). For HTTP (dev) use Lax and Secure=false.
            var isHttps = Request?.IsHttps ?? false;
            var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Expires = expires,
                Secure = isHttps,
                SameSite = isHttps ? Microsoft.AspNetCore.Http.SameSiteMode.None : Microsoft.AspNetCore.Http.SameSiteMode.Lax
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
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
