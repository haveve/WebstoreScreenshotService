using System.Text.Json.Serialization;
using WebsiteScreenshotService.Model.ScreeshotModel.Components;

namespace WebsiteScreenshotService.Model.ScreenshotOptions;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
public class InputScreenshotModel
{
    /// <summary>
    /// Gets or sets the URL of the webpage to capture.
    /// </summary>
    public required string Url { get; set; }

    public ScreenshotQualityMode Mode { get; set; }

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
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScreenshotType
{
    Png,
    Jpeg,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScreenshotQualityMode
{
    High,
    Medium,
    Low
}