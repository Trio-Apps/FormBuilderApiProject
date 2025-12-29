using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixDocumentTypesParentMenuNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENT_TYPES_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENT_TYPES_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES",
                column: "ParentMenuId",
                principalTable: "DOCUMENT_TYPES",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENT_TYPES_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENT_TYPES_DOCUMENT_TYPES_ParentMenuId",
                table: "DOCUMENT_TYPES",
                column: "ParentMenuId",
                principalTable: "DOCUMENT_TYPES",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
