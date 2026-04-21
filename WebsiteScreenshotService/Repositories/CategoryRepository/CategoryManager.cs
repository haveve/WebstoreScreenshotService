using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Services.Caching;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public class CategoryManager : ICategoryManager
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly ICacheManager _cache;

    public CategoryManager(
        ICategoryRepository categoryRepository,
        IUserContextAccessor userContextAccessor,
        ICacheManager cache)
    {
        _categoryRepository = categoryRepository;
        _userContextAccessor = userContextAccessor;
        _cache = cache;
    }

    public async Task<Category?> AddAsync(CategoryCreateModel category, Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var result = await _categoryRepository.AddAsync(category, userId);

        if(result is not null)
            await InvalidateCategoryCache(result.Id, userId);

        return result;
    }

    public async Task RemoveAsync(Guid categoryId, Guid userId = default)
    {
        await _categoryRepository.RemoveAsync(categoryId);
        await InvalidateCategoryCache(categoryId, userId);
    }

    public async Task<Category?> UpdateAsync(CategoryUpdateModel model, Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var result = await _categoryRepository.UpdateAsync(model);

        if (result is not null)
            await InvalidateCategoryCache(result.Id, userId);

        return result;
    }

    public async ValueTask<Category?> GetByIdAsync(Guid categoryId)
    {
        var key = CacheKeys.Category.ById(categoryId);

        return await _cache.GetOrSetAsync(
            key,
            () => _categoryRepository.GetByIdAsync(categoryId),
            new CacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
    }

    public async ValueTask<List<Category>> GetAllAsync(Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var key = CacheKeys.Category.List(userId);

        return await _cache.GetOrSetAsync(
            key,
            () => _categoryRepository.GetAllAsync(userId),
            new CacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
    }

    private async Task InvalidateCategoryCache(Guid categoryId, Guid userId)
    {
        await _cache.RemoveAsync(CacheKeys.Category.ById(categoryId));
        await _cache.RemoveAsync(CacheKeys.Category.List(userId));
    }
}