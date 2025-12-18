using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using ScreenshotWorker.Model;
using ScreenshotWorker.Settings;
using ScreenshotWorker.Settings.InitializationStep;

namespace ScreenshotWorker.Services.ContentInitialization;

public class ScrollToPageEndStep(IOptions<BrowserServiceSettings> _browserServiceSettings) : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.Scroll;

    public async Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        var scrollSettings = settings as ScrollInitializationStepSettings
            ?? throw new ArgumentException($"Invalid settings type for {StepName}. Expected {nameof(ScrollInitializationStepSettings)}.", nameof(settings));

        if (screenshotOptions.Clip.Height.HasValue)
            return;

        var timeout = scrollSettings.ExecutionTimeoutInSeconds * 1000;

        page.SetDefaultTimeout(timeout);

        var waitForPossibleContentLoadMs = scrollSettings.WaitForPossibleContentLoad * 1000;
        var scrollDelay = scrollSettings.PollingInterval * 1000;

        var scrollScript = $@"
(() => {{
    const scrollStep = window.innerHeight / 5;
    const scrollDelay = {scrollDelay};
    const waitForPossibleContentLoad = {waitForPossibleContentLoadMs};
    const maxExecutionTimeout = {timeout};

    const scrollPromise = new Promise(resolve => {{
        let clearIntervalId;
        window.__scrollCompleted = false;

        window.__scrollInterval = setInterval(() => {{
            const scrolled = window.scrollY;
            const currentScrolledHeight = window.innerHeight + scrolled;
            const atBottom = (currentScrolledHeight + scrollStep) >= document.documentElement.scrollHeight;

            if (clearIntervalId && atBottom)
                return;

            if (clearIntervalId)
                clearTimeout(clearIntervalId);

            if (atBottom) {{
                clearIntervalId = setTimeout(() => {{
                    clearInterval(window.__scrollInterval);
                    window.__scrollCompleted = true;
                    resolve(true);
                }}, waitForPossibleContentLoad);
            }} else {{
                window.scrollBy(0, scrollStep);
            }}
        }}, scrollDelay);
    }});

    const timeoutPromise = new Promise((_, reject) => 
        setTimeout(() => reject('Scroll script timeout exceeded'), maxExecutionTimeout)
    );

    return Promise.race([scrollPromise, timeoutPromise]);
}})();
";

        try
        {
            await page.EvaluateAsync(scrollScript);
        }
        catch (Exception ex) when (ex is PlaywrightException || ex is TimeoutException)
        {
        }
        finally
        {
            page.SetDefaultTimeout(_browserServiceSettings.Value.DefaultTimeout * 1000);
            await page.EvaluateAsync(@"
                clearInterval(window.__scrollInterval); 
                window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
                window.scrollTo(0, 0);
            ");
        }
    }
}
