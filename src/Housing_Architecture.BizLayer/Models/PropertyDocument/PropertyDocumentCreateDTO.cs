using Microsoft.AspNetCore.Http;

namespace Housing_Architecture.BizLayer.Models.PropertyDocument;

public class PropertyDocumentCreateDTO
{
    public int PropertyId { get; set; }
    public IFormFile File { get; set; } = null!;
}
