using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Amenity;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IAmenityService
{
    ResponseModel<AmenityDTO> CreateAmenity(string amenityName);
    ResponseModel<AmenityDTO> GetAmenityById(int amenityId);
    ResponseModel<IEnumerable<AmenityDTO>> GetAllAmenities();
    ResponseModel<AmenityDTO> UpdateAmenity(int amenityId, string newAmenityName);
    ResponseModel<bool> DeleteAmenity(int amenityId);
}
