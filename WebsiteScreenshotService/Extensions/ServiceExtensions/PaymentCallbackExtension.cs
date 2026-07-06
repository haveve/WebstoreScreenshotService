using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.OrderRepository;
using WebsiteScreenshotService.Repositories.PaymentRepository;
using WebsiteScreenshotService.Repositories.Subscription;
using WebsiteScreenshotService.Services.Payment;
using WebsiteScreenshotService.Services.Payment.Models;
using WebsiteScreenshotService.Services.Synchronization;

namespace WebsiteScreenshotService.Extensions.ServiceExtensions;

public class PaymentCallback { }

public static class PaymentCallbackExtension
{
    public static void UsePaymentCallback(this IApplicationBuilder app)
    {
        var paymentDataProcessor =
            app.ApplicationServices.GetRequiredService<IPaymentProviderDataProcessor>();

        var paymentProvider =
            app.ApplicationServices.GetRequiredService<IPaymentProvider>();

        var locks = new AsyncKeyedLocker<string>();

        app.Use(async (context, next) =>
        {
            if (!context.Request.Path.Equals("/payment-callback", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            var paymentAttemptRepository =
                app.ApplicationServices.GetRequiredService<IPaymentAttemptRepository>();

            var orderRepository =
                app.ApplicationServices.GetRequiredService<IOrderRepository>();

            var subscriptionRepository =
                app.ApplicationServices.GetRequiredService<ISubscriptionRepository>();

            var logger = app.ApplicationServices.GetRequiredService<ILogger<PaymentCallback>>();

            try
            {
                var result = await paymentDataProcessor.ProcessCallbackRequestDataAsync(context.Request);

                if (!result.IsSuccess)
                {
                    logger.LogWarning(result.ErrorMessage);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return;
                }

                using var _ = await locks.AcquireAsync(result.Value!.PaymentTransactionId);

                var attempt = await paymentAttemptRepository
                    .GetByProviderPaymentIdAsync(result.Value!.PaymentTransactionId);

                if (attempt is null)
                {
                    logger.LogWarning("PaymentAttempt not found {Id}", result.Value.PaymentTransactionId);
                    return;
                }

                var order = await orderRepository.GetAsync(attempt.OrderId);

                if (order is null)
                {
                    logger.LogWarning("Order not found {OrderId}", attempt.OrderId);
                    return;
                }

                var parsedRequest = result.Value!;
                var callbackResult = await paymentProvider.ProcessCallback(parsedRequest);

                context.Response.StatusCode = StatusCodes.Status200OK;

                if (callbackResult.Status == PaymentCallbackStatus.PaymentSucceeded)
                {
                    if (attempt.Status == PaymentAttemptStatus.Succeeded)
                        return;

                    await paymentAttemptRepository.MarkSucceededAsync(attempt.Id);

                    if (order.Status != OrderStatus.Paid)
                    {
                        await paymentAttemptRepository.MarkPrimaryAsync(attempt.Id);
                        await orderRepository.MarkPaidAsync(order.Id);
                        var points = order.Lines.Sum(x => x.Quantity);
                        await subscriptionRepository.AddPointsAsync(points, order.UserId);
                    }

                    return;
                }

                if (callbackResult.Status == PaymentCallbackStatus.PaymentFailed)
                {
                    if (attempt.Status == PaymentAttemptStatus.Failed)
                        return;

                    await paymentAttemptRepository.MarkFailedAsync(attempt.Id);
                    return;
                }

                if (callbackResult.SubscriptionInfo is null)
                    return;

                var subscriptionInfo = callbackResult.SubscriptionInfo;

                if (callbackResult.Status == PaymentCallbackStatus.SubscriptionUpdated)
                {
                    var subscription = await subscriptionRepository.GetByProviderIdAsync(subscriptionInfo.ProviderSubscriptionId);

                    if (!subscription.IsSuccess || !subscriptionInfo.PeriodEnd.HasValue)
                        return;

                    await subscriptionRepository.ProlongAsync(subscription.Value!.Id, subscriptionInfo.PeriodEnd.Value);
                }

                if (callbackResult.Status == PaymentCallbackStatus.SubscriptionCreated)
                {
                    if (!subscriptionInfo.PeriodEnd.HasValue)
                        return;

                    var subscriptionResult = await subscriptionRepository.GetByProviderIdAsync(subscriptionInfo.ProviderSubscriptionId);

                    if (!subscriptionResult.IsSuccess)
                        return;

                    var subscription = subscriptionResult.Value!;
                    await subscriptionRepository.ProlongAsync(subscription.Id, subscriptionInfo.PeriodEnd.Value);
                }

                if (callbackResult.Status == PaymentCallbackStatus.SubscriptionCanceled)
                {
                    var subscription = await subscriptionRepository.GetByProviderIdAsync(subscriptionInfo.ProviderSubscriptionId);

                    if (!subscription.IsSuccess)
                        return;

                    await subscriptionRepository.CancelAndRemoveAsync(subscription.Value!.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        });
    }
}
