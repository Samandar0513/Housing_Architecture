using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyPhotoController : ControllerBase
{
    private readonly IPropertyPhotoService _photoService;

    public PropertyPhotoController(IPropertyPhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpPost("{propertyId}")]
    [Authorize]
    public async Task<IActionResult> UploadPhoto(int propertyId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { IsSuccess = false, Message = "Fayl tanlanmagan" });

        var result = await _photoService.UploadPhotoAsync(propertyId, file);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{propertyId}")]
    public IActionResult GetPhotosByPropertyId(int propertyId)
    {
        var result = _photoService.GetPhotosByPropertyId(propertyId);
        return Ok(result);
    }

    [HttpDelete("{photoId}")]
    [Authorize]
    public IActionResult DeletePhoto(int photoId)
    {
        var result = _photoService.DeletePhoto(photoId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
