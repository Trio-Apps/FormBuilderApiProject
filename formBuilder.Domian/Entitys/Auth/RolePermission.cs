using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FormBuilder.API.Models
{
    [Table("ROLE_PERMISSIONS")]
    public class RolePermission
    {
        [Key]
        [Column("RolePermissionID")]
        public int RolePermissionID { get; set; }

        [Required]
        [Column("RoleID")]
        public string RoleID { get; set; }

        [Required]
        [Column("PermissionID")]
        public int PermissionID { get; set; }

        [Column("AssignedDate")]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        // ✅ Navigation properties - يجب أن تكون هذه الأسماء مطابقة للاستخدام في DbContext
        [ForeignKey("RoleID")]
        public virtual IdentityRole Role { get; set; } // ✅ اسم: Role

        [ForeignKey("PermissionID")]
        public virtual Permission Permission { get; set; } // ✅ اسم: Permission
    }
}