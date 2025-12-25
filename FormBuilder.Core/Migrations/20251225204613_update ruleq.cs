using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class updateruleq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RuleJson",
                table: "FORM_RULES",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ConditionField",
                table: "FORM_RULES",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionOperator",
                table: "FORM_RULES",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionValue",
                table: "FORM_RULES",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConditionValueType",
                table: "FORM_RULES",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FORM_RULE_ACTIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FieldCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Expression = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsElseAction = table.Column<bool>(type: "bit", nullable: false),
                    ActionOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORM_RULE_ACTIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FORM_RULE_ACTIONS_FORM_RULES_RuleId",
                        column: x => x.RuleId,
                        principalTable: "FORM_RULES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FORM_RULE_ACTIONS_RuleId",
                table: "FORM_RULE_ACTIONS",
                column: "RuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FORM_RULE_ACTIONS");

            migrationBuilder.DropColumn(
                name: "ConditionField",
                table: "FORM_RULES");

            migrationBuilder.DropColumn(
                name: "ConditionOperator",
                table: "FORM_RULES");

            migrationBuilder.DropColumn(
                name: "ConditionValue",
                table: "FORM_RULES");

            migrationBuilder.DropColumn(
                name: "ConditionValueType",
                table: "FORM_RULES");

            migrationBuilder.AlterColumn<string>(
                name: "RuleJson",
                table: "FORM_RULES",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
