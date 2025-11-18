// Models/Role.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("ROLES")]
    public class Role
    {
        [Key]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("RoleName")]
        public string RoleName { get; set; }

        [MaxLength(255)]
        [Column("Description")]
        public string Description { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}