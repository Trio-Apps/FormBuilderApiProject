using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class edits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForeignTabName",
                table: "FORM_TABS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignFieldName",
                table: "FORM_FIELDS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignHintText",
                table: "FORM_FIELDS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignPlaceholder",
                table: "FORM_FIELDS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignValidationMessage",
                table: "FORM_FIELDS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignDescription",
                table: "FORM_BUILDER",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignFormName",
                table: "FORM_BUILDER",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignTypeName",
                table: "FIELD_TYPES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignOptionText",
                table: "FIELD_OPTIONS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForeignTabName",
                table: "FORM_TABS");

            migrationBuilder.DropColumn(
                name: "ForeignFieldName",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ForeignHintText",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ForeignPlaceholder",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ForeignValidationMessage",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ForeignDescription",
                table: "FORM_BUILDER");

            migrationBuilder.DropColumn(
                name: "ForeignFormName",
                table: "FORM_BUILDER");

            migrationBuilder.DropColumn(
                name: "ForeignTypeName",
                table: "FIELD_TYPES");

            migrationBuilder.DropColumn(
                name: "ForeignOptionText",
                table: "FIELD_OPTIONS");
        }
    }
}
