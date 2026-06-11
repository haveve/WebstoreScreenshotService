using Shared.Core.Utils;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.BasketRepository;
using WebsiteScreenshotService.Repositories.OrderRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.OrderRepository;

public class OrderManager(
    IBasketManager basketManager,
    IOrderRepository orderRepository,
    IUserContextAccessor userContextAccessor
) : IOrderManager
{
    private readonly IBasketManager _basketManager = basketManager;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly AsyncReadWriteLockManager<Guid> _locks = new();

    public async Task<OrderEntity> CreateFromBasketAsync()
    {
        var userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        using var _ = await _locks.EnterWriteAsync(userId);

        var basketResult = await _basketManager.GetCalculatedBasketAsync(userId);

        if(!basketResult.IsSuccess)
            throw new InvalidOperationException(basketResult.ErrorMessage!);

        var basket = basketResult.Value;

        if (basket is null || basket.Lines.Length == 0)
            throw new InvalidOperationException("Basket is empty");

        var orderId = Guid.NewGuid();

        var order = new OrderEntity
        {
            Id = orderId,
            UserId = userId,
            TotalAmount = basket.Totals,
            Currency = basket.Currency,
            Status = OrderStatus.PendingPayment,
            CreatedAt = DateTime.UtcNow,
            Lines = basket.Lines.Select(l => new OrderLineEntity
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = l.ProductId,
                Quantity = l.Quantity,
                UnitPrice = l.Price / Math.Max(1, l.Quantity),
                ProductName = "Points"
            }).ToList()
        };

        await _orderRepository.CreateAsync(order);
        await _basketManager.ClearAsync(userId);

        return order;
    }

    public async Task<OrderEntity?> GetAsync(Guid orderId)
    {
        var userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;
        return await _orderRepository.GetByUserAndIdAsync(userId, orderId);
    }

    public async Task<PaginationResult<OrderEntity>> GetByUserPagedAsync(
        Paging paging,
        Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;
        return await _orderRepository.GetByUserPagedAsync(userId.Value, paging);
    }
}
