using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public interface ICategoryManager
{
    Task<Result<Category>> AddAsync(CategoryCreateModel category, Guid userId = default);

    Task<Result> RemoveAsync(Guid categoryId, Guid userId = default);

    Task<Result<Category>> UpdateAsync(CategoryUpdateModel model, Guid userId = default);

    Task<Result<Category>> GetByIdAsync(Guid categoryId, Guid userId = default);

    Task<Result<List<Category>>> GetAllAsync(Guid userId = default);
}
