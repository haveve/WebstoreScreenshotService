using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.OrderRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.OrderRepository;

public class OrderRepository(ScreenshotDbContext context) : IOrderRepository
{
    private readonly ScreenshotDbContext _context = context;

    public async Task<OrderEntity> CreateAsync(OrderEntity order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<OrderEntity?> GetAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<PaginationResult<OrderEntity>> GetByUserPagedAsync(
        Guid userId,
        Paging paging)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.Lines)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(paging.Index * paging.Index)
            .Take(paging.Size)
            .ToListAsync();

        return new PaginationResult<OrderEntity>
        (
            TotalCount: totalCount,
            Items: items
        );
    }

    public async Task<OrderEntity?> GetByUserAndIdAsync(Guid userId, Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.Id == orderId);
    }

    public async Task MarkPaidAsync(Guid orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            throw new InvalidOperationException("Order not found");

        order.Status = OrderStatus.Paid;

        await _context.SaveChangesAsync();
    }

    public async Task CancelAsync(Guid orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            return;

        order.Status = OrderStatus.Cancelled;

        await _context.SaveChangesAsync();
    }
}