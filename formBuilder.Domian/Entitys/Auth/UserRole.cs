// Models/UserRole.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("USER_ROLES")]
    public class UserRole
    {
        [Key]
        [Column("UserRoleID")]
        public int UserRoleID { get; set; }

        [Required]
        [Column("UserID")]
        public string UserID { get; set; } // string to match IdentityUser Id

        [Required]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}