using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class FORM_RULES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_RULES",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_RULES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_RULES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_RULES",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_RULES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_RULES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_RULES");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_RULES",
                newName: "Id");
        }
    }
}
