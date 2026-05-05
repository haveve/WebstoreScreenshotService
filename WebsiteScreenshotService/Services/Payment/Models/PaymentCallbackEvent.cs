namespace WebsiteScreenshotService.Services.Payment.Models;

public class PaymentCallbackInput
{
    public required string PaymentTransactionId { get; set; }

    public required Dictionary<string, string> Metadata { get; set; }
}

public class PaymentCallbackResult
{
    public required string PaymentTransactionId { get; set; }

    public Subscription? SubscriptionInfo { get; set; }

    public required string Provider { get; set; }

    public required PaymentCallbackStatus Status { get; set; }
}

public class Subscription
{
    public required string SubscriptionId { get; set; }

    public required DateTime? PeriodEnd { get; set; }
}


public enum PaymentCallbackStatus
{
    // =========================
    // 💳 PAYMENTS
    // =========================
    PaymentSucceeded,
    PaymentProcessing,
    PaymentFailed,
    PaymentCanceled,

    // =========================
    // 💸 REFUNDS
    // =========================
    RefundSucceeded,
    RefundFailed,

    // =========================
    // 🔁 SUBSCRIPTION BILLING
    // =========================
    SubscriptionPaymentSucceeded,   // invoice.payment_succeeded
    SubscriptionPaymentFailed,      // invoice.payment_failed
    SubscriptionPaymentActionRequired,

    // =========================
    // 🔄 SUBSCRIPTION LIFECYCLE
    // =========================
    SubscriptionCreated,
    SubscriptionActivated,
    SubscriptionUpdated,
    SubscriptionCanceled,
    SubscriptionPaused,
    SubscriptionResumed,
}
