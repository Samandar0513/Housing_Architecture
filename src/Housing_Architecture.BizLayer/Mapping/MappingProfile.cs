using AutoMapper;
using Housing_Architecture.BizLayer.Models.Property;
using Housing_Architecture.BizLayer.Models.PropertyDocument;
using Housing_Architecture.BizLayer.Models.User;
using Housing_Architecture.Domain.Entities;

namespace Housing_Architecture.BizLayer.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();

        CreateMap<Property, PropertyDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.District != null ? src.District.Name : null))
            .ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.District != null && src.District.Region != null ? src.District.Region.Name : null))
            .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType.ToString()))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency.ToString()))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos != null ? src.Photos.Select(x => x.FilePath).ToList() : new List<string>()))
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.PropertyAmenities != null ? src.PropertyAmenities.Where(pa => pa.Amenity != null).Select(pa => pa.Amenity.Name).ToList() : new List<string>()))
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents != null ? src.Documents : new List<PropertyDocument>()));

        CreateMap<PropertyDocument, PropertyDocumentDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
