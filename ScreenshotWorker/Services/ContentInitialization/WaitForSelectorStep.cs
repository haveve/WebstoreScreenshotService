using Microsoft.Playwright;
using ScreenshotWorker.Settings.InitializationStep;
using Shared.Core.Contracts.ScreeshotModel.Components;

namespace ScreenshotWorker.Services.ContentInitialization;

public class WaitForSelectorStep : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.WaitForSelector;

    public ValueTask<bool> IsAvailable(IPage page, ScreenshotOptionsModel screenshotOptions)
        => ValueTask.FromResult(!string.IsNullOrWhiteSpace(screenshotOptions.AdvancedConfiguration?.WaitForSelector));

    public async Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        try
        {
            await page.WaitForSelectorAsync(screenshotOptions.AdvancedConfiguration?.WaitForSelector!, new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = settings.ExecutionTimeoutInSeconds * 1000
            });
        }
        catch (Exception ex) when (ex is PlaywrightException || ex is TimeoutException)
        {
        }
    }
}
