using formBuilder.Domian.Entitys;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FormBuilder.Infrastructure.Data
{
    public class FormBuilderDbContext : DbContext
    {
        public FormBuilderDbContext(DbContextOptions<FormBuilderDbContext> options) : base(options) { }

   

        // ----------------------
        // Form Builder Tables
        // ----------------------
        public DbSet<FORM_BUILDER> FORM_BUILDER { get; set; }
        public DbSet<FORM_TABS> FORM_TABS { get; set; }
        public DbSet<FIELD_TYPES> FIELD_TYPES { get; set; }
        public DbSet<FORM_FIELDS> FORM_FIELDS { get; set; }
        public DbSet<FIELD_OPTIONS> FIELD_OPTIONS { get; set; }
        public DbSet<FIELD_DATA_SOURCES> FIELD_DATA_SOURCES { get; set; }
        public DbSet<FORM_RULES> FORM_RULES { get; set; }
        public DbSet<FORMULAS> FORMULAS { get; set; }
        public DbSet<FORMULA_VARIABLES> FORMULA_VARIABLES { get; set; }
        public DbSet<FORM_VALIDATION_RULES> FORM_VALIDATION_RULES { get; set; }
        public DbSet<FORM_SUBMISSIONS> FORM_SUBMISSIONS { get; set; }
        public DbSet<FORM_SUBMISSION_VALUES> FORM_SUBMISSION_VALUES { get; set; }
        public DbSet<FORM_SUBMISSION_ATTACHMENTS> FORM_SUBMISSION_ATTACHMENTS { get; set; }
        public DbSet<ATTACHMENT_TYPES> ATTACHMENT_TYPES { get; set; }
        public DbSet<FORM_ATTACHMENT_TYPES> FORM_ATTACHMENT_TYPES { get; set; }
        public DbSet<DOCUMENT_TYPES> DOCUMENT_TYPES { get; set; }
        public DbSet<PROJECTS> PROJECTS { get; set; }
        public DbSet<DOCUMENT_SERIES> DOCUMENT_SERIES { get; set; }
        public DbSet<FORM_GRIDS> FORM_GRIDS { get; set; }
        public DbSet<FORM_GRID_COLUMNS> FORM_GRID_COLUMNS { get; set; }
        public DbSet<FORM_SUBMISSION_GRID_ROWS> FORM_SUBMISSION_GRID_ROWS { get; set; }
        public DbSet<FORM_SUBMISSION_GRID_CELLS> FORM_SUBMISSION_GRID_CELLS { get; set; }
        public DbSet<APPROVAL_WORKFLOWS> APPROVAL_WORKFLOWS { get; set; }
        public DbSet<APPROVAL_STAGES> APPROVAL_STAGES { get; set; }
        public DbSet<APPROVAL_STAGE_ASSIGNEES> APPROVAL_STAGE_ASSIGNEES { get; set; }
        public DbSet<APPROVAL_DELEGATIONS> APPROVAL_DELEGATIONS { get; set; }
        public DbSet<DOCUMENT_APPROVAL_HISTORY> DOCUMENT_APPROVAL_HISTORY { get; set; }
        public DbSet<SMTP_CONFIGS> SMTP_CONFIGS { get; set; }
        public DbSet<EMAIL_TEMPLATES> EMAIL_TEMPLATES { get; set; }
        public DbSet<ALERT_RULES> ALERT_RULES { get; set; }
        public DbSet<FORM_BUTTONS> FORM_BUTTONS { get; set; }
        public DbSet<CRYSTAL_LAYOUTS> CRYSTAL_LAYOUTS { get; set; }
        public DbSet<ADOBE_SIGNATURE_CONFIG> ADOBE_SIGNATURE_CONFIG { get; set; }
        public DbSet<OUTLOOK_APPROVAL_CONFIG> OUTLOOK_APPROVAL_CONFIG { get; set; }
        public DbSet<SAP_OBJECT_MAPPINGS> SAP_OBJECT_MAPPINGS { get; set; }
        public DbSet<SAP_FIELD_MAPPINGS> SAP_FIELD_MAPPINGS { get; set; }
        public DbSet<REFRESH_TOKENS> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            // FORM_BUILDER relationships
            // ----------------------
            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORM_TABS)
                .WithOne(ft => ft.FORM_BUILDER)
                .HasForeignKey(ft => ft.FormBuilderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORM_GRIDS)
                .WithOne(fg => fg.FORM_BUILDER)
                .HasForeignKey(fg => fg.FormBuilderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORM_RULES)
                .WithOne(fr => fr.FORM_BUILDER)
                .HasForeignKey(fr => fr.FormBuilderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORMULAS)
                .WithOne(f => f.FORM_BUILDER)
                .HasForeignKey(f => f.FormBuilderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORM_VALIDATION_RULES)
                .WithOne(fvr => fvr.FORM_BUILDER)
                .HasForeignKey(fvr => fvr.FormBuilderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORM_SUBMISSIONS)
                .WithOne(fs => fs.FORM_BUILDER)
                .HasForeignKey(fs => fs.FormBuilderId)
                .OnDelete(DeleteBehavior.Restrict); // prevent multiple cascade paths

            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORM_BUTTONS)
                .WithOne(fb => fb.FORM_BUILDER)
                .HasForeignKey(fb => fb.FormBuilderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_BUILDER>()
                .HasMany(fb => fb.FORM_ATTACHMENT_TYPES)
                .WithOne(fat => fat.FORM_BUILDER)
                .HasForeignKey(fat => fat.FormBuilderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----------------------
            // FORM_TABS relationships
            // ----------------------
            modelBuilder.Entity<FORM_TABS>()
                .HasMany(ft => ft.FORM_FIELDS)
                .WithOne(ff => ff.FORM_TABS)
                .HasForeignKey(ff => ff.TabId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_TABS>()
                .HasMany(ft => ft.FORM_GRIDS)
                .WithOne(fg => fg.FORM_TABS)
                .HasForeignKey(fg => fg.TabId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------------------
            // FORM_FIELDS relationships
            // ----------------------
            modelBuilder.Entity<FORM_FIELDS>()
                .HasMany(ff => ff.FIELD_OPTIONS)
                .WithOne(fo => fo.FORM_FIELDS)
                .HasForeignKey(fo => fo.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_FIELDS>()
                .HasMany(ff => ff.FIELD_DATA_SOURCES)
                .WithOne(fds => fds.FORM_FIELDS)
                .HasForeignKey(fds => fds.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_FIELDS>()
                .HasMany(ff => ff.SAP_FIELD_MAPPINGS)
                .WithOne(sfm => sfm.FORM_FIELDS)
                .HasForeignKey(sfm => sfm.FormFieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_FIELDS>()
                .HasMany(ff => ff.FORMULA_VARIABLES)
                .WithOne(fv => fv.FORM_FIELDS)
                .HasForeignKey(fv => fv.SourceFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------------------
            // FORMULAS relationships
            // ----------------------
            modelBuilder.Entity<FORMULAS>()
                .HasOne(f => f.RESULT_FIELD)
                .WithMany()
                .HasForeignKey(f => f.ResultFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------------------
            // FORM_SUBMISSIONS relationships
            // ----------------------
            modelBuilder.Entity<FORM_SUBMISSIONS>()
                .HasMany(fs => fs.FORM_SUBMISSION_VALUES)
                .WithOne(fsv => fsv.FORM_SUBMISSIONS)
                .HasForeignKey(fsv => fsv.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FORM_SUBMISSIONS>()
                .HasMany(fs => fs.FORM_SUBMISSION_ATTACHMENTS)
                .WithOne(fsa => fsa.FORM_SUBMISSIONS)
                .HasForeignKey(fsa => fsa.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FORM_SUBMISSIONS>()
                .HasMany(fs => fs.FORM_SUBMISSION_GRID_ROWS)
                .WithOne(fsgr => fsgr.FORM_SUBMISSIONS)
                .HasForeignKey(fsgr => fsgr.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_SUBMISSIONS>()
                .HasMany(fs => fs.DOCUMENT_APPROVAL_HISTORY)
                .WithOne(dah => dah.FORM_SUBMISSIONS)
                .HasForeignKey(dah => dah.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DOCUMENT_TYPES>()
                .HasMany(dt => dt.FORM_SUBMISSIONS)
                .WithOne(fs => fs.DOCUMENT_TYPES)
                .HasForeignKey(fs => fs.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------------------
            // APPROVAL WORKFLOWS & STAGES
            // ----------------------
            modelBuilder.Entity<APPROVAL_WORKFLOWS>()
                .HasMany(aw => aw.APPROVAL_STAGES)
                .WithOne(ast => ast.APPROVAL_WORKFLOWS)
                .HasForeignKey(ast => ast.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<APPROVAL_STAGES>()
                .HasMany(ast => ast.APPROVAL_STAGE_ASSIGNEES)
                .WithOne(asa => asa.APPROVAL_STAGES)
                .HasForeignKey(asa => asa.StageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<APPROVAL_STAGES>()
                .HasMany(ast => ast.DOCUMENT_APPROVAL_HISTORY)
                .WithOne(dah => dah.APPROVAL_STAGES)
                .HasForeignKey(dah => dah.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------------------
            // GRIDS relationships
            // ----------------------
            modelBuilder.Entity<FORM_GRIDS>()
                .HasMany(fg => fg.FORM_GRID_COLUMNS)
                .WithOne(fgc => fgc.FORM_GRIDS)
                .HasForeignKey(fgc => fgc.GridId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FORM_GRIDS>()
                .HasMany(fg => fg.FORM_SUBMISSION_GRID_ROWS)
                .WithOne(fsgr => fsgr.FORM_GRIDS)
                .HasForeignKey(fsgr => fsgr.GridId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FORM_GRID_COLUMNS>()
                .HasMany(fgc => fgc.FORM_SUBMISSION_GRID_CELLS)
                .WithOne(fsgc => fsgc.FORM_GRID_COLUMNS)
                .HasForeignKey(fsgc => fsgc.ColumnId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FORM_SUBMISSION_GRID_ROWS>()
                .HasMany(fsgr => fsgr.FORM_SUBMISSION_GRID_CELLS)
                .WithOne(fsgc => fsgc.FORM_SUBMISSION_GRID_ROWS)
                .HasForeignKey(fsgc => fsgc.RowId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----------------------
            // DECIMAL PRECISION
            // ----------------------
            modelBuilder.Entity<FORM_FIELDS>().Property(f => f.MinValue).HasPrecision(18, 4);
            modelBuilder.Entity<FORM_FIELDS>().Property(f => f.MaxValue).HasPrecision(18, 4);
            modelBuilder.Entity<FORM_SUBMISSION_VALUES>().Property(f => f.ValueNumber).HasPrecision(18, 4);
            modelBuilder.Entity<APPROVAL_STAGES>().Property(a => a.MinAmount).HasPrecision(18, 4);
            modelBuilder.Entity<APPROVAL_STAGES>().Property(a => a.MaxAmount).HasPrecision(18, 4);
            modelBuilder.Entity<FORM_SUBMISSION_GRID_CELLS>().Property(f => f.ValueNumber).HasPrecision(18, 4);


            // ----------------------
            // INDEXES
            // ----------------------
            modelBuilder.Entity<FORM_BUILDER>().HasIndex(fb => fb.FormCode).IsUnique();
            modelBuilder.Entity<FORM_SUBMISSIONS>().HasIndex(fs => fs.DocumentNumber).IsUnique();
            modelBuilder.Entity<DOCUMENT_TYPES>().HasIndex(dt => dt.Code).IsUnique();
            modelBuilder.Entity<PROJECTS>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<DOCUMENT_SERIES>().HasIndex(ds => ds.SeriesCode).IsUnique();
        }
    }
}