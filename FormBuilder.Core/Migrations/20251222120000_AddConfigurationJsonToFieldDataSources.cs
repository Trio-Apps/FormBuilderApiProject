using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigurationJsonToFieldDataSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfigurationJson",
                table: "FIELD_DATA_SOURCES",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigurationJson",
                table: "FIELD_DATA_SOURCES");
        }
    }
}

