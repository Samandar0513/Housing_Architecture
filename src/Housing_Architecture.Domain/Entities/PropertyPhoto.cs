namespace Housing_Architecture.Domain.Entities;

public class PropertyPhoto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Property Property { get; set; } = null!;
}
