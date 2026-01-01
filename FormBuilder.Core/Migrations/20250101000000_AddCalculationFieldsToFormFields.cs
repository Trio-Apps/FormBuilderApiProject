using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCalculationFieldsToFormFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalculationMode",
                table: "FORM_FIELDS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpressionText",
                table: "FORM_FIELDS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecalculateOn",
                table: "FORM_FIELDS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultType",
                table: "FORM_FIELDS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationMode",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ExpressionText",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "RecalculateOn",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "ResultType",
                table: "FORM_FIELDS");
        }
    }
}

