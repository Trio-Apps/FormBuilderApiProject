// Data/AuthDbContext.cs
using FormBuilder.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.API.Data
{
    public class AuthDbContext : IdentityDbContext<AppUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        // Custom authorization DbSets - Security فقط
        public DbSet<Role> CustomRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rename Identity tables
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUsers");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles");
            });

            // Custom Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLES");
                entity.HasKey(e => e.RoleID);
                entity.HasIndex(e => e.RoleName).IsUnique();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Permission configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("PERMISSIONS");
                entity.HasKey(e => e.PermissionID);
                entity.HasIndex(e => e.PermissionName).IsUnique();

                entity.Property(e => e.PermissionName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.Property(e => e.Category)
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("USER_ROLES");
                entity.HasKey(e => e.UserRoleID);
                entity.HasIndex(e => new { e.UserID, e.RoleID }).IsUnique();

                entity.HasOne(ur => ur.AppUser)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.StartDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // RolePermission configuration
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("ROLE_PERMISSIONS");
                entity.HasKey(e => e.RolePermissionID);
                entity.HasIndex(e => new { e.RoleID, e.PermissionID }).IsUnique();

                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.AssignedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}