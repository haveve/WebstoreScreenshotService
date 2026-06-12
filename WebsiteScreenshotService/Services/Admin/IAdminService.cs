using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Utils;
using WebsiteScreenshotService.Services.Admin.Models;

namespace WebsiteScreenshotService.Services.Admin;

public interface IAdminService
{
    Task<Result<AdminStatisticsModel>> GetStatisticsAsync();

    Task<Result<AdminPagedResult<AdminUserModel>>> GetUsersAsync(
        int skip,
        int take);

    Task<Result<AdminUserModel>> GetUserAsync(Guid userId);

    Task<Result> EnableUserAsync(Guid userId);

    Task<Result> DisableUserAsync(Guid userId);

    Task<Result<AdminPagedResult<AdminLogModel>>> GetLogsAsync(
        int skip,
        int take,
        Severity? severity = null);

    Task<Result<AdminHealthModel>> GetHealthAsync();

    Task<Result> SetMaintenanceModeAsync(bool enabled);

    Task<Result<MaintenanceModeModel>> GetMaintenanceModeAsync();
}

public record AdminHealthModel(
    bool DatabaseHealthy,
    bool BlobStorageHealthy,
    bool RabbitMqHealthy,
    bool StripeHealthy,
    DateTime CheckedAt);

public record AdminMaintenanceResultModel(
    bool CacheCleared,
    int LogsRemoved);

public record AdminStatisticsModel(
    int TotalUsers,
    int DisabledUsers,
    int TotalScreenshots,
    int ActiveSubscriptions,
    decimal Revenue,
    int ErrorLogs,
    int CriticalLogs);

public record MaintenanceModeModel(
    bool Enabled,
    DateTime UpdatedAtUtc);