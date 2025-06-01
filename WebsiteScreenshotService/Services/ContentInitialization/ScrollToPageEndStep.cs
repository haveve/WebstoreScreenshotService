using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebsiteScreenshotService.Settings;

namespace WebsiteScreenshotService.Services.ContentInitialization;

public class ScrollToPageEndStep(IOptions<BrowserServiceSettings> browserServiceSettings) : IContentInitializationStep
{
    private readonly BrowserServiceSettings _browserServiceSettings = browserServiceSettings.Value;

    public async Task InitializeAsync(WebDriver webDriver)
    {
        try
        {
            var variables = @$"
                const scrollStep = window.innerHeight / 10;
                const scrollDelay = 50;
                const waitForPossibleContentLoad = 3000;
            ";

            webDriver.ExecuteScript(variables + @"
            var __clearInterval;

            window.__scrollInterval = setInterval(() => {
                const scrolled = window.scrollY;
                const atBottom = (window.innerHeight * 1.2 + scrolled) >= document.documentElement.scrollHeight;

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

            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_browserServiceSettings.ScrollTimeout))
            {
                PollingInterval = TimeSpan.FromMilliseconds(50),
                Message = "Waiting for the page to load and scroll to the bottom."
            };

            wait.Until(driver =>
            {
                var js = (IJavaScriptExecutor)driver;
                var scrollCompleted = Convert.ToBoolean(js.ExecuteScript("return !!window.__scrollCompleted"));

                return scrollCompleted;
            });
        }
        catch (WebDriverTimeoutException)
        {
            // If the timeout occurs, we can still proceed with the screenshot.
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
            await Task.Delay(1000);
        }
    }
}
