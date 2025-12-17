namespace Housing_Architecture.Domain.Entities;

public class PropertyAmenity
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int AmenityId { get; set; }

    // Navigation
    public Property Property { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}
