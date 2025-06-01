using OpenQA.Selenium;

namespace WebsiteScreenshotService.Services.ContentInitialization;

public class ContentInitializationManager(IEnumerable<IContentInitializationStep> contentInitializationSteps) : IContentInitializationManager
{
    public async Task InitializeContentAsync(WebDriver webDriver)
    {
        ArgumentNullException.ThrowIfNull(webDriver, nameof(webDriver));

        await Task.Delay(2000);

        foreach (var step in contentInitializationSteps)
            await step.InitializeAsync(webDriver);
    }
}
