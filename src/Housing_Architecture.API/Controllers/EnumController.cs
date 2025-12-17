using Housing_Architecture.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnumController : ControllerBase
{
    [HttpGet("property-types")]
    public IActionResult GetPropertyTypes()
    {
        var types = Enum.GetValues<PropertyType>()
            .Select(t => new { Id = (int)t, Name = t.ToString() })
            .ToList();
        return Ok(types);
    }

    [HttpGet("currency-types")]
    public IActionResult GetCurrencyTypes()
    {
        var types = Enum.GetValues<CurrencyType>()
            .Select(t => new { Id = (int)t, Name = t.ToString() })
            .ToList();
        return Ok(types);
    }

    [HttpGet("document-statuses")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult GetDocumentStatuses()
    {
        var statuses = Enum.GetValues<DocumentStatus>()
            .Select(s => new { Id = (int)s, Name = s.ToString() })
            .ToList();
        return Ok(statuses);
    }

    [HttpGet("user-roles")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetUserRoles()
    {
        var roles = Enum.GetValues<UserRole>()
            .Select(r => new { Id = (int)r, Name = r.ToString() })
            .ToList();
        return Ok(roles);
    }
}
