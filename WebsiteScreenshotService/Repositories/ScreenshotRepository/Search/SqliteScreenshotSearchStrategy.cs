using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Search;

public class SqliteScreenshotSearchStrategy : IScreenshotSearchStrategy
{
    public IQueryable<ScreenshotEntity> ApplySearch(IQueryable<ScreenshotEntity> query, string? term, SearchScope scope)
    {
        if (string.IsNullOrWhiteSpace(term) || scope == SearchScope.None)
            return query;

        term = term.Trim();

        return scope switch
        {
            SearchScope.Title =>
                query.Where(s =>
                    s.Title != null && s.Title.Contains(term)),
            SearchScope.All =>
                query.Where(s =>
                    (s.Title != null && s.Title.Contains(term)) ||
                    (s.Description != null && s.Description.Contains(term))),
            _ => throw new ArgumentException($"Incorrect search scope was set with value {scope}")

        };
    }
}
