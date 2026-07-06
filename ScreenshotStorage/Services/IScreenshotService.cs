namespace ScreenshotStorage.Services;

public interface IScreenshotService
{
    public Task SaveAsync(string userId, string screenshotId, ScreenshotData data);

    public Task<bool> DeleteAsync(string userId, string screenshotId);
}
