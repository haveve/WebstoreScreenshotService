using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services;

public interface IProductPriceManager
{
    ValueTask<Result<List<ProductPrice>>> GetPricesAsync(Guid[] productIds);

    ValueTask<Result<Money>> GetPointsPriceAsync(int points);
}

public record ProductPrice(Guid ProductId, Money Price);