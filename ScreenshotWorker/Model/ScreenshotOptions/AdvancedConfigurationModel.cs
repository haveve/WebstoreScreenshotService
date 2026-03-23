using ScreenshotWorker.Utils.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ScreenshotWorker.Model.ScreenshotOptions;

public class AdvancedConfigurationModel
{
    /// <summary>
    /// Browser locale, e.g., "en-US". Defaults to "en-US".
    /// </summary>
    [RegularExpressionWithTimeout(@"^[a-z]{2}-[A-Z]{2}$", ErrorMessage = "Locale must be in format xx-XX, e.g., en-US.")]
    public string Locale { get; set; } = "en-US";

    /// <summary>
    /// Browser timezone, IANA identifier, e.g., "Europe/Kyiv". Defaults to UTC.
    /// </summary>
    [RegularExpressionWithTimeout(@"^[A-Za-z]+\/[A-Za-z_]+$", ErrorMessage = "TimezoneId must be a valid IANA timezone, e.g., Europe/Kyiv.")]
    public string TimezoneId { get; set; } = "UTC";

    /// <summary>
    /// Color scheme for screenshot rendering.
    /// </summary>
    [EnumDataType(typeof(ColorSchemeOption))]
    public ColorSchemeOption ColorScheme { get; set; } = ColorSchemeOption.Light;

    [StringLength(200)]
    [RegularExpressionWithTimeout(@"^[a-zA-Z0-9#\.\[\]\-_\s,:>+~=""'()*]+$", ErrorMessage = "Invalid selector format.")]
    public string? WaitForSelector { get; set; }

    public ResourceBlockOptions BlockResources { get; set; }

    [MaxCollectionCount(20, ErrorMessage = "Maximum 30 headers allowed.")]
    public List<HeaderModel> Headers { get; set; } = [];

    [MaxCollectionCount(15, ErrorMessage = "Maximum 20 cookies allowed.")]
    public List<CookieModel> Cookies { get; set; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ColorSchemeOption
{
    Light,
    Dark,
    NoPreference
}

[Flags]
public enum ResourceBlockOptions
{
    None = 0,
    Images = 1 << 0,
    Fonts = 1 << 1,
    Media = 1 << 2,
    Scripts = 1 << 3,
    Stylesheets = 1 << 4,

    All = Images | Fonts | Media | Scripts | Stylesheets
}