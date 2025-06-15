using OpenQA.Selenium;
using WebsiteScreenshotService.Model;

namespace WebsiteScreenshotService.Services.ContentInitialization;

public interface IContentInitializationManager
{
    public void InitializeContent(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions);
}
