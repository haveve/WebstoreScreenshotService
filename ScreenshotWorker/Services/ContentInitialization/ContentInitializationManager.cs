using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using ScreenshotWorker.Settings;
using ScreenshotWorker.Settings.InitializationStep;
using Shared.Core.Contracts.ScreeshotModel.Components;

namespace ScreenshotWorker.Services.ContentInitialization;

public class ContentInitializationManager(IEnumerable<IContentInitializationStep> contentInitializationSteps, IOptions<BrowserServiceSettings> browserServiceSettings) : IContentInitializationManager
{
    private readonly BrowserServiceSettings _browserServiceSettings = browserServiceSettings.Value;

    public async Task InitializeContentAsync(IPage page, ScreenshotOptionsModel screenshotOptions)
    {
        ArgumentNullException.ThrowIfNull(page, nameof(page));

        await Task.Delay(TimeSpan.FromSeconds(_browserServiceSettings.DefaultWaitTimeout));

        foreach (var step in contentInitializationSteps)
        {
            if (!await step.IsAvailable(page, screenshotOptions))
                continue;

            var setting = GetSettings(step.StepName, _browserServiceSettings);
            await step.InitializeAsync(page, screenshotOptions, setting);
            await Task.Delay(TimeSpan.FromSeconds(_browserServiceSettings.DefaultWaitTimeout));
        }
    }

    private static ContentInitializationStepSettings GetSettings(string stepName, BrowserServiceSettings settings)
        => settings.ContentInitializationSteps.TryGetValue(stepName, out var stepSettings)
            ? stepSettings
            : throw new KeyNotFoundException($"Content initialization step '{stepName}' not found in settings.");
}
