using Microsoft.Extensions.Options;
using ScreenshotWorker.Settings;
using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;

public class ScreenshotStorageManager(IOptions<ScreenshotStorageConfigurations> options) : IScreenshotStorageManager
{
    private readonly string baseUrl = options.Value.Url.TrimEnd('/');

    public string GetScreenshotUrl(Screenshot screenshot)
       => $"{baseUrl}/{screenshot.UserId}/{screenshot.Id}";
}