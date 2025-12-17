using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegionController : ControllerBase
{
    private readonly IRegionService _regionService;

    public RegionController(IRegionService regionService)
    {
        _regionService = regionService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateRegion([FromQuery] string regionName)
    {
        var result = _regionService.CreateRegion(regionName);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetRegionById), new { regionId = result.Data?.Id }, result);
    }

    [HttpGet("{regionId}")]
    public IActionResult GetRegionById(int regionId)
    {
        var result = _regionService.GetRegionById(regionId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAllRegions()
    {
        var result = _regionService.GetAllRegions();
        return Ok(result);
    }

    [HttpPut("{regionId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateRegion(int regionId, [FromQuery] string newRegionName)
    {
        var result = _regionService.UpdateRegion(regionId, newRegionName);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{regionId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteRegion(int regionId)
    {
        var result = _regionService.DeleteRegion(regionId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
