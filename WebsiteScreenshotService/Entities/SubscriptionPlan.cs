using System.Text.Json.Serialization;

namespace WebsiteScreenshotService.Entities;

/// <summary>
/// Represents a subscription plan with a type and the number of screenshots left.
/// </summary>
/// <param name="Type">The type of the subscription plan.</param>
/// <param name="ScreenshotLeft">The number of screenshots left in the subscription plan.</param>
public record SubscriptionPlan(SubscriptionType Type, long Points)
{
    /// <summary>
    /// Gets a regular subscription plan with a default number of screenshots.
    /// </summary>
    /// <returns>A regular subscription plan.</returns>
    public static SubscriptionPlan GetRegularSubscriptionPlan()
        => new(Type: SubscriptionType.Regular, Points: 250);

    public static SubscriptionPlan GetProSubscriptionPlan()
        => new(Type: SubscriptionType.Pro, Points: 5_000);

    public static SubscriptionPlan GetAdvancedSubscriptionPlan()
        => new(Type: SubscriptionType.Advanced, Points: 50_000);
}

/// <summary>
/// Defines the types of subscription plans.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionType
{
    /// <summary>
    /// Represents a regular subscription plan.
    /// </summary>
    Regular = 1,

    Pro = 2,

    Advanced = 3,
}
