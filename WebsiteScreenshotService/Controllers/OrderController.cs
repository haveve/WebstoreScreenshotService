using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.OrderRepository;
using WebsiteScreenshotService.Repositories.ProductRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.Checkout;

namespace WebsiteScreenshotService.Controllers;

[Authorize(Roles = UserRoles.User)]
[ApiController]
[Route("order/[action]")]
public class OrderController : ControllerBase
{
    private readonly IOrderManager _orderManager;
    private readonly ICheckoutManager _checkoutManager;
    private readonly IProductManager _productManager;
    private readonly IProductPriceManager _productPriceManager;

    public OrderController(
        IOrderManager orderManager,
        ICheckoutManager checkoutManager,
        IProductManager productManager,
        IProductPriceManager productPriceManager)
    {
        _orderManager = orderManager;
        _checkoutManager = checkoutManager;
        _productManager = productManager;
        _productPriceManager = productPriceManager;
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> Get(Guid orderId)
    {
        var order = await _orderManager.GetAsync(orderId);

        if (order is null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        var result = await _orderManager.GetByUserPagedAsync(new(Index: page, Size: pageSize));
        return Ok(result);
    }

    [HttpPost("{orderId:guid}/retry")]
    public async Task<IActionResult> Retry(Guid orderId)
    {
        var result = await _checkoutManager.RetryPaymentAsync(orderId);
        return Ok(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var result = await _checkoutManager.StartCheckoutAsync(new StartCheckoutRequest());
        return Ok(result);
    }

    [HttpPost("{subscriptionId:guid}/subscription")]
    public async Task<IActionResult> StartSubscription(Guid subscriptionId)
    {
        var productResult = await _productManager.GetProductByIdAsync(subscriptionId);

        if (!productResult.IsSuccess)
            return NotFound();

        var product = productResult.Value!;

        var pricesResult = await _productPriceManager.GetPricesAsync([product.Id]);

        if (!pricesResult.IsSuccess)
            return NotFound();

        var productPriceInfo = pricesResult.Value!.Single();
        var subscriptionRequest = new StartSubscriptionRequest
        {
            SubscriptionInfo = new()
            {
                Price = productPriceInfo.Price,
                Duration = Enum.Parse<SubscriptionPeriod>(product.AdditionalInfo["Duration"]),
                SubscriptionType = Enum.Parse<SubscriptionType>(product.AdditionalInfo["SubscriptionType"])
            }
        };

        var result = await _checkoutManager.StartSubscriptionAsync(subscriptionRequest);
        return Ok(result);
    }

    [HttpPost("refund/{paymentAttemptId:guid}")]
    public async Task<IActionResult> Refund(Guid paymentAttemptId)
    {
        var refundRequest = new RefundPaymentRequest() { PaymentAttemptId = paymentAttemptId };
        await _checkoutManager.RefundAsync(refundRequest);
        return NoContent();
    }
}