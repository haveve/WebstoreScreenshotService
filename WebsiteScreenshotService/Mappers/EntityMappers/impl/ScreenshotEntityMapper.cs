using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Mappers.EntityMappers.impl;

public class ScreenshotEntityMapper : IScreenshotEntityMapper
{
    public Screenshot FromEntity(ScreenshotEntity entity)
        => new(entity.Id, entity.WebsiteUrl, entity.CreatedAt, entity.State, entity.Type, entity.Title, entity.Description);
}
