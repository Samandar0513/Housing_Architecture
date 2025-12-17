using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AmenityController : ControllerBase
{
    private readonly IAmenityService _amenityService;

    public AmenityController(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateAmenity([FromQuery] string amenityName)
    {
        var result = _amenityService.CreateAmenity(amenityName);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetAmenityById), new { amenityId = result.Data?.Id }, result);
    }

    [HttpGet("{amenityId}")]
    public IActionResult GetAmenityById(int amenityId)
    {
        var result = _amenityService.GetAmenityById(amenityId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAllAmenities()
    {
        var result = _amenityService.GetAllAmenities();
        return Ok(result);
    }

    [HttpPut("{amenityId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateAmenity(int amenityId, [FromQuery] string newAmenityName)
    {
        var result = _amenityService.UpdateAmenity(amenityId, newAmenityName);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{amenityId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteAmenity(int amenityId)
    {
        var result = _amenityService.DeleteAmenity(amenityId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
