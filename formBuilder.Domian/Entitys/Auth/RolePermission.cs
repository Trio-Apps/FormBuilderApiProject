// Models/RolePermission.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("ROLE_PERMISSIONS")]
    public class RolePermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("RolePermissionID")]
        public int RolePermissionID { get; set; }

        [Required]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Required]
        [Column("PermissionID")]
        public int PermissionID { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("AssignedDate")]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }

        [ForeignKey("PermissionID")]
        public virtual Permission Permission { get; set; }
    }
}