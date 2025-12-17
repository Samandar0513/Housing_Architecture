using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : ControllerBase
{
    private readonly IDistrictService _districtService;

    public DistrictController(IDistrictService districtService)
    {
        _districtService = districtService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateDistrict([FromQuery] string districtName, [FromQuery] int regionId)
    {
        var result = _districtService.CreateDistrict(districtName, regionId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetDistrictById), new { districtId = result.Data?.Id }, result);
    }

    [HttpGet("{districtId}")]
    public IActionResult GetDistrictById(int districtId)
    {
        var result = _districtService.GetDistrictById(districtId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAllDistricts()
    {
        var result = _districtService.GetAllDistricts();
        return Ok(result);
    }

    [HttpGet("by-region/{regionId}")]
    public IActionResult GetDistrictsByRegionId(int regionId)
    {
        var result = _districtService.GetDistrictsByRegionId(regionId);
        return Ok(result);
    }

    [HttpPut("{districtId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateDistrict(int districtId, [FromQuery] string newDistrictName, [FromQuery] int? newRegionId)
    {
        var result = _districtService.UpdateDistrict(districtId, newDistrictName, newRegionId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{districtId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteDistrict(int districtId)
    {
        var result = _districtService.DeleteDistrict(districtId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
