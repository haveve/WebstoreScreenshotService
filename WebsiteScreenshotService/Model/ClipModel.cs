using PuppeteerSharp.Media;
using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model;

/// <summary>
/// Represents a rectangular region of a webpage to capture in a screenshot.
/// </summary>
public class ClipModel
{
    /// <summary>
    /// Gets or sets the x coordinate of the element in pixels.
    /// </summary>
    /// <value>The x coordinate in pixels.</value>
    [Range(1, 10000)]
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate of the element in pixels.
    /// </summary>
    /// <value>The y coordinate in pixels.</value>
    [Range(1, 10000)]
    public int Y { get; set; }

    /// <summary>
    /// Gets or sets the width of the element in pixels.
    /// </summary>
    /// <value>The width in pixels.</value>
    [Range(1, 10000)]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the element in pixels.
    /// </summary>
    /// <value>The height in pixels.</value>
    [Range(1, 10000)]
    public int Height { get; set; }

    public Clip? ToPuppeteerClip()
    {
        return new Clip
        {
            X = X,
            Y = Y,
            Width = Width,
            Height = Height
        };
    }
}
