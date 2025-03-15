using PuppeteerSharp;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services;

/// <summary>
/// Provides services for browser operations, including taking screenshots.
/// </summary>
public class BrowserService : IBrowserService
{
    private readonly LazyAsync<IBrowser> browser = new(() => Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }));

    /// <summary>
    /// Takes a screenshot of a webpage based on the specified options.
    /// </summary>
    /// <param name="screenshotOptionsModel">The options for taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the screenshot as a stream.</returns>
    public async Task<Stream> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var browserValue = await browser.GetValueAsync();

        using var page = await browserValue.NewPageAsync();
        await page.GoToAsync(screenshotOptionsModel.Url);

        var options = FormatScreenshotOptions(screenshotOptionsModel);
        var screenshotStream = await page.ScreenshotStreamAsync(options);

        return screenshotStream;
    }

    /// <summary>
    /// Formats the screenshot options from the model to the PuppeteerSharp options.
    /// </summary>
    /// <param name="screenshotOptions">The screenshot options model.</param>
    /// <returns>The formatted screenshot options.</returns>
    private static ScreenshotOptions FormatScreenshotOptions(ScreenshotOptionsModel screenshotOptions)
    => new()
    {
        Clip = screenshotOptions.Clip,
        Quality = screenshotOptions.Quality,
        FullPage = screenshotOptions.FullSreen ?? false,
        Type = screenshotOptions.ScreenshotType,
        CaptureBeyondViewport = true,
    };
}
