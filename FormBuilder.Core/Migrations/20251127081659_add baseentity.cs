using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.Core.Migrations
{
    /// <inheritdoc />
    public partial class addbaseentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "FORM_GRIDS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_GRID_COLUMNS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_BUTTONS",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FORM_ATTACHMENT_TYPES",
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

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "SMTP_CONFIGS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SMTP_CONFIGS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SMTP_CONFIGS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "SAP_OBJECT_MAPPINGS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SAP_OBJECT_MAPPINGS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SAP_OBJECT_MAPPINGS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "SAP_FIELD_MAPPINGS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SAP_FIELD_MAPPINGS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SAP_FIELD_MAPPINGS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "PROJECTS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PROJECTS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "PROJECTS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "OUTLOOK_APPROVAL_CONFIG",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OUTLOOK_APPROVAL_CONFIG",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OUTLOOK_APPROVAL_CONFIG",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "OUTLOOK_APPROVAL_CONFIG",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORMULAS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORMULAS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORMULAS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORMULA_VARIABLES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORMULA_VARIABLES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FORMULA_VARIABLES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORMULA_VARIABLES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_VALIDATION_RULES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_VALIDATION_RULES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_VALIDATION_RULES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSIONS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FORM_SUBMISSIONS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_SUBMISSIONS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_VALUES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_VALUES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FORM_SUBMISSION_VALUES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_VALUES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_GRID_ROWS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_GRID_ROWS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_GRID_ROWS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_GRID_CELLS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_GRID_CELLS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FORM_SUBMISSION_GRID_CELLS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_GRID_CELLS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_ATTACHMENTS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_GRIDS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_GRIDS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_GRIDS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_GRID_COLUMNS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_GRID_COLUMNS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_GRID_COLUMNS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_BUTTONS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_BUTTONS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_BUTTONS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "FORM_ATTACHMENT_TYPES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "FORM_ATTACHMENT_TYPES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FORM_ATTACHMENT_TYPES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "EMAIL_TEMPLATES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "EMAIL_TEMPLATES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "EMAIL_TEMPLATES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "DOCUMENT_TYPES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DOCUMENT_TYPES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "DOCUMENT_TYPES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "DOCUMENT_SERIES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DOCUMENT_SERIES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "DOCUMENT_SERIES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DOCUMENT_APPROVAL_HISTORY",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DOCUMENT_APPROVAL_HISTORY",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "DOCUMENT_APPROVAL_HISTORY",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CRYSTAL_LAYOUTS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CRYSTAL_LAYOUTS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "CRYSTAL_LAYOUTS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ATTACHMENT_TYPES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ATTACHMENT_TYPES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ATTACHMENT_TYPES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "APPROVAL_WORKFLOWS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "APPROVAL_WORKFLOWS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "APPROVAL_WORKFLOWS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "APPROVAL_STAGES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "APPROVAL_STAGES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "APPROVAL_STAGES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "APPROVAL_STAGES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "APPROVAL_STAGE_ASSIGNEES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "APPROVAL_STAGE_ASSIGNEES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "APPROVAL_STAGE_ASSIGNEES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "APPROVAL_STAGE_ASSIGNEES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "APPROVAL_DELEGATIONS",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "APPROVAL_DELEGATIONS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "APPROVAL_DELEGATIONS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ALERT_RULES",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ALERT_RULES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ALERT_RULES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ADOBE_SIGNATURE_CONFIG",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ADOBE_SIGNATURE_CONFIG",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ADOBE_SIGNATURE_CONFIG",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ADOBE_SIGNATURE_CONFIG",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SMTP_CONFIGS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SMTP_CONFIGS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SMTP_CONFIGS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SAP_OBJECT_MAPPINGS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SAP_OBJECT_MAPPINGS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SAP_OBJECT_MAPPINGS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SAP_FIELD_MAPPINGS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SAP_FIELD_MAPPINGS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SAP_FIELD_MAPPINGS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PROJECTS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PROJECTS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "PROJECTS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OUTLOOK_APPROVAL_CONFIG");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OUTLOOK_APPROVAL_CONFIG");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OUTLOOK_APPROVAL_CONFIG");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "OUTLOOK_APPROVAL_CONFIG");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORMULAS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORMULAS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORMULAS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORMULA_VARIABLES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORMULA_VARIABLES");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FORMULA_VARIABLES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORMULA_VARIABLES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_VALIDATION_RULES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_VALIDATION_RULES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_VALIDATION_RULES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_SUBMISSIONS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_VALUES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_VALUES");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FORM_SUBMISSION_VALUES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_VALUES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_GRID_ROWS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_GRID_ROWS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_GRID_ROWS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_GRID_CELLS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_GRID_CELLS");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FORM_SUBMISSION_GRID_CELLS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_GRID_CELLS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_SUBMISSION_ATTACHMENTS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_SUBMISSION_ATTACHMENTS");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FORM_SUBMISSION_ATTACHMENTS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_SUBMISSION_ATTACHMENTS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_GRIDS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_GRIDS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_GRIDS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_GRID_COLUMNS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_GRID_COLUMNS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_GRID_COLUMNS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_BUTTONS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_BUTTONS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_BUTTONS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FORM_ATTACHMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FORM_ATTACHMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FORM_ATTACHMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "EMAIL_TEMPLATES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "EMAIL_TEMPLATES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "EMAIL_TEMPLATES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "DOCUMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "DOCUMENT_SERIES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DOCUMENT_SERIES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "DOCUMENT_SERIES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "DOCUMENT_APPROVAL_HISTORY");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CRYSTAL_LAYOUTS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CRYSTAL_LAYOUTS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "CRYSTAL_LAYOUTS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ATTACHMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ATTACHMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ATTACHMENT_TYPES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "APPROVAL_WORKFLOWS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "APPROVAL_WORKFLOWS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "APPROVAL_WORKFLOWS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "APPROVAL_STAGES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "APPROVAL_STAGES");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "APPROVAL_STAGES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "APPROVAL_STAGES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "APPROVAL_STAGE_ASSIGNEES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "APPROVAL_STAGE_ASSIGNEES");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "APPROVAL_STAGE_ASSIGNEES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "APPROVAL_STAGE_ASSIGNEES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "APPROVAL_DELEGATIONS");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ALERT_RULES");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ALERT_RULES");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ALERT_RULES");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ADOBE_SIGNATURE_CONFIG");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ADOBE_SIGNATURE_CONFIG");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ADOBE_SIGNATURE_CONFIG");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ADOBE_SIGNATURE_CONFIG");

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
                table: "FORM_GRIDS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_GRID_COLUMNS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_BUTTONS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FORM_ATTACHMENT_TYPES",
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
        }
    }
}
