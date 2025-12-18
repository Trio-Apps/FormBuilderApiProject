using AutoMapper;
using FormBuilder.API.DTOs;
using FormBuilder.Domian.Entitys.FormBuilder;

namespace FormBuilder.Services.Mappings
{
    public class FormGridColumnProfile : Profile
    {
        public FormGridColumnProfile()
        {
            CreateMap<FORM_GRID_COLUMNS, FormGridColumnDto>()
                .ForMember(dest => dest.GridName, opt => opt.MapFrom(src => src.FORM_GRIDS != null ? src.FORM_GRIDS.GridName : null))
                .ForMember(dest => dest.FormBuilderName, opt => opt.MapFrom(src => src.FORM_GRIDS != null && src.FORM_GRIDS.FORM_BUILDER != null ? src.FORM_GRIDS.FORM_BUILDER.FormName : null))
                .ForMember(dest => dest.FieldTypeName, opt => opt.MapFrom(src => src.FIELD_TYPES != null ? src.FIELD_TYPES.TypeName : null));

            CreateMap<CreateFormGridColumnDto, FORM_GRID_COLUMNS>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ColumnOrder, opt => opt.MapFrom(src => src.ColumnOrder ?? 0))
                .ForMember(dest => dest.FORM_GRIDS, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_TYPES, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_SUBMISSION_GRID_CELLS, opt => opt.Ignore());

            CreateMap<UpdateFormGridColumnDto, FORM_GRID_COLUMNS>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_GRIDS, opt => opt.Ignore())
                .ForMember(dest => dest.FIELD_TYPES, opt => opt.Ignore())
                .ForMember(dest => dest.FORM_SUBMISSION_GRID_CELLS, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
