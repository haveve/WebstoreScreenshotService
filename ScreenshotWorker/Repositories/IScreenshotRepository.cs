using Shared.Core.Contracts.ScreeshotModel.Components;

namespace ScreenshotWorker.Repositories;

public interface IScreenshotRepository
{
    public Task<bool> SaveScreenshot(SaveScreenshotModel saveScreenshotModel, CancellationToken cancellationToken = default);
}

public record SaveScreenshotModel(string ScreenshotId, string UserId, byte[] ScreenshotData, ScreenshotType ContentType);