using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Utils.Attributes;

namespace WebsiteScreenshotService.Model;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
public class ScreenshotOptionsModel
{
    /// <summary>
    /// Gets or sets the URL of the webpage to capture.
    /// </summary>
    [Url]
    [Required]
    public string Url { get; set; } = default!;

    /// <summary>
    /// Gets or sets the type of screenshot to capture (e.g., PNG, JPEG).
    /// </summary>
    [DefaultValue(ScreenshotType.Png)]
    public ScreenshotType ScreenshotType { get; set; }

    /// <summary>
    /// Gets or sets the quality of the screenshot if the type is JPEG. Ignored for PNG.
    /// </summary>
    [Range(0, 100)]
    [EmptyIf<ScreenshotType>(nameof(ScreenshotType), ScreenshotType.Png)]
    public int? Quality { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to capture the full screen. Cannot be specified with Clip.
    /// </summary>
    [OnlyOneSpecified(nameof(Clip))]
    public bool? FullScreen { get; set; }

    /// <summary>
    /// Gets or sets the clipping region of the screenshot. Cannot be specified with FullScreen.
    /// </summary>
    [OnlyOneSpecified(nameof(FullScreen))]
    public ClipModel? Clip { get; set; }
}
