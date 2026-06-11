using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories.BasketRepository;
using WebsiteScreenshotService.Repositories.BasketRepository.Models;
using WebsiteScreenshotService.Repositories.ProductRepository;

namespace WebsiteScreenshotService.Controllers;

[Authorize]
[ApiController]
[Route("basket/[action]")]
public class BasketController : ControllerBase
{
    private readonly IBasketManager _basketManager;
    private readonly IProductManager _productManager;

    public BasketController(IBasketManager basketManager, IProductManager productManager)
    {
        _basketManager = basketManager;
        _productManager = productManager;
    }

    [HttpPost("addProduct")]
    public async Task<IActionResult> AddToBasket([FromBody] AddToBasketRequest request)
    {
        var product = await _productManager.GetProductByIdAsync(request.ProductId);

        if(!product.IsSuccess)
            return BadRequest(new ErrorResponse($"Product with Id {request.ProductId} does not exist"));

        var lines = new[]
        {
            new AddLineModel
            (
                ProductId: request.ProductId,
                Quantity: request.Quantity
            )
        };

        await _basketManager.AddProductsAsync(lines);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetBasket()
    {
        var result = await _basketManager.GetCalculatedBasketAsync();
        return Ok(result);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        await _basketManager.ClearAsync();
        return Ok();
    }
}

public class AddToBasketRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}