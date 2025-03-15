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
    public static List<Claim> GetUserClaims(this User user)
    {
        return new() {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
            };
    }
}

