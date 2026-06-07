using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.ProductRepository.Models;

public record ProductFilterModel(ProductType[] ProductTypes, Guid[] Ids);