using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Services.Caching;
using WebsiteScreenshotService.Utils;

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

    public async Task<Result<Category>> AddAsync(CategoryCreateModel category, Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var categoryResult = await _categoryRepository.AddAsync(category, userId);

        if (categoryResult.IsSuccess)
        {
            var addedCategory = categoryResult.Value!;
            await InvalidateCategoryCache(addedCategory.Id, addedCategory.UserId);
        }

        return categoryResult;
    }

    public async Task<Result> RemoveAsync(Guid categoryId, Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var categoryResult = await GetByIdAsync(categoryId, userId);

        if (!categoryResult.IsSuccess)
            return Result.Error(categoryResult.ErrorMessage!);

        await _categoryRepository.RemoveAsync(categoryId);
        await InvalidateCategoryCache(categoryId, userId);

        return Result.Success;
    }

    public async Task<Result<Category>> UpdateAsync(CategoryUpdateModel model, Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var categoryResult = await GetByIdAsync(model.Id, userId);

        if (!categoryResult.IsSuccess)
            return Result<Category>.Error(categoryResult.ErrorMessage!);

        var categoryUpdateResult = await _categoryRepository.UpdateAsync(model);

        if (categoryUpdateResult.IsSuccess)
        {
            var updatedCategory = categoryUpdateResult.Value!;
            await InvalidateCategoryCache(updatedCategory.Id, updatedCategory.UserId);
        }

        return categoryUpdateResult;
    }

    public async Task<Result<Category>> GetByIdAsync(Guid categoryId, Guid userId = default)
    {
        if (userId == default)
            userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var key = CacheKeys.Category.ById(categoryId);

        var categoryResult = await _cache.GetOrSetAsync(
            key,
            () => _categoryRepository.GetByIdAsync(categoryId),
            new CacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return ValidateUserRights(categoryResult, userId);
    }

    public async Task<Result<List<Category>>> GetAllAsync(Guid userId = default)
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

    private static Result<Category> ValidateUserRights(Result<Category> categoryResult, Guid userId)
    {
        if (categoryResult.IsSuccess && categoryResult.Value!.UserId != userId)
            return Result<Category>.Error("User does not own that category");

        return categoryResult;
    }
}