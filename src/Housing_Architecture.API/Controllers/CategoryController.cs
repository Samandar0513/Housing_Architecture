using Housing_Architecture.BizLayer.Models.Category;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housing_Architecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult CreateCategory([FromBody] CategoryCreateDTO categoryCreateDTO)
    {
        var result = _categoryService.CreateCategory(categoryCreateDTO);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetCategoryById), new { categoryId = result.Data?.Id }, result);
    }

    [HttpGet("{categoryId}")]
    public IActionResult GetCategoryById(int categoryId)
    {
        var result = _categoryService.GetCategoryById(categoryId);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAllCategories()
    {
        var result = _categoryService.GetAllCategories();
        return Ok(result);
    }

    [HttpPut("{categoryId}")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryCreateDTO categoryUpdateDTO)
    {
        var result = _categoryService.UpdateCategory(categoryId, categoryUpdateDTO);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{categoryId}")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult DeleteCategory(int categoryId)
    {
        var result = _categoryService.DeleteCategory(categoryId);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
