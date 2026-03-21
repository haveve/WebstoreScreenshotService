using ScreenshotWorker.Utils.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ScreenshotWorker.Model.ScreenshotOptions;

public class CookieModel
{
    /// <summary>
    /// Cookie name.
    /// Must be alphanumeric + underscore or dash.
    /// Prevents injection attacks.
    /// </summary>
    [Required]
    [SafeCookieString(50, @"^[a-zA-Z0-9_\-]+$")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Cookie value.
    /// Printable characters only, no CR/LF.
    /// Prevents JS or header injection attacks.
    /// </summary>
    [Required]
    [SafeCookieString(200)]
    public string Value { get; set; } = default!;

    /// <summary>
    /// Domain that can receive the cookie.
    /// Must start with a dot, RFC 6265 compliant.
    /// Prevents header injection.
    /// </summary>
    [Required]
    [SafeCookieDomain]
    public string Domain { get; set; } = default!;

    /// <summary>
    /// Cookie path. Default "/".
    /// Must be RFC 6265 compliant.
    /// </summary>
    [SafeCookiePath]
    public string Path { get; set; } = "/";

    /// <summary>
    /// Unix timestamp (seconds) when cookie expires.
    /// Null means session cookie.
    /// </summary>
    public float? Expires { get; set; }

    /// <summary>
    /// Send cookie only via HTTPS.
    /// </summary>
    public bool Secure { get; set; }

    /// <summary>
    /// Prevent client-side JS access.
    /// </summary>
    public bool HttpOnly { get; set; }

    /// <summary>
    /// SameSite policy.
    /// Strict / Lax / None
    /// </summary>
    public SameSiteMode? SameSite { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SameSiteMode
{
    Strict,
    Lax,
    None
}
