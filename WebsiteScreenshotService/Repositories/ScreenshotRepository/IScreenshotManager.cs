using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public interface IScreenshotManager
{
    public Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot);

    public Task<Result> DeleteAsync(IReadOnlyCollection<string> ids);

    public Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel screenshot);

    public Task<Result<Screenshot>> UpdateStateAsync(string screenshotId, ScreenshotState state);

    public ValueTask<Result<PaginationResult<Screenshot>>> GetScreenshots(ScreenshotPaging? paging = null, Guid userId = default);

    public ValueTask<Result<Screenshot>> GetScreenshot(string screenshotId);
}