using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

using EFFunctions = Microsoft.EntityFrameworkCore.EF;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Search;

public class PostgresScreenshotSearchStrategy : IScreenshotSearchStrategy
{
    public IQueryable<ScreenshotEntity> ApplySearch(IQueryable<ScreenshotEntity> query, string? term, SearchScope scope)
    {
        if (string.IsNullOrWhiteSpace(term) || scope == SearchScope.None)
            return query;

        var tsQuery = EFFunctions.Functions.WebSearchToTsQuery("simple", term);

        return scope switch
        {
            SearchScope.Title =>
                query.Where(s => s.TitleVector.Matches(tsQuery)),
            SearchScope.All =>
                query.Where(s => s.SearchVector.Matches(tsQuery)),
            _ => throw new ArgumentException($"Incorrect search scope was set with value {scope}")
        };
    }
}
