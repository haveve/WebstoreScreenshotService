namespace WebsiteScreenshotService.Model.ScreenshotOptions;

public interface IElementModel
{
    public string Selector { get; set; }

    public IElementClip Clip { get; set; }
}

public interface IElementClip
{
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