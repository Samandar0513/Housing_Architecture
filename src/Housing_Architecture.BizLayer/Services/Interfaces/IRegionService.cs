using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Region;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IRegionService
{
    ResponseModel<RegionDTO> CreateRegion(string regionName);
    ResponseModel<RegionDTO> GetRegionById(int regionId);
    ResponseModel<IEnumerable<RegionDTO>> GetAllRegions();
    ResponseModel<RegionDTO> UpdateRegion(int regionId, string newRegionName);
    ResponseModel<bool> DeleteRegion(int regionId);
}
