using System.Security.Claims;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Services;

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
        yield return new(Constants.Claims.UserId, user.Id.ToString());
        yield return new(Constants.Claims.TokenType, Constants.Claims.TokenTypes.Authorization.ToString());
    }

    public static IEnumerable<Claim> GetConfirmationTokenClaims(this ConfirmationData confirmationData)
    {
        yield return new(Constants.Claims.UserId, confirmationData.UserId.ToString());
        yield return new(Constants.Claims.ScreenshotId, confirmationData.ScreenshotId);
        yield return new(Constants.Claims.ConfirmationTokenId, confirmationData.TokenId);
        yield return new(Constants.Claims.ScreenshotCost, confirmationData.PointsCost.ToString());
        yield return new(Constants.Claims.TokenType, Constants.Claims.TokenTypes.Confirmation.ToString());
    }
}

