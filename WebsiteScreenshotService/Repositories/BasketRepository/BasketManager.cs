using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.BasketRepository.Models;
using WebsiteScreenshotService.Repositories.ProductRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.BasketRepository;

public class BasketManager(
    IBasketRepository basketRepository,
    IUserContextAccessor userContextAccessor,
    IProductPriceManager productPriceManager,
    IProductManager productManager) : IBasketManager
{
    private readonly IBasketRepository _basketRepository = basketRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IProductPriceManager _productPriceManager = productPriceManager;
    private readonly IProductManager _productManager = productManager;

    public async Task<Result<Basket>> GetCalculatedBasketAsync(Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;
        var basket = await _basketRepository.GetByUserIdAsync(userId.Value);

        if (basket is null)
        {
            basket = new BasketEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                Lines = []
            };

            await _basketRepository.CreateBasket(basket);
        }

        return Result<Basket>.Success(await CalculateBasketAsync(basket));
    }

    private async Task<Basket> CalculateBasketAsync(BasketEntity basket)
    {
        if (basket.Lines.Count == 0)
        {
            return new Basket(
                basket.Id,
                basket.UserId,
                [],
                0m,
                "USD"
            );
        }

        var productIds = basket.Lines
            .Select(x => x.ProductId)
            .Distinct()
            .ToArray();

        var productsResult = await _productManager.GetProductsAsync(
            new (
                ProductTypes: [],
                Ids: productIds
            )
        );

        if (!productsResult.IsSuccess)
            throw new InvalidOperationException(productsResult.ErrorMessage);

        var products = productsResult.Value!;

        var totalPoints = basket.Lines.Sum(x => x.Quantity);
        var priceResult = await _productPriceManager.GetPointsPriceAsync(totalPoints);

        if (!priceResult.IsSuccess)
            throw new InvalidOperationException(priceResult.ErrorMessage);

        var money = priceResult.Value!;

        var lines = basket.Lines.Select(line =>
        {
            var product = products.First(p => p.Id == line.ProductId);
            var unitPrice = money.Amount / totalPoints;

            return new BasketLine(
                ProductId: line.ProductId,
                Quantity: line.Quantity,
                Price: decimal.Round(unitPrice * line.Quantity, 2)
            );
        }).ToArray();

        return new Basket(
            Id: basket.Id,
            UserId: basket.UserId,
            Lines: lines,
            Totals: money.Amount,
            Currency: money.Currency
        );
    }

    public async Task<ConditionalResult> AddProductsAsync(AddLineModel[] basketLines, Guid? userId = null)
    {
        if (basketLines.Length == 0)
            return ConditionalResult.Success(true);

        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var basket = await _basketRepository.GetByUserIdAsync(userId.Value);

        if (basket is null)
        {
            basket = new BasketEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                Lines = []
            };

            await _basketRepository.CreateBasket(basket);
        }

        await _basketRepository.ModifyBasket(userId.Value, basket =>
        {
            foreach (var line in basketLines)
            {
                var existing = basket.Lines
                    .FirstOrDefault(x => x.ProductId == line.ProductId);

                if (existing is null)
                {
                    basket.Lines.Add(new BasketLineEntity
                    {
                        Id = Guid.NewGuid(),
                        BasketId = basket.Id,
                        ProductId = line.ProductId,
                        Quantity = line.Quantity
                    });
                }
                else
                {
                    existing.Quantity += line.Quantity;
                }
            }

            return Task.FromResult(true);
        });

        return ConditionalResult.Success(true);
    }

    public async Task<ConditionalResult> ClearAsync(Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;
        await _basketRepository.ClearAsync(userId.Value);

        return ConditionalResult.Success(true);
    }
}
