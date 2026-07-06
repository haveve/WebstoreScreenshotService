using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ProductRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ProductRepository;

public interface IProductManager
{
    public ValueTask<Result<List<Product>>> GetProductsAsync(ProductFilterModel filterModel);

    public ValueTask<Result<Product>> GetProductByIdAsync(Guid productId);
}
