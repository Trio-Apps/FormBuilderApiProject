using AutoMapper;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormFields;
using CreateFormFieldDto = FormBuilder.Core.DTOS.FormFields.CreateFormFieldDto;
using UpdateFormFieldDto = FormBuilder.API.Models.UpdateFormFieldDto;

namespace FormBuilder.Services.Mappings
{
    public class FormFieldProfile : Profile
    {
        public FormFieldProfile()
        {
            CreateMap<FORM_FIELDS, FormFieldDto>()
                .ForMember(dest => dest.FieldTypeName, opt => opt.MapFrom(src => src.FIELD_TYPES != null ? src.FIELD_TYPES.TypeName : null))
                .ForMember(dest => dest.FieldOptions, opt => opt.MapFrom(src => src.FIELD_OPTIONS != null ? src.FIELD_OPTIONS.Where(fo => fo.IsActive) : new List<FormBuilder.Domian.Entitys.froms.FIELD_OPTIONS>()));

            CreateMap<CreateFormFieldDto, FORM_FIELDS>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_TABS, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_TYPES, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_OPTIONS, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_DATA_SOURCES, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<UpdateFormFieldDto, FORM_FIELDS>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TabId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_TABS, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_TYPES, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_OPTIONS, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_DATA_SOURCES, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
