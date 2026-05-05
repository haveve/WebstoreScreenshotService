namespace WebsiteScreenshotService.Services.Payment.Models;

public record Money(decimal Amount, string Currency)
{
    public long ToMinorUnits()
        => (long)Math.Round(Amount * 100, MidpointRounding.AwayFromZero);
}
