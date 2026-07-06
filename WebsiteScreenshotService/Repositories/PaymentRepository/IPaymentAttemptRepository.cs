using WebsiteScreenshotService.Repositories._EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.PaymentRepository;

public interface IPaymentAttemptRepository
{
    Task<PaymentAttemptEntity> CreateAsync(PaymentAttemptEntity attempt);

    Task<PaymentAttemptEntity?> GetAsync(Guid id);

    Task<PaymentAttemptEntity?> GetByProviderPaymentIdAsync(string providerPaymentId);

    Task SetProviderPaymentIdAsync(Guid attemptId, string providerPaymentId);

    Task MarkSucceededAsync(Guid attemptId);

    Task MarkRefundedAsync(Guid attemptId);

    Task MarkFailedAsync(Guid attemptId);

    Task MarkPrimaryAsync(Guid attemptId);

    Task<int> CountPrimaryRefundsAsync(Guid userId, DateTime since);
}