// DTOs/AuthDtos.cs
using System.ComponentModel.DataAnnotations;

namespace FormBuilder.API.DTOs
{
    // DTOs/UserRegisterDto.cs
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    namespace FormBuilder.API.DTOs
    {
        public class UserRegisterDto
        {
            [Required(ErrorMessage = "Username is required")]
            [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
            [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and digits")]
            [JsonPropertyName("username")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
            [JsonPropertyName("password")]
            public string Password { get; set; } = string.Empty;
        }
    }

    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public UserInfoDto User { get; set; }
    }
}