namespace Shared.Core.Validation.Attributes;

/// <summary>
/// Validates cookie domain strictly (RFC 6265).
/// </summary>
public class SafeCookieDomainAttribute : SafeCookieStringAttribute
{
    private static readonly string DomainPattern =
        @"^(\.[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*)$"; // must start with dot, valid domain chars

    public SafeCookieDomainAttribute(int maxLength)
        : base(maxLength, DomainPattern)
    {
    }
}
