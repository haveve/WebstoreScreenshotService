using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.EF;

namespace WebsiteScreenshotService.Repositories.BasketRepository;

public class BasketRepository(ScreenshotDbContext context) : IBasketRepository
{
    private readonly ScreenshotDbContext _context = context;

    public async Task<BasketEntity?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Baskets
            .AsNoTracking()
            .Include(b => b.Lines)
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task CreateBasket(BasketEntity basket)
    {
        _context.Baskets.Add(basket);
        await _context.SaveChangesAsync();
    }

    public async Task ModifyBasket(Guid userId, Func<BasketEntity, Task<bool>> modify)
    {
        var basket = await _context.Baskets
            .Include(b => b.Lines)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (basket is null)
            throw new Exception("Basket does not exist");

        var modified = await modify(basket);

        if(modified)
            await _context.SaveChangesAsync();
    }

    public async Task AddProductsAsync(Guid userId, BasketLineEntity[] basketLines)
    {
        var basket = await _context.Baskets
            .Include(b => b.Lines)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (basket is null)
            throw new Exception("Basket does not exist");

        foreach (var line in basketLines)
            basket.Lines.Add(line);

        await _context.SaveChangesAsync();
    }

    public async Task ClearAsync(Guid userId)
    {
        var deletedCount = await _context.Baskets
            .Where(b => b.UserId == userId)
            .ExecuteDeleteAsync();
    }
}