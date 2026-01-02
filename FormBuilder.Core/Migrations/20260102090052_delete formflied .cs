using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class deleteformflied : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FORM_FIELDS_FIELD_TYPES_FieldTypeId",
                table: "FORM_FIELDS");

            migrationBuilder.DropForeignKey(
                name: "FK_FORM_GRID_COLUMNS_FIELD_TYPES_FieldTypeId",
                table: "FORM_GRID_COLUMNS");

            migrationBuilder.DropTable(
                name: "FIELD_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_FORM_GRID_COLUMNS_FieldTypeId",
                table: "FORM_GRID_COLUMNS");

            migrationBuilder.DropIndex(
                name: "IX_FORM_FIELDS_FieldTypeId",
                table: "FORM_FIELDS");

            migrationBuilder.AlterColumn<int>(
                name: "FieldTypeId",
                table: "FORM_GRID_COLUMNS",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FieldTypeId",
                table: "FORM_FIELDS",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FieldTypeId",
                table: "FORM_GRID_COLUMNS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FieldTypeId",
                table: "FORM_FIELDS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FIELD_TYPES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllowMultiple = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ForeignTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HasOptions = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIELD_TYPES", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FORM_GRID_COLUMNS_FieldTypeId",
                table: "FORM_GRID_COLUMNS",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_FieldTypeId",
                table: "FORM_FIELDS",
                column: "FieldTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_FIELDS_FIELD_TYPES_FieldTypeId",
                table: "FORM_FIELDS",
                column: "FieldTypeId",
                principalTable: "FIELD_TYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_GRID_COLUMNS_FIELD_TYPES_FieldTypeId",
                table: "FORM_GRID_COLUMNS",
                column: "FieldTypeId",
                principalTable: "FIELD_TYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
