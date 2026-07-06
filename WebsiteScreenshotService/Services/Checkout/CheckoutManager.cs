using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.BasketRepository;
using WebsiteScreenshotService.Repositories.OrderRepository;
using WebsiteScreenshotService.Repositories.PaymentRepository;
using WebsiteScreenshotService.Repositories.Subscription;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services.Payment;
using WebsiteScreenshotService.Services.Payment.Models;
using WebsiteScreenshotService.Services.Payment.Stripe;
using WebsiteScreenshotService.Services.Synchronization;

namespace WebsiteScreenshotService.Services.Checkout;

public class CheckoutManager(
    IBasketRepository basketRepository,
    IOrderRepository orderRepository,
    IOrderManager orderManager,
    IPaymentAttemptRepository paymentAttemptRepository,
    IPaymentProvider paymentProvider,
    IUserContextAccessor userContextAccessor,
    IUserManager _userManager,
    ISubscriptionRepository subscriptionManager,
    ILogger<CheckoutManager> logger) : ICheckoutManager
{
    private readonly AsyncKeyedLocker<Guid> _locks = new();

    private readonly AsyncKeyedLocker<Guid> _retryLocks = new();

    private readonly AsyncKeyedLocker<Guid> _refundLocks = new();

    private Task<IDisposable> Lock(Guid userId)
        => _locks.AcquireAsync(userId);

    private Task<IDisposable> RetryLock(Guid orderId)
        => _retryLocks.AcquireAsync(orderId);

    private Task<IDisposable> RefundLock(Guid transactionId)
        => _refundLocks.AcquireAsync(transactionId);

    public async Task<StartCheckoutResult> StartCheckoutAsync(StartCheckoutRequest request)
    {
        var userId = request.UserId ?? userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await Lock(userId);

        var order = await orderManager.CreateFromBasketAsync();

        var money = new Money(order.TotalAmount, order.Currency);

        var attempt = await paymentAttemptRepository.CreateAsync(new PaymentAttemptEntity
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            UserId = userId,
            Amount = money.Amount,
            Provider = paymentProvider.ProviderName,
            Status = PaymentAttemptStatus.Pending,
            IsPrimary = true,
            CreatedAt = DateTime.UtcNow
        });

        var payment = await paymentProvider.StartPayment(new StartPaymentRequest
        {
            OrderId = order.Id,
            UserId = userId.ToString(),
            Amount = money
        });

        if (payment.Status == StartPaymentStatus.Failed)
            throw new InvalidOperationException("Payment initialization failed");

        await paymentAttemptRepository.SetProviderPaymentIdAsync(
            attempt.Id,
            payment.PaymentId
        );

        await basketRepository.ClearAsync(userId);

        return new StartCheckoutResult
        {
            OrderId = order.Id,
            PaymentAttemptId = attempt.Id,
            Metadata = payment.Metadata
        };
    }

    public async Task<StartCheckoutResult> RetryPaymentAsync(Guid orderId)
    {
        var userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await RetryLock(orderId);

        var order = await orderRepository.GetAsync(orderId);

        if (order is null || order.UserId != userId)
            throw new InvalidOperationException("Order not found");

        if (order.Status == OrderStatus.Paid)
            throw new InvalidOperationException("Order already paid");

        var attempt = await paymentAttemptRepository.CreateAsync(new PaymentAttemptEntity
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            UserId = userId,
            Amount = order.TotalAmount,
            Provider = paymentProvider.ProviderName,
            Status = PaymentAttemptStatus.Pending,
            IsPrimary = false,
            CreatedAt = DateTime.UtcNow
        });

        var payment = await paymentProvider.StartPayment(new StartPaymentRequest
        {
            OrderId = order.Id,
            UserId = userId.ToString(),
            Amount = new Money(order.TotalAmount, order.Currency)
        });

        await paymentAttemptRepository.SetProviderPaymentIdAsync(
            attempt.Id,
            payment.PaymentId
        );

        return new StartCheckoutResult
        {
            OrderId = order.Id,
            PaymentAttemptId = attempt.Id,
            Metadata = payment.Metadata
        };
    }

    public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest request)
    {
        var userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await RefundLock(userId);

        var attempt = await paymentAttemptRepository.GetAsync(request.PaymentAttemptId);

        if (attempt is null || attempt.UserId != userId)
            throw new InvalidOperationException("Payment attempt not found");

        if (attempt.Status == PaymentAttemptStatus.Refunded)
            throw new InvalidOperationException("Already refunded");

        if (string.IsNullOrWhiteSpace(attempt.ProviderPaymentId))
            throw new InvalidOperationException("Missing provider payment id");

        var refund = await paymentProvider.Refund(new RefundRequest
        {
            PaymentTransactionId = attempt.ProviderPaymentId
        });

        if (refund.Status != RefundStatus.Succeeded)
            throw new InvalidOperationException("Refund failed");

        await paymentAttemptRepository.MarkRefundedAsync(attempt.Id);

        var primaryRefundCount = await paymentAttemptRepository.CountPrimaryRefundsAsync(
            userId,
            since: DateTime.UtcNow.AddMonths(-6));

        if (primaryRefundCount >= 3)
        {
            var result = await _userManager.DisableUserAsync(userId);
            if (!result.IsSuccess)
                logger.LogError(result.ErrorMessage);
        }

        return new RefundPaymentResult
        {
            PaymentAttemptId = attempt.Id,
            Success = true
        };
    }

    public async Task<StartSubscriptionResult> StartSubscriptionAsync(
        StartSubscriptionRequest request)
    {
        var userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await Lock(userId);

        var currentSubscription = await subscriptionManager.GetActiveByUserIdAsync(userId);

        if (currentSubscription is not null)
            throw new InvalidOperationException(
                "User already has an active subscription");

        var result = await paymentProvider.CreateSubscription(
            new CreateSubscriptionRequest
            {
                UserId = userId.ToString(),
                SubscriptionInfo = request.SubscriptionInfo,
                Metadata = []
            });

        if (result.Status is not CreateSubscriptionStatus.Created and not CreateSubscriptionStatus.AlreadyExistsIncomplete)
        {
            throw new InvalidOperationException(
                "Failed to initialize subscription");
        }

        if (!result.Metadata.TryGetValue(
                StripeConstants.MetadataClientSecret,
                out var clientSecret))
        {
            throw new InvalidOperationException(
                "Client secret was not returned");
        }

        return new StartSubscriptionResult
        {
            ProviderSubscriptionId = result.SubscriptionId,
            ClientSecret = clientSecret
        };
    }
}

