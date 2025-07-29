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
            Timeout = _browserServiceSettings.PageLoadTimeout * 1000,
        });

        await _contentInitializationManager.InitializeContentAsync(page, screenshotOptionsModel);

        return await page.ScreenshotAsync(FormatScreenshotOptions(screenshotOptionsModel));
    }

    private static PageScreenshotOptions FormatScreenshotOptions(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var options = new PageScreenshotOptions()
        {
            FullPage = true,
            Type = MatchScreenshotType(screenshotOptionsModel),
        };

        if (!screenshotOptionsModel.Clip.Height.HasValue)
            return options;

        options.Clip = new()
        {
            Width = screenshotOptionsModel.Clip.Width,
            Height = screenshotOptionsModel.Clip.Height.Value
        };

        options.FullPage = false;

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
            Headless = true,
            Args = [
            "--disable-gpu",
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--disable-setuid-sandbox",
            //"--ignore-certificate-errors"
            ],
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = false,
            ViewportSize = new ViewportSize()
            {
                Width = screenshotOptionsModel.Clip.Width,
                Height = 720
            },
        });

        context.SetDefaultNavigationTimeout(_browserServiceSettings.PageLoadTimeout * 1000);
        context.SetDefaultTimeout(_browserServiceSettings.ScriptLoadTimeout * 1000);

        return context;
    }
}
