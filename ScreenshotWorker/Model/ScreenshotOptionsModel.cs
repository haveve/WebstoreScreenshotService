using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ScreenshotWorker.Model;

/// <summary>
/// Represents the options for taking a screenshot.
/// </summary>
public class ScreenshotOptionsModel
{
    /// <summary>
    /// Gets or sets the URL of the webpage to capture.
    /// </summary>
    [Url]
    [Required]
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the type of screenshot to capture (e.g., PNG, JPEG).
    /// </summary>
    [DefaultValue(ScreenshotType.Png)]
    public ScreenshotType ScreenshotType { get; set; }

    /// <summary>
    /// Gets or sets the clipping region of the screenshot.
    /// </summary>
    [Required]
    public required ClipModel Clip { get; set; }
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
