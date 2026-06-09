using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ProductRepository;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services;

public sealed class ProductPriceManager : IProductPriceManager
{
    private readonly ProductPricingConfiguration _config;

    private readonly IUserContextAccessor _contextAccessor;

    private readonly IProductManager _productManager;

    public ProductPriceManager(IConfiguration configuration, IUserContextAccessor contextAccessor, IProductManager productManager)
    {
        _config =
            configuration
                .GetSection("Products")
                .Get<ProductPricingConfiguration>()
            ?? throw new InvalidOperationException("Missing or invalid 'Products' configuration");
        _contextAccessor = contextAccessor;
        _productManager = productManager;
    }

    public async ValueTask<Result<List<ProductPrice>>> GetPricesAsync(Guid[] productIds)
    {
        var products = await _productManager.GetProductsAsync(new(ProductTypes: [], Ids: productIds));

        if (!products.IsSuccess)
            return Result<List<ProductPrice>>.Error(products.ErrorMessage!);

        var prices = new List<ProductPrice>(products.Value!.Count);

        foreach (var product in products.Value!)
        {
            if (!_config.Subscriptions.TryGetValue(product.Id, out PriceConfig? priceConfig))
                return Result<List<ProductPrice>>.Error($"Price not configured for product {product.Id}");

            if(product.Type == ProductType.Points)
                return Result<List<ProductPrice>>.Error($"Price for points is calculated real-time and cannot be fetched");

            prices.Add(new(product.Id, ToMoney(priceConfig)));
        }

        return Result<List<ProductPrice>>.Success(prices);
    }

    private static Money ToMoney(PriceConfig cfg)
        => new(cfg.Amount, cfg.Currency);

    public ValueTask<Result<Money>> GetPointsPriceAsync(int points)
    {
        var amount = (points / 1000m) * _config.Points.PricePer1000;

        var subscriptionType = _contextAccessor.GetCurrentUser().SubscriptionPlan.Type;
        var subscriptionPlanDiscount = subscriptionType switch
        {
            SubscriptionType.Regular => 0M,
            SubscriptionType.Pro => 0.05M,
            SubscriptionType.Advanced => 0.1M,
            _ => -1M
        };

        if (subscriptionPlanDiscount == -1)
        {
            var error = Result<Money>.Error($"Invalid subscription type '{subscriptionType}' was encountered during points calculation");
            return ValueTask.FromResult(error);
        }

        amount *= Math.Max(0, 1 - subscriptionPlanDiscount - GetAmountDiscount(points));

        var money = new Money(
            decimal.Round(amount, 2),
            _config.Points.Currency);

        return ValueTask.FromResult(Result<Money>.Success(money));
    }

    private static decimal GetAmountDiscount(int points)
        => points switch
        {
            < 5_000 => 0M,
            < 100_000 => 0.05M,
            < 200_000 => 0.1M,
            _ => 0.2M
        };
}


public class ProductPricingConfiguration
{
    [Required]
    public PointsConfig Points { get; set; } = default!;

    [Required]
    public Dictionary<Guid, PriceConfig> Subscriptions { get; set; } = default!;
}

public class PointsConfig
{
    [Required]
    public decimal PricePer1000 { get; set; }

    [Required]
    public string Currency { get; set; } = "USD";
}

public class PriceConfig
{
    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = "USD";
}