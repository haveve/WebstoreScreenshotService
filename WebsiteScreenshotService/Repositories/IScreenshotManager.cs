using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories;

public interface IScreenshotManager
{
    public Task<Screenshot> CreateAsync(Screenshot screenshot);

    public Task<Screenshot> UpdateAsync(Screenshot screenshot);

    public Task DeleteAsync(string screenshotId);

    public ValueTask<PaginationResult<Screenshot>> GetScreenshots(Paging? paging);

    public ValueTask<Screenshot> GetScreenshot(string screenshotId);
}