using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.Subscription;

public class SubscriptionRepository(ScreenshotDbContext context, IScreenshotManager screenshotManager) : ISubscriptionRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;

    public async Task<ConditionalResult> CanMakeScreenshotAsync(Guid userId)
    {
        var canMakeScreenshot = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.SubscriptionPlan.ScreenshotLeft > 0)
            .FirstOrDefaultAsync();

        return ConditionalResult.Success(canMakeScreenshot);
    }

    public async Task<Result> ScreenshotWasMadeAsync(Guid userId)
    {
        var subscription = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.SubscriptionPlan)
            .FirstOrDefaultAsync();

        if (subscription is null)
            return Result.Error("User does not exist");

        if (subscription.ScreenshotLeft > 0)
        {
            subscription.ScreenshotLeft--;
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }

    public async Task<Result> RedeemScreenshotAsync(string screenshotId, Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.SubscriptionPlan)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.SubscriptionPlan is null)
            return Result.Error("User does not exist"); ;

        user.SubscriptionPlan.ScreenshotLeft++;

        await _context.SaveChangesAsync();

        await _screenshotManager.UpdateStateAsync(
            screenshotId,
            ScreenshotState.Failed);

        return Result.Success;
    }
}
