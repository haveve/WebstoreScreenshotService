using WebsiteScreenshotService.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

public class HeaderModel: IHeaderModel
{
    /// <summary>
    /// HTTP header name.
    /// Example: Accept-Language
    /// Must contain only letters, digits, and dash.
    /// Prevents header injection attacks.
    /// </summary>
    [Required]
    [RegularExpressionWithTimeout(
        @"^[A-Za-z0-9\-]+$",
        ErrorMessage = "Header name contains invalid characters.")]
    [StringLength(100)] // optional limit to prevent abuse
    public string Name { get; set; } = default!;

    /// <summary>
    /// HTTP header value.
    /// Prevents CR/LF injection.
    /// Max length 4000.
    /// </summary>
    [Required]
    [SafeCookieString(1000)] // reuse SafeCookieString to prevent CR/LF injection
    public string Value { get; set; } = default!;
}
