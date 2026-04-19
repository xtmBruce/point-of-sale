using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;

namespace SmartPOS.API.Controllers
{
    public class CategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ParentId { get; set; }  // accept as string to handle empty string
        public string Type { get; set; } = "general";
        public bool IsActive { get; set; } = true;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _db.Categories
                .Where(c => c.IsActive && c.DeletedAt == null)
                .ToListAsync();

            var lookup = all.ToDictionary(c => c.Id);
            var roots = new List<Category>();
            foreach (var cat in all)
            {
                if (cat.ParentId.HasValue && lookup.TryGetValue(cat.ParentId.Value, out var parent))
                    parent.Children.Add(cat);
                else
                    roots.Add(cat);
            }

            return Ok(new { categories = roots, flat = all });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _db.Categories.Include(c => c.Children).FirstOrDefaultAsync(c => c.Id == id);
            return category == null ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { error = "Name is required" });

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Parse parent_id safely (ignore empty string)
            if (!string.IsNullOrWhiteSpace(dto.ParentId) && Guid.TryParse(dto.ParentId, out var parentGuid))
            {
                var parent = await _db.Categories.FindAsync(parentGuid);
                if (parent == null) return BadRequest(new { error = "Parent category not found" });
                category.ParentId = parentGuid;
                category.Level = parent.Level + 1;
                category.Path = $"{parent.Path}/{category.Id}";
            }
            else
            {
                category.Level = 0;
                category.Path = category.Id.ToString();
            }

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CategoryDto dto)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.Type = dto.Type;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();
            category.IsActive = false;
            category.DeletedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
