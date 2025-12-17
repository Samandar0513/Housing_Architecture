namespace Housing_Architecture.BizLayer.Models.PropertyDocument;

public class PropertyDocumentDTO
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
