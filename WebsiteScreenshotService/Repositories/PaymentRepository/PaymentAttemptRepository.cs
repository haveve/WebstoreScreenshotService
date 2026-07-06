using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.EF;

namespace WebsiteScreenshotService.Repositories.PaymentRepository;

public class PaymentAttemptRepository(ScreenshotDbContext context) : IPaymentAttemptRepository
{
    private readonly ScreenshotDbContext _context = context;

    public async Task<PaymentAttemptEntity> CreateAsync(PaymentAttemptEntity attempt)
    {
        _context.PaymentAttempts.Add(attempt);
        await _context.SaveChangesAsync();
        return attempt;
    }

    public async Task<PaymentAttemptEntity?> GetAsync(Guid id)
    {
        return await _context.PaymentAttempts
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PaymentAttemptEntity?> GetByProviderPaymentIdAsync(string providerPaymentId)
    {
        return await _context.PaymentAttempts
            .FirstOrDefaultAsync(x => x.ProviderPaymentId == providerPaymentId);
    }

    public async Task SetProviderPaymentIdAsync(Guid attemptId, string providerPaymentId)
    {
        var attempt = await GetAsync(attemptId);

        if (attempt is null)
            throw new InvalidOperationException("Payment attempt not found");

        attempt.ProviderPaymentId = providerPaymentId;

        await _context.SaveChangesAsync();
    }

    public async Task MarkPrimaryAsync(Guid attemptId)
    {
        var attempt = await GetAsync(attemptId);

        if (attempt is null)
            throw new InvalidOperationException("Payment attempt not found");

        attempt.IsPrimary = true;

        await _context.SaveChangesAsync();
    }

    public async Task MarkRefundedAsync(Guid attemptId)
    {
        var attempt = await GetAsync(attemptId);

        if (attempt is null)
            throw new InvalidOperationException("Payment attempt not found");

        attempt.Status = PaymentAttemptStatus.Refunded;
        attempt.Refunded = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task MarkSucceededAsync(Guid attemptId)
    {
        var attempt = await GetAsync(attemptId);

        if (attempt is null)
            throw new InvalidOperationException("Payment attempt not found");

        attempt.Status = PaymentAttemptStatus.Succeeded;

        await _context.SaveChangesAsync();
    }

    public async Task MarkFailedAsync(Guid attemptId)
    {
        var attempt = await GetAsync(attemptId);

        if (attempt is null)
            throw new InvalidOperationException("Payment attempt not found");

        attempt.Status = PaymentAttemptStatus.Failed;

        await _context.SaveChangesAsync();
    }

    public async Task<int> CountPrimaryRefundsAsync(Guid userId, DateTime since)
    {
        return await _context.PaymentAttempts
            .Where(x =>
                x.UserId == userId &&
                x.IsPrimary &&
                x.Status == PaymentAttemptStatus.Refunded &&
                x.Refunded >= since)
            .CountAsync();
    }
}
