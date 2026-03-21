using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using ScreenshotWorker.Model;
using ScreenshotWorker.Model.ScreenshotOptions;
using ScreenshotWorker.Settings;
using ScreenshotWorker.Settings.InitializationStep;
using ScreenshotWorker.Utils;

namespace ScreenshotWorker.Services.ContentInitialization;

public class ScrollToPageEndStep(IOptions<BrowserServiceSettings> browserServiceSettings) : IContentInitializationStep
{
    private readonly string _scrollScript = FileHelper.LoadEmbeddedFile(Path.Combine("Scripts", "ContentInitialization", "ScrollToPageEndStep.js"));

    private readonly BrowserServiceSettings _browserServiceSettings = browserServiceSettings.Value;

    public string StepName => ContentInitializationStepsNames.Scroll;

    public ValueTask<bool> IsAvailable(IPage page, ScreenshotOptionsModel screenshotOptions)
        => ValueTask.FromResult(screenshotOptions.Clip?.Height is null && screenshotOptions.ContentLoadingOptions.HasFlag(ContentLoadingOptions.ScrollToTheEndOfThePage));

    public async Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        var scrollSettings = settings as ScrollInitializationStepSettings
            ?? throw new ArgumentException($"Invalid settings type for {StepName}. Expected {nameof(ScrollInitializationStepSettings)}.", nameof(settings));

        var scrollDelay = scrollSettings.PollingInterval * 1000;
        var waitForPossibleContentLoad = scrollSettings.WaitForPossibleContentLoad * 1000;
        var maxExecutionTimeout = scrollSettings.ExecutionTimeoutInSeconds * 1000;
        var maxRenderedHeight = ClipModel.MaxHeight;

        page.SetDefaultTimeout(maxExecutionTimeout);

        try
        {
            var scriptsParameters = new ScriptsParameters(scrollDelay, waitForPossibleContentLoad, maxExecutionTimeout, maxRenderedHeight);
            await page.EvaluateAsync(_scrollScript, scriptsParameters);
        }
        catch (Exception ex) when (ex is PlaywrightException || ex is TimeoutException)
        {
        }
        finally
        {
            page.SetDefaultTimeout(_browserServiceSettings.DefaultTimeout * 1000);
            await page.EvaluateAsync(@"
                clearInterval(window.__scrollInterval); 
                window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
                window.scrollTo(0, 0);
            ");
        }
    }

    private record ScriptsParameters(float ScrollDelay, double WaitForPossibleContentLoad, float MaxExecutionTimeout, int MaxRenderedHeight);
}
