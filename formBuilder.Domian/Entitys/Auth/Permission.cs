using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("PERMISSIONS")]
    public class Permission
    {
        [Key]
        [Column("PermissionID")]
        public int PermissionID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("PermissionName")]
        public string PermissionName { get; set; }

        [MaxLength(255)]
        [Column("Description")]
        public string Description { get; set; }

        

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // ✅ Navigation property - يجب أن تطابق الاستخدام في DbContext
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}