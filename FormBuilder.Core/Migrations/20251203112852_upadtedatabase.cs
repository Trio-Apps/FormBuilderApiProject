using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class upadtedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ALERT_RULES_appUsers_TargetUserId",
                table: "ALERT_RULES");

            migrationBuilder.DropForeignKey(
                name: "FK_APPROVAL_DELEGATIONS_appUsers_FromUserId",
                table: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_APPROVAL_DELEGATIONS_appUsers_ToUserId",
                table: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_APPROVAL_STAGE_ASSIGNEES_appUsers_UserId",
                table: "APPROVAL_STAGE_ASSIGNEES");

            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENT_APPROVAL_HISTORY_appUsers_ActionByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.DropForeignKey(
                name: "FK_FORM_BUILDER_appUsers_CreatedByUserId",
                table: "FORM_BUILDER");

            migrationBuilder.DropForeignKey(
                name: "FK_FORM_FIELDS_appUsers_CreatedByUserId",
                table: "FORM_FIELDS");

            migrationBuilder.DropForeignKey(
                name: "FK_FORM_SUBMISSIONS_appUsers_SubmittedByUserId",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.DropIndex(
                name: "IX_FORM_SUBMISSIONS_SubmittedByUserId",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.DropIndex(
                name: "IX_FORM_FIELDS_CreatedByUserId",
                table: "FORM_FIELDS");

            migrationBuilder.DropIndex(
                name: "IX_FORM_BUILDER_CreatedByUserId",
                table: "FORM_BUILDER");

            migrationBuilder.DropIndex(
                name: "IX_DOCUMENT_APPROVAL_HISTORY_ActionByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.DropIndex(
                name: "IX_APPROVAL_STAGE_ASSIGNEES_UserId",
                table: "APPROVAL_STAGE_ASSIGNEES");

            migrationBuilder.DropIndex(
                name: "IX_APPROVAL_DELEGATIONS_FromUserId",
                table: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropIndex(
                name: "IX_APPROVAL_DELEGATIONS_ToUserId",
                table: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropIndex(
                name: "IX_ALERT_RULES_TargetUserId",
                table: "ALERT_RULES");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.DropColumn(
                name: "ActionByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "SMTP_CONFIGS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "SAP_OBJECT_MAPPINGS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "SAP_FIELD_MAPPINGS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PROJECTS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "OUTLOOK_APPROVAL_CONFIG",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORMULAS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORMULA_VARIABLES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_VALIDATION_RULES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_TABS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_SUBMISSIONS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_SUBMISSION_VALUES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_SUBMISSION_GRID_ROWS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_SUBMISSION_GRID_CELLS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_RULES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_GRIDS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_GRID_COLUMNS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_FIELDS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_BUTTONS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_BUILDER",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_ATTACHMENT_TYPES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FIELD_TYPES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FIELD_OPTIONS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FIELD_DATA_SOURCES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "EMAIL_TEMPLATES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DOCUMENT_TYPES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DOCUMENT_SERIES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DOCUMENT_APPROVAL_HISTORY",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CRYSTAL_LAYOUTS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ATTACHMENT_TYPES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "APPROVAL_WORKFLOWS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "APPROVAL_STAGES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "APPROVAL_STAGE_ASSIGNEES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "APPROVAL_DELEGATIONS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ALERT_RULES",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ADOBE_SIGNATURE_CONFIG",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedByUserId",
                table: "FORM_SUBMISSIONS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_BUILDER",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "APPROVAL_STAGE_ASSIGNEES",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ToUserId",
                table: "APPROVAL_DELEGATIONS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FromUserId",
                table: "APPROVAL_DELEGATIONS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "ALERT_RULES",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SMTP_CONFIGS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SAP_OBJECT_MAPPINGS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SAP_FIELD_MAPPINGS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PROJECTS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OUTLOOK_APPROVAL_CONFIG",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORMULAS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORMULA_VARIABLES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_VALIDATION_RULES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_TABS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_SUBMISSIONS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_SUBMISSION_VALUES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_SUBMISSION_GRID_ROWS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_SUBMISSION_GRID_CELLS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_RULES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_GRIDS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_GRID_COLUMNS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_FIELDS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_BUTTONS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_BUILDER",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_ATTACHMENT_TYPES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FIELD_TYPES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FIELD_OPTIONS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FIELD_DATA_SOURCES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EMAIL_TEMPLATES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DOCUMENT_TYPES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DOCUMENT_SERIES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DOCUMENT_APPROVAL_HISTORY",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CRYSTAL_LAYOUTS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ATTACHMENT_TYPES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "APPROVAL_WORKFLOWS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "APPROVAL_STAGES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "APPROVAL_STAGE_ASSIGNEES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "APPROVAL_DELEGATIONS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ALERT_RULES",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ADOBE_SIGNATURE_CONFIG",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedByUserId",
                table: "FORM_SUBMISSIONS",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "FORM_SUBMISSIONS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_BUILDER",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "APPROVAL_STAGE_ASSIGNEES",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ToUserId",
                table: "APPROVAL_DELEGATIONS",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FromUserId",
                table: "APPROVAL_DELEGATIONS",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "ALERT_RULES",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_SUBMISSIONS_SubmittedByUserId",
                table: "FORM_SUBMISSIONS",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_FIELDS_CreatedByUserId",
                table: "FORM_FIELDS",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FORM_BUILDER_CreatedByUserId",
                table: "FORM_BUILDER",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_APPROVAL_HISTORY_ActionByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY",
                column: "ActionByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_STAGE_ASSIGNEES_UserId",
                table: "APPROVAL_STAGE_ASSIGNEES",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_DELEGATIONS_FromUserId",
                table: "APPROVAL_DELEGATIONS",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_DELEGATIONS_ToUserId",
                table: "APPROVAL_DELEGATIONS",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ALERT_RULES_TargetUserId",
                table: "ALERT_RULES",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ALERT_RULES_appUsers_TargetUserId",
                table: "ALERT_RULES",
                column: "TargetUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_APPROVAL_DELEGATIONS_appUsers_FromUserId",
                table: "APPROVAL_DELEGATIONS",
                column: "FromUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_APPROVAL_DELEGATIONS_appUsers_ToUserId",
                table: "APPROVAL_DELEGATIONS",
                column: "ToUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_APPROVAL_STAGE_ASSIGNEES_appUsers_UserId",
                table: "APPROVAL_STAGE_ASSIGNEES",
                column: "UserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENT_APPROVAL_HISTORY_appUsers_ActionByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY",
                column: "ActionByUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_BUILDER_appUsers_CreatedByUserId",
                table: "FORM_BUILDER",
                column: "CreatedByUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_FIELDS_appUsers_CreatedByUserId",
                table: "FORM_FIELDS",
                column: "CreatedByUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FORM_SUBMISSIONS_appUsers_SubmittedByUserId",
                table: "FORM_SUBMISSIONS",
                column: "SubmittedByUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
