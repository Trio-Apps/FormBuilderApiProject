using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddGridIdToFormFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GridId",
                table: "FORM_FIELDS",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_GridId",
                table: "FORM_FIELDS",
                column: "GridId");

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_FIELDS_FORM_GRIDS_GridId",
                table: "FORM_FIELDS",
                column: "GridId",
                principalTable: "FORM_GRIDS",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FORM_FIELDS_FORM_GRIDS_GridId",
                table: "FORM_FIELDS");

            migrationBuilder.DropIndex(
                name: "IX_FORM_FIELDS_GridId",
                table: "FORM_FIELDS");

            migrationBuilder.DropColumn(
                name: "GridId",
                table: "FORM_FIELDS");
        }
    }
}
