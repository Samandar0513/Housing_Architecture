using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.District;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IDistrictService
{
    ResponseModel<DistrictDTO> CreateDistrict(string districtName, int regionId);
    ResponseModel<DistrictDTO> GetDistrictById(int districtId);
    ResponseModel<IEnumerable<DistrictDTO>> GetAllDistricts();
    ResponseModel<IEnumerable<DistrictDTO>> GetDistrictsByRegionId(int regionId);
    ResponseModel<DistrictDTO> UpdateDistrict(int districtId, string newDistrictName, int? newRegionId);
    ResponseModel<bool> DeleteDistrict(int districtId);
}
