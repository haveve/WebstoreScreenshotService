using System.Collections.Concurrent;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public class InMemoryScreenshotRepository : IScreenshotRepository
{
    private readonly ConcurrentDictionary<string, Screenshot> _store = new();

    public ValueTask<Result<Screenshot>> GetScreenshot(string screenshotId)
    {
        if (_store.TryGetValue(screenshotId, out var screenshot))
            return ValueTask.FromResult(Result<Screenshot>.Success(screenshot));

        return ValueTask.FromResult(Result<Screenshot>.Error($"Screenshot {screenshotId} not found"));
    }

    public ValueTask<Result<PaginationResult<Screenshot>>> GetScreenshots(Paging? paging, Guid userId)
    {
        var userScreenshots = _store.Values
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

        if (paging is null)
        {
            var resultAll = new PaginationResult<Screenshot>(userScreenshots.Count, userScreenshots);
            return ValueTask.FromResult(Result<PaginationResult<Screenshot>>.Success(resultAll));
        }

        var skip = paging.Page * paging.PageSize;
        var items = userScreenshots.Skip(skip).Take(paging.PageSize).ToList();

        var result = new PaginationResult<Screenshot>(userScreenshots.Count, items);
        return ValueTask.FromResult(Result<PaginationResult<Screenshot>>.Success(result));
    }

    public Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot)
    {
        var entity = new Screenshot(
            screenshot.Id,
            screenshot.WebsiteUrl,
            screenshot.UserId,
            DateTime.UtcNow,
            ScreenshotState.New,
            screenshot.Title,
            screenshot.Description);

        if (!_store.TryAdd(entity.Id, entity))
            return Task.FromResult(Result<Screenshot>.Error($"Screenshot {entity.Id} already exists"));

        return Task.FromResult(Result<Screenshot>.Success(entity));
    }

    public Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel update)
    {
        if (!_store.TryGetValue(update.Id, out var existing))
            return Task.FromResult(Result<Screenshot>.Error($"Screenshot {update.Id} not found"));

        var updated = existing with
        {
            Title = update.Title ?? existing.Title,
            Description = update.Description ?? existing.Description
        };

        _store[update.Id] = updated;
        return Task.FromResult(Result<Screenshot>.Success(updated));
    }

    public Task<Result<Screenshot>> UpdateStateAsync(string screenshotId, ScreenshotState state)
    {
        if (!_store.TryGetValue(screenshotId, out var existing))
            return Task.FromResult(Result<Screenshot>.Error($"Screenshot {screenshotId} not found"));

        var updated = existing with { State = state };
        _store[screenshotId] = updated;

        return Task.FromResult(Result<Screenshot>.Success(updated));
    }
}
