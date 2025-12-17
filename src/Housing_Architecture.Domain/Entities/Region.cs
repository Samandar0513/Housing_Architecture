namespace Housing_Architecture.Domain.Entities;

public class Region
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation
    public ICollection<District> Districts { get; set; } = new List<District>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
