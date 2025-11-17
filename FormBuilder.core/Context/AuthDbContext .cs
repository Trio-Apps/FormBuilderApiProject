// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using FormBuilder.API.Models;

namespace FormBuilder.API.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> USERS { get; set; }
        public DbSet<Role> ROLES { get; set; }
        public DbSet<Permission> PERMISSIONS { get; set; }
        public DbSet<UserRole> USER_ROLES { get; set; }
        public DbSet<RolePermission> ROLE_PERMISSIONS { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                // الطريقة الجديدة في EF Core 9
                entity.Property(e => e.CreatedDate)
                    .ValueGeneratedOnAdd(); // تم التعديل هنا
            });

            // Configure Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleID);
                entity.HasIndex(e => e.RoleName).IsUnique();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);
            });

            // Configure Permission
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PermissionID);
                entity.HasIndex(e => e.PermissionName).IsUnique();

                entity.Property(e => e.PermissionName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.Property(e => e.Category)
                    .HasMaxLength(50);

                // الطريقة الجديدة في EF Core 9
                entity.Property(e => e.CreatedDate)
                    .ValueGeneratedOnAdd(); // تم التعديل هنا
            });

            // Configure UserRole
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleID);

                // Composite unique constraint
                entity.HasIndex(e => new { e.UserID, e.RoleID }).IsUnique();

                // Relationships
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleID)
                      .OnDelete(DeleteBehavior.Cascade);

                // الطريقة الجديدة في EF Core 9
                entity.Property(e => e.StartDate)
                    .ValueGeneratedOnAdd(); // تم التعديل هنا
            });

            // Configure RolePermission
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionID);

                // Composite unique constraint
                entity.HasIndex(e => new { e.RoleID, e.PermissionID }).IsUnique();

                // Relationships
                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionID)
                      .OnDelete(DeleteBehavior.Cascade);

                // الطريقة الجديدة في EF Core 9
                entity.Property(e => e.AssignedDate)
                    .ValueGeneratedOnAdd(); // تم التعديل هنا
            });
        }
    }
}