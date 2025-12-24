using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class updaterule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExecutionOrder",
                table: "FORM_RULES",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionOrder",
                table: "FORM_RULES");
        }
    }
}
