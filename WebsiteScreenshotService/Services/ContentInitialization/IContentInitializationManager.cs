using OpenQA.Selenium;

namespace WebsiteScreenshotService.Services.ContentInitialization;

public interface IContentInitializationManager
{
    public Task InitializeContentAsync(WebDriver webDriver);
}
