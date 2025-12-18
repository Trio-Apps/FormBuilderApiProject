using formBuilder.Domian.Interfaces;
using AutoMapper;
using FluentValidation;
using FormBuilder.Application.Abstractions;
using FormBuilder.core.Repository;
using FormBuilder.Core.IServices;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Core.Models;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Interfaces;
using FormBuilder.Infrastructure.Repositories;
using FormBuilder.Infrastructure.Repository;
using FormBuilder.Services;
using FormBuilder.Services.Mappings;
using FormBuilder.Services.Repository;
using FormBuilder.Services.Services;
using FormBuilder.Services.Services.FileStorage;
using FormBuilder.Services.Validators.FormBuilder;
using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFormBuilderServices(this IServiceCollection services)
        {
            // AutoMapper profiles
            services.AddAutoMapper(typeof(FormBuilderProfile).Assembly);

            // Accounts
            services.AddScoped<IaccountService, accountService>();
            services.AddScoped<IunitOfwork, UnitOfWork>();

            // Form Builder
            services.AddScoped<IFormBuilderService, FormBuilderService>();
            services.AddScoped<IFormBuilderRepository, FormBuilderRepository>();

            // Tabs
            services.AddScoped<IFormTabService, FormTabService>();
            services.AddScoped<IFormTabRepository, FormTabRepository>();

            // Fields
            services.AddScoped<IFormFieldService, FormFieldService>();
            services.AddScoped<IFormFieldRepository, FormFieldRepository>();

            // Field Types
            services.AddScoped<IFieldTypesService, FieldTypesService>();
            services.AddScoped<IFieldTypesRepository, FieldTypesRepository>();

            // Rules
            services.AddScoped<IFORM_RULESService, FORM_RULESService>();
            services.AddScoped<IFORM_RULESRepository, FORM_RULESRepository>();

            // Options
            services.AddScoped<IFieldOptionsService, FieldOptionsService>();
            services.AddScoped<IFieldOptionsRepository, FieldOptionsRepository>();

            // Data Sources
            services.AddScoped<IFieldDataSourcesService, FieldDataSourcesService>();
            services.AddScoped<IFieldDataSourcesRepository, FieldDataSourcesRepository>();

            // Submissions
            services.AddScoped<IFormSubmissionsService, FormSubmissionsService>();
            services.AddScoped<IFormSubmissionsRepository, FormSubmissionsRepository>();

            // Attachments
            services.AddScoped<IAttachmentTypeService, AttachmentTypeService>();
            services.AddScoped<IAttachmentTypeRepository, AttachmentTypeRepository>();

            services.AddScoped<IFormAttachmentTypeService, FormAttachmentTypeService>();
            services.AddScoped<IFormAttachmentTypeRepository, FormAttachmentTypeRepository>();

            // Documents
            services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();

            services.AddScoped<IDocumentSeriesService, DocumentSeriesService>();
            services.AddScoped<IDocumentSeriesRepository, DocumentSeriesRepository>();

            // Projects
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectRepository, ProjectRepository>();

            // Submission Values
            services.AddScoped<IFormSubmissionValuesService, FormSubmissionValuesService>();
            services.AddScoped<IFormSubmissionValuesRepository, FormSubmissionValuesRepository>();

            // Submission Attachments
            services.AddScoped<IFormSubmissionAttachmentsService, FormSubmissionAttachmentsService>();
            services.AddScoped<IFormSubmissionAttachmentsRepository, FormSubmissionAttachmentsRepository>();

            // Grid
            services.AddScoped<IFormGridService, FormGridService>();
            services.AddScoped<IFormGridRepository, FormGridRepository>();

            services.AddScoped<IFormGridColumnService, FormGridColumnService>();
            services.AddScoped<IFormGridColumnRepository, FormGridColumnRepository>();

            services.AddScoped<IFormSubmissionGridRowService, FormSubmissionGridRowService>();
            services.AddScoped<IFormSubmissionGridRowRepository, FormSubmissionGridRowRepository>();

            services.AddScoped<IFormSubmissionGridCellService, FormSubmissionGridCellService>();
            services.AddScoped<IFormSubmissionGridCellRepository, FormSubmissionGridCellRepository>();

            // Formula
            services.AddScoped<IFormulaService, FormulaService>();
            services.AddScoped<IFormulasRepository, FormulasRepository>();
            services.AddScoped<IFormulaVariableService, FormulaVariableService>();

            // Roles
            services.AddScoped<IRoleService, RoleService>();

            // permissions
           services.AddScoped<IUserPermissionService, UserPermissionService>();
            // Approval Workflow
            services.AddScoped<IApprovalWorkflowService, ApprovalWorkflowService>();
            services.AddScoped<IApprovalWorkflowRepository, ApprovalWorkflowRepository>();
            services.AddScoped<IApprovalStageRepository, ApprovalStageRepository>();
             services.AddScoped<IApprovalStageService, ApprovalStageService>();

            // File Storage
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}
