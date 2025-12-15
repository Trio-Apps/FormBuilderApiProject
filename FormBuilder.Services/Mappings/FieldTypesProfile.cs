using AutoMapper;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.API.Models;

namespace FormBuilder.Services.Mappings
{
    public class FieldTypesProfile : Profile
    {
        public FieldTypesProfile()
        {
            CreateMap<FIELD_TYPES, FieldTypeDto>();
            CreateMap<FieldTypeCreateDto, FIELD_TYPES>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<FieldTypeUpdateDto, FIELD_TYPES>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<FIELD_TYPES, FieldTypeDropdownDto>();
        }
    }
}

