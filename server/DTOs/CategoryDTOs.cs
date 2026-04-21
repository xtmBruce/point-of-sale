using System.Text.Json.Serialization;

namespace SmartPOS.API.DTOs
{
    public class CreateCategoryRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("parent_id")]
        public string? ParentId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = "general";

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Category name is required";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }

    public class UpdateCategoryRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }

    public class CategoryResponseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("parent_id")]
        public Guid? ParentId { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = "general";

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("children")]
        public List<CategoryResponseDto> Children { get; set; } = new();
    }
}
