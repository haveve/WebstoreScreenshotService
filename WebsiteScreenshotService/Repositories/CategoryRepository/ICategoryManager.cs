using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public interface ICategoryManager
{
    Task<Result<Category>> AddAsync(CategoryCreateModel category, Guid? userId = null);

    Task<Result> RemoveAsync(Guid categoryId, Guid? userId = null);

    Task<Result<Category>> UpdateAsync(CategoryUpdateModel model, Guid? userId = null);

    Task<Result<Category>> GetByIdAsync(Guid categoryId, Guid? userId = null);

    Task<Result<List<Category>>> GetAllAsync(Guid? userId = null);
}
