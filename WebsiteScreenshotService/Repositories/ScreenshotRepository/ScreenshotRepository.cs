using Microsoft.EntityFrameworkCore;
using System.Collections;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Search;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public class ScreenshotRepository(ScreenshotDbContext context, IScreenshotSearchStrategy screenshotSearchStrategy) : IScreenshotRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly IScreenshotSearchStrategy _screenshotSearchStrategy = screenshotSearchStrategy;

    public async Task<Result<ScreenshotEntity>> GetScreenshot(string screenshotId)
    {
        var entity = await _context.Screenshots
            .AsNoTracking()
            .Include(s => s.Categories)
            .FirstOrDefaultAsync(s => s.Id == screenshotId);

        if (entity is null)
            return Result<ScreenshotEntity>.Error($"Screenshot {screenshotId} not found");

        return Result<ScreenshotEntity>.Success(entity);
    }

    public async Task DeleteAsync(IReadOnlyCollection<string> ids)
    {
        if(ids.Count == 0) 
            return;

        if (ids.Count == 1)
        {
            var id = ids.First();
            await _context.Screenshots.Where(s => s.Id == id).ExecuteDeleteAsync();
            return;
        }

        await _context.Screenshots
            .Where(s => ids.Contains(s.Id))
            .ExecuteDeleteAsync();
    }

    public async Task<Result<PaginationResult<ScreenshotEntity>>> GetScreenshots(ScreenshotPaging? paging, Guid userId)
    {
        var query = _context.Screenshots
            .AsNoTracking()
            .Include(s => s.Categories)
            .Where(s => s.UserId == userId);

        if (paging is not null)
            query = _screenshotSearchStrategy.ApplySearch(query, paging.Query, paging.SearchScope);

        // Category filter
        if (paging?.CategoryIds is { Count: > 0 })
        {
            query = query.Where(s =>
                s.Categories.Any(c => paging.CategoryIds.Contains(c.Id)));
        }

        var total = await query.CountAsync();

        if (paging is null)
        {
            var all = await query
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return Result<PaginationResult<ScreenshotEntity>>.Success(
                new PaginationResult<ScreenshotEntity>(
                    total,
                    all));
        }

        var page = paging.Page;
        var pageSize = paging.PageSize;

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Result<PaginationResult<ScreenshotEntity>>.Success(
            new PaginationResult<ScreenshotEntity>(
                total,
                items));
    }

    public async Task<Result<ScreenshotEntity>> MakeAsync(ScreenshotCreateModel screenshot)
    {
        var entity = MapFromCreateModel(screenshot);

        _context.Screenshots.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Result<ScreenshotEntity>.Error($"Screenshot {entity.Id} already exists");
        }

        return Result<ScreenshotEntity>.Success(entity);
    }

    private static ScreenshotEntity MapFromCreateModel(ScreenshotCreateModel m)
        => new()
        {
            Id = m.Id,
            WebsiteUrl = m.WebsiteUrl,
            UserId = m.UserId,
            CreatedAt = DateTime.UtcNow,
            State = ScreenshotState.New,
            Type = m.Type,
            Title = m.Title,
            Description = m.Description,
            Cost = m.PointsCost
        };

    public async Task<Result<ScreenshotEntity>> UpdateAsync(ScreenshotUpdateModel update)
    {
        var entity = await _context.Screenshots
            .Include(s => s.Categories)
            .FirstOrDefaultAsync(s => s.Id == update.Id);

        if (entity is null)
            return Result<ScreenshotEntity>.Error($"Screenshot {update.Id} not found");

        entity.Title = update.Title ?? entity.Title;
        entity.Description = update.Description ?? entity.Description;

        if (update.Categories is not null)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => update.Categories.Contains(c.Id))
                .ToListAsync();

            entity.Categories.Clear();

            foreach (var category in categories)
                entity.Categories.Add(category);
        }

        await _context.SaveChangesAsync();

        return Result<ScreenshotEntity>.Success(entity);
    }

    public async Task<Result<ScreenshotEntity>> UpdateStateAsync(string screenshotId, ScreenshotState state)
    {
        var entity = await _context.Screenshots
            .FirstOrDefaultAsync(s => s.Id == screenshotId);

        if (entity is null)
            return Result<ScreenshotEntity>.Error($"Screenshot {screenshotId} not found");

        entity.State = state;

        _context.Screenshots.Update(entity);
        await _context.SaveChangesAsync();

        return Result<ScreenshotEntity>.Success(entity);
    }
}
