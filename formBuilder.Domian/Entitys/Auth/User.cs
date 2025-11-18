// Models/AppUser.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("appUsers")]
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        [Column("DisplayName")]
        public string DisplayName { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        // Navigation Properties for Custom Role System
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}