using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public interface ICategoryManager
{
    Task<Category?> AddAsync(CategoryCreateModel category, Guid userId = default);

    Task RemoveAsync(Guid categoryId, Guid userId = default);

    Task<Category?> UpdateAsync(CategoryUpdateModel model, Guid userId = default);

    ValueTask<Category?> GetByIdAsync(Guid categoryId);

    ValueTask<List<Category>> GetAllAsync(Guid userId = default);
}
