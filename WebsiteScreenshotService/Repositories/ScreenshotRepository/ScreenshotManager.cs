using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public class ScreenshotManager(IScreenshotRepository screenshotRepository, IUserContextAccessor userContextAccessor) : IScreenshotManager
{
    private readonly IScreenshotRepository _screenshotRepository = screenshotRepository;

    public async ValueTask<Result<Screenshot>> GetScreenshot(string screenshotId)
        => await _screenshotRepository.GetScreenshot(screenshotId);

    public async ValueTask<Result<PaginationResult<Screenshot>>> GetScreenshots(ScreenshotPaging? paging = null, Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        return await _screenshotRepository.GetScreenshots(paging, userId);
    }

    public async Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot)
        => await _screenshotRepository.MakeAsync(screenshot);

    public async Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel screenshot)
        => await _screenshotRepository.UpdateAsync(screenshot);

    public async Task<Result<Screenshot>> UpdateStateAsync(string screenshotId, ScreenshotState state)
        => await _screenshotRepository.UpdateStateAsync(screenshotId, state);
}
