namespace WebsiteScreenshotService.Services.Admin;

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
using WebsiteScreenshotService.Utils;

public sealed class AdminService(
    ScreenshotDbContext db,
    IUserManager userManager,
    IUserEntityMapper userEntityMapper,
    IOptions<BlobConfigurations> blobConfigurations,
    IBusControl bus,
    IPaymentProvider paymentProvider)
    : IAdminService
{
    private readonly ScreenshotDbContext _db = db;
    private readonly IUserManager _userManager = userManager;
    private readonly IUserEntityMapper _userEntityMapper = userEntityMapper;
    private readonly BlobConfigurations _blobConfigurations = blobConfigurations.Value;
    private readonly IPaymentProvider _paymentProvider = paymentProvider;
    private readonly IBusControl _bus = bus;

    public async Task<Result<AdminStatisticsModel>> GetStatisticsAsync()
    {
        var totalUsers = await _db.Users.CountAsync();

        var disabledUsers = await _db.Users
            .CountAsync(x => x.IsDisactivated);

        var totalScreenshots = await _db.Screenshots.CountAsync();

        var activeSubscriptions = await _db.Subscriptions
            .CountAsync(x => x.IsActive);

        var revenue = await _db.PaymentAttempts
            .Where(x => x.Status == PaymentAttemptStatus.Succeeded)
            .SumAsync(x => (decimal?)x.Amount) ?? 0m;

        var errorLogs = await _db.Set<LogEntity>()
            .CountAsync(x => x.Severity == Severity.Error);

        var criticalLogs = await _db.Set<LogEntity>()
            .CountAsync(x => x.Severity == Severity.Critical);

        return Result<AdminStatisticsModel>.Success(
            new(
                totalUsers,
                disabledUsers,
                totalScreenshots,
                activeSubscriptions,
                revenue,
                errorLogs,
                criticalLogs));
    }

    public async Task<Result<AdminPagedResult<AdminUserModel>>> GetUsersAsync(
        int skip,
        int take)
    {
        var total = await _db.Users.CountAsync();

        var users = await _db.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var result = users
            .Select(x =>
            {
                var data = _userEntityMapper.Decrypt(x.EncryptedData);

                return new AdminUserModel(
                    x.Id,
                    data.NickName,
                    data.Email,
                    x.IsDisactivated,
                    x.CreatedAt,
                    x.SubscriptionPlan.Type.ToString(),
                    x.SubscriptionPlan.Points);
            })
            .ToList();

        return Result<AdminPagedResult<AdminUserModel>>
            .Success(new(result, total));
    }

    public async Task<Result<AdminUserModel>> GetUserAsync(Guid userId)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
            return Result<AdminUserModel>.Error("User not found");

        var data = _userEntityMapper.Decrypt(user.EncryptedData);

        return Result<AdminUserModel>.Success(
            new(
                user.Id,
                data.NickName,
                data.Email,
                user.IsDisactivated,
                user.CreatedAt,
                user.SubscriptionPlan.Type.ToString(),
                user.SubscriptionPlan.Points));
    }

    public Task<Result> EnableUserAsync(Guid userId)
        => _userManager.EnableUserAsync(userId);

    public Task<Result> DisableUserAsync(Guid userId)
        => _userManager.DisableUserAsync(userId);

    public async Task<Result<AdminPagedResult<AdminLogModel>>> GetLogsAsync(
        int skip,
        int take,
        Severity? severity = null)
    {
        var query = _db.Logs
            .AsNoTracking();

        if (severity.HasValue)
            query = query.Where(x => x.Severity == severity.Value);

        var total = await query.CountAsync();

        var logs = await query
            .OrderByDescending(x => x.Created)
            .Skip(skip)
            .Take(take)
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
