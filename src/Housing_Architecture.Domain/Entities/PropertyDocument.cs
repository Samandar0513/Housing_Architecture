using Housing_Architecture.Domain.Enums;

namespace Housing_Architecture.Domain.Entities;

public class PropertyDocument
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Property Property { get; set; } = null!;
}
