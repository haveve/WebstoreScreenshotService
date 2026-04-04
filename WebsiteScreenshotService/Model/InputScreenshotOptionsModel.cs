using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebsiteScreenshotService.Model.ScreenshotOptions;
using WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;
using WebsiteScreenshotService.Utils.Attributes;

namespace WebsiteScreenshotService.Model;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
[RequireOneOf(nameof(Clip), nameof(Element), ErrorMessage = "You must provide either Clip or Element.")]
public class InputScreenshotOptionsModel
{
    /// <summary>
    /// Gets or sets the URL of the webpage to capture.
    /// </summary>
    [Required]
    [MaxLength(2048)]
    [SafeUrl(ErrorMessage = "The provided URL is not allowed.")]
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the type of screenshot to capture (e.g., PNG, JPEG).
    /// </summary>
    [DefaultValue(ScreenshotType.Png)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ScreenshotType ScreenshotType { get; set; }

    public ScreenshotQualityMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the clipping region of the screenshot.
    /// </summary>
    public ClipModel? Clip { get; set; }

    public ElementModel? Element { get; set; }

    public ModalModel? ModalModel { get; set; }

    public HighlightWordModel? HighlightWord { get; set; }

    public AdvancedConfigurationModel? AdvancedConfiguration { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScreenshotQualityMode
{
    High,
    Medium,
    Low
}