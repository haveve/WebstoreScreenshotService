namespace WebsiteScreenshotService.Services.Payment;

using WebsiteScreenshotService.Services.Payment.Models;

public interface IPaymentProvider
{
    string ProviderName { get; }

    Task<StartPaymentResult> StartPayment(StartPaymentRequest request);

    Task<RefundResult> Refund(RefundRequest request);

    Task<PaymentCallbackResult> ProcessCallback(PaymentCallbackInput paymentCallbackInput);

    Task<CreateSubscriptionResult> CreateSubscription(CreateSubscriptionRequest request);

    Task<ChangeSubscriptionPlanResult> ChangeSubscriptionPlan(ChangeSubscriptionPlanRequest request);

    Task<CancelSubscriptionResult> CancelSubscription(CancelSubscriptionRequest request);
}
