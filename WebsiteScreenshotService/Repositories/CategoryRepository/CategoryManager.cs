using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public class CategoryManager(ICategoryRepository categoryRepository, IUserContextAccessor userContextAccessor) : ICategoryManager
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;

    public async Task<Category> AddAsync(CategoryCreateModel category, Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        return await _categoryRepository.AddAsync(category, userId);
    }

    public async Task RemoveAsync(Guid categoryId)
        => await _categoryRepository.RemoveAsync(categoryId);

    public async Task<Category?> UpdateAsync(CategoryUpdateModel model)
        => await _categoryRepository.UpdateAsync(model);

    public async ValueTask<CategoryEntity?> GetByIdAsync(Guid categoryId)
        => await _categoryRepository.GetByIdAsync(categoryId);

    public async ValueTask<List<Category>> GetAllAsync(Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        return await _categoryRepository.GetAllAsync(userId);
    }
}