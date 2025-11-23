using FormBuilder.API.Models;
using Microsoft.AspNetCore.Identity; // ✅ استخدام الإصدار الصحيح
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FormBuilder.API.Data
{
    public class AuthDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        // ✅ Custom Tables
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ تكوين RolePermission relationships
            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role) // ✅ Role وليس pr
                .WithMany()
                .HasForeignKey(rp => rp.RoleID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission) // ✅ Permission وليس pr
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionID)
                .OnDelete(DeleteBehavior.Cascade);
           
            // Table names
            builder.Entity<AppUser>().ToTable("appUsers");
            builder.Entity<IdentityRole>().ToTable("AspNetRoles");
            builder.Entity<Permission>().ToTable("PERMISSIONS");
            builder.Entity<RolePermission>().ToTable("ROLE_PERMISSIONS");
            builder.Entity<RefreshToken>()
                          .HasOne(rt => rt.User)
                           .WithMany()
                          .HasForeignKey(rt => rt.UserId);
        }
    }
}