// Models/User.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("USERS")]
    public class User : IdentityUser<int>
    {
    


        [Required]
        [MaxLength(50)]
        [Column("Username")]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("Password")]
        public string Password { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        [Column("Email")]
        public string Email { get; set; }

        
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        // Navigation Properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}