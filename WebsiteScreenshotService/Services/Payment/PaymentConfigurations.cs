namespace WebsiteScreenshotService.Services.Payment;

public record PaymentConfigurations(SensitiveConfigurations SensitiveData, Configurations Data);

public record SensitiveConfigurations(Dictionary<string, string> Data);

public record Configurations(Dictionary<string, string> Data);