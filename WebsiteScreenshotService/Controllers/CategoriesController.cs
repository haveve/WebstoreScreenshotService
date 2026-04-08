using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Repositories.CategoryRepository;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;

namespace WebsiteScreenshotService.Controllers;

[Authorize]
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
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateModel model)
    {
        var category = await _categoryManager.AddAsync(model);
        return Ok(category);
    }
}