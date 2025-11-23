using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ATTACHMENT_TYPES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxSizeMB = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATTACHMENT_TYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FIELD_TYPES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    HasOptions = table.Column<bool>(type: "bit", nullable: false),
                    AllowMultiple = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIELD_TYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PERMISSIONS",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERMISSIONS", x => x.PermissionID);
                });

            migrationBuilder.CreateTable(
                name: "PROJECTS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROJECTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMTP_CONFIGS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Host = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    UseSsl = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordEncrypted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FromEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FromDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMTP_CONFIGS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APPROVAL_DELEGATIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPROVAL_DELEGATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APPROVAL_DELEGATIONS_appUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_APPROVAL_DELEGATIONS_appUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_appUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_appUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_appUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FORM_BUILDER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_BUILDER", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_BUILDER_appUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_appUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_appUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ROLE_PERMISSIONS",
                columns: table => new
                {
                    RolePermissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionID = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE_PERMISSIONS", x => x.RolePermissionID);
                    table.ForeignKey(
                        name: "FK_ROLE_PERMISSIONS_AspNetRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ROLE_PERMISSIONS_PERMISSIONS_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "PERMISSIONS",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DOCUMENT_TYPES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    MenuCaption = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MenuOrder = table.Column<int>(type: "int", nullable: false),
                    ParentMenuId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENT_TYPES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DOCUMENT_TYPES_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_ATTACHMENT_TYPES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    AttachmentTypeId = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_ATTACHMENT_TYPES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_ATTACHMENT_TYPES_ATTACHMENT_TYPES_AttachmentTypeId",
                        column: x => x.AttachmentTypeId,
                        principalTable: "ATTACHMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_ATTACHMENT_TYPES_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_BUTTONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    ButtonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ButtonCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ButtonOrder = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVisibleDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_BUTTONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_BUTTONS_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_RULES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RuleJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_RULES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_RULES_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_TABS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    TabName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TabCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TabOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_TABS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_TABS_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ADOBE_SIGNATURE_CONFIG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADOBE_SIGNATURE_CONFIG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ADOBE_SIGNATURE_CONFIG_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APPROVAL_WORKFLOWS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPROVAL_WORKFLOWS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APPROVAL_WORKFLOWS_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRYSTAL_LAYOUTS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    LayoutName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LayoutPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRYSTAL_LAYOUTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CRYSTAL_LAYOUTS_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DOCUMENT_SERIES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    SeriesCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NextNumber = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENT_SERIES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DOCUMENT_SERIES_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DOCUMENT_SERIES_PROJECTS_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PROJECTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EMAIL_TEMPLATES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TemplateCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubjectTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyTemplateHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmtpConfigId = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMAIL_TEMPLATES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EMAIL_TEMPLATES_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EMAIL_TEMPLATES_SMTP_CONFIGS_SmtpConfigId",
                        column: x => x.SmtpConfigId,
                        principalTable: "SMTP_CONFIGS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OUTLOOK_APPROVAL_CONFIG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Mailbox = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RulesJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OUTLOOK_APPROVAL_CONFIG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OUTLOOK_APPROVAL_CONFIG_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SAP_OBJECT_MAPPINGS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    SapObjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDraftOnly = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAP_OBJECT_MAPPINGS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SAP_OBJECT_MAPPINGS_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_FIELDS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TabId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FieldCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldOrder = table.Column<int>(type: "int", nullable: false),
                    Placeholder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HintText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValueJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    RegexPattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisibilityRuleJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadOnlyRuleJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_FIELDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_FIELDS_FIELD_TYPES_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FIELD_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_FIELDS_FORM_TABS_TabId",
                        column: x => x.TabId,
                        principalTable: "FORM_TABS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_FIELDS_appUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FORM_GRIDS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    GridName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GridCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TabId = table.Column<int>(type: "int", nullable: true),
                    GridOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_GRIDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_GRIDS_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_GRIDS_FORM_TABS_TabId",
                        column: x => x.TabId,
                        principalTable: "FORM_TABS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "APPROVAL_STAGES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    StageOrder = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    IsFinalStage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPROVAL_STAGES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APPROVAL_STAGES_APPROVAL_WORKFLOWS_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "APPROVAL_WORKFLOWS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_SUBMISSIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubmittedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_SUBMISSIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSIONS_DOCUMENT_SERIES_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "DOCUMENT_SERIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSIONS_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSIONS_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSIONS_appUsers_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ALERT_RULES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TriggerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConditionJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailTemplateId = table.Column<int>(type: "int", nullable: true),
                    NotificationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TargetRoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERT_RULES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ALERT_RULES_DOCUMENT_TYPES_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DOCUMENT_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ALERT_RULES_EMAIL_TEMPLATES_EmailTemplateId",
                        column: x => x.EmailTemplateId,
                        principalTable: "EMAIL_TEMPLATES",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ALERT_RULES_appUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FIELD_DATA_SOURCES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApiUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RequestBodyJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValuePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIELD_DATA_SOURCES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FIELD_DATA_SOURCES_FORM_FIELDS_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FIELD_OPTIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OptionValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OptionOrder = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIELD_OPTIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FIELD_OPTIONS_FORM_FIELDS_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_VALIDATION_RULES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: true),
                    ExpressionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_VALIDATION_RULES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_VALIDATION_RULES_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_VALIDATION_RULES_FORM_FIELDS_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FORMULAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpressionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultFieldId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORMULAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORMULAS_FORM_BUILDER_FormBuilderId",
                        column: x => x.FormBuilderId,
                        principalTable: "FORM_BUILDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORMULAS_FORM_FIELDS_ResultFieldId",
                        column: x => x.ResultFieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SAP_FIELD_MAPPINGS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormFieldId = table.Column<int>(type: "int", nullable: false),
                    SapFieldName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAP_FIELD_MAPPINGS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SAP_FIELD_MAPPINGS_FORM_FIELDS_FormFieldId",
                        column: x => x.FormFieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_GRID_COLUMNS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GridId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ColumnCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColumnOrder = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    DefaultValueJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationRuleJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_GRID_COLUMNS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_GRID_COLUMNS_FIELD_TYPES_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FIELD_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_GRID_COLUMNS_FORM_GRIDS_GridId",
                        column: x => x.GridId,
                        principalTable: "FORM_GRIDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APPROVAL_STAGE_ASSIGNEES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPROVAL_STAGE_ASSIGNEES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APPROVAL_STAGE_ASSIGNEES_APPROVAL_STAGES_StageId",
                        column: x => x.StageId,
                        principalTable: "APPROVAL_STAGES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_APPROVAL_STAGE_ASSIGNEES_appUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOCUMENT_APPROVAL_HISTORY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    ActionByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENT_APPROVAL_HISTORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DOCUMENT_APPROVAL_HISTORY_APPROVAL_STAGES_StageId",
                        column: x => x.StageId,
                        principalTable: "APPROVAL_STAGES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCUMENT_APPROVAL_HISTORY_FORM_SUBMISSIONS_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "FORM_SUBMISSIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOCUMENT_APPROVAL_HISTORY_appUsers_ActionByUserId",
                        column: x => x.ActionByUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FORM_SUBMISSION_ATTACHMENTS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_SUBMISSION_ATTACHMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_ATTACHMENTS_FORM_FIELDS_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_ATTACHMENTS_FORM_SUBMISSIONS_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "FORM_SUBMISSIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FORM_SUBMISSION_GRID_ROWS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    GridId = table.Column<int>(type: "int", nullable: false),
                    RowIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_SUBMISSION_GRID_ROWS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_GRID_ROWS_FORM_GRIDS_GridId",
                        column: x => x.GridId,
                        principalTable: "FORM_GRIDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_GRID_ROWS_FORM_SUBMISSIONS_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "FORM_SUBMISSIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_SUBMISSION_VALUES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    FieldCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ValueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueBool = table.Column<bool>(type: "bit", nullable: true),
                    ValueJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_SUBMISSION_VALUES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_VALUES_FORM_FIELDS_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_VALUES_FORM_SUBMISSIONS_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "FORM_SUBMISSIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FORMULA_VARIABLES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormulaId = table.Column<int>(type: "int", nullable: false),
                    VariableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SourceFieldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORMULA_VARIABLES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORMULA_VARIABLES_FORMULAS_FormulaId",
                        column: x => x.FormulaId,
                        principalTable: "FORMULAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FORMULA_VARIABLES_FORM_FIELDS_SourceFieldId",
                        column: x => x.SourceFieldId,
                        principalTable: "FORM_FIELDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FORM_SUBMISSION_GRID_CELLS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowId = table.Column<int>(type: "int", nullable: false),
                    ColumnId = table.Column<int>(type: "int", nullable: false),
                    ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ValueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueBool = table.Column<bool>(type: "bit", nullable: true),
                    ValueJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_SUBMISSION_GRID_CELLS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_GRID_CELLS_FORM_GRID_COLUMNS_ColumnId",
                        column: x => x.ColumnId,
                        principalTable: "FORM_GRID_COLUMNS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FORM_SUBMISSION_GRID_CELLS_FORM_SUBMISSION_GRID_ROWS_RowId",
                        column: x => x.RowId,
                        principalTable: "FORM_SUBMISSION_GRID_ROWS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ADOBE_SIGNATURE_CONFIG_DocumentTypeId",
                table: "ADOBE_SIGNATURE_CONFIG",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ALERT_RULES_DocumentTypeId",
                table: "ALERT_RULES",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ALERT_RULES_EmailTemplateId",
                table: "ALERT_RULES",
                column: "EmailTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ALERT_RULES_TargetUserId",
                table: "ALERT_RULES",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_DELEGATIONS_FromUserId",
                table: "APPROVAL_DELEGATIONS",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_DELEGATIONS_ToUserId",
                table: "APPROVAL_DELEGATIONS",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_STAGE_ASSIGNEES_StageId",
                table: "APPROVAL_STAGE_ASSIGNEES",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_STAGE_ASSIGNEES_UserId",
                table: "APPROVAL_STAGE_ASSIGNEES",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_STAGES_WorkflowId",
                table: "APPROVAL_STAGES",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_WORKFLOWS_DocumentTypeId",
                table: "APPROVAL_WORKFLOWS",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "appUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "appUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CRYSTAL_LAYOUTS_DocumentTypeId",
                table: "CRYSTAL_LAYOUTS",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_APPROVAL_HISTORY_ActionByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY",
                column: "ActionByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_APPROVAL_HISTORY_StageId",
                table: "DOCUMENT_APPROVAL_HISTORY",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_APPROVAL_HISTORY_SubmissionId",
                table: "DOCUMENT_APPROVAL_HISTORY",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_SERIES_DocumentTypeId",
                table: "DOCUMENT_SERIES",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_SERIES_ProjectId",
                table: "DOCUMENT_SERIES",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_SERIES_SeriesCode",
                table: "DOCUMENT_SERIES",
                column: "SeriesCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_TYPES_Code",
                table: "DOCUMENT_TYPES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_TYPES_FormBuilderId",
                table: "DOCUMENT_TYPES",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_EMAIL_TEMPLATES_DocumentTypeId",
                table: "EMAIL_TEMPLATES",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EMAIL_TEMPLATES_SmtpConfigId",
                table: "EMAIL_TEMPLATES",
                column: "SmtpConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FIELD_DATA_SOURCES_FieldId",
                table: "FIELD_DATA_SOURCES",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FIELD_OPTIONS_FieldId",
                table: "FIELD_OPTIONS",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_ATTACHMENT_TYPES_AttachmentTypeId",
                table: "FORM_ATTACHMENT_TYPES",
                column: "AttachmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_ATTACHMENT_TYPES_FormBuilderId",
                table: "FORM_ATTACHMENT_TYPES",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_BUILDER_CreatedByUserId",
                table: "FORM_BUILDER",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_BUILDER_FormCode",
                table: "FORM_BUILDER",
                column: "FormCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FORM_BUTTONS_FormBuilderId",
                table: "FORM_BUTTONS",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_CreatedByUserId",
                table: "FORM_FIELDS",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_FieldTypeId",
                table: "FORM_FIELDS",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_TabId",
                table: "FORM_FIELDS",
                column: "TabId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_GRID_COLUMNS_FieldTypeId",
                table: "FORM_GRID_COLUMNS",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_GRID_COLUMNS_GridId",
                table: "FORM_GRID_COLUMNS",
                column: "GridId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_GRIDS_FormBuilderId",
                table: "FORM_GRIDS",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_GRIDS_TabId",
                table: "FORM_GRIDS",
                column: "TabId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_RULES_FormBuilderId",
                table: "FORM_RULES",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_ATTACHMENTS_FieldId",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_ATTACHMENTS_SubmissionId",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_GRID_CELLS_ColumnId",
                table: "FORM_SUBMISSION_GRID_CELLS",
                column: "ColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_GRID_CELLS_RowId",
                table: "FORM_SUBMISSION_GRID_CELLS",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_GRID_ROWS_GridId",
                table: "FORM_SUBMISSION_GRID_ROWS",
                column: "GridId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_GRID_ROWS_SubmissionId",
                table: "FORM_SUBMISSION_GRID_ROWS",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_VALUES_FieldId",
                table: "FORM_SUBMISSION_VALUES",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSION_VALUES_SubmissionId",
                table: "FORM_SUBMISSION_VALUES",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSIONS_DocumentNumber",
                table: "FORM_SUBMISSIONS",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSIONS_DocumentTypeId",
                table: "FORM_SUBMISSIONS",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSIONS_FormBuilderId",
                table: "FORM_SUBMISSIONS",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSIONS_SeriesId",
                table: "FORM_SUBMISSIONS",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSIONS_SubmittedByUserId",
                table: "FORM_SUBMISSIONS",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_TABS_FormBuilderId",
                table: "FORM_TABS",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_VALIDATION_RULES_FieldId",
                table: "FORM_VALIDATION_RULES",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_VALIDATION_RULES_FormBuilderId",
                table: "FORM_VALIDATION_RULES",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORMULA_VARIABLES_FormulaId",
                table: "FORMULA_VARIABLES",
                column: "FormulaId");

            migrationBuilder.CreateIndex(
                name: "IX_FORMULA_VARIABLES_SourceFieldId",
                table: "FORMULA_VARIABLES",
                column: "SourceFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FORMULAS_FormBuilderId",
                table: "FORMULAS",
                column: "FormBuilderId");

            migrationBuilder.CreateIndex(
                name: "IX_FORMULAS_ResultFieldId",
                table: "FORMULAS",
                column: "ResultFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_OUTLOOK_APPROVAL_CONFIG_DocumentTypeId",
                table: "OUTLOOK_APPROVAL_CONFIG",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECTS_Code",
                table: "PROJECTS",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ROLE_PERMISSIONS_PermissionID",
                table: "ROLE_PERMISSIONS",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_ROLE_PERMISSIONS_RoleID",
                table: "ROLE_PERMISSIONS",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_SAP_FIELD_MAPPINGS_FormFieldId",
                table: "SAP_FIELD_MAPPINGS",
                column: "FormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SAP_OBJECT_MAPPINGS_DocumentTypeId",
                table: "SAP_OBJECT_MAPPINGS",
                column: "DocumentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADOBE_SIGNATURE_CONFIG");

            migrationBuilder.DropTable(
                name: "ALERT_RULES");

            migrationBuilder.DropTable(
                name: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropTable(
                name: "APPROVAL_STAGE_ASSIGNEES");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CRYSTAL_LAYOUTS");

            migrationBuilder.DropTable(
                name: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.DropTable(
                name: "FIELD_DATA_SOURCES");

            migrationBuilder.DropTable(
                name: "FIELD_OPTIONS");

            migrationBuilder.DropTable(
                name: "FORM_ATTACHMENT_TYPES");

            migrationBuilder.DropTable(
                name: "FORM_BUTTONS");

            migrationBuilder.DropTable(
                name: "FORM_RULES");

            migrationBuilder.DropTable(
                name: "FORM_SUBMISSION_ATTACHMENTS");

            migrationBuilder.DropTable(
                name: "FORM_SUBMISSION_GRID_CELLS");

            migrationBuilder.DropTable(
                name: "FORM_SUBMISSION_VALUES");

            migrationBuilder.DropTable(
                name: "FORM_VALIDATION_RULES");

            migrationBuilder.DropTable(
                name: "FORMULA_VARIABLES");

            migrationBuilder.DropTable(
                name: "OUTLOOK_APPROVAL_CONFIG");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ROLE_PERMISSIONS");

            migrationBuilder.DropTable(
                name: "SAP_FIELD_MAPPINGS");

            migrationBuilder.DropTable(
                name: "SAP_OBJECT_MAPPINGS");

            migrationBuilder.DropTable(
                name: "EMAIL_TEMPLATES");

            migrationBuilder.DropTable(
                name: "APPROVAL_STAGES");

            migrationBuilder.DropTable(
                name: "ATTACHMENT_TYPES");

            migrationBuilder.DropTable(
                name: "FORM_GRID_COLUMNS");

            migrationBuilder.DropTable(
                name: "FORM_SUBMISSION_GRID_ROWS");

            migrationBuilder.DropTable(
                name: "FORMULAS");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PERMISSIONS");

            migrationBuilder.DropTable(
                name: "SMTP_CONFIGS");

            migrationBuilder.DropTable(
                name: "APPROVAL_WORKFLOWS");

            migrationBuilder.DropTable(
                name: "FORM_GRIDS");

            migrationBuilder.DropTable(
                name: "FORM_SUBMISSIONS");

            migrationBuilder.DropTable(
                name: "FORM_FIELDS");

            migrationBuilder.DropTable(
                name: "DOCUMENT_SERIES");

            migrationBuilder.DropTable(
                name: "FIELD_TYPES");

            migrationBuilder.DropTable(
                name: "FORM_TABS");

            migrationBuilder.DropTable(
                name: "DOCUMENT_TYPES");

            migrationBuilder.DropTable(
                name: "PROJECTS");

            migrationBuilder.DropTable(
                name: "FORM_BUILDER");

            migrationBuilder.DropTable(
                name: "appUsers");
        }
    }
}
