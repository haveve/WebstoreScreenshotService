using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ScreenshotWorker;
using ScreenshotWorker.Services.ContentInitialization;
using ScreenshotWorker.Model;

namespace ScreenshotWorker.Services.ContentInitialization;

public class ScrollToPageEndStep : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.Scroll;

    public Task InitializeAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        var scrollSettings = settings as ScrollInitializationStepSettings
            ?? throw new ArgumentException($"Invalid settings type for {StepName}. Expected {nameof(ScrollInitializationStepSettings)}.", nameof(settings));

        try
        {
            var variables = @$"
                const scrollStep = window.innerHeight / 10;
                const scrollDelay = 50;
                const waitForPossibleContentLoad = {scrollSettings.WaitForPossibleContentLoad * 1000};
                const maxHeightToRender = {screenshotOptions.Clip.Height ?? 0}
            ";

            webDriver.ExecuteScript(variables + @"
            var __clearInterval;

            window.__scrollInterval = setInterval(() => {
                const scrolled = window.scrollY;
                const currentScrolledHeight = window.innerHeight + scrolled;
                const reachedMaxHeight = maxHeightToRender && currentScrolledHeight >= maxHeightToRender;
                const atBottom = reachedMaxHeight || (currentScrolledHeight + scrollStep) >= document.documentElement.scrollHeight;

                if(__clearInterval && atBottom)
                    return;

               __clearInterval && clearInterval(__clearInterval);

                if (atBottom) {
                  __clearInterval = setTimeout(() => {
                        clearInterval(window.__scrollInterval);
                        window.__scrollCompleted = true;
                    }, waitForPossibleContentLoad);
                } else {
                    window.scrollBy(0, scrollStep);
                }
            }, scrollDelay);
            ");

            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(settings.ExecutionTimeout))
            {
                PollingInterval = TimeSpan.FromSeconds(settings.PoolingTimeout),
                Message = "Waiting for the page to load and scroll to the bottom."
            };

            wait.Until(driver =>
            {
                var js = (IJavaScriptExecutor)driver;
                var scrollCompleted = Convert.ToBoolean(js.ExecuteScript("return !!window.__scrollCompleted"));

                return scrollCompleted;
            });

            return Task.CompletedTask;
        }
        catch (WebDriverTimeoutException)
        {
            // If the timeout occurs, we can still proceed with the screenshot.
            return Task.CompletedTask;
        }
        finally
        {
            webDriver.ExecuteScript(@"
                clearInterval(window.__scrollInterval); 
                window.scrollTo({
                  top: document.body.scrollHeight,
                  behavior: 'smooth'
                });
                window.scrollTo(0, 0);");
        }
    }
}
