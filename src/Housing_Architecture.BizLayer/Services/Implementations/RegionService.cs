using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Region;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class RegionService : IRegionService
{
    private readonly AppDbContext _db;

    public RegionService(AppDbContext db)
    {
        _db = db;
    }

    public ResponseModel<RegionDTO> CreateRegion(string regionName)
    {
        if (string.IsNullOrWhiteSpace(regionName))
        {
            return ResponseModel<RegionDTO>.Fail("Xatolik", "Viloyat nomi kiritilmadi!");
        }

        var region = new Region
        {
            Name = regionName
        };

        _db.Regions.Add(region);
        _db.SaveChanges();

        var regionDTO = new RegionDTO
        {
            Id = region.Id,
            Name = region.Name
        };

        return ResponseModel<RegionDTO>.Ok(regionDTO, "Viloyat muvaffaqiyatli yaratildi.");
    }

    public ResponseModel<RegionDTO> GetRegionById(int regionId)
    {
        var region = _db.Regions.FirstOrDefault(r => r.Id == regionId);
        if (region == null)
        {
            return ResponseModel<RegionDTO>.Fail("Xatolik", "Viloyat topilmadi!");
        }

        var regionDTO = new RegionDTO
        {
            Id = region.Id,
            Name = region.Name
        };

        return ResponseModel<RegionDTO>.Ok(regionDTO, "Viloyat muvaffaqiyatli topildi.");
    }

    public ResponseModel<IEnumerable<RegionDTO>> GetAllRegions()
    {
        var regions = _db.Regions.ToList();
        var regionDTOs = regions.Select(region => new RegionDTO
        {
            Id = region.Id,
            Name = region.Name
        });

        return ResponseModel<IEnumerable<RegionDTO>>.Ok(regionDTOs, "Barcha viloyatlar muvaffaqiyatli olindi.");
    }

    public ResponseModel<RegionDTO> UpdateRegion(int regionId, string newRegionName)
    {
        var region = _db.Regions.FirstOrDefault(r => r.Id == regionId);
        if (region == null)
        {
            return ResponseModel<RegionDTO>.Fail("Xatolik", "Viloyat topilmadi!");
        }

        if (string.IsNullOrWhiteSpace(newRegionName))
        {
            return ResponseModel<RegionDTO>.Fail("Xatolik", "Yangi viloyat nomi kiritilmadi!");
        }

        region.Name = newRegionName;
        _db.Update(region);
        _db.SaveChanges();

        var regionDTO = new RegionDTO
        {
            Id = region.Id,
            Name = region.Name
        };

        return ResponseModel<RegionDTO>.Ok(regionDTO, "Viloyat muvaffaqiyatli yangilandi.");
    }

    public ResponseModel<bool> DeleteRegion(int regionId)
    {
        var region = _db.Regions.FirstOrDefault(r => r.Id == regionId);
        if (region == null)
        {
            return ResponseModel<bool>.Fail("Xatolik", "Viloyat topilmadi!");
        }

        _db.Regions.Remove(region);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Viloyat muvaffaqiyatli o'chirildi.");
    }
}
