using WebsiteScreenshotService.Utils;
using WebsiteScreenshotService.Services.Admin.Models;

namespace WebsiteScreenshotService.Services.Admin;

public interface IAdminService
{
    Task<Result<AdminStatisticsModel>> GetStatisticsAsync(AdminStatisticsQuery query);

    Task<Result<AdminPagedResult<AdminUserModel>>> GetUsersAsync(AdminUserQuery query);

    Task<Result<AdminUserModel>> GetUserDetailsAsync(Guid userId);

    Task<Result> EnableUserAsync(Guid userId);

    Task<Result> DisableUserAsync(Guid userId);

    Task<Result<AdminPagedResult<AdminLogModel>>> GetLogsAsync(AdminLogQuery query);

    Task<Result<AdminHealthModel>> GetHealthAsync();

    Task<Result> SetMaintenanceModeAsync(bool enabled);

    Task<Result<MaintenanceModeModel>> GetMaintenanceModeAsync();
}