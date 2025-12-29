using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class updatedocumenttype2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENT_SERIES_PROJECTS_ProjectId",
                table: "DOCUMENT_SERIES");

            migrationBuilder.DropForeignKey(
                name: "FK_FORM_SUBMISSIONS_DOCUMENT_SERIES_SeriesId",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENT_SERIES_PROJECTS_ProjectId",
                table: "DOCUMENT_SERIES",
                column: "ProjectId",
                principalTable: "PROJECTS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_SUBMISSIONS_DOCUMENT_SERIES_SeriesId",
                table: "FORM_SUBMISSIONS",
                column: "SeriesId",
                principalTable: "DOCUMENT_SERIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENT_SERIES_PROJECTS_ProjectId",
                table: "DOCUMENT_SERIES");

            migrationBuilder.DropForeignKey(
                name: "FK_FORM_SUBMISSIONS_DOCUMENT_SERIES_SeriesId",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENT_SERIES_PROJECTS_ProjectId",
                table: "DOCUMENT_SERIES",
                column: "ProjectId",
                principalTable: "PROJECTS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_SUBMISSIONS_DOCUMENT_SERIES_SeriesId",
                table: "FORM_SUBMISSIONS",
                column: "SeriesId",
                principalTable: "DOCUMENT_SERIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
