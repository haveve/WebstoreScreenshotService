using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public interface IScreenshotRepository
{
    public Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot);

    public Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel screenshot);

    public Task<Result<Screenshot>> UpdateStateAsync(string screenshotId, ScreenshotState state);

    public ValueTask<Result<PaginationResult<Screenshot>>> GetScreenshots(Paging? paging, Guid userId);

    public ValueTask<Result<Screenshot>> GetScreenshot(string screenshotId);
}