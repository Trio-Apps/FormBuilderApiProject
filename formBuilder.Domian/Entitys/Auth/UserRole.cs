// Models/UserRole.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    [Table("USER_ROLES")]
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserRoleID")]
        public int UserRoleID { get; set; }

        [Required]
        [Column("UserID")]
        public int UserID { get; set; }

        [Required]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}