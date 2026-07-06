namespace WebsiteScreenshotService.Entities;

public record Basket(Guid Id, Guid UserId, BasketLine[] Lines, decimal Totals, string Currency);

public record BasketLine(Guid ProductId, int Quantity, decimal Price);