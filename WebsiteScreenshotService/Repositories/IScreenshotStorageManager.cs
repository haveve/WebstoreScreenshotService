using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories;

public interface IScreenshotStorageManager
{
    public string GetScreenshotUrl(Screenshot screenshot);
}