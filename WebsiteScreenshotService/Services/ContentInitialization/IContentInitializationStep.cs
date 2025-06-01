using OpenQA.Selenium;

namespace WebsiteScreenshotService.Services.ContentInitialization;

public interface IContentInitializationStep
{
    public Task InitializeAsync(WebDriver webDriver);
}
