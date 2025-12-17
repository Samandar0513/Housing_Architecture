using Housing_Architecture.Domain.Enums;

namespace Housing_Architecture.BizLayer.Models.Property;

public class PropertyCreateDTO
{
    public int CategoryId { get; set; }
    public int DistrictId { get; set; }
    public int RegionId { get; set; }
    public PropertyType PropertyType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CurrencyType Currency { get; set; }
    public decimal? TotalArea { get; set; }
    public int? Rooms { get; set; }
    public int? Floor { get; set; }
    public int? BuiltYear { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public List<string>? Photos { get; set; }
    public List<int>? AmenityIds { get; set; }
    public bool IsActive { get; set; } = true;
}
