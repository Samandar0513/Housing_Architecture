using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Filters;
using Housing_Architecture.BizLayer.Models.Property;
using Housing_Architecture.BizLayer.Services;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly SearchPropertiesService _searchProperties;

    public PropertyController(IPropertyService propertyService, SearchPropertiesService searchProperties)
    {
        _propertyService = propertyService;
        _searchProperties = searchProperties;
    }

    [HttpGet("search")]
    public IActionResult SearchProperties([FromQuery] PropertyFilterDTO filter)
    {
        var result = _searchProperties.SearchProperties(filter);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public IActionResult CreateProperty( [FromBody] PropertyCreateDTO propertyCreateDTO)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = _propertyService.CreateProperty(userId, propertyCreateDTO);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetPropertyById), new { propertyId = result.Data?.Id }, result);
    }

    [HttpGet("{propertyId}")]
    public IActionResult GetPropertyById(int propertyId)
    {
        var result = _propertyService.GetPropertyById(propertyId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAllProperties()
    {
        var result = _propertyService.GetAllProperties();
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public IActionResult GetAllPropertiesByUserId()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = _propertyService.GetAllPropertiesByUserId(userId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet("admin||moderator/{userId}")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult GetAllPropertiesByUserIdForAdmin(int userId)
    {
        var result = _propertyService.GetAllPropertiesByUserId(userId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPut("{propertyId}")]
    [Authorize]
    public IActionResult UpdateProperty(int propertyId, [FromBody] PropertyUpdateDTO propertyUpdateDTO)
    {
        int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = _propertyService.UpdateProperty(propertyId, propertyUpdateDTO, currentUserId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{propertyId}")]
    [Authorize]
    public IActionResult DeleteProperty(int propertyId)
    {
        var result = _propertyService.DeleteProperty(propertyId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
