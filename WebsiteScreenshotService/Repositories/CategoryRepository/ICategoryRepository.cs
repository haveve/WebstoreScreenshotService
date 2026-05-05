using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public interface ICategoryRepository
{
    Task<Result<Category>> AddAsync(CategoryCreateModel category, Guid userId);

    Task<Result> RemoveAsync(Guid categoryId);

    Task<Result<Category>> UpdateAsync(CategoryUpdateModel model);

    Task<Result<Category>> GetByIdAsync(Guid categoryId);

    Task<Result<List<Category>>> GetAllAsync(Guid userId);
}
