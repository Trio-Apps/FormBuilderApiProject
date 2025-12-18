using AutoMapper;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domian.Entitys.FromBuilder;

namespace FormBuilder.Services.Mappings
{
    public class FormSubmissionAttachmentsProfile : Profile
    {
        public FormSubmissionAttachmentsProfile()
        {
            CreateMap<FORM_SUBMISSION_ATTACHMENTS, FormSubmissionAttachmentDto>()
                .ForMember(dest => dest.SubmissionDocumentNumber, opt => opt.MapFrom(src => src.FORM_SUBMISSIONS != null ? src.FORM_SUBMISSIONS.DocumentNumber : null))
                .ForMember(dest => dest.FieldCode, opt => opt.MapFrom(src => src.FORM_FIELDS != null ? src.FORM_FIELDS.FieldCode : null))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.FORM_FIELDS != null ? src.FORM_FIELDS.FieldName : null))
                .ForMember(dest => dest.FileSizeFormatted, opt => opt.Ignore()) // Will be formatted manually
                .ForMember(dest => dest.DownloadUrl, opt => opt.Ignore()); // Will be set manually

            CreateMap<CreateFormSubmissionAttachmentDto, FORM_SUBMISSION_ATTACHMENTS>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.FORM_SUBMISSIONS, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_FIELDS, opt => opt.Ignore());

            CreateMap<UpdateFormSubmissionAttachmentDto, FORM_SUBMISSION_ATTACHMENTS>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SubmissionId, opt => opt.Ignore())
                .ForMember(dest => dest.FieldId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedDate, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_SUBMISSIONS, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_FIELDS, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
