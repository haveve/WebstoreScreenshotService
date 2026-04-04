using Microsoft.EntityFrameworkCore;
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

    private static Screenshot MapToDomain(ScreenshotEntity e)
        => new(
            e.Id,
            e.WebsiteUrl,
            e.UserId,
            e.CreatedAt,
            e.State,
            e.Type,
            e.Title,
            e.Description);

    public async ValueTask<Result<Screenshot>> GetScreenshot(string screenshotId)
    {
        var entity = await _context.Screenshots
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == screenshotId);

        if (entity is null)
            return Result<Screenshot>.Error($"Screenshot {screenshotId} not found");

        return Result<Screenshot>.Success(MapToDomain(entity));
    }

    public async ValueTask<Result<PaginationResult<Screenshot>>> GetScreenshots(ScreenshotPaging? paging, Guid userId)
    {
        var query = _context.Screenshots
            .AsNoTracking()
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

            return Result<PaginationResult<Screenshot>>.Success(
                new PaginationResult<Screenshot>(
                    total,
                    [.. all.Select(MapToDomain)]));
        }

        var page = paging.Page;
        var pageSize = paging.PageSize;

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Result<PaginationResult<Screenshot>>.Success(
            new PaginationResult<Screenshot>(
                total,
                [.. items.Select(MapToDomain)]));
    }

    public async Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot)
    {
        var entity = MapFromCreateModel(screenshot);

        _context.Screenshots.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Result<Screenshot>.Error($"Screenshot {entity.Id} already exists");
        }

        return Result<Screenshot>.Success(MapToDomain(entity));
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
            Description = m.Description
        };

    public async Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel update)
    {
        var entity = await _context.Screenshots
            .FirstOrDefaultAsync(s => s.Id == update.Id);

        if (entity is null)
            return Result<Screenshot>.Error($"Screenshot {update.Id} not found");

        entity.Title = update.Title ?? entity.Title;
        entity.Description = update.Description ?? entity.Description;

        _context.Screenshots.Update(entity);
        await _context.SaveChangesAsync();

        return Result<Screenshot>.Success(MapToDomain(entity));
    }

    public async Task<Result<Screenshot>> UpdateStateAsync(string screenshotId, ScreenshotState state)
    {
        var entity = await _context.Screenshots
            .FirstOrDefaultAsync(s => s.Id == screenshotId);

        if (entity is null)
            return Result<Screenshot>.Error($"Screenshot {screenshotId} not found");

        entity.State = state;

        _context.Screenshots.Update(entity);
        await _context.SaveChangesAsync();

        return Result<Screenshot>.Success(MapToDomain(entity));
    }
}
