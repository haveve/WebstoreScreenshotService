namespace WebsiteScreenshotService.Model.ScreenshotOptions;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
public interface IScreenshotOptionsModel
{
    /// <summary>
    /// Gets or sets the URL of the webpage to capture.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the type of screenshot to capture (e.g., PNG, JPEG).
    /// </summary>
    public ScreenshotType ScreenshotType { get; set; }

    /// <summary>
    /// Gets or sets the clipping region of the screenshot.
    /// </summary>
    public IClipModel? Clip { get; set; }

    public IElementModel? Element { get; set; }

    public IModalModel? ModalModel { get; set; }

    public IHighlightWordModel? HighlightWord { get; set; }

    public IAdvancedConfigurationModel? AdvancedConfiguration { get; set; }

    public ContentLoadingOptions ContentLoadingOptions { get; set; }
}

[Flags]
public enum ContentLoadingOptions
{
    None = 0,
    WaitForRequestsToComplete = 1 << 0,
    ScrollToTheEndOfThePage = 1 << 1,
    All = WaitForRequestsToComplete | ScrollToTheEndOfThePage
}

/// <summary>
/// Screenshot file type.
/// </summary>
public enum ScreenshotType
{
    Png,
    Jpeg,
}