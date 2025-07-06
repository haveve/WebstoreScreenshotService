using ScreenshotWorker.Model;
using ScreenshotWorker.Services.ContentInitialization;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using Microsoft.Extensions.Options;

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
    public async Task<Stream> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var screenshot = await TakeScreenshot(screenshotOptionsModel);

        var screenshotResult = screenshotOptionsModel.Clip.Height.HasValue
            ? ResizeScreenshot(screenshot, screenshotOptionsModel)
            : new MemoryStream(screenshot);

        return screenshotResult;
    }

    private async Task<byte[]> TakeScreenshot(ScreenshotOptionsModel screenshotOptionsModel)
    {
        using var driver = CreateDriver();

        var window = driver.Manage().Window;
        window.Size = new(screenshotOptionsModel.Clip.Width, window.Size.Height);

        driver.Navigate().GoToUrl(screenshotOptionsModel.Url);

        await _contentInitializationManager.InitializeContentAsync(driver, screenshotOptionsModel);

        var screenshot = driver.GetFullPageScreenshot();

        return screenshot.AsByteArray;
    }

    private FirefoxDriver CreateDriver()
    {
        var service = FirefoxDriverService.CreateDefaultService();
        var options = new FirefoxOptions
        {
            AcceptInsecureCertificates = true,
            PageLoadStrategy = PageLoadStrategy.Normal
        };

        options.AddArgument("--headless");

        var driver = new FirefoxDriver(service, options);

        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(_browserServiceSettings.PageLoadTimeout);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(_browserServiceSettings.ScriptLoadTimeout);

        return driver;
    }

    private static MemoryStream ResizeScreenshot(byte[] inputStream, ScreenshotOptionsModel screenshotOptionsModel)
    {
        using var image = Image.Load(inputStream);

        if (image.Height <= screenshotOptionsModel.Clip.Height)
            return new MemoryStream(inputStream);

        image.Mutate(x => x.Crop(image.Width, screenshotOptionsModel.Clip.Height!.Value));

        IImageEncoder imageEncoder = screenshotOptionsModel.ScreenshotType switch
        {
            ScreenshotType.Png => new PngEncoder(),
            ScreenshotType.Jpeg => new JpegEncoder(),
            _ => throw new NotImplementedException("Invalid image type")
        };

        var outputStream = new MemoryStream();
        image.Save(outputStream, imageEncoder);

        outputStream.Seek(0, SeekOrigin.Begin);

        return outputStream;
    }
}
