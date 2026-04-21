using System;
using System.Text.Json.Serialization;

namespace SmartPOS.API.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public Guid? ShopId { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public object? User { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class CreateUserRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } = "cashier";

        [JsonPropertyName("shop_id")]
        public Guid? ShopId { get; set; }

        // Validation helper
        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                errorMessage = "Username is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                errorMessage = "Email is required";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                errorMessage = "Password is required";
                return false;
            }

            if (Password.Length < 8)
            {
                errorMessage = "Password must be at least 8 characters";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }

    public class UpdateUserRequest
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("shop_id")]
        public Guid? ShopId { get; set; }
    }

    public class RoleUpdateRequest
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
    }

    // Shop DTOs
    public class CreateShopRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

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

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("manager_id")]
        public Guid? ManagerId { get; set; }

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Shop name is required";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }

    public class UpdateShopRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

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

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("manager_id")]
        public Guid? ManagerId { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }
}
