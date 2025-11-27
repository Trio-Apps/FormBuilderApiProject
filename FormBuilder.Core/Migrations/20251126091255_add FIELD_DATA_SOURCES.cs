using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class addFIELD_DATA_SOURCES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FIELD_DATA_SOURCES",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FIELD_DATA_SOURCES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FIELD_DATA_SOURCES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FIELD_DATA_SOURCES",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FIELD_DATA_SOURCES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FIELD_DATA_SOURCES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FIELD_DATA_SOURCES");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FIELD_DATA_SOURCES",
                newName: "Id");
        }
    }
}
