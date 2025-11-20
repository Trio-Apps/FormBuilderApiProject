using Microsoft.AspNetCore.Identity; // ✅ هذا هو الصحيح
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("appUsers")]
    public class AppUser : IdentityUser // ✅ من Microsoft.AspNetCore.Identity
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
    }
}