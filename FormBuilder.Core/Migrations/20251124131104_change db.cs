using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class changedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_TABS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_FIELDS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_BUILDER",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_TABS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_TABS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_TABS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_FIELDS",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_TABS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_TABS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_TABS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_FIELDS");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_TABS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_FIELDS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_BUILDER",
                newName: "Id");
        }
    }
}
