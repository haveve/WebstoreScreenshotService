using ScreenshotWorker.Model;
using ScreenshotWorker.Services.ContentInitialization;
using Microsoft.Extensions.Options;
using ScreenshotWorker.Settings;
using Microsoft.Playwright;

namespace ScreenshotWorker.Services;

/// <summary>
/// Provides services for browser operations, including taking screenshots.
/// </summary>
public class BrowserService(IContentInitializationManager contentInitializationManager, IOptions<BrowserServiceSettings> _browserServiceSettings) : IBrowserService
{
    private readonly IContentInitializationManager _contentInitializationManager = contentInitializationManager;
    private readonly BrowserServiceSettings _browserServiceSettings = _browserServiceSettings.Value;

    /// <summary>
    /// Takes a screenshot of a webpage based on the specified options.
    /// </summary>
    /// <param name="screenshotOptionsModel">The options for taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the screenshot as a stream.</returns>
    public async Task<byte[]> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        await using var context = await CreateBrowserContextAsync(screenshotOptionsModel);
        var page = await context.NewPageAsync();

        await page.GotoAsync(screenshotOptionsModel.Url, new()
        {
            WaitUntil = WaitUntilState.Load,
        });

        await _contentInitializationManager.InitializeContentAsync(page, screenshotOptionsModel);

        return await page.ScreenshotAsync(FormatScreenshotOptions(screenshotOptionsModel));
    }

    private static PageScreenshotOptions FormatScreenshotOptions(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var options = new PageScreenshotOptions()
        {
            FullPage = !screenshotOptionsModel.Clip.Height.HasValue,
            Type = MatchScreenshotType(screenshotOptionsModel)
        };

        return options;
    }

    private static Microsoft.Playwright.ScreenshotType MatchScreenshotType(ScreenshotOptionsModel screenshotOptionsModel)
    {
        return screenshotOptionsModel.ScreenshotType switch
        {
            Model.ScreenshotType.Png => Microsoft.Playwright.ScreenshotType.Png,
            Model.ScreenshotType.Jpeg => Microsoft.Playwright.ScreenshotType.Jpeg,
            _ => throw new NotImplementedException("Invalid image type")
        };
    }

    private async Task<IBrowserContext> CreateBrowserContextAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var playwright = await Playwright.CreateAsync();

        var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            Args = new[]
            {
                "--start-maximized",
                "--disable-blink-features=AutomationControlled" // Reduce bot detection
            }
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36",
            BypassCSP = true, // Prevent fonts/styles from being blocked
            JavaScriptEnabled = true,
            Locale = "en-US",
            HasTouch = false,
            AcceptDownloads = true,
            ViewportSize = new ViewportSize()
            {
                Width = screenshotOptionsModel.Clip.Width,
                Height = screenshotOptionsModel.Clip.Height ?? 720
            },
        });

        //await context.SetExtraHTTPHeadersAsync(new Dictionary<string, string>
        //{
        //    ["Accept-Language"] = "en-US,en;q=0.9",
        //    ["Upgrade-Insecure-Requests"] = "1"
        //});

        context.SetDefaultNavigationTimeout(_browserServiceSettings.NavigationTimeout * 1000);
        context.SetDefaultTimeout(_browserServiceSettings.DefaultTimeout * 1000);

        return context;
    }
}
