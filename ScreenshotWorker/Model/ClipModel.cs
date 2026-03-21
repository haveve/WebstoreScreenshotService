using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Model;

/// <summary>
/// Represents a rectangular region of a webpage to capture in a screenshot.
/// </summary>
public class ClipModel
{
    public const int MaxWidth = 500;

    public const int MaxHeight = 500;

    /// <summary>
    /// Gets or sets the width of the element in pixels.
    /// </summary>
    /// <value>The width in pixels.</value>
    [Required]
    [Range(1, MaxWidth)]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the element in pixels. If is not specified takes the full height of the page.
    /// </summary>
    /// <value>The height in pixels.</value>
    [Range(1, MaxHeight)]
    public int? Height { get; set; }
}
