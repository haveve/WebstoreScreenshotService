using Microsoft.Playwright;
using ScreenshotWorker.Model;
using ScreenshotWorker.Settings.InitializationStep;

namespace ScreenshotWorker.Services.ContentInitialization;

public class ScrollToPageEndStep : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.Scroll;

    public async Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        var scrollSettings = settings as ScrollInitializationStepSettings
            ?? throw new ArgumentException($"Invalid settings type for {StepName}. Expected {nameof(ScrollInitializationStepSettings)}.", nameof(settings));

        var waitForPossibleContentLoadMs = scrollSettings.WaitForPossibleContentLoad * 1000;
        var maxHeightToRender = screenshotOptions.Clip.Height ?? 0;
        var scrollDelay = scrollSettings.PollingInterval * 1000;

        var scrollScript = $@"
            (() => {{
                const scrollStep = window.innerHeight / 10;
                const scrollDelay = {scrollDelay};
                const waitForPossibleContentLoad = {waitForPossibleContentLoadMs};
                const maxHeightToRender = {maxHeightToRender};

                return new Promise(resolve => {{
                    let clearIntervalId;
                    window.__scrollCompleted = false;

                    window.__scrollInterval = setInterval(() => {{
                        const scrolled = window.scrollY;
                        const currentScrolledHeight = window.innerHeight + scrolled;
                        const reachedMaxHeight = maxHeightToRender && currentScrolledHeight >= maxHeightToRender;
                        const atBottom = reachedMaxHeight || (currentScrolledHeight + scrollStep) >= document.documentElement.scrollHeight;

                        if(clearIntervalId && atBottom)
                            return;

                        if(clearIntervalId)
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
            }})();
        ";

        try
        {
            await page.EvaluateAsync(scrollScript);
        }
        catch (PlaywrightException) 
        { 
        }
        finally
        {
            await page.EvaluateAsync(@"
                clearInterval(window.__scrollInterval); 
                window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
                window.scrollTo(0, 0);
            ");
        }
    }
}
