using Microsoft.Playwright;
using ScreenshotWorker.Model.ScreenshotOptions;
using ScreenshotWorker.Settings.InitializationStep;

namespace ScreenshotWorker.Services.ContentInitialization;

public class WaitForElementToAppearStep : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.WaitForElementToAppear;

    public ValueTask<bool> IsAvailable(IPage page, ScreenshotOptionsModel screenshotOptions)
        => ValueTask.FromResult(!string.IsNullOrWhiteSpace(screenshotOptions.Element?.Selector));

    public async Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        try
        {
            var selector = screenshotOptions.Element!.Selector;
            await page.WaitForSelectorAsync(selector, new() { Timeout = settings.ExecutionTimeoutInSeconds * 1000 });
        }
        catch (Exception ex) when (ex is PlaywrightException || ex is TimeoutException)
        {
        }
    }
}
