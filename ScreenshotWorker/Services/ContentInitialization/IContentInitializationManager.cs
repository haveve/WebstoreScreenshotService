using OpenQA.Selenium;
using ScreenshotWorker.Model;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationManager
{
    public Task InitializeContentAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions);
}
