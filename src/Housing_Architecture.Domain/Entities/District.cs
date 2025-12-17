namespace Housing_Architecture.Domain.Entities;

public class District
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RegionId { get; set; }

    // Navigation
    public Region Region { get; set; } = null!;
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
