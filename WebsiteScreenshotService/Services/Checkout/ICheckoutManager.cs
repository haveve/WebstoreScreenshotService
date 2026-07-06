using System.Collections.ObjectModel;
using WebsiteScreenshotService.Services.Payment.Models;

namespace WebsiteScreenshotService.Services.Checkout;

public interface ICheckoutManager
{
    Task<StartCheckoutResult> StartCheckoutAsync(StartCheckoutRequest request);

    Task<StartCheckoutResult> RetryPaymentAsync(Guid orderId);

    Task<StartSubscriptionResult> StartSubscriptionAsync(StartSubscriptionRequest request);

    Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest request);
}

public class StartCheckoutResult
{
    public Guid OrderId { get; init; }

    public Guid PaymentAttemptId { get; init; }

    public ReadOnlyDictionary<string, string> Metadata { get; init; } = default!;
}

public class StartCheckoutRequest
{
    public Guid? UserId { get; init; }
}

public sealed record StartSubscriptionRequest
{
    public required SubscriptionInfo SubscriptionInfo { get; init; }
}

public sealed record StartSubscriptionResult
{
    public required string ProviderSubscriptionId { get; init; }

    public required string ClientSecret { get; init; }
}

public class RefundPaymentRequest
{
    public Guid PaymentAttemptId { get; set; }
}

public class RefundPaymentResult
{
    public Guid PaymentAttemptId { get; set; }
    public bool Success { get; set; }
}