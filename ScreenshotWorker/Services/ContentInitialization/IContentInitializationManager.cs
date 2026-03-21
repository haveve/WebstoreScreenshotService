using Microsoft.Playwright;
using ScreenshotWorker.Model.ScreenshotOptions;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationManager
{
    public Task InitializeContentAsync(IPage page, ScreenshotOptionsModel screenshotOptions);
}
