using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;

public interface IScreenshotStorageManager
{
    public string GetScreenshotUrl(Screenshot screenshot, Guid userId);
}