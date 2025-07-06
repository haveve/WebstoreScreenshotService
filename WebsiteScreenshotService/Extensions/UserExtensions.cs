using System.Security.Claims;
using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="User"/> class.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Gets a list of claims for the specified user.
    /// </summary>
    /// <param name="user">The user to get claims for.</param>
    /// <returns>A list of <see cref="Claim"/> objects representing the user's claims.</returns>
    public static IEnumerable<Claim> GetUserClaims(this User user)
    {
        yield return new(ClaimTypes.NameIdentifier, user.Id.ToString());
    }
}

