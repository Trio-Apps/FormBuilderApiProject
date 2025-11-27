using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class editDOCUMENT_TYPES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES",
                column: "ParentMenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENT_TYPES_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES",
                column: "ParentMenuId",
                principalTable: "DOCUMENT_TYPES",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENT_TYPES_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES");
        }
    }
}
