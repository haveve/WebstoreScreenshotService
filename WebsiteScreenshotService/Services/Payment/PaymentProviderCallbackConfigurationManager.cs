namespace WebsiteScreenshotService.Services.Payment;

public class PaymentProviderCallbackConfigurationManager
    : IPaymentProviderCallbackConfigurationManager
{
    private readonly Configurations _config;
    private readonly SensitiveConfigurations _sensitive;

    public PaymentProviderCallbackConfigurationManager(IConfiguration config)
    {
        var section = config.GetSection("Payment:Callback");

        _config = section.GetSection("Config").Get<Configurations>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Callback:Config configuration section");

        _sensitive = section.GetSection("SensetiveConfig").Get<SensitiveConfigurations>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Callback:SensetiveConfig configuration section");
    }

    public Configurations GetConfigurations()
        => _config;

    public SensitiveConfigurations GetSensitiveConfigurations()
        => _sensitive;
}
