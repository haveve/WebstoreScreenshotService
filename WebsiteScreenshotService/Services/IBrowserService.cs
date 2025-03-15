using WebsiteScreenshotService.Model;

namespace WebsiteScreenshotService.Services;

/// <summary>
/// Defines the contract for browser services, including operations for taking screenshots.
/// </summary>
public interface IBrowserService
{
    /// <summary>
    /// Takes a screenshot of a webpage based on the specified options.
    /// </summary>
    /// <param name="screenshotOptionsModel">The options for taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the screenshot as a stream.</returns>
    public Task<Stream> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel);
}

