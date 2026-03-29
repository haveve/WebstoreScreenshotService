using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Utils.Attributes;
using System.Text.Json.Serialization;

namespace WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
[RequireOneOf(nameof(Clip), nameof(Element), ErrorMessage = "You must provide either Clip or Element.")]
public class ScreenshotOptionsModel: IScreenshotOptionsModel
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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ScreenshotType ScreenshotType { get; set; }

    /// <summary>
    /// Gets or sets the clipping region of the screenshot.
    /// </summary>
    public IClipModel? Clip { get; set; }

    public IElementModel? Element { get; set; }

    public IModalModel? ModalModel { get; set; }

    public IHighlightWordModel? HighlightWord { get; set; }

    public IAdvancedConfigurationModel? AdvancedConfiguration { get; set; }

    [Required]
    public required ContentLoadingOptions ContentLoadingOptions { get; set; }
}