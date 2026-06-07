using WebsiteScreenshotService.Repositories._EF.DbEntities;

namespace WebsiteScreenshotService.Repositories.BasketRepository;

public interface IBasketRepository
{
    Task<BasketEntity?> GetByUserIdAsync(Guid userId);

    Task CreateBasket(BasketEntity basket);

    Task AddProductsAsync(Guid userId, BasketLineEntity[] basketLines);

    Task ModifyBasket(Guid userId, Func<BasketEntity, Task<bool>> modify);

    Task ClearAsync(Guid userId);
}
