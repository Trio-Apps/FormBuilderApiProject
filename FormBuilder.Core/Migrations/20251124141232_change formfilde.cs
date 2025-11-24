using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class changeformfilde : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FIELD_TYPES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FIELD_OPTIONS",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FIELD_TYPES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FIELD_TYPES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FIELD_TYPES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FIELD_OPTIONS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FIELD_OPTIONS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FIELD_OPTIONS",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FIELD_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FIELD_TYPES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FIELD_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FIELD_OPTIONS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FIELD_OPTIONS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FIELD_OPTIONS");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FIELD_TYPES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FIELD_OPTIONS",
                newName: "Id");
        }
    }
}
