using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMultilingualFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add multilingual fields to FORM_BUILDER
            migrationBuilder.AddColumn<string>(
                name: "ForeignFormName",
                table: "FORM_BUILDER",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignDescription",
                table: "FORM_BUILDER",
                type: "nvarchar(max)",
                nullable: true);

            // Add multilingual fields to FORM_TABS
            migrationBuilder.AddColumn<string>(
                name: "ForeignTabName",
                table: "FORM_TABS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            // Add multilingual fields to FORM_FIELDS
            migrationBuilder.AddColumn<string>(
                name: "ForeignFieldName",
                table: "FORM_FIELDS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignPlaceholder",
                table: "FORM_FIELDS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignHintText",
                table: "FORM_FIELDS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForeignValidationMessage",
                table: "FORM_FIELDS",
                type: "nvarchar(max)",
                nullable: true);

            // Add multilingual fields to FIELD_OPTIONS
            migrationBuilder.AddColumn<string>(
                name: "ForeignOptionText",
                table: "FIELD_OPTIONS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            // Add multilingual fields to FIELD_TYPES
            migrationBuilder.AddColumn<string>(
                name: "ForeignTypeName",
                table: "FIELD_TYPES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove multilingual fields from FIELD_TYPES
            migrationBuilder.DropColumn(
                name: "ForeignTypeName",
                table: "FIELD_TYPES");

            // Remove multilingual fields from FIELD_OPTIONS
            migrationBuilder.DropColumn(
                name: "ForeignOptionText",
                table: "FIELD_OPTIONS");

            // Remove multilingual fields from FORM_FIELDS
            migrationBuilder.DropColumn(
                name: "ForeignValidationMessage",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ForeignHintText",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ForeignPlaceholder",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ForeignFieldName",
                table: "FORM_FIELDS");

            // Remove multilingual fields from FORM_TABS
            migrationBuilder.DropColumn(
                name: "ForeignTabName",
                table: "FORM_TABS");

            // Remove multilingual fields from FORM_BUILDER
            migrationBuilder.DropColumn(
                name: "ForeignDescription",
                table: "FORM_BUILDER");

            migrationBuilder.DropColumn(
                name: "ForeignFormName",
                table: "FORM_BUILDER");
        }
    }
}
