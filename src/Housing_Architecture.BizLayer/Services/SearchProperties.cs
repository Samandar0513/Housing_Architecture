using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Filters;
using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.Property;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Housing_Architecture.BizLayer.Services;

public class SearchPropertiesService
{
    private readonly AppDbContext _db;

    public SearchPropertiesService(AppDbContext db)
    {
        _db = db;
    }

    public ResponseModel<PagedResult<PropertyDTO>> SearchProperties(PropertyFilterDTO filter)
    {
        var query = _db.Properties
            .Include(p => p.Category)
            .Include(p => p.District).ThenInclude(d => d.Region)
            .Include(p => p.Photos)
            .Include(p => p.PropertyAmenities).ThenInclude(pa => pa.Amenity)
            .AsQueryable();

        // Category filter
        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId);

        // Location filters
        if (filter.RegionId.HasValue)
            query = query.Where(p => p.RegionId == filter.RegionId);

        if (filter.DistrictId.HasValue)
            query = query.Where(p => p.DistrictId == filter.DistrictId);

        // Property type
        if (filter.PropertyType.HasValue)
            query = query.Where(p => p.PropertyType == filter.PropertyType);

        // Price range with currency
        if (filter.MinPrice.HasValue)
        {
            if (filter.Currency.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice && p.Currency == filter.Currency);
            else
                query = query.Where(p => p.Price >= filter.MinPrice);
        }

        if (filter.MaxPrice.HasValue)
        {
            if (filter.Currency.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice && p.Currency == filter.Currency);
            else
                query = query.Where(p => p.Price <= filter.MaxPrice);
        }

        // Rooms
        if (filter.MinRooms.HasValue)
            query = query.Where(p => p.Rooms >= filter.MinRooms);

        if (filter.MaxRooms.HasValue)
            query = query.Where(p => p.Rooms <= filter.MaxRooms);

        // Area
        if (filter.MinArea.HasValue)
            query = query.Where(p => p.TotalArea >= filter.MinArea);

        if (filter.MaxArea.HasValue)
            query = query.Where(p => p.TotalArea <= filter.MaxArea);

        // Floor
        if (filter.MinFloor.HasValue)
            query = query.Where(p => p.Floor >= filter.MinFloor);

        if (filter.MaxFloor.HasValue)
            query = query.Where(p => p.Floor <= filter.MaxFloor);

        // Built year
        if (filter.MinBuiltYear.HasValue)
            query = query.Where(p => p.BuiltYear >= filter.MinBuiltYear);

        if (filter.MaxBuiltYear.HasValue)
            query = query.Where(p => p.BuiltYear <= filter.MaxBuiltYear);

        // New building
        if (filter.IsNewBuilding.HasValue)
        {
            var threshold = DateTime.Now.Year - 2;
            if (filter.IsNewBuilding.Value)
                query = query.Where(p => p.BuiltYear >= threshold);
            else
                query = query.Where(p => p.BuiltYear < threshold);
        }

        // Amenities
        if (filter.AmenityIds != null && filter.AmenityIds.Any())
        {
            query = query.Where(p => p.PropertyAmenities
                .Any(pa => filter.AmenityIds.Contains(pa.AmenityId)));
        }

        // Sorting
        query = filter.SortBy?.ToLower() switch
        {
            "price" => filter.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(p => p.Price)
                : query.OrderByDescending(p => p.Price),
            "area" => filter.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(p => p.TotalArea)
                : query.OrderByDescending(p => p.TotalArea),
            "views" => filter.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(p => p.ViewsCount)
                : query.OrderByDescending(p => p.ViewsCount),
            "createdat" => filter.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = query.Count();

        var properties = query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var propertyDTOs = properties.Select(p => new PropertyDTO
        {
            Id = p.Id,
            UserId = p.UserId,
            CategoryName = p.Category?.Name,
            DistrictName = p.District?.Name,
            RegionName = p.District?.Region?.Name,
            PropertyType = p.PropertyType.ToString(),
            Description = p.Description,
            Price = p.Price,
            Currency = p.Currency.ToString(),
            TotalArea = p.TotalArea,
            Rooms = p.Rooms,
            Floor = p.Floor,
            BuiltYear = p.BuiltYear,
            ViewsCount = p.ViewsCount,
            ContactName = p.ContactName,
            ContactPhone = p.ContactPhone,
            CreatedAt = p.CreatedAt,
            Photos = p.Photos?.Select(x => x.FilePath).ToList() ?? new List<string>(),
            Amenities = p.PropertyAmenities?
                .Where(pa => pa.Amenity != null)
                .Select(pa => pa.Amenity.Name)
                .ToList() ?? new List<string>()
        }).ToList();

        var pagedResult = PagedResult<PropertyDTO>.Create(
            propertyDTOs,
            totalCount,
            filter.PageNumber,
            filter.PageSize);

        return ResponseModel<PagedResult<PropertyDTO>>.Ok(pagedResult, "Mulklar topildi.");
    }
}
