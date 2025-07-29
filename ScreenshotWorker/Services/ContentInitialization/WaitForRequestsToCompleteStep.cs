using Microsoft.Playwright;
using ScreenshotWorker.Model;
using ScreenshotWorker.Settings.InitializationStep;

namespace ScreenshotWorker.Services.ContentInitialization;

public class WaitForRequestsToCompleteStep : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.RequestsToComplete;

    public async Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        try
        {
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = settings.ExecutionTimeout * 1000 });
        }
        catch (PlaywrightException)
        {
        }
    }
}
