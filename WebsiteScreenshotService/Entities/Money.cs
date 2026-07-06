namespace WebsiteScreenshotService.Entities;

public record Money(decimal Amount, string Currency)
{
    public long ToMinorUnits()
        => (long)Math.Round(Amount * 100, MidpointRounding.AwayFromZero);
}
