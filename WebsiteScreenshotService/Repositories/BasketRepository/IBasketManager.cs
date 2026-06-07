using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.BasketRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.BasketRepository;

public interface IBasketManager
{
    Task<Result<Basket>> GetCalculatedBasketAsync(Guid? userId = null);

    Task<ConditionalResult> AddProductsAsync(AddLineModel[] basketLines, Guid? userId = null);

    Task<ConditionalResult> ClearAsync(Guid? userId = null);
}
