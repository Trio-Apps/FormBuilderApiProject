using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.core.migrationdb
{
    /// <inheritdoc />
    public partial class InitAppDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FIELD_TYPES",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIELD_TYPES", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FORM_BUILDERS",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FormCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_BUILDERS", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FORM_TABS",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormBuilderID = table.Column<int>(type: "int", nullable: false),
                    TabName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TabOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_TABS", x => x.id);
                    table.ForeignKey(
                        name: "FK_FORM_TABS_FORM_BUILDERS_FormBuilderID",
                        column: x => x.FormBuilderID,
                        principalTable: "FORM_BUILDERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORM_FIELDS",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TabID = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldTypeID = table.Column<int>(type: "int", nullable: false),
                    FieldOrder = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    ValidationRules = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_FIELDS", x => x.id);
                    table.ForeignKey(
                        name: "FK_FORM_FIELDS_FIELD_TYPES_FieldTypeID",
                        column: x => x.FieldTypeID,
                        principalTable: "FIELD_TYPES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FORM_FIELDS_FORM_TABS_TabID",
                        column: x => x.TabID,
                        principalTable: "FORM_TABS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FORM_BUILDERS_FormCode",
                table: "FORM_BUILDERS",
                column: "FormCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_FieldTypeID",
                table: "FORM_FIELDS",
                column: "FieldTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_TabID_FieldOrder",
                table: "FORM_FIELDS",
                columns: new[] { "TabID", "FieldOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FORM_TABS_FormBuilderID_TabOrder",
                table: "FORM_TABS",
                columns: new[] { "FormBuilderID", "TabOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FORM_FIELDS");

            migrationBuilder.DropTable(
                name: "FIELD_TYPES");

            migrationBuilder.DropTable(
                name: "FORM_TABS");

            migrationBuilder.DropTable(
                name: "FORM_BUILDERS");
        }
    }
}
