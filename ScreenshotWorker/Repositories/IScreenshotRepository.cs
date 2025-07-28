using ScreenshotWorker.Model;

namespace ScreenshotWorker.Repositories;

public interface IScreenshotRepository
{
    public Task<bool> SaveScreenshot(string screenshotId, string userId, byte[] screenshotData, ScreenshotType contentType, CancellationToken cancellationToken = default);
}

