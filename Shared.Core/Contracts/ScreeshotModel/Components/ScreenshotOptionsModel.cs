namespace Shared.Core.Contracts.ScreeshotModel.Components;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
public class ScreenshotOptionsModel
{
    /// <summary>
    /// Gets or sets the URL of the webpage to capture.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the type of screenshot to capture (e.g., PNG, JPEG).
    /// </summary>
    public ScreenshotType ScreenshotType { get; set; }

    /// <summary>
    /// Gets or sets the clipping region of the screenshot.
    /// </summary>
    public ClipModel? Clip { get; set; }

    public ElementModel? Element { get; set; }

    public ModalModel? ModalModel { get; set; }

    public HighlightWordModel? HighlightWord { get; set; }

    public AdvancedConfigurationModel? AdvancedConfiguration { get; set; }

    public required ContentLoadingOptions ContentLoadingOptions { get; set; }
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
    Png = 1,
    Jpeg = 2,
}