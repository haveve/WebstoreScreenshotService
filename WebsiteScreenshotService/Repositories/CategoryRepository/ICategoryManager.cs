using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public interface ICategoryManager
{
    Task<Category> AddAsync(CategoryCreateModel category, Guid userId = default);

    Task RemoveAsync(Guid categoryId);

    Task<Category?> UpdateAsync(CategoryUpdateModel model);

    ValueTask<CategoryEntity?> GetByIdAsync(Guid categoryId);

    ValueTask<List<Category>> GetAllAsync(Guid userId = default);
}