//var basket = await basketRepository.GetByUserIdAsync(userId);

//if (basket is null || basket.Lines.Count == 0)
//    throw new InvalidOperationException("Basket is empty");

//var products = await productManager.GetProductsAsync(
//    new ProductFilterModel(
//        ProductTypes: [],
//        Ids: [.. basket.Lines.Select(x => x.ProductId)]
//    )
//);

//if (!products.IsSuccess)
//    throw new InvalidOperationException(products.ErrorMessage);

//if (products.Value!.Any(p => p.Type != ProductType.Points))
//    throw new InvalidOperationException("Only points allowed");

//var totalPoints = basket.Lines.Sum(x => x.Quantity);
//var priceResult = await priceManager.GetPointsPriceAsync(totalPoints);

//if (!priceResult.IsSuccess)
//    throw new InvalidOperationException(priceResult.ErrorMessage);

//var money = priceResult.Value!;

//var orderId = Guid.CreateVersion7();

//var order = await orderRepository.CreateAsync(new OrderEntity
//{
//    Id = orderId,
//    UserId = userId,
//    TotalAmount = money.Amount,
//    Currency = money.Currency,
//    Status = OrderStatus.PendingPayment,
//    CreatedAt = DateTime.UtcNow,
//    Lines = basket.Lines.Select(l => new OrderLineEntity
//    {
//        Id = Guid.CreateVersion7(),
//        OrderId = orderId,
//        ProductId = l.ProductId,
//        Quantity = l.Quantity,
//        UnitPrice = money.Amount,
//        ProductName = "Points"
//    }).ToList()
//});
