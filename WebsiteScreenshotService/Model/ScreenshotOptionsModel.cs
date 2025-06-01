using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    /// Gets or sets the clipping region of the screenshot.
    /// </summary>
    public ClipModel Clip { get; set; } = default!;
}

/// <summary>
/// Screenshot file type.
/// </summary>
public enum ScreenshotType
{
    Png,
    Jpeg,
    Pdf,
}
