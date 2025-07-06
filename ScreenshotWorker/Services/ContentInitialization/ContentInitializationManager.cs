using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using ScreenshotWorker.Model;

namespace ScreenshotWorker.Services.ContentInitialization;

public class ContentInitializationManager(IEnumerable<IContentInitializationStep> contentInitializationSteps, IOptions<BrowserServiceSettings> browserServiceSettings) : IContentInitializationManager
{
    private readonly BrowserServiceSettings _browserServiceSettings = browserServiceSettings.Value;

    public async Task InitializeContentAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions)
    {
        ArgumentNullException.ThrowIfNull(webDriver, nameof(webDriver));

        foreach (var step in contentInitializationSteps)
           await step.InitializeScriptsAsync(webDriver, screenshotOptions);

        await Task.Delay(TimeSpan.FromSeconds(_browserServiceSettings.InitialPageLoadTimeout));

        foreach (var step in contentInitializationSteps)
        {
            var setting = GetSettings(step.StepName, _browserServiceSettings);
            await step.InitializeAsync(webDriver, screenshotOptions, setting);
            await Task.Delay(TimeSpan.FromSeconds(_browserServiceSettings.DefaultWaitTimeout));
        }
    }

    private static ContentInitializationStepSettings GetSettings(string stepName, BrowserServiceSettings settings)
        => settings.ContentInitializationSteps.TryGetValue(stepName, out var stepSettings)
            ? stepSettings
            : throw new KeyNotFoundException($"Content initialization step '{stepName}' not found in settings.");
}
