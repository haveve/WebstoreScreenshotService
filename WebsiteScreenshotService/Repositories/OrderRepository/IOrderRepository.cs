using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.OrderRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.OrderRepository;

public interface IOrderRepository
{
    Task<OrderEntity> CreateAsync(OrderEntity order);

    Task<OrderEntity?> GetAsync(Guid orderId);

    Task<OrderEntity?> GetByUserAndIdAsync(Guid userId, Guid orderId);

    Task<PaginationResult<OrderEntity>> GetByUserPagedAsync(Guid userId, Paging paging);

    Task MarkPaidAsync(Guid orderId);

    Task CancelAsync(Guid orderId);
}
