using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyDocumentController : ControllerBase
{
    private readonly IPropertyDocumentService _documentService;

    public PropertyDocumentController(IPropertyDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("{propertyId}")]
    [Authorize]
    public async Task<IActionResult> UploadDocument(int propertyId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { IsSuccess = false, Message = "Fayl tanlanmagan" });

        var result = await _documentService.UploadDocumentAsync(propertyId, file);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{documentId}")]
    [Authorize]
    public IActionResult GetDocumentById(int documentId)
    {
        var result = _documentService.GetDocumentById(documentId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet("property/{propertyId}")]
    [Authorize]
    public IActionResult GetDocumentsByPropertyId(int propertyId)
    {
        var result = _documentService.GetDocumentsByPropertyId(propertyId);
        return Ok(result);
    }

    [HttpPut("{documentId}/status")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult UpdateDocumentStatus(int documentId, [FromQuery] string status, [FromQuery] string? rejectionReason = null)
    {
        var result = _documentService.UpdateDocumentStatus(documentId, status, rejectionReason);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    // Moderator uchun - tekshiruv kutayotgan hujjatlar
    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult GetPendingDocuments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = _documentService.GetPendingDocuments(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpDelete("{documentId}")]
    [Authorize]
    public IActionResult DeleteDocument(int documentId)
    {
        var result = _documentService.DeleteDocument(documentId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
