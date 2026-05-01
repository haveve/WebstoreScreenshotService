using System.Text.Json.Serialization;

namespace WebsiteScreenshotService.Model.ScreeshotModel.Components;

public class CookieModel
{
    /// <summary>
    /// Cookie name.
    /// Must be alphanumeric + underscore or dash.
    /// Prevents injection attacks.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Cookie value.
    /// Printable characters only, no CR/LF.
    /// Prevents JS or header injection attacks.
    /// </summary>
    public string Value { get; set; } = default!;

    /// <summary>
    /// Domain that can receive the cookie.
    /// Must start with a dot, RFC 6265 compliant.
    /// Prevents header injection.
    /// </summary>
    public string Domain { get; set; } = default!;

    /// <summary>
    /// Cookie path. Default "/".
    /// Must be RFC 6265 compliant.
    /// </summary>
    public string Path { get; set; } = "/";

    public DateTime? Expires { get; set; }

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
