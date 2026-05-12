using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository;

public interface IScreenshotRepository
{
    public Task<Result<ScreenshotEntity>> MakeAsync(ScreenshotCreateModel screenshot);

    public Task<Result<ScreenshotEntity>> UpdateAsync(ScreenshotUpdateModel screenshot);

    public Task DeleteAsync(IReadOnlyCollection<string> ids);

    public Task<Result<ScreenshotEntity>> UpdateStateAsync(string screenshotId, ScreenshotState state);

    public Task<Result<PaginationResult<ScreenshotEntity>>> GetScreenshots(ScreenshotPaging? paging, Guid userId);

    public Task<Result<ScreenshotEntity>> GetScreenshot(string screenshotId);
}