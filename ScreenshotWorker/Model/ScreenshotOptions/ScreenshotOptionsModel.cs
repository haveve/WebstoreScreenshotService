using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ScreenshotWorker.Utils.Attributes;
using System.Text.Json.Serialization;

namespace ScreenshotWorker.Model.ScreenshotOptions;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
[RequireOneOf(nameof(Clip), nameof(Element), ErrorMessage = "You must provide either Clip or Element.")]
public class ScreenshotOptionsModel
{
    /// <summary>
    /// Gets or sets the URL of the webpage to capture.
    /// </summary>
    [Required]
    [SafeUrl(ErrorMessage = "The provided URL is not allowed.")]
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the type of screenshot to capture (e.g., PNG, JPEG).
    /// </summary>
    [DefaultValue(ScreenshotType.Png)]
    public ScreenshotType ScreenshotType { get; set; }

    /// <summary>
    /// Gets or sets the clipping region of the screenshot.
    /// </summary>
    public ClipModel? Clip { get; set; }

    public ElementModel? Element { get; set; }

    public ModalModel? ModalModel { get; set; }

    public HighlightWordModel? HighlightWord { get; set; }

    public AdvancedConfigurationModel? AdvancedConfiguration { get; set; }

    [Required]
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