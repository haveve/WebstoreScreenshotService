using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public interface ICategoryRepository
{
    Task<Category> AddAsync(CategoryCreateModel category, Guid userId);

    Task RemoveAsync(Guid categoryId);

    Task<Category?> UpdateAsync(CategoryUpdateModel model);

    Task<CategoryEntity?> GetByIdAsync(Guid categoryId);

    Task<List<Category>> GetAllAsync(Guid userId);
}
