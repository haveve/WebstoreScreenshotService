using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories.CategoryRepository.Models;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.CategoryRepository;

public class CategoryRepository(ScreenshotDbContext context, ICategoryEntityMapper categoryEntityMapper) : ICategoryRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly ICategoryEntityMapper _categoryEntityMapper = categoryEntityMapper;

    public async Task<Result<Category>> AddAsync(CategoryCreateModel category, Guid userId)
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

        return Result<Category>.Success(_categoryEntityMapper.FromEntity(categoryEntity));
    }

    public async Task<Result> RemoveAsync(Guid categoryId)
    {
        var affected = await _context.Categories
            .Where(c => c.Id == categoryId)
            .ExecuteDeleteAsync();

        if (affected == 0)
            return Result.Error("Category not found or already deleted");

        return Result.Success;
    }

    public async Task<Result<Category>> UpdateAsync(CategoryUpdateModel model)
    {
        var entity = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == model.Id);

        if (entity is null)
            return Result<Category>.Error("Category not found or access denied");

        entity.Name = model.Name;
        entity.Color = model.Color;

        await _context.SaveChangesAsync();

        return Result<Category>.Success(_categoryEntityMapper.FromEntity(entity));
    }

    public async Task<Result<Category>> GetByIdAsync(Guid categoryId)
    {
        var entity = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (entity is null)
            return Result<Category>.Error("Category not found");

        return Result<Category>.Success(_categoryEntityMapper.FromEntity(entity));
    }

    public async Task<Result<List<Category>>> GetAllAsync(Guid userId)
    {
        var entities = await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var result = entities
            .Select(_categoryEntityMapper.FromEntity)
            .ToList();

        return Result<List<Category>>.Success(result);
    }
}