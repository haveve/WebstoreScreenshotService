using OpenQA.Selenium;
using WebsiteScreenshotService.Model;

namespace WebsiteScreenshotService.Services.ContentInitialization;

public interface IContentInitializationStep
{
    public void InitializeScripts(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions);   

    public void Initialize(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions);
}
