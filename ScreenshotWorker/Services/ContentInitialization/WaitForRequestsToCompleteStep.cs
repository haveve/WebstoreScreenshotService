using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ScreenshotWorker.Model;

namespace ScreenshotWorker.Services.ContentInitialization;

public class WaitForRequestsToCompleteStep : IContentInitializationStep
{
    public string StepName => ContentInitializationStepsNames.RequestsToComplete;

    public Task InitializeScriptsAsync(WebDriver webDriver, ScreenshotOptionsModel _)
    {
        webDriver.ExecuteScript(@"
                if (!window.__requestTrackerInjected) {
                    window.__activeResources = 0;

                    // Patch fetch
                    const origFetch = window.fetch;
                    window.fetch = function() {
                        window.__activeResources++;
                        return origFetch.apply(this, arguments).finally(() => window.__activeResources--);
                    };

                    // Patch XHR
                    const origOpen = XMLHttpRequest.prototype.open;
                    const origSend = XMLHttpRequest.prototype.send;
                    XMLHttpRequest.prototype.open = function() {
                        this.addEventListener('loadend', () => window.__activeResources--);
                        return origOpen.apply(this, arguments);
                    };
                    XMLHttpRequest.prototype.send = function() {
                        window.__activeResources++;
                        return origSend.apply(this, arguments);
                    };

                    // Track <img>, <script>, <link>
                    const trackElement = (el) => {
                        if (!el || el.__trackingAttached) return;
                        el.__trackingAttached = true;
                        window.__activeResources++;
                        const done = () => window.__activeResources--;

                        el.addEventListener('load', done, { once: true });
                        el.addEventListener('error', done, { once: true });
                    };

                    const observer = new MutationObserver(() => {
                        document.querySelectorAll('img[src], script[src], link[rel=stylesheet][href]').forEach(trackElement);
                    });
                    observer.observe(document, { childList: true, subtree: true });

                    // Initially mark current elements
                    document.querySelectorAll('img[src], script[src], link[rel=stylesheet][href]').forEach(trackElement);

                    window.__requestTrackerInjected = true;
                }
            ");
        return Task.CompletedTask;
    }

    public Task InitializeAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings settings)
    {
        try
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(settings.ExecutionTimeout))
            {
                PollingInterval = TimeSpan.FromMilliseconds(1000),
                Message = "Waiting for the all requests to complete."
            };

            Thread.Sleep(wait.PollingInterval);

            wait.Until(driver =>
            {
                var js = (IJavaScriptExecutor)driver;

                var activeRequestsPresent = js.ExecuteScript(@"return !!window.__activeResources;");

                return Convert.ToBoolean(activeRequestsPresent);
            });

            return Task.CompletedTask;
        }
        catch (WebDriverTimeoutException)
        {
            // If the timeout occurs, we can still proceed with the screenshot.
            return Task.CompletedTask;
        }
    }
}
