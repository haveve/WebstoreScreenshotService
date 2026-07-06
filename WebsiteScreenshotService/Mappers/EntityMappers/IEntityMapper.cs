namespace WebsiteScreenshotService.Mappers.EntityMappers;

public interface IEntityMapper<TEntity, TModel>
    where TEntity : class
    where TModel : class
{
    public TModel FromEntity(TEntity entity);
}
