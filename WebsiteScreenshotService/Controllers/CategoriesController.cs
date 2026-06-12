using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Model;
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

    [HttpPost]
    [ActionName("updateCategory")]
    public async Task<IActionResult> ChangeCategory([FromBody] CategoryUpdateModel model)
    {
        var category = await _categoryManager.UpdateAsync(model);

        if (!category.IsSuccess)
            return BadRequest(new ErrorResponse(category.ErrorMessage!));

        return Ok(category.Value);
    }
}