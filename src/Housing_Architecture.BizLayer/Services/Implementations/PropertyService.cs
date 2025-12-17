using AutoMapper;
using FluentValidation;
using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Property;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;
using Housing_Architecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class PropertyService : IPropertyService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IValidator<PropertyCreateDTO> _validator;
    private readonly IFileStorageService _fileStorage;
    private const string PROPERTY_BUCKET = "property-images";

    public PropertyService(
        AppDbContext db,
        IMapper mapper,
        IValidator<PropertyCreateDTO> validator,
        IFileStorageService fileStorageService)
    {
        _db = db;
        _mapper = mapper;
        _validator = validator;
        _fileStorage = fileStorageService;
    }

    public ResponseModel<PropertyDTO> CreateProperty(int userId, PropertyCreateDTO propertyCreateDTO)
    {
        var validationResult = _validator.Validate(propertyCreateDTO);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ResponseModel<PropertyDTO>.Fail("Validation xatoligi", errors);
        }

        var property = new Property
        {
            UserId = userId,
            CategoryId = propertyCreateDTO.CategoryId,
            DistrictId = propertyCreateDTO.DistrictId,
            RegionId = propertyCreateDTO.RegionId,
            PropertyType = propertyCreateDTO.PropertyType,
            Description = propertyCreateDTO.Description,
            Price = propertyCreateDTO.Price,
            Currency = propertyCreateDTO.Currency,
            TotalArea = propertyCreateDTO.TotalArea,
            Rooms = propertyCreateDTO.Rooms,
            Floor = propertyCreateDTO.Floor,
            BuiltYear = propertyCreateDTO.BuiltYear,
            ContactName = propertyCreateDTO.ContactName,
            ContactPhone = propertyCreateDTO.ContactPhone,
            ViewsCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        _db.Properties.Add(property);
        _db.SaveChanges();

        if (propertyCreateDTO.Photos != null && propertyCreateDTO.Photos.Any())
        {
            var photos = propertyCreateDTO.Photos.Select(url => new PropertyPhoto
            {
                PropertyId = property.Id,
                FilePath = url,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _db.PropertyPhotos.AddRange(photos);
        }

        if (propertyCreateDTO.AmenityIds != null && propertyCreateDTO.AmenityIds.Any())
        {
            var amenities = propertyCreateDTO.AmenityIds.Select(aid => new PropertyAmenity
            {
                PropertyId = property.Id,
                AmenityId = aid
            }).ToList();

            _db.PropertyAmenities.AddRange(amenities);
        }

        _db.SaveChanges();

        var created = _db.Properties
            .Include(p => p.District).ThenInclude(d => d.Region)
            .Include(p => p.Category)
            .Include(p => p.Photos)
            .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
            .FirstOrDefault(p => p.Id == property.Id);

        var propertyDto = _mapper.Map<PropertyDTO>(created);
        return ResponseModel<PropertyDTO>.Ok(propertyDto, "Mulk muvaffaqiyatli yaratildi.");
    }

    public ResponseModel<PropertyDTO> GetPropertyById(int propertyId)
    {
        var property = _db.Properties
            .Include(p => p.District).ThenInclude(d => d.Region)
            .Include(p => p.Category)
            .Include(p => p.Photos)
            .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
            .FirstOrDefault(p => p.Id == propertyId);

        if (property == null)
        {
            return ResponseModel<PropertyDTO>.Fail("Xatolik", "Mulk topilmadi!");
        }

        property.ViewsCount += 1;
        _db.SaveChanges();

        var propertyDto = _mapper.Map<PropertyDTO>(property);
        return ResponseModel<PropertyDTO>.Ok(propertyDto, "Mulk muvaffaqiyatli topildi.");
    }

    public ResponseModel<IEnumerable<PropertyDTO>> GetAllProperties()
    {
        var properties = _db.Properties
        .Include(p => p.District).ThenInclude(d => d.Region)
        .Include(p => p.Category)
        .Include(p => p.Photos)
        .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
        .Include(p => p.Documents).Where(p => p.Documents.Any() && p.Documents.Any(d => d.Status == DocumentStatus.Approved))
        .Where(p=>p.IsActive) // faqat faol mulklar
        .ToList();

        var propertyDTOs = _mapper.Map<IEnumerable<PropertyDTO>>(properties);
        return ResponseModel<IEnumerable<PropertyDTO>>.Ok(propertyDTOs, "Barcha mulklar muvaffaqiyatli olindi.");
    }

    public ResponseModel<IEnumerable<PropertyDTO>> GetAllPropertiesByUserId(int userId)
    {
        var properties = _db.Properties
            .Where(p => p.UserId == userId)
            .Include(p => p.District).ThenInclude(d => d.Region)
            .Include(p => p.Category)
            .Include(p => p.Photos)
            .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
            .Include(p => p.Documents) // ✅ Documents ni include qilish
            .ToList();

        if (properties.Count == 0)
        {
            return ResponseModel<IEnumerable<PropertyDTO>>.Fail("Xatolik", "Bu foydalanuvchida mulklar topilmadi!");
        }

        var propertyDTOs = _mapper.Map<IEnumerable<PropertyDTO>>(properties);
        return ResponseModel<IEnumerable<PropertyDTO>>.Ok(propertyDTOs, "Foydalanuvchi mulklari muvaffaqiyatli olindi.");
    }

    public ResponseModel<PropertyDTO> UpdateProperty(int propertyId, PropertyCreateDTO propertyUpdateDTO)
    {
        var property = _db.Properties
            .Include(p => p.Photos)
            .Include(p => p.PropertyAmenities)
            .FirstOrDefault(p => p.Id == propertyId);

        if (property == null)
        {
            return ResponseModel<PropertyDTO>.Fail("Xatolik", "Mulk topilmadi!");
        }

        var validationResult = _validator.Validate(propertyUpdateDTO);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ResponseModel<PropertyDTO>.Fail("Validation xatoligi", errors);
        }

        property.CategoryId = propertyUpdateDTO.CategoryId;
        property.DistrictId = propertyUpdateDTO.DistrictId;
        property.RegionId = propertyUpdateDTO.RegionId;
        property.PropertyType = propertyUpdateDTO.PropertyType;
        property.Description = propertyUpdateDTO.Description;
        property.Price = propertyUpdateDTO.Price;
        property.Currency = propertyUpdateDTO.Currency;
        property.TotalArea = propertyUpdateDTO.TotalArea;
        property.Rooms = propertyUpdateDTO.Rooms;
        property.Floor = propertyUpdateDTO.Floor;
        property.BuiltYear = propertyUpdateDTO.BuiltYear;
        property.ContactName = propertyUpdateDTO.ContactName;
        property.ContactPhone = propertyUpdateDTO.ContactPhone;
        property.IsActive = propertyUpdateDTO.IsActive;

        var oldPhotos = _db.PropertyPhotos.Where(pp => pp.PropertyId == propertyId).ToList();
        if (oldPhotos.Any())
        {
            _db.PropertyPhotos.RemoveRange(oldPhotos);
        }

        if (propertyUpdateDTO.Photos != null && propertyUpdateDTO.Photos.Any())
        {
            var newPhotos = propertyUpdateDTO.Photos.Select(url => new PropertyPhoto
            {
                PropertyId = propertyId,
                FilePath = url,
                CreatedAt = DateTime.UtcNow
            }).ToList();
            _db.PropertyPhotos.AddRange(newPhotos);
        }

        var oldAmenities = _db.PropertyAmenities.Where(pa => pa.PropertyId == propertyId).ToList();
        if (oldAmenities.Any())
        {
            _db.PropertyAmenities.RemoveRange(oldAmenities);
        }

        if (propertyUpdateDTO.AmenityIds != null && propertyUpdateDTO.AmenityIds.Any())
        {
            var newAmenities = propertyUpdateDTO.AmenityIds.Select(aid => new PropertyAmenity
            {
                PropertyId = propertyId,
                AmenityId = aid
            }).ToList();
            _db.PropertyAmenities.AddRange(newAmenities);
        }

        _db.SaveChanges();

        var updated = _db.Properties
            .Include(p => p.District).ThenInclude(d => d.Region)
            .Include(p => p.Category)
            .Include(p => p.Photos)
            .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
            .FirstOrDefault(p => p.Id == propertyId);

        var dto = _mapper.Map<PropertyDTO>(updated);
        return ResponseModel<PropertyDTO>.Ok(dto, "Mulk muvaffaqiyatli yangilandi.");
    }

    public ResponseModel<bool> DeleteProperty(int propertyId)
    {
        var property = _db.Properties.FirstOrDefault(p => p.Id == propertyId);
        if (property == null)
        {
            return ResponseModel<bool>.Fail("Xatolik", "Mulk topilmadi!");
        }

        foreach (var photo in _db.PropertyPhotos.Where(pp => pp.PropertyId == propertyId))
        {
            try
            {
                var uri = new Uri(photo.FilePath);
                var objectName = uri.Segments[^1];
                _fileStorage.RemoveFileAsync(PROPERTY_BUCKET, objectName).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rasmni o'chirishda xatolik: {ex.Message}");
            }
        }

        _db.Properties.Remove(property);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Mulk muvaffaqiyatli o'chirildi.");
    }
}
