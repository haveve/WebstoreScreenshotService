using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Repositories.CategoryRepository;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;

namespace WebsiteScreenshotService.Controllers;

[Authorize(Roles = UserRoles.User, Policy = Policies.User.ManageCategories)]
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
        return Ok(categories);
    }

    [HttpPost]
    [ActionName("createCategory")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateModel model)
    {
        var category = await _categoryManager.AddAsync(model);
        return Ok(category);
    }

    [HttpDelete]
    [ActionName("deleteCategory")]
    public async Task<IActionResult> DeleteCategory([FromQuery] Guid id)
    {
        var category = await _categoryManager.RemoveAsync(id);
        return Ok(category);
    }
}