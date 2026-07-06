using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebsiteScreenshotService.Services.Payment;

public class PaymentProviderCallbackConfigurationManager
    : IPaymentProviderCallbackConfigurationManager
{
    private readonly Configurations _config;
    private readonly SensitiveConfigurations _sensitive;

    public PaymentProviderCallbackConfigurationManager(IConfiguration config)
    {
        var section = config.GetSection("Payment:Callback");

        var data = section.GetSection("Config").Get<Dictionary<string, string>>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Callback:Config configuration section");

        var dataSest = section.GetSection("SensetiveConfig").Get<Dictionary<string, string>>()
            ?? throw new InvalidOperationException(
                "Missing Payment:Callback:SensetiveConfig configuration section");

        _config = new(data);
        _sensitive = new(dataSest);
    }

    public Configurations GetConfigurations()
        => _config;

    public SensitiveConfigurations GetSensitiveConfigurations()
        => _sensitive;
}
