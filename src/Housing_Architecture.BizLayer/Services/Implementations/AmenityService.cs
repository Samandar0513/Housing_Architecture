using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Amenity;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class AmenityService : IAmenityService
{
    private readonly AppDbContext _db;

    public AmenityService(AppDbContext db)
    {
        _db = db;
    }

    public ResponseModel<AmenityDTO> CreateAmenity(string amenityName)
    {
        if (string.IsNullOrWhiteSpace(amenityName))
        {
            return ResponseModel<AmenityDTO>.Fail("Xatolik", "Qulaylik nomi kiritilmadi!");
        }

        var amenity = new Amenity
        {
            Name = amenityName
        };

        _db.Amenities.Add(amenity);
        _db.SaveChanges();

        var amenityDTO = new AmenityDTO
        {
            Id = amenity.Id,
            Name = amenity.Name
        };

        return ResponseModel<AmenityDTO>.Ok(amenityDTO, "Qulaylik muvaffaqiyatli yaratildi.");
    }

    public ResponseModel<AmenityDTO> GetAmenityById(int amenityId)
    {
        var amenity = _db.Amenities.FirstOrDefault(a => a.Id == amenityId);
        if (amenity == null)
        {
            return ResponseModel<AmenityDTO>.Fail("Xatolik", "Qulaylik topilmadi!");
        }

        var amenityDTO = new AmenityDTO
        {
            Id = amenity.Id,
            Name = amenity.Name
        };

        return ResponseModel<AmenityDTO>.Ok(amenityDTO, "Qulaylik muvaffaqiyatli topildi.");
    }

    public ResponseModel<IEnumerable<AmenityDTO>> GetAllAmenities()
    {
        var amenities = _db.Amenities.ToList();
        var amenityDTOs = amenities.Select(amenity => new AmenityDTO
        {
            Id = amenity.Id,
            Name = amenity.Name
        });

        return ResponseModel<IEnumerable<AmenityDTO>>.Ok(amenityDTOs, "Barcha qulayliklar muvaffaqiyatli olindi.");
    }

    public ResponseModel<AmenityDTO> UpdateAmenity(int amenityId, string newAmenityName)
    {
        var amenity = _db.Amenities.FirstOrDefault(a => a.Id == amenityId);

        if (amenity == null)
        {
            return ResponseModel<AmenityDTO>.Fail("Xatolik", "Qulaylik topilmadi!");
        }

        if (string.IsNullOrWhiteSpace(newAmenityName))
        {
            return ResponseModel<AmenityDTO>.Fail("Xatolik", "Yangi qulaylik nomi kiritilmadi!");
        }

        amenity.Name = newAmenityName;
        _db.Update(amenity);
        _db.SaveChanges();

        var amenityDTO = new AmenityDTO
        {
            Id = amenity.Id,
            Name = amenity.Name
        };

        return ResponseModel<AmenityDTO>.Ok(amenityDTO, "Qulaylik muvaffaqiyatli yangilandi.");
    }

    public ResponseModel<bool> DeleteAmenity(int amenityId)
    {
        var amenity = _db.Amenities.FirstOrDefault(a => a.Id == amenityId);
        if (amenity == null)
        {
            return ResponseModel<bool>.Fail("Xatolik", "Qulaylik topilmadi!");
        }

        _db.Amenities.Remove(amenity);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Qulaylik muvaffaqiyatli o'chirildi.");
    }
}
