using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories;

public interface IScreenshotManager
{
    public Task<Screenshot> CreateAsync(Screenshot screenshot);

    public Task<Screenshot> UpdateAsync(Screenshot screenshot);

    public Task DeleteAsync(string screenshotId);

    public ValueTask<string> GetScreenshotUrl(Screenshot screenshot);
}
