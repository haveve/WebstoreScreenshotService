using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Core.Services;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services.Admin.Models;
using WebsiteScreenshotService.Services.Payment;
using WebsiteScreenshotService.Services.Security;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services.Admin;

public sealed class AdminService(
    ScreenshotDbContext db,
    IUserManager userManager,
    IUserEntityMapper userEntityMapper,
    IOptions<BlobConfigurations> blobConfigurations,
    IBusControl bus,
    IPaymentProvider paymentProvider,
    IHashingService hashingService)
    : IAdminService
{
    private readonly ScreenshotDbContext _db = db;
    private readonly IUserManager _userManager = userManager;
    private readonly IUserEntityMapper _userEntityMapper = userEntityMapper;
    private readonly BlobConfigurations _blobConfigurations = blobConfigurations.Value;
    private readonly IPaymentProvider _paymentProvider = paymentProvider;
    private readonly IBusControl _bus = bus;
    private readonly IHashingService _hashingService = hashingService;

    public async Task<Result<AdminPagedResult<AdminUserModel>>> GetUsersAsync(AdminUserQuery query)
    {
        var fromDate = DateTime.UtcNow.AddMonths(-6);

        var nickHash = !string.IsNullOrWhiteSpace(query.NickName)
            ? _hashingService.Hash(query.NickName)
            : null;

        var usersQuery = _db.Users
            .AsNoTracking();

        if (query.IsDisabled.HasValue)
            usersQuery = usersQuery.Where(u => u.IsDisactivated == query.IsDisabled.Value);

        if (nickHash is not null)
            usersQuery = usersQuery.Where(u => u.NickNameHash == nickHash);

        var queryWithRefunds =
            from u in usersQuery
            join p in _db.PaymentAttempts.AsNoTracking()
                .Where(p =>
                    p.Status == PaymentAttemptStatus.Refunded &&
                    p.IsPrimary &&
                    p.Refunded >= fromDate)
            on u.Id equals p.UserId into refunds

            select new
            {
                User = u,
                RefundCount = refunds.Count()
            };

        if (query.RefundCount.HasValue)
        {
            queryWithRefunds = queryWithRefunds
                .Where(x => x.RefundCount == query.RefundCount.Value);
        }

        var total = await queryWithRefunds.CountAsync();

        var users = await queryWithRefunds
            .OrderByDescending(x => x.User.CreatedAt)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new
            {
                x.User,
                x.RefundCount,
            })
            .ToListAsync();

        var result = new List<AdminUserModel>();

        foreach (var x in users)
        {
            var u = x.User;
            var data = _userEntityMapper.Decrypt(u.EncryptedData);

            result.Add(new AdminUserModel(
                u.Id,
                data.NickName,
                data.Email,
                u.IsDisactivated,
                u.CreatedAt,
                u.SubscriptionPlan.Type.ToString(),
                u.SubscriptionPlan.Points,
                []
            ));
        }

        return Result<AdminPagedResult<AdminUserModel>>
            .Success(new(result, total));
    }

    public async Task<Result<AdminUserModel>> GetUserDetailsAsync(Guid userId)
    {
        var fromDate = DateTime.UtcNow.AddMonths(-6);

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
            return Result<AdminUserModel>.Error("User not found");

        var data = _userEntityMapper.Decrypt(user.EncryptedData);

        var refunds = await _db.PaymentAttempts
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.Status == PaymentAttemptStatus.Refunded &&
                x.IsPrimary &&
                x.Refunded >= fromDate)
            .OrderByDescending(x => x.Refunded)
            .Select(x => new AdminUserRefundedPaymentModel(
                x.Id,
                x.OrderId,
                x.Amount,
                x.Status,
                x.Provider,
                x.Refunded))
            .ToListAsync();

        var result = new AdminUserModel(
            user.Id,
            data.NickName,
            data.Email,
            user.IsDisactivated,
            user.CreatedAt,
            user.SubscriptionPlan.Type.ToString(),
            user.SubscriptionPlan.Points,
            refunds
        );

        return Result<AdminUserModel>.Success(result);
    }

    public async Task<Result<AdminStatisticsModel>> GetStatisticsAsync(AdminStatisticsQuery query)
    {
        var fromDate = query.From ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = query.To ?? DateTime.UtcNow;
        var now = DateTime.UtcNow;

        var totalUsersTask = _db.Users.CountAsync();

        var activeSubscriptionsTask = _db.Subscriptions
            .CountAsync(x =>
                x.IsActive &&
                x.CurrentPeriodEnd > now);

        var paymentsQuery = _db.PaymentAttempts
            .AsNoTracking()
            .Where(x =>
                x.Status == PaymentAttemptStatus.Succeeded &&
                x.CreatedAt >= fromDate &&
                x.CreatedAt <= toDate);

        var revenueTask = paymentsQuery.SumAsync(x => x.Amount);

        var monthlyRevenueTask = paymentsQuery
            .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
            .Select(g => g.Sum(x => x.Amount))
            .ToListAsync();

        var usersByTypeTask = _db.Users
            .AsNoTracking()
            .GroupBy(u => u.SubscriptionPlan.Type)
            .Select(g => new
            {
                Type = g.Key,
                Users = g.Count()
            })
            .ToListAsync();

        var revenueByTypeTask =
            (from p in _db.PaymentAttempts.AsNoTracking()
             join u in _db.Users.AsNoTracking()
                 on p.UserId equals u.Id
             where p.Status == PaymentAttemptStatus.Succeeded
                   && p.CreatedAt >= fromDate
                   && p.CreatedAt <= toDate
             group p by u.SubscriptionPlan.Type
             into g
             select new
             {
                 Type = g.Key,
                 Revenue = g.Sum(x => x.Amount)
             }).ToListAsync();

        var mrrByTypeTask = _db.Subscriptions
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.CurrentPeriodEnd > now)
            .GroupBy(x => x.Type)
            .Select(g => new
            {
                Type = g.Key,
                Mrr = g.Sum(s =>
                    s.SubscriptionPeriod == SubscriptionPeriod.Monthly
                        ? s.Amount
                        : s.Amount / 12m)
            })
            .ToListAsync();

        await Task.WhenAll(
            totalUsersTask,
            activeSubscriptionsTask,
            revenueTask,
            monthlyRevenueTask,
            usersByTypeTask,
            revenueByTypeTask,
            mrrByTypeTask
        );

        var usersByType = usersByTypeTask.Result;
        var revenueByType = revenueByTypeTask.Result;
        var mrrByType = mrrByTypeTask.Result;

        var monthlyRevenue = monthlyRevenueTask.Result;

        var avgMonthlyRevenue = monthlyRevenue.Count == 0
            ? 0m
            : monthlyRevenue.Average();

        var byType = usersByType
            .Select(u =>
            {
                var revenue = revenueByType
                    .FirstOrDefault(r => r.Type == u.Type)?.Revenue ?? 0m;

                var mrr = mrrByType
                    .FirstOrDefault(m => m.Type == u.Type)?.Mrr ?? 0m;

                return new AdminSubscriptionStatisticsModel(
                    u.Type.ToString(),
                    u.Users,
                    revenue,
                    mrr
                );
            })
            .ToList();

        return Result<AdminStatisticsModel>.Success(
            new AdminStatisticsModel(
                TotalUsers: await totalUsersTask,
                ActiveSubscriptions: await activeSubscriptionsTask,
                TotalRevenue: await revenueTask,
                Mrr: mrrByType.Sum(x => x.Mrr),
                byType
            )
        );
    }

    public Task<Result> EnableUserAsync(Guid userId)
        => _userManager.EnableUserAsync(userId);

    public Task<Result> DisableUserAsync(Guid userId)
        => _userManager.DisableUserAsync(userId);

    public async Task<Result<AdminPagedResult<AdminLogModel>>> GetLogsAsync(AdminLogQuery query)
    {
        var logsQuery = _db.Logs
            .AsNoTracking();


        if (query.Severity.HasValue)
            logsQuery = logsQuery.Where(x => x.Severity == query.Severity.Value);

        if (!string.IsNullOrWhiteSpace(query.Message))
        {
            logsQuery = logsQuery.Where(x =>
                EF.Functions.ILike(x.Message, $"%{query.Message}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.TraceId))
        {
            logsQuery = logsQuery.Where(x =>
                x.PropertiesJson != null &&
                EF.Functions.ILike(x.PropertiesJson, $"%\"traceId\":\"{query.TraceId}\"%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Machine))
        {
            logsQuery = logsQuery.Where(x =>
                x.PropertiesJson != null &&
                EF.Functions.ILike(x.PropertiesJson, $"%\"machine\":\"{query.Machine}\"%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Environment))
        {
            logsQuery = logsQuery.Where(x =>
                x.PropertiesJson != null &&
                EF.Functions.ILike(x.PropertiesJson, $"%\"environment\":\"{query.Environment}\"%"));
        }

        if (!string.IsNullOrWhiteSpace(query.JsonField) &&
            !string.IsNullOrWhiteSpace(query.JsonValue))
        {
            var field = query.JsonField;
            var value = query.JsonValue;

            logsQuery = logsQuery.Where(x =>
                x.PropertiesJson != null &&
                EF.Functions.ILike(x.PropertiesJson, $"%\"{field}\":\"{value}\"%"));
        }
        var total = await logsQuery.CountAsync();

        var logs = await logsQuery
            .OrderByDescending(x => x.Created)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new AdminLogModel(
                x.Id,
                x.Message,
                x.Severity,
                x.Source,
                x.PropertiesJson,
                x.Created))
            .ToListAsync();

        return Result<AdminPagedResult<AdminLogModel>>
            .Success(new(logs, total));
    }

    public async Task<Result<AdminHealthModel>> GetHealthAsync()
    {
        bool databaseHealthy;
        bool blobHealthy;
        bool rabbitHealthy;
        bool stripeHealthy;

        try
        {
            databaseHealthy = await _db.Database.CanConnectAsync();
        }
        catch
        {
            databaseHealthy = false;
        }

        try
        {
            var container = new BlobContainerClient(
                _blobConfigurations.ConnectionString,
                _blobConfigurations.ContainerName);

            blobHealthy = await container.ExistsAsync();
        }
        catch
        {
            blobHealthy = false;
        }

        try
        {
            rabbitHealthy = _bus.CheckHealth().Status == BusHealthStatus.Healthy;
        }
        catch
        {
            rabbitHealthy = false;
        }

        try
        {
            stripeHealthy = await _paymentProvider.TestAsync();
        }
        catch
        {
            stripeHealthy = false;
        }

        return Result<AdminHealthModel>.Success(
            new(
                databaseHealthy,
                blobHealthy,
                rabbitHealthy,
                stripeHealthy,
                DateTime.UtcNow));
    }

    private static readonly Lock MaintenanceLock = new();

    private static bool _maintenanceEnabled;

    private static DateTime _updatedAtUtc = DateTime.UtcNow;

    public Task<Result> SetMaintenanceModeAsync(bool enabled)
    {
        lock (MaintenanceLock)
        {
            _maintenanceEnabled = enabled;
            _updatedAtUtc = DateTime.UtcNow;
        }

        return Task.FromResult(Result.Success);
    }

    public Task<Result<MaintenanceModeModel>> GetMaintenanceModeAsync()
    {
        bool enabled;
        DateTime updatedAt;

        lock (MaintenanceLock)
        {
            enabled = _maintenanceEnabled;
            updatedAt = _updatedAtUtc;
        }

        return Task.FromResult(
            Result<MaintenanceModeModel>.Success(
                new MaintenanceModeModel(
                    enabled,
                    updatedAt)));
    }
}
