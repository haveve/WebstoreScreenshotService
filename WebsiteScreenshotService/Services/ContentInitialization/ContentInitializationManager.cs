using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Settings;

namespace WebsiteScreenshotService.Services.ContentInitialization;

public class ContentInitializationManager(IEnumerable<IContentInitializationStep> contentInitializationSteps, IOptions<BrowserServiceSettings> browserServiceSettings) : IContentInitializationManager
{
    private readonly BrowserServiceSettings _browserServiceSettings = browserServiceSettings.Value;

    public void InitializeContent(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions)
    {
        ArgumentNullException.ThrowIfNull(webDriver, nameof(webDriver));

        foreach (var step in contentInitializationSteps)
            step.InitializeScripts(webDriver, screenshotOptions);

        Thread.Sleep(TimeSpan.FromSeconds(_browserServiceSettings.InitialPageLoadDelay));

        foreach (var step in contentInitializationSteps)
            step.Initialize(webDriver, screenshotOptions);
    }
}
