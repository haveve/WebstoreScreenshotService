namespace WebsiteScreenshotService.Services.Payment.Stripe;

public static class StripeConstants
{
    public const string BaseUrl = "https://api.stripe.com/v1/";

    public const string PaymentIntents = "payment_intents";
    public const string Subscriptions = "subscriptions";
    public const string Customers = "customers";
    public const string Refunds = "refunds";
    public const string Coupons = "coupons";

    // METADATA KEYS
    public const string MetadataType = "type";
    public const string MetadataSubscriptionId = "subscription_id";
    public const string MetadataPeriodEnd = "period_end";
    public const string MetadataPaymentIntent = "payment_intent";
    public const string MetadataProviderCustomerId = "ProviderCustomerId";
    public const string MetadataClientSecret = "MetadataClientSecret";


    public static class Events
    {
        // PaymentIntent
        public const string PaymentSucceeded = "payment_intent.succeeded";
        public const string PaymentFailed = "payment_intent.payment_failed";
        public const string PaymentProcessing = "payment_intent.processing";
        public const string PaymentCanceled = "payment_intent.canceled";

        // Invoice (subscriptions)
        public const string InvoiceSucceeded = "invoice.payment_succeeded";
        public const string InvoiceFailed = "invoice.payment_failed";
        public const string InvoiceUpcoming = "invoice.upcoming";
        public const string InvoiceActionRequired = "invoice.payment_action_required";

        // Subscription lifecycle
        public const string SubscriptionCreated = "customer.subscription.created";
        public const string SubscriptionUpdated = "customer.subscription.updated";
        public const string SubscriptionDeleted = "customer.subscription.deleted";
        public const string SubscriptionPaused = "customer.subscription.paused";
        public const string SubscriptionResumed = "customer.subscription.resumed";

        // Refunds
        public const string RefundCreated = "refund.created";
        public const string ChargeRefunded = "charge.refunded";
        public const string RefundFailed = "refund.failed";
    }

    public static readonly HashSet<string> SupportedEvents = [
        // payments
        Events.PaymentSucceeded,
        Events.PaymentFailed,
        Events.PaymentProcessing,
        Events.PaymentCanceled,

        // invoices
        Events.InvoiceSucceeded,
        Events.InvoiceFailed,
        Events.InvoiceUpcoming,
        Events.InvoiceActionRequired,

        // subscriptions
        Events.SubscriptionCreated,
        Events.SubscriptionUpdated,
        Events.SubscriptionDeleted,
        Events.SubscriptionPaused,
        Events.SubscriptionResumed,

        // refunds
        Events.RefundCreated,
        Events.ChargeRefunded,
        Events.RefundFailed
    ];

    public static class EventGroups
    {
        public static readonly HashSet<string> PaymentEvents = [
            Events.PaymentSucceeded,
            Events.PaymentFailed,
            Events.PaymentProcessing,
            Events.PaymentCanceled
        ];

        public static readonly HashSet<string> SubscriptionEvents = [
            Events.SubscriptionCreated,
            Events.SubscriptionUpdated,
            Events.SubscriptionDeleted,
            Events.SubscriptionPaused,
            Events.SubscriptionResumed,
            Events.InvoiceSucceeded,
            Events.InvoiceFailed
        ];

        public static readonly HashSet<string> RefundEvents = [
            Events.RefundCreated,
            Events.ChargeRefunded,
            Events.RefundFailed
        ];
    }
}
