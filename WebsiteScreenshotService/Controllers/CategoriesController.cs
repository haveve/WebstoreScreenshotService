using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories.CategoryRepository;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;

namespace WebsiteScreenshotService.Controllers;

[Authorize(Policy = Policies.User.ManageCategories)]
[ApiController]
[Route("categories/[action]")]
public class CategoriesController(ICategoryManager categoryManager) : ControllerBase
{
    private readonly ICategoryManager _categoryManager = categoryManager;

    [HttpGet]
    [ActionName("getCategories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryManager.GetAllAsync();

        if (!categories.IsSuccess)
            return BadRequest(new ErrorResponse(categories.ErrorMessage!));

        return Ok(categories.Value);
    }

    [HttpPost]
    [ActionName("createCategory")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateModel model)
    {
        var category = await _categoryManager.AddAsync(model);

        if (!category.IsSuccess)
            return BadRequest(new ErrorResponse(category.ErrorMessage!));

        return Ok(category.Value);
    }

    [HttpDelete]
    [ActionName("deleteCategory")]
    public async Task<IActionResult> DeleteCategory([FromQuery] Guid id)
    {
        var category = await _categoryManager.RemoveAsync(id);
        
        if (!category.IsSuccess)
            return BadRequest(new ErrorResponse(category.ErrorMessage!));

        return Ok();
    }
}