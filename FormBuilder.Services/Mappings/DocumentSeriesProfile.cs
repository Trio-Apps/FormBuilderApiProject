using AutoMapper;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domian.Entitys.FromBuilder;

namespace FormBuilder.Services.Mappings
{
    public class DocumentSeriesProfile : Profile
    {
        public DocumentSeriesProfile()
        {
            CreateMap<DOCUMENT_SERIES, DocumentSeriesDto>()
                .ForMember(dest => dest.DocumentTypeName, opt => opt.MapFrom(src => src.DOCUMENT_TYPES != null ? src.DOCUMENT_TYPES.Name : null))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.PROJECTS != null ? src.PROJECTS.Name : null));

            CreateMap<CreateDocumentSeriesDto, DOCUMENT_SERIES>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.DOCUMENT_TYPES, opt => opt.Ignore())
                .ForMember(dest => dest.PROJECTS, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_SUBMISSIONS, opt => opt.Ignore());

            CreateMap<UpdateDocumentSeriesDto, DOCUMENT_SERIES>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.DOCUMENT_TYPES, opt => opt.Ignore())
                .ForMember(dest => dest.PROJECTS, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_SUBMISSIONS, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
