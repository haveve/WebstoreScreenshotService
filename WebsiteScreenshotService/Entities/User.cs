namespace WebsiteScreenshotService.Entities;

/// <summary>
/// Represents a user entity with basic user information and subscription plan details.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The first name of the user.</param>
/// <param name="Surname">The surname of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="Password">The password of the user.</param>
/// <param name="SubscriptionPlan">The subscription plan associated with the user.</param>
public record User(Guid Id, string Name, string Surname, string Email, string Password, SubscriptionPlan SubscriptionPlan);
