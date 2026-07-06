using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Mappers.EntityMappers;

public interface IScreenshotEntityMapper : IEntityMapper<ScreenshotEntity, Screenshot>;
