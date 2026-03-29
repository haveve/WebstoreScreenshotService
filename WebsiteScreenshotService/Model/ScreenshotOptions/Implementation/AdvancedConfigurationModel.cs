using WebsiteScreenshotService.Utils.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

public class AdvancedConfigurationModel: IAdvancedConfigurationModel
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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [EnumDataType(typeof(ColorSchemeOption))]
    public ColorSchemeOption ColorScheme { get; set; } = ColorSchemeOption.Light;

    [StringLength(200)]
    [RegularExpressionWithTimeout(@"^[a-zA-Z0-9#\.\[\]\-_\s,:>+~=""'()*]+$", ErrorMessage = "Invalid selector format.")]
    public string? WaitForSelector { get; set; }

    public ResourceBlockOptions BlockResources { get; set; }

    [MaxCollectionCount(20, ErrorMessage = "Maximum 30 headers allowed.")]
    public List<IHeaderModel> Headers { get; set; } = [];

    [MaxCollectionCount(15, ErrorMessage = "Maximum 20 cookies allowed.")]
    public List<ICookieModel> Cookies { get; set; } = [];
}