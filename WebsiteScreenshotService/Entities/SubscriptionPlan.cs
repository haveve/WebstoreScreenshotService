namespace WebsiteScreenshotService.Entities;

/// <summary>
/// Represents a subscription plan with a type and the number of screenshots left.
/// </summary>
/// <param name="Type">The type of the subscription plan.</param>
/// <param name="ScreenshotLeft">The number of screenshots left in the subscription plan.</param>
public record SubscriptionPlan(SubscriptionType Type, int ScreenshotLeft)
{
    /// <summary>
    /// Gets the number of screenshots left in the subscription plan.
    /// </summary>
    public int ScreenshotLeft { get; private set; } = ScreenshotLeft;

    /// <summary>
    /// Gets a regular subscription plan with a default number of screenshots.
    /// </summary>
    /// <returns>A regular subscription plan.</returns>
    public static SubscriptionPlan GetRegularSubscriptionPlan()
        => new(Type: SubscriptionType.Regular, ScreenshotLeft: 50);

    /// <summary>
    /// Determines whether the user can make a screenshot based on the remaining screenshots.
    /// </summary>
    /// <returns><c>true</c> if the user can make a screenshot; otherwise, <c>false</c>.</returns>
    public bool CanMakeScreenshot() => ScreenshotLeft > 0;

    /// <summary>
    /// Decrements the number of screenshots left by one.
    /// </summary>
    public void ScreenshotWasMade() => ScreenshotLeft--;
}

/// <summary>
/// Defines the types of subscription plans.
/// </summary>
public enum SubscriptionType
{
    /// <summary>
    /// Represents a regular subscription plan.
    /// </summary>
    Regular = 1,

    Pro = 2,

    Advanced = 3,
}
