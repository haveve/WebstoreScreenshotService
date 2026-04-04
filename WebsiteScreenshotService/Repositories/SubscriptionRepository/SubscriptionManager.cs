using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.Subscription;

public class SubscriptionManager(ISubscriptionRepository screenshotRepository, IUserContextAccessor userContextAccessor) : ISubscriptionManager
{
    private readonly ISubscriptionRepository _screenshotRepository = screenshotRepository;

    private readonly AsyncLockManager<Guid> _lockPerUser = new();

    private Task<IDisposable> EnterLockAsync(Guid userId, bool isRead = true)
    {
        return isRead
            ? _lockPerUser.EnterReadAsync(userId)
            : _lockPerUser.EnterWriteAsync(userId);
    }

    public async Task<SubscriptionPlan> GetUserSubscriptionAsync(Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await EnterLockAsync(userId);

        return await _screenshotRepository.GetUserSubscriptionAsync(userId)
            ?? throw new InvalidOperationException("User subscription not found.");
    }

    public async Task<bool> CanMakeScreenshotAsync(Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await EnterLockAsync(userId);
        return await _screenshotRepository.CanMakeScreenshotAsync(userId);
    }

    public async Task<Result<SubscriptionPlan>> ScreenshotWasMadeAsync(Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await EnterLockAsync(userId, isRead: false);

        if (!await _screenshotRepository.CanMakeScreenshotAsync(userId))
            return Result<SubscriptionPlan>.Error("You cannot make screenshot any more because you ran out of available screenshots");

        var subscriptionPlan = await _screenshotRepository.ScreenshotWasMadeAsync(userId)
            ?? throw new InvalidOperationException("User subscription not found.");

        return Result<SubscriptionPlan>.Success(subscriptionPlan);
    }

    public async Task RedeemScreenshotAsync(string screenshotId, Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await EnterLockAsync(userId, isRead: false);
        await _screenshotRepository.RedeemScreenshotAsync(screenshotId, userId);
    }
}
