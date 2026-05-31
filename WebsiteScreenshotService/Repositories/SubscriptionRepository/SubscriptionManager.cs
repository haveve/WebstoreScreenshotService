using Shared.Core.Utils;
using WebsiteScreenshotService.Repositories.SubscriptionRepository.Models;
using WebsiteScreenshotService.Services.Caching;
using WebsiteScreenshotService.Services.Caching.Services;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.Subscription;

public class SubscriptionManager(
    ISubscriptionRepository screenshotRepository,
    IUserContextAccessor userContextAccessor,
    ICacheManager cacheManager,
    IUserCacheService userCache) : ISubscriptionManager
{
    private readonly ISubscriptionRepository _screenshotRepository = screenshotRepository;

    private readonly AsyncReadWriteLockManager<Guid> _lockPerUser = new();

    private readonly ICacheManager _cacheManager = cacheManager;

    private readonly IUserCacheService _userCache = userCache;

    private Task<IDisposable> EnterLockAsync(Guid userId, bool isRead = true)
    {
        return isRead
            ? _lockPerUser.EnterReadAsync(userId)
            : _lockPerUser.EnterWriteAsync(userId);
    }

    public async Task<ConditionalResult> CanMakeScreenshotAsync(CanMakeScreenshotModel model, Guid? userId = null)
    {
        userId ??= userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await EnterLockAsync(userId.Value);
        return await _screenshotRepository.CanMakeScreenshotAsync(model, userId.Value);
    }

    public async Task<Result> ScreenshotWasMadeAsync(MakeScreenshotModel model, Guid? userId = null)
    {
        userId ??= userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await EnterLockAsync(userId.Value, isRead: false);

        var result = await _screenshotRepository.CanMakeScreenshotAsync(new(model.PointsCost), userId.Value);

        if (!result.IsSuccess)
            return Result.Error(result.ErrorMessage!);

        if (!result.Value!)
            return Result.Error("You cannot make screenshot any more because you ran out of available screenshots");

        var subscription = await _screenshotRepository.ScreenshotWasMadeAsync(model, userId.Value);

        if (!subscription.IsSuccess)
            return Result.Error(subscription.ErrorMessage!);

        await InvalidateUserCacheAsync(userId.Value);

        return Result.Success;
    }

    public async Task<Result> RedeemScreenshotAsync(RedeemScreenshotModel model, Guid? userId = null)
    {
        userId ??= userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await EnterLockAsync(userId.Value, isRead: false);
        return await _screenshotRepository.RedeemScreenshotAsync(model, userId.Value);
    }

    private async Task InvalidateUserCacheAsync(Guid userId)
    {
        await _cacheManager.RemoveAsync(_userCache.Profile(userId));
    }
}
