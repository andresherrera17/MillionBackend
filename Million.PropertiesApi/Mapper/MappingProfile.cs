using AutoMapper;
using Million.PropertiesApi.Core.Dtos;
using Million.PropertiesApi.Core.Models;

namespace Million.PropertiesApi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Owner, OwnerDto>().ReverseMap();

            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<PropertyImage, PropertyImageDto>().ReverseMap();

            CreateMap<PropertyTrace, PropertyTraceDto>().ReverseMap();

            CreateMap<Property, PropertyDetailsDto>()
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Traces, opt => opt.Ignore());
        }
    }
}
