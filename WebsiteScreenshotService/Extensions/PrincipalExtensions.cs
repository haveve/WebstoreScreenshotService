using System.Security.Claims;

namespace WebsiteScreenshotService.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ClaimsPrincipal"/> and <see cref="HttpContext"/> classes.
/// </summary>
public static class PrincipalExtensions
{
    /// <summary>
    /// Updates or adds a claim in the current user's identity.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="key">The key of the claim to update or add.</param>
    /// <param name="value">The value of the claim to update or add.</param>
    public static void UpdateClaim(this HttpContext httpContext, string key, string value)
    {
        if (httpContext.User.Identity is not ClaimsIdentity identity)
            return;

        var existingClaim = identity.FindFirst(key);
        if (existingClaim != null)
            identity.RemoveClaim(existingClaim);

        identity.AddClaim(new Claim(key, value));
        httpContext.User = new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Gets the value of a specific claim from the current principal.
    /// </summary>
    /// <param name="currentPrincipal">The current claims principal.</param>
    /// <param name="key">The key of the claim to retrieve.</param>
    /// <returns>The value of the claim if found; otherwise, null.</returns>
    public static string? GetClaimValue(this ClaimsPrincipal currentPrincipal, string key)
    {
        if (currentPrincipal.Identity is not ClaimsIdentity identity)
            return null;

        var claim = identity.Claims.FirstOrDefault(c => c.Type == key);

        return claim?.Value;
    }

    /// <summary>
    /// Gets the user ID from the current principal's claims.
    /// </summary>
    /// <param name="currentPrincipal">The current claims principal.</param>
    /// <returns>The user ID if found and valid; otherwise, null.</returns>
    public static Guid? GetUserId(this ClaimsPrincipal currentPrincipal)
        => Guid.TryParse(currentPrincipal.GetClaimValue(ClaimTypes.NameIdentifier), out Guid Id) ? Id : null;

    /// <summary>
    /// Gets the user name from the current principal's claims.
    /// </summary>
    /// <param name="currentPrincipal">The current claims principal.</param>
    /// <returns>The user name if found; otherwise, null.</returns>
    public static string? GetUserName(this ClaimsPrincipal currentPrincipal)
        => currentPrincipal.GetClaimValue(ClaimTypes.Name);

    /// <summary>
    /// Gets the user email from the current principal's claims.
    /// </summary>
    /// <param name="currentPrincipal">The current claims principal.</param>
    /// <returns>The user email if found; otherwise, null.</returns>
    public static string? GetUserEmail(this ClaimsPrincipal currentPrincipal)
        => currentPrincipal.GetClaimValue(ClaimTypes.Email);
}

