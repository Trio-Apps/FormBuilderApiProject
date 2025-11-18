using FormBuilder.API.Models;
using FormBuilder.API.Models.FormBuilder.API.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FormBuilders> FormBuilders { get; set; }
    public DbSet<FORM_TABS> FormTabs { get; set; }
    public DbSet<FormField> FormFields { get; set; }
    public DbSet<FieldType> FieldTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // FormBuilder -> FormTabs relationship
        modelBuilder.Entity<FORM_TABS>(entity =>
        {
            entity.HasOne(ft => ft.FormBuilder)
                  .WithMany(fb => fb.FormTabs)
                  .HasForeignKey(ft => ft.FormBuilderID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // FormTab -> FormFields relationship
        modelBuilder.Entity<FormField>(entity =>
        {
           

            entity.HasOne(ff => ff.FieldType)
                  .WithMany(ft => ft.FormFields)
                  .HasForeignKey(ff => ff.FieldTypeID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Add unique constraints
        modelBuilder.Entity<FormBuilders>(entity =>
        {
            entity.HasIndex(e => e.FormCode).IsUnique();
        });

        modelBuilder.Entity<FORM_TABS>(entity =>
        {
            entity.HasIndex(e => new { e.FormBuilderID, e.TabOrder }).IsUnique();
        });

        modelBuilder.Entity<FormField>(entity =>
        {
            entity.HasIndex(e => new { e.TabID, e.FieldOrder }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}