using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Search;

public interface IScreenshotSearchStrategy
{
    public IQueryable<ScreenshotEntity> ApplySearch(IQueryable<ScreenshotEntity> query, string? term, SearchScope scope);
}
