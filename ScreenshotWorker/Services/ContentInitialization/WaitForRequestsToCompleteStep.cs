using Microsoft.Playwright;
using ScreenshotWorker.Settings.InitializationStep;
using Shared.Core.Contracts.ScreeshotModel.Components;

namespace ScreenshotWorker.Services.ContentInitialization;

public class WaitForRequestsToCompleteStep : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.RequestsToComplete;

    public ValueTask<bool> IsAvailable(IPage page, ScreenshotOptionsModel screenshotOptions)
        => ValueTask.FromResult(screenshotOptions.ContentLoadingOptions.HasFlag(ContentLoadingOptions.WaitForRequestsToComplete));

    public async Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        try
        {
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = settings.ExecutionTimeoutInSeconds * 1000 });
        }
        catch (Exception ex) when (ex is PlaywrightException || ex is TimeoutException)
        {
        }
    }
}
