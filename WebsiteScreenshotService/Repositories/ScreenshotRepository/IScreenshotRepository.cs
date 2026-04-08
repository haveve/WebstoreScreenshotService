using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public interface IScreenshotRepository
{
    public Task<Result<Screenshot>> MakeAsync(ScreenshotCreateModel screenshot);

    public Task<Result<Screenshot>> UpdateAsync(ScreenshotUpdateModel screenshot);

    public Task DeleteAsync(string id);

    public Task<Result<Screenshot>> UpdateStateAsync(string screenshotId, ScreenshotState state);

    public Task<Result<PaginationResult<Screenshot>>> GetScreenshots(ScreenshotPaging? paging, Guid userId);

    public Task<Result<Screenshot>> GetScreenshot(string screenshotId);
}