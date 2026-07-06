using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Services.Caching;
using WebsiteScreenshotService.Services.Caching.Services;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public class ScreenshotManager(
    IScreenshotRepository screenshotRepository,
    IUserContextAccessor userContextAccessor,
    IScreenshotEntityMapper screenshotEntityMapper,
    ICacheManager cache,
    IScreenshotCacheService screenshotCache)
    : IScreenshotManager
{
    private readonly IScreenshotRepository _screenshotRepository = screenshotRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IScreenshotEntityMapper _screenshotEntityMapper = screenshotEntityMapper;
    private readonly ICacheManager _cache = cache;
    private readonly IScreenshotCacheService _screenshotCache = screenshotCache;

    public async ValueTask<Result<Screenshot>> GetScreenshot(string screenshotId)
    {
        var result = await GetScreenshotInternal(screenshotId);
        return FormatScreenshotResult(result);
    }

    private async ValueTask<Result<ScreenshotEntity>> GetScreenshotInternal(string screenshotId)
    {
        var key = _screenshotCache.ById(screenshotId);

        var result = await _cache.GetOrSetAsync(
            key,
            () => _screenshotRepository.GetScreenshot(screenshotId),
            CacheOptions.Screenshot.Entry);

        return result;
    }

    public async ValueTask<Result<PaginationResult<Screenshot>>> GetScreenshots(
        ScreenshotPaging? paging = null,
        Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        if (!ShouldCacheList(paging))
            return await GetScreenshotsInternal(paging, userId.Value);

        var cachePaging = paging is not null
            ? new PagingCacheModel(paging.Page, paging.PageSize)
            : null;

        var key = _screenshotCache.List(userId.Value, cachePaging);

        return await _cache.GetOrSetAsync(
            key,
            () => GetScreenshotsInternal(paging, userId.Value),
            CacheOptions.Screenshot.List);
    }

    public async Task<Result> DeleteAsync(IReadOnlyCollection<string> ids)
    {
        if (ids.Count == 0)
            return Result.Success;

        const int maxAttempts = 3;

        foreach (var id in ids.Take(maxAttempts))
        {
            var result = await GetScreenshotInternal(id);

            if (!result.IsSuccess)
                continue;

            var userId = result.Value!.UserId;

            await _screenshotRepository.DeleteAsync(ids);
            await InvalidateListCacheAsync(userId);

            return Result.Success;
        }

        return Result.Error($"deletion process was aborted because more than {maxAttempts} first products weren't found in the database");
    }

    public async Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot)
    {
        var entity = await _screenshotRepository.MakeAsync(screenshot);

        var result = FormatScreenshotResult(entity);

        if (result.IsSuccess)
            await InvalidateListCacheAsync(entity.Value!.UserId);

        return result;
    }

    public async Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel screenshot)
    {
        var entity = await _screenshotRepository.UpdateAsync(screenshot);

        var result = FormatScreenshotResult(entity);

        if (result.IsSuccess)
            await InvalidateListCacheAsync(entity.Value!.UserId);

        return result;
    }

    public async Task<Result<Screenshot>> UpdateStateAsync(
        string screenshotId,
        ScreenshotState state)
    {
        var entity = await _screenshotRepository.UpdateStateAsync(
            screenshotId,
            state);

        var result = FormatScreenshotResult(entity);

        if (result.IsSuccess)
            await InvalidateListCacheAsync(entity.Value!.UserId);

        return result;
    }

    private async Task<Result<PaginationResult<Screenshot>>> GetScreenshotsInternal(
        ScreenshotPaging? paging,
        Guid userId)
    {
        var screenshots = await _screenshotRepository.GetScreenshots(
            paging,
            userId);

        if (!screenshots.IsSuccess)
            return Result<PaginationResult<Screenshot>>
                .Error(screenshots.ErrorMessage!);

        var data = screenshots.Value!;

        var result = new PaginationResult<Screenshot>(
            data.TotalCount,
            [.. data.Items.Select(_screenshotEntityMapper.FromEntity)]);

        return Result<PaginationResult<Screenshot>>
            .Success(result);
    }

    private async Task InvalidateListCacheAsync(Guid userId)
        => await _screenshotCache.InvalidateAsync();

    private static bool ShouldCacheList(ScreenshotPaging? paging)
    {
        if (paging is null)
            return true;

        return paging.Page == 1
            && paging.PageSize <= 50
            && string.IsNullOrWhiteSpace(paging.Query)
            && (paging.CategoryIds is null || paging.CategoryIds.Count == 0);
    }

    private Result<Screenshot> FormatScreenshotResult(
        Result<ScreenshotEntity> entity)
    {
        if (!entity.IsSuccess)
            return Result<Screenshot>.Error(entity.ErrorMessage!);

        return Result<Screenshot>.Success(
            _screenshotEntityMapper.FromEntity(entity.Value!));
    }
}