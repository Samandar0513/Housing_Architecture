using Housing_Architecture.Domain.Enums;

namespace Housing_Architecture.Domain.Entities;

public class Property
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int DistrictId { get; set; }
    public int RegionId { get; set; }
    public PropertyType PropertyType { get; set; } = PropertyType.Sale;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.USD;
    public decimal? TotalArea { get; set; }
    public int? Rooms { get; set; }
    public int? Floor { get; set; }
    public int? BuiltYear { get; set; }
    public int ViewsCount { get; set; } = 0;
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public District District { get; set; } = null!;
    public Region Region { get; set; } = null!;
    public ICollection<PropertyDocument> Documents { get; set; } = new List<PropertyDocument>();
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    public ICollection<PropertyPhoto> Photos { get; set; } = new List<PropertyPhoto>();
}
