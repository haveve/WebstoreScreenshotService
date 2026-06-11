namespace WebsiteScreenshotService.Services.Payment;

public class PaymentProviderConfigurationManager
    : IPaymentProviderConfigurationManager
{
    private readonly Configurations _config;
    private readonly SensitiveConfigurations _sensitive;

    public PaymentProviderConfigurationManager(IConfiguration config)
    {
        var section = config.GetSection("Payment:Processing");

        var data = section.GetSection("Config").Get<Dictionary<string, string>>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Processing:Config configuration section");

        var dataSest = section.GetSection("SensetiveConfig").Get<Dictionary<string, string>>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Processing:SensetiveConfig configuration section");

        _config = new(data);
        _sensitive = new(dataSest);
    }

    public Configurations GetConfigurations() => _config;

    public SensitiveConfigurations GetSensitiveConfigurations() => _sensitive;
}
