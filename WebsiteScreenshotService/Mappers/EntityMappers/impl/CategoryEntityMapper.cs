using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Mappers.EntityMappers.impl;

public class CategoryEntityMapper : ICategoryEntityMapper
{
    public Category FromEntity(CategoryEntity entity)
        => new(entity.UserId, entity.Id, entity.Name, entity.Color);
}
