namespace WebsiteScreenshotService.Services.Payment;

using WebsiteScreenshotService.Services.Payment.Models;

public interface IPaymentProvider
{
    public string ProviderName { get; }

    Task<bool> TestAsync();

    Task<StartPaymentResult> StartPayment(StartPaymentRequest request);

    Task<RefundResult> Refund(RefundRequest request);

    Task<PaymentCallbackResult> ProcessCallback(PaymentCallbackInput paymentCallbackInput);

    Task<CreateSubscriptionResult> CreateSubscription(CreateSubscriptionRequest request);

    Task<CancelSubscriptionResult> CancelSubscription(CancelSubscriptionRequest request);
}
