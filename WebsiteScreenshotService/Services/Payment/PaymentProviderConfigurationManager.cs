namespace WebsiteScreenshotService.Services.Payment;

public class PaymentProviderConfigurationManager
    : IPaymentProviderConfigurationManager
{
    private readonly Configurations _config;
    private readonly SensitiveConfigurations _sensitive;

    public PaymentProviderConfigurationManager(IConfiguration config)
    {
        var section = config.GetSection("Payment:Processing");

        _config = section.GetSection("Config").Get<Configurations>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Processing:Config configuration section");

        _sensitive = section.GetSection("SensetiveConfig").Get<SensitiveConfigurations>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Processing:SensetiveConfig configuration section");
    }

    public Configurations GetConfigurations() => _config;

    public SensitiveConfigurations GetSensitiveConfigurations() => _sensitive;
}
