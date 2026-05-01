namespace WebsiteScreenshotService.Model.ScreeshotModel.Components;

public class ElementModel
{
    public required string Selector { get; set; }

    public required ElementClip Clip { get; set; }
}

public class ElementClip
{
    public const int MaxWidth = 5000;

    public const int MaxHeight = 7000;

    /// <summary>
    /// Gets or sets the width of the element in pixels.
    /// </summary>
    /// <value>The width in pixels.</value>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the element in pixels. If is not specified takes the full height of the page.
    /// </summary>
    /// <value>The height in pixels.</value>
    public int Height { get; set; }
}