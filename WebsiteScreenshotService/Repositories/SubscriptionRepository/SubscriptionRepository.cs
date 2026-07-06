using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Repositories.SubscriptionRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.Subscription;

public class SubscriptionRepository(ScreenshotDbContext context, IScreenshotManager screenshotManager) : ISubscriptionRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;

    public async Task<bool> HasActiveSubscriptionAsync(Guid userId)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.IsActive &&
                x.CurrentPeriodEnd > DateTime.UtcNow);
    }

    public async Task<SubscriptionEntity?> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.IsActive &&
                x.CurrentPeriodEnd > DateTime.UtcNow);
    }

    public async Task<Result<SubscriptionEntity>> GetByProviderIdAsync(string subscriptionId)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.ProviderSubscriptionId == subscriptionId);

        if (subscription is null)
            return Result<SubscriptionEntity>.Error("Subscription wasn't found");

        return Result<SubscriptionEntity>.Success(subscription);
    }

    public async Task<SubscriptionEntity> AddAsync(SubscriptionEntity subscription)
    {
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<Result> ProlongAsync(Guid subscriptionId, DateTime newPeriodEnd)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(x => x.Id == subscriptionId);

        if (subscription is null)
            return Result.Error("Subscription not found");

        subscription.CurrentPeriodEnd = newPeriodEnd;

        if (!subscription.IsActive)
            subscription.IsActive = true;   

        await _context.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<Result> CancelAndRemoveAsync(Guid subscriptionId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(x => x.Id == subscriptionId);

        if (subscription is null)
            return Result.Error("Subscription not found");

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == subscription.UserId);

        user?.SubscriptionPlan.Type = SubscriptionType.Regular;

        _context.Subscriptions.Remove(subscription);

        await _context.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<ConditionalResult> CanMakeScreenshotAsync(CanMakeScreenshotModel model, Guid userId)
    {
        var canMakeScreenshot = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.SubscriptionPlan.Points >= model.PointsCost)
            .FirstOrDefaultAsync();

        return ConditionalResult.Success(canMakeScreenshot);
    }

    public async Task<Result> ScreenshotWasMadeAsync(MakeScreenshotModel model, Guid userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user is null)
            return Result.Error("User does not exist");

        if (user.SubscriptionPlan.Points >= model.PointsCost)
        {
            user.SubscriptionPlan.Points -= model.PointsCost;
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }

    public async Task<Result> RedeemScreenshotAsync(RedeemScreenshotModel model, Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.SubscriptionPlan is null)
            return Result.Error("User does not exist");

        user.SubscriptionPlan.Points += model.PointsCost;

        await _context.SaveChangesAsync();

        await _screenshotManager.UpdateStateAsync(
            model.ScreenshotId,
            ScreenshotState.Failed);

        return Result.Success;
    }

    public async Task<Result> AddPointsAsync(int points, Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.SubscriptionPlan is null)
            return Result.Error("User does not exist");

        user.SubscriptionPlan.Points += points;

        await _context.SaveChangesAsync();

        return Result.Success;
    }
}
