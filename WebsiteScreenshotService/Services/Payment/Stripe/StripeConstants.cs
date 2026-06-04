namespace WebsiteScreenshotService.Services.Payment.Stripe;

public static class StripeConstants
{
    public static class Settings 
    {
        public static class Sensitive
        {
            public const string Secret = "Secret";
        }

        public static class Ordinal
        {
            public const string Url = "Url";
        }
    }

    public const string ProviderName = "Stripe";

    public const string PaymentIntents = "payment_intents";
    public const string Subscriptions = "subscriptions";
    public const string Customers = "customers";
    public const string Refunds = "refunds";

    public const string MetadataType = "type";
    public const string MetadataSubscriptionId = "subscription_id";
    public const string MetadataPeriodEnd = "period_end";
    public const string MetadataProviderCustomerId = "provider_customer_id";
    public const string MetadataClientSecret = "client_secret";

    public static class Events
    {
        public const string PaymentSucceeded = "payment_intent.succeeded";
        public const string PaymentFailed = "payment_intent.payment_failed";
        public const string PaymentProcessing = "payment_intent.processing";
        public const string PaymentCanceled = "payment_intent.canceled";

        public const string SubscriptionCreated = "customer.subscription.created";
        public const string SubscriptionUpdated = "customer.subscription.updated";
        public const string SubscriptionDeleted = "customer.subscription.deleted";

        public const string RefundCreated = "refund.created";
        public const string ChargeRefunded = "charge.refunded";
    }
}
