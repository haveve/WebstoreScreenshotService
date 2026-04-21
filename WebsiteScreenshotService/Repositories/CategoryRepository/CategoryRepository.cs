using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public class CategoryRepository(ScreenshotDbContext context, ICategoryEntityMapper categoryEntityMapper) : ICategoryRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly ICategoryEntityMapper _categoryEntityMapper = categoryEntityMapper;

    public async Task<Category?> AddAsync(CategoryCreateModel category, Guid userId)
    {
        var categoryEntity = new CategoryEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Color = category.Color,
            Name = category.Name,
        };

        await _context.Categories.AddAsync(categoryEntity);
        await _context.SaveChangesAsync();

        return _categoryEntityMapper.FromEntity(categoryEntity);
    }

    public async Task RemoveAsync(Guid categoryId)
    {
        await _context.Categories
           .Where(c => c.Id == categoryId)
           .ExecuteDeleteAsync();
    }

    public async Task<Category?> UpdateAsync(CategoryUpdateModel model)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == model.Id);

        if (category == null)
            return null;

        category.Name = model.Name;
        category.Color = model.Color;

        await _context.SaveChangesAsync();
        return _categoryEntityMapper.FromEntity(category);
    }

    public async Task<Category?> GetByIdAsync(Guid categoryId)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        return category is not null ? _categoryEntityMapper.FromEntity(category) : null;
    }

    public async Task<List<Category>> GetAllAsync(Guid userId)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return [.. categories.Select(_categoryEntityMapper.FromEntity)];
    }
}