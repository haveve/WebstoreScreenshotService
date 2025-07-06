using System.Collections.Concurrent;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Utils.Synchronization;

namespace WebsiteScreenshotService.Repositories;

public class SubscriptionManager(ISubscriptionRepository screenshotRepository, IUserContextAccessor userContextAccessor) : ISubscriptionManager
{
    private readonly ISubscriptionRepository _screenshotRepository = screenshotRepository;

    private readonly ConcurrentDictionary<Guid, AsyncSlimReaderWriterLock> LockPerUser = new();

    private Task<IDisposable> EnterLockAsync(Guid userId, bool isRead = true)
    {
        var userLock = LockPerUser.GetOrAdd(userId, _ => new AsyncSlimReaderWriterLock());
        return isRead
            ? userLock.EnterReadLockAsync()
            : userLock.EnterWriteLockAsync();
    }

    public async Task<SubscriptionPlan> GetUserSubscriptionAsync(Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().Id;

        using var _ = await EnterLockAsync(userId);

        return await _screenshotRepository.GetUserSubscriptionAsync(userId)
            ?? throw new InvalidOperationException("User subscription not found.");
    }

    public async Task<bool> CanMakeScreenshotAsync(Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().Id;

        using var _ = await EnterLockAsync(userId);
        return await _screenshotRepository.CanMakeScreenshotAsync(userId);
    }

    public async Task<SubscriptionPlan> ScreenshotWasMadeAsync(Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().Id;

        using var _ = await EnterLockAsync(userId, isRead: false);

        return await _screenshotRepository.ScreenshotWasMadeAsync(userId)
            ?? throw new InvalidOperationException("User subscription not found.");
    }

    public async Task IncrementScreenshotCountAsync(Guid userId = default)
    {
        if (userId == default)
            userId = userContextAccessor.GetCurrentUser().Id;

        using var _ = await EnterLockAsync(userId, isRead: false);
        await _screenshotRepository.IncrementScreenshotCountAsync(userId);
    }
}
