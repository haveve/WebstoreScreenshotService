using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Mappers.EntityMappers.impl;

public class ScreenshotEntityMapper(ICategoryEntityMapper categoryEntityMapper) : IScreenshotEntityMapper
{
    public Screenshot FromEntity(ScreenshotEntity entity)
        => new(entity.Id, entity.WebsiteUrl, entity.CreatedAt, entity.State, entity.Type, [.. entity.Categories.Select(categoryEntityMapper.FromEntity)], entity.Title, entity.Description);
}
