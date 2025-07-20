using ScreenshotWorker.Model;

namespace ScreenshotWorker;

public interface IScreenshotRepository
{
    public Task<bool> SaveScreenshot(string screenshotId, string userId, byte[] screenshotData, ScreenshotType contentType, CancellationToken cancellationToken = default);
}

