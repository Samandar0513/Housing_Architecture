using Housing_Architecture.Domain.Enums;

namespace Housing_Architecture.BizLayer.Filters;

public class PropertyFilterDTO
{
    public int? CategoryId { get; set; }
    public int? RegionId { get; set; }
    public int? DistrictId { get; set; }
    public PropertyType? PropertyType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public CurrencyType? Currency { get; set; }
    public int? MinRooms { get; set; }
    public int? MaxRooms { get; set; }
    public decimal? MinArea { get; set; }
    public decimal? MaxArea { get; set; }
    public int? MinFloor { get; set; }
    public int? MaxFloor { get; set; }
    public int? MinBuiltYear { get; set; }
    public int? MaxBuiltYear { get; set; }
    public bool? IsNewBuilding { get; set; }
    public List<int>? AmenityIds { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
