using Microsoft.Playwright;
using ScreenshotWorker.Model;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationManager
{
    public Task InitializeContentAsync(IPage page, ScreenshotOptionsModel screenshotOptions);
}
