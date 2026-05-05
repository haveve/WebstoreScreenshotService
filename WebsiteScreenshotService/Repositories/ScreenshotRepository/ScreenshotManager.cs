using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public class ScreenshotManager(IScreenshotRepository screenshotRepository, IUserContextAccessor userContextAccessor, IScreenshotEntityMapper screenshotEntityMapper) : IScreenshotManager
{
    private readonly IScreenshotRepository _screenshotRepository = screenshotRepository;
    private readonly IScreenshotEntityMapper _screenshotEntityMapper = screenshotEntityMapper;

    public async ValueTask<Result<Screenshot>> GetScreenshot(string screenshotId)
    {
        var entity = await _screenshotRepository.GetScreenshot(screenshotId);
        return FormatScreenshotResult(entity);
    }

    public async ValueTask<Result<PaginationResult<Screenshot>>> GetScreenshots(ScreenshotPaging? paging = null, Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        var screenshots = await _screenshotRepository.GetScreenshots(paging, userId);

        if (!screenshots.IsSuccess)
            return Result<PaginationResult<Screenshot>>.Error(screenshots.ErrorMessage!);

        var screenshotsData = screenshots.Value!;
        var result = new PaginationResult<Screenshot>(screenshotsData.TotalCount,
            [.. screenshotsData.Items.Select(_screenshotEntityMapper.FromEntity)]);

        return Result<PaginationResult<Screenshot>>.Success(result);
    }

    public async Task DeleteAsync(string id)
        => await _screenshotRepository.DeleteAsync(id);

    public async Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot)
    {
        var entity = await _screenshotRepository.MakeAsync(screenshot);
        return FormatScreenshotResult(entity);
    }

    public async Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel screenshot)
    {
        var entity = await _screenshotRepository.UpdateAsync(screenshot);
        return FormatScreenshotResult(entity);
    }

    public async Task<Result<Screenshot>> UpdateStateAsync(string screenshotId, ScreenshotState state)
    {
        var entity = await _screenshotRepository.UpdateStateAsync(screenshotId, state);
        return FormatScreenshotResult(entity);
    }

    private Result<Screenshot> FormatScreenshotResult(Result<ScreenshotEntity> entity)
    {
        if (!entity.IsSuccess)
            return Result<Screenshot>.Error(entity.ErrorMessage!);

        return Result<Screenshot>.Success(_screenshotEntityMapper.FromEntity(entity.Value!));
    }
}
