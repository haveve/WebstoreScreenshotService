namespace WebsiteScreenshotService.Model.ScreenshotOptions;

public interface IHighlightWordModel
{
    /// <summary>
    /// Word to highlight. Safe for JS injection, supports Unicode (ru/ua/chinese/etc).
    /// </summary>
    public string Word { get; set; }

    /// <summary>
    /// Highlight color (hex format only). Safe for JS injection.
    /// </summary>
    public string Color { get; set; }
}

