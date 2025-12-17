using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.District;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class DistrictService : IDistrictService
{
    private readonly AppDbContext _db;

    public DistrictService(AppDbContext db)
    {
        _db = db;
    }

    public ResponseModel<DistrictDTO> CreateDistrict(string districtName, int regionId)
    {
        if (string.IsNullOrWhiteSpace(districtName))
        {
            return ResponseModel<DistrictDTO>.Fail("Xatolik", "Tuman nomi kiritilmadi!");
        }

        var region = _db.Regions.FirstOrDefault(r => r.Id == regionId);
        if (region == null)
        {
            return ResponseModel<DistrictDTO>.Fail("Xatolik", "Viloyat topilmadi!");
        }

        var district = new District
        {
            Name = districtName,
            RegionId = regionId
        };

        _db.Districts.Add(district);
        _db.SaveChanges();

        var districtDTO = new DistrictDTO
        {
            Id = district.Id,
            Name = district.Name,
            RegionId = district.RegionId,
            RegionName = region.Name
        };

        return ResponseModel<DistrictDTO>.Ok(districtDTO, "Tuman muvaffaqiyatli yaratildi.");
    }

    public ResponseModel<DistrictDTO> GetDistrictById(int districtId)
    {
        var district = _db.Districts
            .Include(d => d.Region)
            .FirstOrDefault(d => d.Id == districtId);

        if (district == null)
        {
            return ResponseModel<DistrictDTO>.Fail("Xatolik", "Tuman topilmadi!");
        }

        var districtDTO = new DistrictDTO
        {
            Id = district.Id,
            Name = district.Name,
            RegionId = district.RegionId,
            RegionName = district.Region?.Name
        };

        return ResponseModel<DistrictDTO>.Ok(districtDTO, "Tuman muvaffaqiyatli topildi.");
    }

    public ResponseModel<IEnumerable<DistrictDTO>> GetAllDistricts()
    {
        var districts = _db.Districts
            .Include(d => d.Region)
            .ToList();

        var districtDTOs = districts.Select(district => new DistrictDTO
        {
            Id = district.Id,
            Name = district.Name,
            RegionId = district.RegionId,
            RegionName = district.Region?.Name
        });

        return ResponseModel<IEnumerable<DistrictDTO>>.Ok(districtDTOs, "Barcha tumanlar muvaffaqiyatli olindi.");
    }

    public ResponseModel<IEnumerable<DistrictDTO>> GetDistrictsByRegionId(int regionId)
    {
        var districts = _db.Districts
            .Include(d => d.Region)
            .Where(d => d.RegionId == regionId)
            .ToList();

        var districtDTOs = districts.Select(district => new DistrictDTO
        {
            Id = district.Id,
            Name = district.Name,
            RegionId = district.RegionId,
            RegionName = district.Region?.Name
        });

        return ResponseModel<IEnumerable<DistrictDTO>>.Ok(districtDTOs, "Viloyat tumanlari muvaffaqiyatli olindi.");
    }

    public ResponseModel<DistrictDTO> UpdateDistrict(int districtId, string newDistrictName, int? newRegionId)
    {
        var district = _db.Districts.FirstOrDefault(d => d.Id == districtId);
        if (district == null)
        {
            return ResponseModel<DistrictDTO>.Fail("Xatolik", "Tuman topilmadi!");
        }

        if (!string.IsNullOrWhiteSpace(newDistrictName))
        {
            district.Name = newDistrictName;
        }

        if (newRegionId.HasValue)
        {
            var region = _db.Regions.FirstOrDefault(r => r.Id == newRegionId);
            if (region == null)
            {
                return ResponseModel<DistrictDTO>.Fail("Xatolik", "Yangi viloyat topilmadi!");
            }
            district.RegionId = newRegionId.Value;
        }

        _db.Update(district);
        _db.SaveChanges();

        var updatedDistrict = _db.Districts
            .Include(d => d.Region)
            .FirstOrDefault(d => d.Id == districtId);

        var districtDTO = new DistrictDTO
        {
            Id = updatedDistrict!.Id,
            Name = updatedDistrict.Name,
            RegionId = updatedDistrict.RegionId,
            RegionName = updatedDistrict.Region?.Name
        };

        return ResponseModel<DistrictDTO>.Ok(districtDTO, "Tuman muvaffaqiyatli yangilandi.");
    }

    public ResponseModel<bool> DeleteDistrict(int districtId)
    {
        var district = _db.Districts.FirstOrDefault(d => d.Id == districtId);
        if (district == null)
        {
            return ResponseModel<bool>.Fail("Xatolik", "Tuman topilmadi!");
        }

        _db.Districts.Remove(district);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Tuman muvaffaqiyatli o'chirildi.");
    }
}
