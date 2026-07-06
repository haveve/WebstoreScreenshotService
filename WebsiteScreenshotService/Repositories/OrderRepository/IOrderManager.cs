using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.OrderRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.OrderRepository;

public interface IOrderManager
{
    Task<OrderEntity> CreateFromBasketAsync();

    Task<OrderEntity?> GetAsync(Guid orderId);

    Task<PaginationResult<OrderEntity>> GetByUserPagedAsync(
        Paging paging,
        Guid? userId = null);
}
