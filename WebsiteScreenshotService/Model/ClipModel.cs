using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model;

/// <summary>
/// Represents a rectangular region of a webpage to capture in a screenshot.
/// </summary>
public class ClipModel
{
    /// <summary>
    /// Gets or sets the width of the element in pixels.
    /// </summary>
    /// <value>The width in pixels.</value>
    [Range(1, 10000)]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the element in pixels. If is not specified takes the full height of the page.
    /// </summary>
    /// <value>The height in pixels.</value>
    [Range(1, 10000)]
    public int? Height { get; set; }
}
