using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Model;

/// <summary>
/// Represents a user model with basic user information and subscription plan details.
/// </summary>
/// <param name="Name">The first name of the user.</param>
/// <param name="Surname">The surname of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="SubscriptionPlan">The subscription plan associated with the user.</param>
public record UserModel(string Name, string Surname, string Email, SubscriptionPlan SubscriptionPlan)
{
    /// <summary>
    /// Creates a <see cref="UserModel"/> instance from a <see cref="User"/> entity.
    /// </summary>
    /// <param name="user">The user entity to convert.</param>
    /// <returns>A new <see cref="UserModel"/> instance.</returns>
    public static UserModel GetModel(User user)
        => new(user.Name, user.Surname, user.Email, user.SubscriptionPlan);
}

