using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;

namespace WebsiteScreenshotService.Repositories.Subscription;

public class SubscriptionRepository(ScreenshotDbContext context, IScreenshotManager screenshotManager) : ISubscriptionRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;

    public async Task<SubscriptionPlan?> GetUserSubscriptionAsync(Guid userId)
    {
        var plan = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.SubscriptionPlan)
            .FirstOrDefaultAsync();

        return plan is not null
            ? new(plan.Type, plan.ScreenshotLeft)
            : null;
    }

    public async Task<bool> CanMakeScreenshotAsync(Guid userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.SubscriptionPlan.ScreenshotLeft > 0)
            .FirstOrDefaultAsync();
    }

    public async Task<SubscriptionPlan?> ScreenshotWasMadeAsync(Guid userId)
    {
        var subscription = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.SubscriptionPlan)
            .FirstOrDefaultAsync();

        if (subscription is null)
            return null;

        if (subscription.ScreenshotLeft > 0)
        {
            subscription.ScreenshotLeft--;
            await _context.SaveChangesAsync();
        }

        return new(subscription.Type, subscription.ScreenshotLeft);
    }

    public async Task RedeemScreenshotAsync(string screenshotId, Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.SubscriptionPlan)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.SubscriptionPlan is null)
            return;

        user.SubscriptionPlan.ScreenshotLeft++;

        await _context.SaveChangesAsync();

        await _screenshotManager.UpdateStateAsync(
            screenshotId,
            ScreenshotState.Failed);
    }
}
