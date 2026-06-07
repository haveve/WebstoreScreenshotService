using System.Collections.ObjectModel;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.ProductRepository.Models;
using WebsiteScreenshotService.Services.Payment.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.ProductRepository;

public class ProductManager : IProductManager
{
    public ValueTask<Result<Product>> GetProductByIdAsync(Guid productId)
    {
        var product = ProductCatalog.Products.FirstOrDefault(p => p.Id == productId);
        var result = product is null
             ? Result<Product>.Error("Product wasn't found")
             : Result<Product>.Success(product);

        return ValueTask.FromResult(result);
    }

    public ValueTask<Result<List<Product>>> GetProductsAsync(ProductFilterModel filterModel)
    {
        var products = ApplyFilter(filterModel).ToList();
        var result = Result<List<Product>>.Success(products);

        return ValueTask.FromResult(result);
    }

    private static IEnumerable<Product> ApplyFilter(ProductFilterModel filterModel)
    {
        IEnumerable<Product> enumerable = ProductCatalog.Products;

        if (filterModel.ProductTypes.Length > 0)
            enumerable = enumerable.Where(p => filterModel.ProductTypes.Contains(p.Type));

        if (filterModel.Ids.Length > 0)
            enumerable = enumerable.Where(p => filterModel.Ids.Contains(p.Id));

        return enumerable;
    }

    private static class ProductCatalog
    {
        public static readonly Guid PointsProductId =
            Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static readonly Guid ProMonthlyProductId =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static readonly Guid ProYearlyProductId =
            Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static readonly Guid AdvancedMonthlyProductId =
            Guid.Parse("44444444-4444-4444-4444-444444444444");

        public static readonly Guid AdvancedYearlyProductId =
            Guid.Parse("55555555-5555-5555-5555-555555555555");

        private static ReadOnlyDictionary<string, string> FormatSubscriptionInfo(SubscriptionPeriod duration, SubscriptionType subscriptionType)
        {
            var dictionary = new Dictionary<string, string>()
            {
                {"Duration", duration.ToString() },
                {"SubscriptionType", subscriptionType.ToString() }
            };

            return dictionary.AsReadOnly();
        }

        public static readonly IReadOnlyList<Product> Products =
        [
            new
            (
                Id: PointsProductId,
                Name: "Points",
                Type: ProductType.Points,
                AdditionalInfo: ReadOnlyDictionary<string, string>.Empty
            ),
            new
            (
                Id: ProMonthlyProductId,
                Name: "Pro Monthly",
                Type: ProductType.Subscription,
                AdditionalInfo: FormatSubscriptionInfo(SubscriptionPeriod.Monthly, SubscriptionType.Pro)
            ),
            new
            (
                Id: ProYearlyProductId,
                Name: "Pro Yearly",
                Type: ProductType.Subscription,
                AdditionalInfo: FormatSubscriptionInfo(SubscriptionPeriod.Yearly, SubscriptionType.Pro)
            ),
            new
            (
                Id: AdvancedMonthlyProductId,
                Name: "Advanced Monthly",
                Type: ProductType.Subscription,
                AdditionalInfo: FormatSubscriptionInfo(SubscriptionPeriod.Monthly, SubscriptionType.Advanced)
            ),
            new
            (
                Id: AdvancedYearlyProductId,
                Name: "Advanced Yearly",
                Type: ProductType.Subscription,
                AdditionalInfo: FormatSubscriptionInfo(SubscriptionPeriod.Yearly, SubscriptionType.Advanced)
            )
        ];
    }
}
